#pragma warning disable CS8618

using ArcticFoxEngine.Gui;
using ArcticFoxEngine.Render;
using ImGuiNET;
using SharpDX.Direct3D12;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Buffers;
using Image = SixLabors.ImageSharp.Image;
using ImDrawIdx = System.UInt16;

namespace ArcticFoxEngine.ImGuiIntegration {

	unsafe internal static class RenderImGui {

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

		static ConstantBuffer<Matrix> constantBuffer;
		static GraphicsCommandList cmdList;

		public static Texture renderTexture;
		static DescriptorHeap rtvDescHeap;
		public static Texture depthTexture;
		static DescriptorHeap dsvDescHeap;


		static readonly Dictionary<IntPtr, (Texture, int)> textureResources = new();
		private static List<Texture> textureRegisterQueue;
		static RenderImGui() {
			textureRegisterQueue = new List<Texture>();
		}


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
			CreatePipelineState();
			CreateFontsTexture();

			cmdList = Graphics.CreateDirectCommandList();

			renderTexture = new Texture(MainWindow.width, MainWindow.height, flags: ResourceFlags.AllowRenderTarget | ResourceFlags.AllowUnorderedAccess);
			renderTexture.name = "ImGui Render Texture";
			DescriptorHeapDescription rtvHeapDescription = new DescriptorHeapDescription() {
				DescriptorCount = 1,
				Flags = DescriptorHeapFlags.None,
				Type = DescriptorHeapType.RenderTargetView,
			};
			rtvDescHeap = Graphics.device.CreateDescriptorHeap(rtvHeapDescription);
			Graphics.device.CreateRenderTargetView(renderTexture.resource, null, rtvDescHeap.CPUDescriptorHandleForHeapStart);

			depthTexture = new Texture(MainWindow.width, MainWindow.height, format: Format.D32_Float, flags: ResourceFlags.AllowDepthStencil);
			depthTexture.name = "ImGui Depth Texture";
			DescriptorHeapDescription dsvHeapDescription = new DescriptorHeapDescription() {
				DescriptorCount = 1,
				Flags = DescriptorHeapFlags.None,
				Type = DescriptorHeapType.DepthStencilView,
			};
			dsvDescHeap = Graphics.device.CreateDescriptorHeap(dsvHeapDescription);
			Graphics.device.CreateDepthStencilView(depthTexture.resource, null, dsvDescHeap.CPUDescriptorHandleForHeapStart);
			
			for (int i = 0; i < textureRegisterQueue.Count; i ++) {
				textureRegisterQueue[i].imGuiID = RegisterTexture(textureRegisterQueue[i]);
			}
			textureRegisterQueue.Clear();

		}

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
					Filter = Filter.MinMagMipPoint,
					AddressUVW = TextureAddressMode.Wrap,
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
			constantBuffer = new ConstantBuffer<Matrix>(1);

			Graphics.device.CopyDescriptorsSimple(constantBuffer.numElements, descriptorHeap.CPUDescriptorHandleForHeapStart, constantBuffer.GetCBVDescriptorLocation(), DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);

		}

		static void CreatePipelineState() {

			ShaderBytecode vertexShader = Shader.CompileShader(".res/Shaders/ImGui/ImGui_shaders.hlsl", Shader.ShaderType.Vertex);
			ShaderBytecode pixelShader = Shader.CompileShader(".res/Shaders/ImGui/ImGui_shaders.hlsl", Shader.ShaderType.Pixel);

			// Input format
			InputElement[] inputElementDescs = new InputElement[] {
				new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32_Float, 0, 0),
				new InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, 8, 0),
				new InputElement("COLOR", 0, SharpDX.DXGI.Format.R8G8B8A8_UNorm, 16, 0),
			};

			DepthStencilOperationDescription defaultStencilOp = new DepthStencilOperationDescription() {
				FailOperation = StencilOperation.Keep,
				DepthFailOperation = StencilOperation.Keep,
				PassOperation = StencilOperation.Keep,
				Comparison = Comparison.Always
			};
			DepthStencilStateDescription depthState = new DepthStencilStateDescription() {

				IsDepthEnabled = true,
				DepthWriteMask = DepthWriteMask.All,
				DepthComparison = Comparison.Always,

				IsStencilEnabled = false,
				StencilReadMask = 0xff,
				StencilWriteMask = 0xff,
				FrontFace = defaultStencilOp,
				BackFace = defaultStencilOp,

			};

			RasterizerStateDescription rasterState = RasterizerStateDescription.Default();
			rasterState.CullMode = CullMode.None;

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


		internal static void Update(float deltaTime, Action DoRender) {
			var io = ImGui.GetIO();
			io.DeltaTime = deltaTime;
			ImGui.NewFrame();
			DoRender?.Invoke();
			ImGui.Render();
		}
		private static ImDrawDataPtr? UpdateImGuiDrawList() {

			
			ImGuiInput.Update();
			Update(Profiler.deltaTime, GuiManager.Render);

			

			ImDrawDataPtr data = ImGui.GetDrawData();
			// Avoid rendering when minimized
			if (data.DisplaySize.x <= 0.0f || data.DisplaySize.y <= 0.0f) { return null; }
			

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
			float L = data.DisplayPos.x;
			float R = data.DisplayPos.x + data.DisplaySize.x;
			float T = data.DisplayPos.y;
			float B = data.DisplayPos.y + data.DisplaySize.y;
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

		internal static void Render() {

			

			ImDrawDataPtr? dataNull = UpdateImGuiDrawList();
			if (dataNull == null) { return; }

			Graphics.WaitForDirectCommandQueue();
			Graphics.ResetDirectCommandList(cmdList);
			cmdList.PipelineState = pipelineState;

			
			ImDrawDataPtr data = (ImDrawDataPtr)dataNull;


			// Indicate that the back buffer will be used as a render target
			cmdList.ResourceBarrierTransition(renderTexture.resource, Texture.defaultState, ResourceStates.RenderTarget);

			// Set render target and depth stencil
			CpuDescriptorHandle rtvHandle = rtvDescHeap.CPUDescriptorHandleForHeapStart;
			CpuDescriptorHandle dsvHandle = dsvDescHeap.CPUDescriptorHandleForHeapStart;

			cmdList.SetRenderTargets(rtvHandle, dsvHandle);
			cmdList.ClearRenderTargetView(rtvHandle, new SharpDX.Mathematics.Interop.RawColor4(0f, 0f, 0f, 0f));
			cmdList.ClearDepthStencilView(dsvHandle, ClearFlags.FlagsDepth, 1f, 0);

			#region Rendering

			SharpDX.ViewportF viewport = new SharpDX.ViewportF(0f, 0f, data.DisplaySize.x, data.DisplaySize.y, 0f, 1f);
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


						cmdList.SetScissorRectangles(new SharpDX.Mathematics.Interop.RawRectangle((int)cmd.ClipRect.x, (int)cmd.ClipRect.y, (int)cmd.ClipRect.z, (int)cmd.ClipRect.w));

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

			cmdList.ResourceBarrierTransition(renderTexture.resource, ResourceStates.RenderTarget, Texture.defaultState);


			cmdList.Close();

			Graphics.ExecuteDirectCommandList(cmdList);
			Graphics.WaitForDirectCommandQueue();
			Graphics.AlphaBlendTextures(Graphics.mainTexture, renderTexture, Graphics.mainTexture);
			Graphics.WaitForComputeCommandQueue();
			ImGuiInput.ReSetLastCursor();

		}

		internal static void Resize(int width, int height) {
			ImGui.GetIO().DisplaySize = new Vector2(width, height);
		}


		#region Textures

		internal static IntPtr CreateImageTexture(Image<Rgba32> image, SharpDX.DXGI.Format format, string name) {

			Texture texture = new Texture(image.Width, image.Height);
			texture.name = name;

			if (!image.DangerousTryGetSinglePixelMemory(out Memory<Rgba32> memory)) {
				throw new Exception("Make sure to initialize MemoryAllocator.Default!");
			}

			Rgba32[] pixelArray = memory.ToArray();
			byte[] imageData = new byte[pixelArray.Length * 4];
			for (int i = 0; i < pixelArray.Length; i++) {
				imageData[i * 4 + 0] = pixelArray[i].R;
				imageData[i * 4 + 1] = pixelArray[i].G;
				imageData[i * 4 + 2] = pixelArray[i].B;
				imageData[i * 4 + 3] = pixelArray[i].A;
			}


			texture.SetData(imageData);

			return texture.imGuiID;

		}
		internal static bool RemoveImageTexture(IntPtr handle) {
			var tex = RenderImGui.DeRegisterTexture(handle);
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
			else {
				fixed (ushort* p = &fontCustomGlyphRange[0]) {
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
			for (int i = 0; i < pixelArray.Length; i++) {
				pixelArray[i] = pixels[i];
			}
			Texture fontTex = new Texture(width, height);
			fontTex.name = "ImGui Font Texture";
			Graphics.device.CreateShaderResourceView(fontTex.resource, null, descriptorHeap.CPUDescriptorHandleForHeapStart + Graphics.descriptorHeapIncrement * descriptorHeapIndex);

			//fontTex.PrepareAsShaderResource(descriptorHeap, descriptorHeapIndex);

			fontTex.SetData(pixelArray);

			io.Fonts.SetTexID(fontTex.imGuiID);
			io.Fonts.ClearTexData();

		}
		public static IntPtr RegisterTexture(Texture texture) {
			
			if (descriptorHeap == null) {
				textureRegisterQueue.Add(texture);
				return IntPtr.Zero;
			}

			IntPtr imguiID = texture.GetNativePointer();
			texture.CreateExternalShaderResourceView(descriptorHeap.CPUDescriptorHandleForHeapStart + Graphics.descriptorHeapIncrement * descriptorHeapIndex);
			//Graphics.device.CreateShaderResourceView(texture.resource, null, descriptorHeap.CPUDescriptorHandleForHeapStart + Rendering.Rendering.descriptorHeapIncrement * descriptorHeapIndex);
			//texture.PrepareAsShaderResource(descriptorHeap, descriptorHeapIndex);
			textureResources.TryAdd(imguiID, (texture, descriptorHeapIndex));
			descriptorHeapIndex++;
			return imguiID;
		}
		public static Texture? DeRegisterTexture(IntPtr texturePtr) {
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
				RenderImGui.UpdateFontTexture(fontPathName, fontSize, fontCustomGlyphRange, fontLanguage);
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
				handle = RenderImGui.CreateImageTexture(image, srgb ? SharpDX.DXGI.Format.R8G8B8A8_UNorm_SRgb : SharpDX.DXGI.Format.R8G8B8A8_UNorm, filePath);
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
				handle = RenderImGui.CreateImageTexture(image, srgb ? SharpDX.DXGI.Format.R8G8B8A8_UNorm_SRgb : SharpDX.DXGI.Format.R8G8B8A8_UNorm, name);
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
				return RenderImGui.RemoveImageTexture(data.Handle);
			}

			return false;
		}

		#endregion

		
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