using ImGuiNET;
using SharpDX.Direct3D12;
using SharpDX.DXGI;

namespace ArcticFoxEngine.Rendering {
	public class UnlitRenderPipeline : Shader {

		public override string name => "Unlit";

		public TextureSlot mainTexSlot = new TextureSlot(ShaderVisibility.Pixel);
		public TextureSampler sampler = new TextureSampler(ShaderVisibility.Pixel) {
			addressUVW = TextureAddressMode.Wrap,
			filter = Filter.MinimumMinMagMipPoint,
		};

		public UnlitRenderPipeline() {


			rootSignature = CreateRootSignature(
				new DataSlot[] { projectionInfoSlot, transformInfoSlot },
				new BufferSlot[] { },
				new TextureSlot[] { mainTexSlot },
				new TextureSampler[] { sampler }
			);
			pipelineState = CreatePipelineObject();

		}

		private PipelineState CreatePipelineObject() {

			InputElement[] inputLayout = new InputElement[] {
				new InputElement("SV_Position", 0, Format.R32G32B32_Float, 0, 0), // 12 bytes
				new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 12, 0), // 16 bytes
				new InputElement("TEXCOORD", 0, Format.R32G32_Float, 28, 0), // 8 bytes
				new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 36, 0), // 16 bytes
			};

			RasterizerStateDescription rasterStateDescription = RasterizerStateDescription.Default();

			DepthStencilOperationDescription stencilOperationDesc = new DepthStencilOperationDescription() {
				FailOperation = StencilOperation.Keep,
				DepthFailOperation = StencilOperation.Keep,
				PassOperation = StencilOperation.Keep,
				Comparison = Comparison.Always
			};
			DepthStencilStateDescription depthStencilDesc = new DepthStencilStateDescription() {
				IsDepthEnabled = true,
				DepthWriteMask = DepthWriteMask.All,
				DepthComparison = Comparison.Less,
				IsStencilEnabled = false,
				StencilReadMask = 0xff,
				StencilWriteMask = 0xff,
				FrontFace = stencilOperationDesc,
				BackFace = stencilOperationDesc,
			};

			ShaderBytecode vertexShader = CompileShader(".res/Shaders/VertexShader.hlsl", ShaderType.Vertex);
			ShaderBytecode geometryShader = CompileShader(".res/Shaders/GeometryShader.hlsl", ShaderType.Geometry);
			ShaderBytecode pixelShader = CompileShader(".res/Shaders/Unlit/PixelShader.hlsl", ShaderType.Pixel);

			GraphicsPipelineStateDescription pipelineStateDescription = new GraphicsPipelineStateDescription() {
				InputLayout = inputLayout,
				RootSignature = rootSignature,
				PrimitiveTopologyType = PrimitiveTopologyType.Triangle,
				RasterizerState = rasterStateDescription,
				DepthStencilState = depthStencilDesc,
				DepthStencilFormat = Format.D32_Float,
				BlendState = BlendStateDescription.Default(),
				VertexShader = vertexShader,
				GeometryShader = geometryShader,
				PixelShader = pixelShader,
				RenderTargetCount = 1,
				SampleDescription = new SampleDescription(1, 0),
				StreamOutput = new StreamOutputDescription(),
				SampleMask = int.MaxValue,
				Flags = PipelineStateFlags.None,
			};
			pipelineStateDescription.RenderTargetFormats[0] = Format.R8G8B8A8_UNorm;

			return Graphics.device.CreateGraphicsPipelineState(pipelineStateDescription);

		}


		public override Material GetDefaultMaterial() {
			return new UnlitMaterial();
		}

		protected override void BindGlobalResources() {
			


		}

	}

	public class UnlitMaterial : Material {

		public Texture mainTex;
		

		public override void BindResources(Shader shader) {
			UnlitRenderPipeline unlitShader = (UnlitRenderPipeline)shader;


			if (mainTex == null) {
				unlitShader.mainTexSlot.SetTexture(Rendering.textures[0]);
			}
			else {
				unlitShader.mainTexSlot.SetTexture(mainTex);
			}
			

		}

		int setTextureId = 0;
		public override void Debug() {


			if (ImGui.InputInt("Texture ID", ref setTextureId) == true) {
				mainTex = Rendering.textures[setTextureId];
			}

		}

	}

}
