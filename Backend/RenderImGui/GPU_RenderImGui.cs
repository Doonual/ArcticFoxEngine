#pragma warning disable CS8618

namespace ArcticFoxEngine {


	using ImGuiNET;
	using ImDrawIdx = System.UInt16;
	using System.Collections.Generic;
	using System;
	using System.Linq;
	using SixLabors.ImageSharp;
	using SixLabors.ImageSharp.PixelFormats;
	using System.Buffers;
	using SharpDX.Direct3D12;
	using ArcticFoxEngine.Backend;
	using CoolClassLibrary;
	using ArcticFoxEngine.Debug;
	using ArcticFoxEngine.Backend.RenderImGui;

	unsafe internal static class GPU_RenderImGui {

#nullable enable

		static internal PipelineState pipelineState;
		static RootSignature rootSignature;
		static DescriptorHeap descriptorHeap;
		static int descriptorHeapIndex;

		static Resource vertexBuffer;
		static VertexBufferView vertexBufferView;
		static int vertexBufferSize = 5000, indexBufferSize = 10000;

		static Resource indexBuffer;
		static IndexBufferView indexBufferView;

		static ConstBuffer<Matrix> constantBuffer;
		

		
		static readonly Dictionary<IntPtr, (Texture, int)> textureResources = new();

		

		private static bool replaceFont = false;
		private static ushort[]? fontCustomGlyphRange;
		private static string fontPathName;
		private static float fontSize;
		private static FontGlyphRangeType fontLanguage;
		private static Dictionary<string, (IntPtr Handle, uint Width, uint Height)> loadedTexturesPtrs;


		internal static void Init(int width, int height) {
			descriptorHeapIndex = 1;

			loadedTexturesPtrs = new Dictionary<string, (IntPtr Handle, uint Width, uint Height)>();

			NodeIconBank.Init();
			ImGui.CreateContext();
			var io = ImGui.GetIO();
			io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;  // We can honor the ImDrawCmd::VtxOffset field, allowing for large meshes.
			io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
			io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
			ImGui.StyleColorsDark();
			Resize(width, height);
			CreateDeviceObjects();
		}

		internal static void Update(float deltaTime, Action DoRender) {
			var io = ImGui.GetIO();
			io.DeltaTime = deltaTime;
			ImGui.NewFrame();
			DoRender?.Invoke();
			ImGui.Render();
		}


		private static ImDrawDataPtr? UpdateImGuiDrawList() {

			ImGuiInput.Update();
			Update(Profiler.deltaTime, DebugManager.Render);

			ImDrawDataPtr data = ImGui.GetDrawData();
			// Avoid rendering when minimized
			if (data.DisplaySize.X <= 0.0f || data.DisplaySize.Y <= 0.0f) { return null; }

			#region Vertex buffer creation

			if (vertexBuffer == null || vertexBufferSize < data.TotalVtxCount) {
				vertexBuffer?.Dispose();

				vertexBufferSize = data.TotalVtxCount + 5000;
				vertexBuffer = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(vertexBufferSize * sizeof(ImDrawVert)), ResourceStates.GenericRead);
				vertexBufferView.BufferLocation = vertexBuffer.GPUVirtualAddress;
				vertexBufferView.StrideInBytes = sizeof(ImDrawVert);
				vertexBufferView.SizeInBytes = vertexBufferSize * sizeof(ImDrawVert);
			}

			#endregion
			#region Index buffer creation

			if (indexBuffer == null || indexBufferSize < data.TotalIdxCount) {
				indexBuffer?.Dispose();

				indexBufferSize = data.TotalIdxCount + 10000;

				indexBuffer = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(indexBufferSize * sizeof(ImDrawIdx)), ResourceStates.GenericRead);
				indexBufferView.BufferLocation = indexBuffer.GPUVirtualAddress;
				indexBufferView.SizeInBytes = indexBufferSize * sizeof(ImDrawIdx);
				indexBufferView.Format = SharpDX.DXGI.Format.R16_UInt;

			}

			#endregion
			#region Uploading to vertex buffer and index buffer

			// Upload vertex/index data into a single contiguous GPU buffer
			ImDrawVert* vertexResourcePointer = (ImDrawVert*)vertexBuffer.Map(0);
			ImDrawIdx* indexResourcePointer = (ImDrawIdx*)indexBuffer.Map(0);
			for (int n = 0; n < data.CmdListsCount; n++) {
				ImDrawListPtr cmdlList = data.CmdListsRange[n];

				int vertBytes = cmdlList.VtxBuffer.Size * sizeof(ImDrawVert);
				Buffer.MemoryCopy((void*)cmdlList.VtxBuffer.Data, vertexResourcePointer, vertBytes, vertBytes);


				int idxBytes = cmdlList.IdxBuffer.Size * sizeof(ImDrawIdx);

				Buffer.MemoryCopy((void*)cmdlList.IdxBuffer.Data, indexResourcePointer, idxBytes, idxBytes);

				vertexResourcePointer += cmdlList.VtxBuffer.Size;
				indexResourcePointer += cmdlList.IdxBuffer.Size;
			}
			vertexBuffer.Unmap(0);
			indexBuffer.Unmap(0);

			#endregion
			#region Viewport matrix

			// Setup orthographic projection matrix into our constant buffer
			// Our visible imgui space lies from draw_data.DisplayPos (top left) to draw_data.DisplayPos+data_data.DisplaySize (bottom right). DisplayPos is (0,0) for single viewport apps.
			float L = data.DisplayPos.X;
			float R = data.DisplayPos.X + data.DisplaySize.X;
			float T = data.DisplayPos.Y;
			float B = data.DisplayPos.Y + data.DisplaySize.Y;
			Matrix projMat = new Matrix(
				2.0f / (R - L), 0.0f, 0.0f, 0.0f,
				0.0f, 2.0f / (T - B), 0.0f, 0.0f,
				0.0f, 0.0f, 0.5f, 0.0f,
				(R + L) / (L - R), (T + B) / (B - T), 0.5f, 1.0f
			);
			constantBuffer.Write(new Matrix[] { projMat }, 0);

			#endregion

			return data;

		}

		internal static void Render(Resource renderTarget, DescriptorHeap rtvDescHeap, DescriptorHeap dsvDescHeap) {

			Graphics.WaitForCmdList();
			GraphicsCommandList cmdList = Graphics.CreateGraphicsCommandList(pipelineState);

			ImDrawDataPtr? dataNull = UpdateImGuiDrawList();
			if (dataNull == null) { return; }
			ImDrawDataPtr data = (ImDrawDataPtr)dataNull;

			// Indicate that the back buffer will be used as a render target
			cmdList.ResourceBarrierTransition(renderTarget, ResourceStates.Present, ResourceStates.RenderTarget);

			// Set render target and depth stencil
			CpuDescriptorHandle rtvHandle = rtvDescHeap.CPUDescriptorHandleForHeapStart;
			CpuDescriptorHandle dsvHandle = dsvDescHeap.CPUDescriptorHandleForHeapStart;
			rtvHandle += Graphics.frameIndex * Graphics.rtvHeapIncrement;
			cmdList.SetRenderTargets(rtvHandle, dsvHandle);
			cmdList.ClearDepthStencilView(dsvHandle, ClearFlags.FlagsDepth, 1f, 0);



			#region Rendering

			SharpDX.ViewportF viewport = new SharpDX.ViewportF(0f, 0f, data.DisplaySize.X, data.DisplaySize.Y, 0f, 1f);
			cmdList.SetViewport(viewport);

			cmdList.SetGraphicsRootSignature(rootSignature);
			cmdList.SetDescriptorHeaps(1, new DescriptorHeap[] { descriptorHeap });
			cmdList.SetGraphicsRootDescriptorTable(0, (descriptorHeap.GPUDescriptorHandleForHeapStart));

			int stride = sizeof(ImDrawVert);
			cmdList.SetVertexBuffer(0, vertexBufferView);
			cmdList.SetIndexBuffer(indexBufferView);
			cmdList.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;

			// Render command lists
			// (Because we merged all buffers into a single one, we maintain our own offset into them)
			int global_idx_offset = 0;
			int global_vtx_offset = 0;

			int descHeapInc = Graphics.device.GetDescriptorHandleIncrementSize(DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);

			for (int n = 0; n < data.CmdListsCount; n++) {
				ImDrawListPtr imDrawList = data.CmdListsRange[n];
				for (int i = 0; i < imDrawList.CmdBuffer.Size; i++) {
					ImDrawCmdPtr cmd = imDrawList.CmdBuffer[i];
					if (cmd.UserCallback != IntPtr.Zero) {
						throw new NotImplementedException("user callbacks not implemented");
					}
					else {

						cmdList.SetScissorRectangles(new SharpDX.Mathematics.Interop.RawRectangle((int)cmd.ClipRect.X, (int)cmd.ClipRect.Y, (int)cmd.ClipRect.Z, (int)cmd.ClipRect.W));

						if (textureResources.TryGetValue(cmd.GetTexID(), out var texture)) {
							cmdList.SetGraphicsRootDescriptorTable(1, descriptorHeap.GPUDescriptorHandleForHeapStart + descHeapInc * texture.Item2);
						}
						cmdList.DrawIndexedInstanced((int)cmd.ElemCount, 1, (int)(cmd.IdxOffset + global_idx_offset), (int)(cmd.VtxOffset + global_vtx_offset), 1);

					}
				}
				global_idx_offset += imDrawList.IdxBuffer.Size;
				global_vtx_offset += imDrawList.VtxBuffer.Size;
			}

			#endregion


			ReplaceFontIfRequired();

			cmdList.ResourceBarrierTransition(renderTarget, ResourceStates.RenderTarget, ResourceStates.Present);


			cmdList.Close();

			Graphics.SubmitGraphicsCommandList(cmdList);
			Graphics.ExecuteCommandLists();

			ImGuiInput.ReSetLastCursor();

		}



		
		internal static void Resize(int width, int height) {
			ImGui.GetIO().DisplaySize = new System.Numerics.Vector2(width, height);
		}


		#region Textures

		internal static IntPtr CreateImageTexture(Image<Rgba32> image, SharpDX.DXGI.Format format) {

			Texture texture = new Texture(image.Width, image.Height);
			texture.AddToDescriptorHeap(descriptorHeap, descriptorHeapIndex);

			if (!image.DangerousTryGetSinglePixelMemory(out Memory<Rgba32> memory)) {
				throw new Exception("Make sure to initialize MemoryAllocator.Default!");
			}
			
			Rgba32[] pixelArray = memory.ToArray();
			byte[] imageData = new byte[pixelArray.Length * 4];
			for (int i = 0; i < pixelArray.Length; i ++) {
				imageData[i * 4 + 0] = pixelArray[i].R;
				imageData[i * 4 + 1] = pixelArray[i].G;
				imageData[i * 4 + 2] = pixelArray[i].B;
				imageData[i * 4 + 3] = pixelArray[i].A;
			}

			
			texture.SetData(imageData);

			return RegisterTexture(texture);
			
		}

		internal static bool RemoveImageTexture(IntPtr handle) {
			var tex = GPU_RenderImGui.DeRegisterTexture(handle);
			return tex != null;
		}

		internal static void UpdateFontTexture(string fontPathName, float fontSize, ushort[]? fontCustomGlyphRange, FontGlyphRangeType fontLanguage) {
			var io = ImGui.GetIO();
			DeRegisterTexture(io.Fonts.TexID)?.Dispose();
			io.Fonts.Clear();
			var config = ImGuiNative.ImFontConfig_ImFontConfig();
			if (fontCustomGlyphRange == null) {
				switch (fontLanguage) {
					case FontGlyphRangeType.English:
						io.Fonts.AddFontFromFileTTF(fontPathName, fontSize, config, io.Fonts.GetGlyphRangesDefault());
						break;
					case FontGlyphRangeType.ChineseSimplifiedCommon:
						io.Fonts.AddFontFromFileTTF(fontPathName, fontSize, config, io.Fonts.GetGlyphRangesChineseSimplifiedCommon());
						break;
					case FontGlyphRangeType.ChineseFull:
						io.Fonts.AddFontFromFileTTF(fontPathName, fontSize, config, io.Fonts.GetGlyphRangesChineseFull());
						break;
					case FontGlyphRangeType.Japanese:
						io.Fonts.AddFontFromFileTTF(fontPathName, fontSize, config, io.Fonts.GetGlyphRangesJapanese());
						break;
					case FontGlyphRangeType.Korean:
						io.Fonts.AddFontFromFileTTF(fontPathName, fontSize, config, io.Fonts.GetGlyphRangesKorean());
						break;
					case FontGlyphRangeType.Thai:
						io.Fonts.AddFontFromFileTTF(fontPathName, fontSize, config, io.Fonts.GetGlyphRangesThai());
						break;
					case FontGlyphRangeType.Vietnamese:
						io.Fonts.AddFontFromFileTTF(fontPathName, fontSize, config, io.Fonts.GetGlyphRangesVietnamese());
						break;
					case FontGlyphRangeType.Cyrillic:
						io.Fonts.AddFontFromFileTTF(fontPathName, fontSize, config, io.Fonts.GetGlyphRangesCyrillic());
						break;
					default:
						throw new Exception($"Font Glyph Range (${fontLanguage}) is not supported.");
				}
			}
			else
			{
				fixed (ushort* p = &fontCustomGlyphRange[0])
				{
					io.Fonts.AddFontFromFileTTF(fontPathName, fontSize, config, new IntPtr(p));
				}
			}

			CreateFontsTexture();
			ImGuiNative.ImFontConfig_destroy(config);
		}

		static void CreateFontsTexture() {
			var io = ImGui.GetIO();

			io.Fonts.GetTexDataAsRGBA32(out byte* pixels, out var width, out var height);

			byte[] pixelArray = new byte[width * height * 4];
			for (int i = 0; i < pixelArray.Length; i ++) {
				pixelArray[i] = pixels[i];
			}
			Texture fontTex = new Texture(width, height);
			fontTex.AddToDescriptorHeap(descriptorHeap, descriptorHeapIndex);

			fontTex.SetData(pixelArray);

			io.Fonts.SetTexID(RegisterTexture(fontTex));
			io.Fonts.ClearTexData();

		}

		static IntPtr RegisterTexture(Texture texture) {
			IntPtr imguiID = texture.GetNativePointer();
			textureResources.TryAdd(imguiID, (texture, descriptorHeapIndex));
			descriptorHeapIndex++;
			return imguiID;
		}

		static Texture? DeRegisterTexture(IntPtr texturePtr) {
			if (textureResources.Remove(texturePtr, out var texture)) {
				return texture.Item1;
			}
			else {
				return null;
			}
		}

		static void DeRegisterAllTexture() {
			foreach (var key in textureResources.Keys.ToArray()) {
				DeRegisterTexture(key)?.Dispose();
			}
		}

		#endregion
		#region Fonts

		/// <summary>
		/// Replaces the ImGui font with another one.
		/// </summary>
		/// <param name="pathName">pathname to the TTF font file.</param>
		/// <param name="size">font size to load.</param>
		/// <param name="language">supported language by the font.</param>
		/// <returns>true if the font replacement is valid otherwise false.</returns>
		public static bool ReplaceFont(string pathName, int size, FontGlyphRangeType language) {
			if (!File.Exists(pathName)) {
				return false;
			}

			fontPathName = pathName;
			fontSize = size;
			fontLanguage = language;
			replaceFont = true;
			fontCustomGlyphRange = null;
			return true;
		}

		/// <summary>
		/// Replaces the ImGui font with another one.
		/// </summary>
		/// <param name="pathName">pathname to the TTF font file.</param>
		/// <param name="size">font size to load.</param>
		/// <param name="glyphRange">custom glyph range of the font to load. Read <see cref="FontGlyphRangeType"/> for more detail.</param>
		/// <returns>>true if the font replacement is valid otherwise false.</returns>
		public static bool ReplaceFont(string pathName, int size, ushort[] glyphRange) {
			if (!File.Exists(pathName)) {
				return false;
			}

			fontPathName = pathName;
			fontSize = size;
			fontCustomGlyphRange = glyphRange;
			replaceFont = true;
			return true;
		}

		internal static void ReplaceFontIfRequired() {
			if (replaceFont == true) {
				GPU_RenderImGui.UpdateFontTexture(fontPathName, fontSize, fontCustomGlyphRange, fontLanguage);
				replaceFont = false;
			}
		}

		/// <summary>
		/// Adds the image to the Graphic Device as a texture.
		/// Then returns the pointer of the added texture. It also
		/// cache the image internally rather than creating a new texture on every call,
		/// so this function can be called multiple times per frame.
		/// </summary>
		/// <param name="filePath">Path to the image on disk.</param>
		/// <param name="srgb"> a value indicating whether pixel format is srgb or not.</param>
		/// <param name="handle">output pointer to the image in the graphic device.</param>
		/// <param name="width">width of the loaded texture.</param>
		/// <param name="height">height of the loaded texture.</param>
		public static void AddOrGetImagePointer(string filePath, bool srgb, out IntPtr handle, out uint width, out uint height) {
			if (loadedTexturesPtrs.TryGetValue(filePath, out var data)) {
				handle = data.Handle;
				width = data.Width;
				height = data.Height;
			}
			else {
				var configuration = Configuration.Default.Clone();
				configuration.PreferContiguousImageBuffers = true;
				using var image = Image.Load<Rgba32>(configuration, filePath);
				handle = GPU_RenderImGui.CreateImageTexture(image, srgb ? SharpDX.DXGI.Format.R8G8B8A8_UNorm_SRgb : SharpDX.DXGI.Format.R8G8B8A8_UNorm);
				width = (uint)image.Width;
				height = (uint)image.Height;
				loadedTexturesPtrs.Add(filePath, new(handle, width, height));
			}
		}

		/// <summary>
		/// Adds the image to the Graphic Device as a texture.
		/// Then returns the pointer of the added texture. It also
		/// cache the image internally rather than creating a new texture on every call,
		/// so this function can be called multiple times per frame.
		/// </summary>
		/// <param name="name">user friendly name given to the image.</param>
		/// <param name="image">Image data in <see cref="Image"> format.</param>
		/// <param name="srgb"> a value indicating whether pixel format is srgb or not.</param>
		/// <param name="handle">output pointer to the image in the graphic device.</param>
		public static void AddOrGetImagePointer(string name, Image<Rgba32> image, bool srgb, out IntPtr handle) {
			if (loadedTexturesPtrs.TryGetValue(name, out var data)) {
				handle = data.Handle;
			}
			else {
				handle = GPU_RenderImGui.CreateImageTexture(image, srgb ? SharpDX.DXGI.Format.R8G8B8A8_UNorm_SRgb : SharpDX.DXGI.Format.R8G8B8A8_UNorm);
				loadedTexturesPtrs.Add(name, new(handle, (uint)image.Width, (uint)image.Height));
			}
		}

		/// <summary>
		/// Removes the image from the Overlay.
		/// </summary>
		/// <param name="key">name or pathname which was used to add the image in the first place.</param>
		/// <returns> true if the image is removed otherwise false.</returns>
		public static bool RemoveImage(string key) {
			if (loadedTexturesPtrs.Remove(key, out var data)) {
				return GPU_RenderImGui.RemoveImageTexture(data.Handle);
			}

			return false;
		}

		#endregion

		static void CreateDeviceObjects() {

			#region Root signature

			// Basically what constants are you going to pass to the shaders
			// Create a root signature with one root argument
			RootParameter[] rootParameters = new RootParameter[] {

				new RootParameter(ShaderVisibility.All, new DescriptorRange() {
					RangeType = DescriptorRangeType.ConstantBufferView,
					BaseShaderRegister = 0,
					OffsetInDescriptorsFromTableStart = int.MinValue,
					DescriptorCount = 1
				}),
				new RootParameter(ShaderVisibility.Pixel, new DescriptorRange() {
					RangeType = DescriptorRangeType.ShaderResourceView,
					BaseShaderRegister = 0,
					OffsetInDescriptorsFromTableStart = int.MinValue,
					DescriptorCount = 1,
				}),

			};

			StaticSamplerDescription[] staticSamplerDescription = new StaticSamplerDescription[] {
				new StaticSamplerDescription(ShaderVisibility.Pixel, 0, 0) {
					Filter = SharpDX.Direct3D12.Filter.MinimumMinMagMipPoint,
					AddressUVW = SharpDX.Direct3D12.TextureAddressMode.Border,
				}
			};


			RootSignatureDescription rootSignatureDesc = new RootSignatureDescription(RootSignatureFlags.AllowInputAssemblerInputLayout, rootParameters, staticSamplerDescription);
			rootSignature = Graphics.device.CreateRootSignature(rootSignatureDesc.Serialize());

			#endregion

			DescriptorHeapDescription dhd = new DescriptorHeapDescription() {
				DescriptorCount = 1 + 2048,
				Flags = DescriptorHeapFlags.ShaderVisible,
				Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView,
			};
			descriptorHeap = Graphics.device.CreateDescriptorHeap(dhd);
			constantBuffer = new ConstBuffer<Matrix>(1);
			constantBuffer.AddToDescriptorHeap(descriptorHeap, 0);

			CreatePipelineState();
			CreateFontsTexture();
			
		}

		static void CreatePipelineState() {

			ShaderBytecode vertexShader = Graphics.CompileShader(".res/Shaders/ImGui_shaders.hlsl", Graphics.ShaderType.Vertex);
			ShaderBytecode pixelShader = Graphics.CompileShader(".res/Shaders/ImGui_shaders.hlsl", Graphics.ShaderType.Pixel);

			// Input format
			InputElement[] inputElementDescs = new InputElement[] {
				new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32_Float, 0, 0),
				new InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, 8, 0),
				new InputElement("COLOR", 0, SharpDX.DXGI.Format.R8G8B8A8_UNorm, 16, 0),
			};

			SharpDX.Direct3D12.DepthStencilOperationDescription defaultStencilOp = new SharpDX.Direct3D12.DepthStencilOperationDescription() {
				FailOperation = SharpDX.Direct3D12.StencilOperation.Keep,
				DepthFailOperation = SharpDX.Direct3D12.StencilOperation.Keep,
				PassOperation = SharpDX.Direct3D12.StencilOperation.Keep,
				Comparison = Comparison.Always
			};
			DepthStencilStateDescription depthState = new DepthStencilStateDescription() {

				IsDepthEnabled = true,
				DepthWriteMask = SharpDX.Direct3D12.DepthWriteMask.All,
				DepthComparison = Comparison.Always,

				IsStencilEnabled = false,
				StencilReadMask = 0xff,
				StencilWriteMask = 0xff,
				FrontFace = defaultStencilOp,
				BackFace = defaultStencilOp,

			};

			RasterizerStateDescription rasterState = RasterizerStateDescription.Default();
			rasterState.CullMode = SharpDX.Direct3D12.CullMode.None;

			BlendStateDescription blendState = BlendStateDescription.Default();
			blendState.RenderTarget[0].IsBlendEnabled = true;
			blendState.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha;
			blendState.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
			blendState.RenderTarget[0].SourceAlphaBlend = BlendOption.One;
			blendState.RenderTarget[0].DestinationAlphaBlend = BlendOption.InverseSourceAlpha;

			GraphicsPipelineStateDescription psonDesc = new GraphicsPipelineStateDescription() {

				InputLayout = new InputLayoutDescription(inputElementDescs),
				RootSignature = rootSignature,
				VertexShader = vertexShader,
				PixelShader = pixelShader,
				RasterizerState = rasterState,
				BlendState = blendState,
				DepthStencilFormat = SharpDX.DXGI.Format.D32_Float,
				DepthStencilState = depthState,
				SampleMask = int.MaxValue,
				PrimitiveTopologyType = PrimitiveTopologyType.Triangle,
				RenderTargetCount = 1,
				Flags = PipelineStateFlags.None,
				SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
				StreamOutput = new StreamOutputDescription()

			};
			psonDesc.RenderTargetFormats[0] = SharpDX.DXGI.Format.R8G8B8A8_UNorm;
			pipelineState = Graphics.device.CreateGraphicsPipelineState(psonDesc);
			
		}

		internal static void Dispose() {

			if (loadedTexturesPtrs != null) {
				foreach (var key in loadedTexturesPtrs.Keys.ToArray()) {
					RemoveImage(key);
				}
			}

			DeRegisterAllTexture();
			indexBuffer?.Dispose();
			vertexBuffer?.Dispose();
			constantBuffer?.Dispose();
		}


#nullable restore

	}

	public enum FontGlyphRangeType {
		/// <summary>
		/// Glyph range enough for english language
		/// </summary>
		English,

		/// <summary>
		/// Glyph range enough for english and chinese simplified common language
		/// </summary>
		ChineseSimplifiedCommon,

		/// <summary>
		/// Glyph range enough for english and full chinese language
		/// </summary>
		ChineseFull,

		/// <summary>
		/// Glyph range enough for english and Japanese language
		/// </summary>
		Japanese,

		/// <summary>
		/// Glyph range enough for english and korean language
		/// </summary>
		Korean,

		/// <summary>
		/// Glyph range enough for english and Thai language
		/// </summary>
		Thai,

		/// <summary>
		/// Glyph range enough for english and Vietnamese language
		/// </summary>
		Vietnamese,

		/// <summary>
		/// Glyph range enough for english and few special chars.
		/// </summary>
		Cyrillic,
	}



}

#pragma warning restore CS8618