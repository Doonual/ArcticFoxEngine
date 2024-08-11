#pragma warning disable CS8618

namespace ClickableTransparentOverlay {


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

	unsafe internal sealed class ImGuiRenderer : IDisposable
	{
		const int VertexConstantBufferSize = 16 * 4;

		Resource vertexBuffer;
		VertexBufferView vertexBufferView;

		Resource indexBuffer;
		IndexBufferView indexBufferView;

		ConstBuffer<Matrix> constantBuffer;
		DescriptorHeap constantBufferDh;


		ID3D11Device device;
		ID3D11DeviceContext deviceContext;
		//ID3D11Buffer vertexBuffer;
		//ID3D11Buffer indexBuffer;
		Blob vertexShaderBlob;
		ID3D11VertexShader vertexShader;
		ID3D11InputLayout inputLayout;
		Blob pixelShaderBlob;
		ID3D11PixelShader pixelShader;
		ID3D11SamplerState fontSampler;
		ID3D11RasterizerState rasterizerState;
		ID3D11BlendState blendState;
		ID3D11DepthStencilState depthStencilState;
		int vertexBufferSize = 5000, indexBufferSize = 10000;
		readonly Dictionary<IntPtr, ID3D11ShaderResourceView> textureResources = new();

		public ImGuiRenderer(ID3D11Device device, ID3D11DeviceContext deviceContext, int width, int height)
		{
			this.device = device;
			this.deviceContext = deviceContext;

			device.AddRef();
			deviceContext.AddRef();

			ImGui.CreateContext();
			var io = ImGui.GetIO();
			io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;  // We can honor the ImDrawCmd::VtxOffset field, allowing for large meshes.
			io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
			io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
			ImGui.StyleColorsDark();
			this.Resize(width, height);
			this.CreateDeviceObjects();
		}

		public void Start()
		{
			ImGui.NewFrame();
		}

		public void Update(float deltaTime, Action DoRender)
		{
			var io = ImGui.GetIO();
			io.DeltaTime = deltaTime;
			ImGui.NewFrame();
			DoRender?.Invoke();
			ImGui.Render();
		}

		public void Render(GraphicsCommandList gCmdList) {
			ImDrawDataPtr data = ImGui.GetDrawData();
			// Avoid rendering when minimized
			if (data.DisplaySize.X <= 0.0f || data.DisplaySize.Y <= 0.0f)
				return;

			ID3D11DeviceContext ctx = deviceContext;

			if (vertexBuffer == null || vertexBufferSize < data.TotalVtxCount)
			{
				vertexBuffer?.Dispose();

				vertexBufferSize = data.TotalVtxCount + 5000;
				vertexBuffer = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(vertexBufferSize * sizeof(ImDrawVert)), ResourceStates.GenericRead);
				vertexBufferView.BufferLocation = vertexBuffer.GPUVirtualAddress;
				vertexBufferView.StrideInBytes = sizeof(ImDrawVert);
				vertexBufferView.SizeInBytes = vertexBufferSize * sizeof(ImDrawVert);
			}

			if (indexBuffer == null || indexBufferSize < data.TotalIdxCount)
			{
				indexBuffer?.Dispose();

				indexBufferSize = data.TotalIdxCount + 10000;

				indexBuffer = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(indexBufferSize * sizeof(ImDrawIdx)), ResourceStates.GenericRead);
				indexBufferView.BufferLocation = indexBuffer.GPUVirtualAddress;
				indexBufferView.SizeInBytes = indexBufferSize * sizeof(ImDrawIdx);
				indexBufferView.Format = SharpDX.DXGI.Format.R32_UInt;

			}

			// Upload vertex/index data into a single contiguous GPU buffer
			ImDrawVert* vertexResourcePointer = (ImDrawVert*)vertexBuffer.Map(0);
			ImDrawIdx* indexResourcePointer = (ImDrawIdx*)indexBuffer.Map(0);
			for (int n = 0; n < data.CmdListsCount; n++) {
				var cmdlList = data.CmdListsRange[n];

				var vertBytes = cmdlList.VtxBuffer.Size * sizeof(ImDrawVert);
				Buffer.MemoryCopy((void*)cmdlList.VtxBuffer.Data, vertexResourcePointer, vertBytes, vertBytes);

				var idxBytes = cmdlList.IdxBuffer.Size * sizeof(ImDrawIdx);
				Buffer.MemoryCopy((void*)cmdlList.IdxBuffer.Data, indexResourcePointer, idxBytes, idxBytes);

				vertexResourcePointer += cmdlList.VtxBuffer.Size;
				indexResourcePointer += cmdlList.IdxBuffer.Size;
			}
			vertexBuffer.Unmap(0);
			indexBuffer.Unmap(0);

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


			SetupRenderState(data, gCmdList);
			// Render command lists
			// (Because we merged all buffers into a single one, we maintain our own offset into them)
			int global_idx_offset = 0;
			int global_vtx_offset = 0;
			for (int n = 0; n < data.CmdListsCount; n++)
			{
				var cmdList = data.CmdListsRange[n];
				for (int i = 0; i < cmdList.CmdBuffer.Size; i++)
				{
					var cmd = cmdList.CmdBuffer[i];
					if (cmd.UserCallback != IntPtr.Zero)
					{
						throw new NotImplementedException("user callbacks not implemented");
					}
					else
					{
						gCmdList.SetScissorRectangles(new SharpDX.Mathematics.Interop.RawRectangle((int)cmd.ClipRect.X, (int)cmd.ClipRect.Y, (int)cmd.ClipRect.Z, (int)cmd.ClipRect.W));

						if (textureResources.TryGetValue(cmd.GetTexID(), out var texture))
						{
							//ctx.PSSetShaderResource(0, texture);
						}
						//else {
							gCmdList.DrawIndexedInstanced((int)cmd.ElemCount, 1, (int)(cmd.IdxOffset + global_idx_offset), (int)(cmd.VtxOffset + global_vtx_offset), 1);
						//}

						
					}
				}
				global_idx_offset += cmdList.IdxBuffer.Size;
				global_vtx_offset += cmdList.VtxBuffer.Size;
			}

			//RestoreDX11State(ctx); // only required if imgui is injected + drawn on existing process.
		}

		public void Dispose()
		{
			if (device == null)
				return;

			this.DeRegisterAllTexture();
			fontSampler?.Release();
			indexBuffer?.Dispose();
			vertexBuffer?.Dispose();
			blendState?.Release();
			depthStencilState?.Release();
			rasterizerState?.Release();
			pixelShader?.Release();
			pixelShaderBlob?.Release();
			constantBuffer?.Dispose();
			inputLayout?.Release();
			vertexShader?.Release();
			vertexShaderBlob?.Release();
		}

		public void Resize(int width, int height)
		{
			ImGui.GetIO().DisplaySize = new System.Numerics.Vector2(width, height);
		}

		public IntPtr CreateImageTexture(Image<Rgba32> image, Format format)
		{
			var texDesc = new Texture2DDescription(format, image.Width, image.Height, 1, 1);
			if (!image.DangerousTryGetSinglePixelMemory(out Memory<Rgba32> memory))
			{
				throw new Exception("Make sure to initialize MemoryAllocator.Default!");
			}

			using MemoryHandle imageMemoryHandle = memory.Pin();
			var subResource = new SubresourceData(imageMemoryHandle.Pointer, texDesc.Width * 4);
			using var texture = device.CreateTexture2D(texDesc, new[] { subResource });
			var resViewDesc = new Vortice.Direct3D11.ShaderResourceViewDescription(texture, Vortice.Direct3D.ShaderResourceViewDimension.Texture2D, format, 0, texDesc.MipLevels);
			return RegisterTexture(device.CreateShaderResourceView(texture, resViewDesc));
		}

		public bool RemoveImageTexture(IntPtr handle)
		{
			using var tex = this.DeRegisterTexture(handle);
			return tex != null;
		}

		public void UpdateFontTexture(string fontPathName, float fontSize, ushort[]? fontCustomGlyphRange, FontGlyphRangeType fontLanguage)
		{
			var io = ImGui.GetIO();
			this.DeRegisterTexture(io.Fonts.TexID)?.Dispose();
			io.Fonts.Clear();
			var config = ImGuiNative.ImFontConfig_ImFontConfig();
			if (fontCustomGlyphRange == null)
			{
				switch (fontLanguage)
				{
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

		void SetupRenderState(ImDrawDataPtr drawData, GraphicsCommandList gCmdList)
		{

			var viewport = new SharpDX.ViewportF(0f, 0f, drawData.DisplaySize.X, drawData.DisplaySize.Y, 0f, 1f);
			gCmdList.SetViewport(viewport);

			int stride = sizeof(ImDrawVert);
			//ctx.IASetInputLayout(inputLayout);
			gCmdList.SetVertexBuffer(0, vertexBufferView);
			gCmdList.SetIndexBuffer(indexBufferView);
			gCmdList.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;

			gCmdList.SetGraphicsRootSignature(RenderResources.rootSignature);
			gCmdList.SetDescriptorHeaps(1, new DescriptorHeap[] { constantBufferDh });
			gCmdList.SetGraphicsRootDescriptorTable(0, (constantBufferDh.GPUDescriptorHandleForHeapStart));

		}

		void CreateFontsTexture()
		{
			var io = ImGui.GetIO();
			io.Fonts.GetTexDataAsRGBA32(out byte* pixels, out var width, out var height);
			var texDesc = new Texture2DDescription(Format.R8G8B8A8_UNorm, width, height, 1, 1);
			var subResource = new SubresourceData(pixels, texDesc.Width * 4);
			using var texture = device.CreateTexture2D(texDesc, new[] { subResource });
			var resViewDesc = new Vortice.Direct3D11.ShaderResourceViewDescription(
				texture,
				Vortice.Direct3D.ShaderResourceViewDimension.Texture2D,
				Format.R8G8B8A8_UNorm,
				0,
				texDesc.MipLevels);
			io.Fonts.SetTexID(RegisterTexture(device.CreateShaderResourceView(texture, resViewDesc)));
			io.Fonts.ClearTexData();
		}

		void CreateFontSampler()
		{
			var samplerDesc = new SamplerDescription(
				Vortice.Direct3D11.Filter.MinMagMipLinear,
				Vortice.Direct3D11.TextureAddressMode.Wrap,
				Vortice.Direct3D11.TextureAddressMode.Wrap,
				Vortice.Direct3D11.TextureAddressMode.Wrap,
				0f,
				0,
				ComparisonFunction.Always,
				0f,
				0f);

			this.fontSampler = device.CreateSamplerState(samplerDesc);
		}

		IntPtr RegisterTexture(ID3D11ShaderResourceView texture)
		{
			var imguiID = texture.NativePointer;
			textureResources.TryAdd(imguiID, texture);
			return imguiID;
		}

		ID3D11ShaderResourceView? DeRegisterTexture(IntPtr texturePtr)
		{
			if (textureResources.Remove(texturePtr, out var texture))
			{
				return texture;
			}
			else
			{
				return null;
			}
		}

		void DeRegisterAllTexture()
		{
			foreach (var key in textureResources.Keys.ToArray())
			{
				this.DeRegisterTexture(key)?.Release();
			}
		}

		void CreateDeviceObjects()
		{
			var vertexShaderCode =
				@"
					cbuffer vertexBuffer : register(b0)
					{
						float4x4 ProjectionMatrix;
					};

					struct VS_INPUT
					{
						float2 pos : POSITION;
						float4 col : COLOR0;
						float2 uv  : TEXCOORD0;
					};

					struct PS_INPUT
					{
						float4 pos : SV_POSITION;
						float4 col : COLOR0;
						float2 uv  : TEXCOORD0;
					};

					PS_INPUT main(VS_INPUT input)
					{
						PS_INPUT output;
						output.pos = mul(ProjectionMatrix, float4(input.pos.xy, 0.f, 1.f));
						output.col = input.col;
						output.uv  = input.uv;
						return output;
					}";
			Compiler.Compile(vertexShaderCode, "main", "vs", "vs_4_0", out vertexShaderBlob, out _);
			if (vertexShaderBlob == null)
				throw new Exception("error compiling vertex shader");

			vertexShader = device.CreateVertexShader(vertexShaderBlob);

			var inputElements = new[]
			{
				new InputElementDescription( "POSITION", 0, Format.R32G32_Float,   0, 0, Vortice.Direct3D11.InputClassification.PerVertexData, 0 ),
				new InputElementDescription( "TEXCOORD", 0, Format.R32G32_Float,   8,  0, Vortice.Direct3D11.InputClassification.PerVertexData, 0 ),
				new InputElementDescription( "COLOR",	0, Format.R8G8B8A8_UNorm, 16, 0, Vortice.Direct3D11.InputClassification.PerVertexData, 0 ),
			};

			inputLayout = device.CreateInputLayout(inputElements, vertexShaderBlob);




			DescriptorHeapDescription dhd = new DescriptorHeapDescription() {
				DescriptorCount = 1,
				Flags = DescriptorHeapFlags.ShaderVisible,
				Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView,
			};
			constantBufferDh = Graphics.device.CreateDescriptorHeap(dhd);
			constantBuffer = new ConstBuffer<Matrix>(1);
			constantBuffer.AddToDescriptorHeap(constantBufferDh, 0);

			var pixelShaderCode =
				@"struct PS_INPUT
					{
						float4 pos : SV_POSITION;
						float4 col : COLOR0;
						float2 uv  : TEXCOORD0;
					};

					sampler sampler0;
					Texture2D texture0;

					float4 main(PS_INPUT input) : SV_Target
					{
						return input.col * texture0.Sample(sampler0, input.uv);
					}";
			Compiler.Compile(pixelShaderCode, "main", "ps", "ps_4_0", out pixelShaderBlob, out _);
			if (pixelShaderBlob == null)
				throw new Exception("error compiling pixel shader");

			pixelShader = device.CreatePixelShader(pixelShaderBlob);

			var blendDesc = new BlendDescription(Blend.SourceAlpha, Blend.InverseSourceAlpha, Blend.One, Blend.InverseSourceAlpha);
			blendState = device.CreateBlendState(blendDesc);

			var rasterDesc = new RasterizerDescription(Vortice.Direct3D11.CullMode.None, Vortice.Direct3D11.FillMode.Solid)
			{
				MultisampleEnable = false,
				ScissorEnable = true
			};
			rasterizerState = device.CreateRasterizerState(rasterDesc);

			var depthDesc = new DepthStencilDescription(false, Vortice.Direct3D11.DepthWriteMask.All, ComparisonFunction.Always);
			depthStencilState = device.CreateDepthStencilState(depthDesc);

			this.CreateFontsTexture();
			this.CreateFontSampler();
		}

	}

}

#pragma warning restore CS8618