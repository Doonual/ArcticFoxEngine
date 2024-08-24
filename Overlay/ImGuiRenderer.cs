#pragma warning disable CS8618

namespace ArcticFoxEngine {


	using ImGuiNET;
	using ImDrawIdx = System.UInt16;
	using Vortice.DXGI;
	using Vortice.Direct3D;
	using Vortice.Direct3D11;
	using Vortice.D3DCompiler;
	using Vortice.Mathematics;
	using System.Numerics;
	using System.Collections.Generic;
	using System;
	using System.Linq;
	using SixLabors.ImageSharp;
	using SixLabors.ImageSharp.PixelFormats;
	using System.Buffers;
	using SharpDX.Direct3D12;
	using ArcticFoxEngine;
	using ArcticFoxEngine.Backend;
	using CoolClassLibrary;
	using SixLabors.ImageSharp.Formats;

	unsafe internal sealed class ImGuiRenderer : IDisposable {
		
		const int VertexConstantBufferSize = 16 * 4;

		Resource vertexBuffer;
		VertexBufferView vertexBufferView;

		Resource indexBuffer;
		IndexBufferView indexBufferView;

		ConstBuffer<Matrix> constantBuffer;
		DescriptorHeap constantBufferDh;

		internal PipelineState pipelineState;
		RootSignature rootSignature;


		int vertexBufferSize = 5000, indexBufferSize = 10000;
		readonly Dictionary<IntPtr, (Texture, int)> textureResources = new();

		int descriptorHeapIndex;

		public ImGuiRenderer(int width, int height) {
			descriptorHeapIndex = 1;

			ImGui.CreateContext();
			var io = ImGui.GetIO();
			io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;  // We can honor the ImDrawCmd::VtxOffset field, allowing for large meshes.
			io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
			io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
			ImGui.StyleColorsDark();
			this.Resize(width, height);
			this.CreateDeviceObjects();
		}

		public void Start() {
			ImGui.NewFrame();
		}

		public void Update(float deltaTime, Action DoRender) {
			var io = ImGui.GetIO();
			io.DeltaTime = deltaTime;
			ImGui.NewFrame();
			DoRender?.Invoke();
			ImGui.Render();
		}

		public void Render(GraphicsCommandList gCmdList) {
			
			ImDrawDataPtr data = ImGui.GetDrawData();
			// Avoid rendering when minimized
			if (data.DisplaySize.X <= 0.0f || data.DisplaySize.Y <= 0.0f) { return; }

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

			#region Rendering

			SetupRenderState(data, gCmdList);
			// Render command lists
			// (Because we merged all buffers into a single one, we maintain our own offset into them)
			int global_idx_offset = 0;
			int global_vtx_offset = 0;
			for (int n = 0; n < data.CmdListsCount; n++) {
				var cmdList = data.CmdListsRange[n];
				for (int i = 0; i < cmdList.CmdBuffer.Size; i++) {
					var cmd = cmdList.CmdBuffer[i];
					if (cmd.UserCallback != IntPtr.Zero) {
						throw new NotImplementedException("user callbacks not implemented");
					}
					else {

						gCmdList.SetScissorRectangles(new SharpDX.Mathematics.Interop.RawRectangle((int)cmd.ClipRect.X, (int)cmd.ClipRect.Y, (int)cmd.ClipRect.Z, (int)cmd.ClipRect.W));

						if (textureResources.TryGetValue(cmd.GetTexID(), out var texture)) {
							gCmdList.SetGraphicsRootDescriptorTable(1, constantBufferDh.GPUDescriptorHandleForHeapStart + RenderResources.combinedDescriptorHeapIncrement * texture.Item2);
							//ctx.PSSetShaderResource(0, texture);
						}
						gCmdList.DrawIndexedInstanced((int)cmd.ElemCount, 1, (int)(cmd.IdxOffset + global_idx_offset), (int)(cmd.VtxOffset + global_vtx_offset), 1);

					}
				}
				global_idx_offset += cmdList.IdxBuffer.Size;
				global_vtx_offset += cmdList.VtxBuffer.Size;
			}

			#endregion

		}

		public void Dispose() {

			this.DeRegisterAllTexture();
			indexBuffer?.Dispose();
			vertexBuffer?.Dispose();
			constantBuffer?.Dispose();
		}

		public void Resize(int width, int height) {
			ImGui.GetIO().DisplaySize = new System.Numerics.Vector2(width, height);
		}

		void SetupRenderState(ImDrawDataPtr drawData, GraphicsCommandList gCmdList) {

			var viewport = new SharpDX.ViewportF(0f, 0f, drawData.DisplaySize.X, drawData.DisplaySize.Y, 0f, 1f);
			gCmdList.SetViewport(viewport);

			gCmdList.SetGraphicsRootSignature(rootSignature);
			gCmdList.SetDescriptorHeaps(1, new DescriptorHeap[] { constantBufferDh });
			gCmdList.SetGraphicsRootDescriptorTable(0, (constantBufferDh.GPUDescriptorHandleForHeapStart));

			int stride = sizeof(ImDrawVert);
			gCmdList.SetVertexBuffer(0, vertexBufferView);
			gCmdList.SetIndexBuffer(indexBufferView);
			gCmdList.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;

		}

		#region Textures

		public IntPtr CreateImageTexture(Image<Rgba32> image, SharpDX.DXGI.Format format) {

			Texture texture = new Texture(image.Width, image.Height, constantBufferDh, descriptorHeapIndex);

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

		public bool RemoveImageTexture(IntPtr handle) {
			var tex = this.DeRegisterTexture(handle);
			return tex != null;
		}

		public void UpdateFontTexture(string fontPathName, float fontSize, ushort[]? fontCustomGlyphRange, FontGlyphRangeType fontLanguage) {
			var io = ImGui.GetIO();
			this.DeRegisterTexture(io.Fonts.TexID)?.Dispose();
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

			this.CreateFontsTexture();
			ImGuiNative.ImFontConfig_destroy(config);
		}

		void CreateFontsTexture() {
			var io = ImGui.GetIO();

			io.Fonts.GetTexDataAsRGBA32(out byte* pixels, out var width, out var height);

			byte[] pixelArray = new byte[width * height * 4];
			for (int i = 0; i < pixelArray.Length; i ++) {
				pixelArray[i] = pixels[i];
			}

			Log.Info($"Fonts texture (Width: {width}, Height: {height}, Descriptor index: {descriptorHeapIndex})");
			Texture fontTex = new Texture(width, height, constantBufferDh, descriptorHeapIndex);

			fontTex.SetData(pixelArray);

			io.Fonts.SetTexID(RegisterTexture(fontTex));
			io.Fonts.ClearTexData();

		}

		IntPtr RegisterTexture(Texture texture) {
			IntPtr imguiID = texture.GetNativePointer();
			textureResources.TryAdd(imguiID, (texture, descriptorHeapIndex));
			descriptorHeapIndex++;
			return imguiID;
		}

		Texture? DeRegisterTexture(IntPtr texturePtr) {
			if (textureResources.Remove(texturePtr, out var texture)) {
				return texture.Item1;
			}
			else {
				return null;
			}
		}

		void DeRegisterAllTexture() {
			foreach (var key in textureResources.Keys.ToArray()) {
				this.DeRegisterTexture(key)?.Dispose();
			}
		}

		#endregion

		void CreateDeviceObjects() {

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
			constantBufferDh = Graphics.device.CreateDescriptorHeap(dhd);
			constantBuffer = new ConstBuffer<Matrix>(1);
			constantBuffer.AddToDescriptorHeap(constantBufferDh, 0);

			CreatePipelineState();
			CreateFontsTexture();
			
		}

		void CreatePipelineState() {

			ShaderBytecode vertexShader = Graphics.CompileShader(".res/ImGui_shaders.hlsl", Graphics.ShaderType.Vertex);
			ShaderBytecode pixelShader = Graphics.CompileShader(".res/ImGui_shaders.hlsl", Graphics.ShaderType.Pixel);

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

	}

}

#pragma warning restore CS8618