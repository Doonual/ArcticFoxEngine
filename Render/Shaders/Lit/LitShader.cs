using ArcticFoxEngine.Nodes;
using CoolClassLibrary;
using ImGuiNET;
using SharpDX.Direct3D12;
using SharpDX.DXGI;

namespace ArcticFoxEngine.Render {
	
	public class LitShader : Shader {

		

		public struct LightingWorld {

			public Vector3 sunDir;
			public float sunStrength;
			public float ambientLight;

			public LightingWorld() {
				sunDir = new Vector3(-0.25f, -0.5f, 0.4f).Normalize();
				sunStrength = 0.8f;
				ambientLight = 0.2f;
			}

		}
		public struct LightData {

			public Vector4 pos;
			public Vector3 col;
			public float strength;

		}

		public static ConstBuffer<LightingWorld> lightingInfoBuffer;
		public static StructuredBuffer<LightData> lightBuffer;

		public override string name => "Lit";

		public DataSlot projectionInfoDataSlot = new DataSlot(ShaderVisibility.All);
		public DataSlot transformInfoDataSlot = new DataSlot(ShaderVisibility.All);

		public DataSlot lightingWorldSlot = new DataSlot(ShaderVisibility.Pixel);
		public DataSlot materialInfoSlot = new DataSlot(ShaderVisibility.Pixel);

		public TextureSlot mainTexSlot = new TextureSlot(ShaderVisibility.Pixel);
		public TextureSlot normalTexSlot = new TextureSlot(ShaderVisibility.Pixel);

		public BufferSlot lightInfoSlot = new BufferSlot(16, ShaderVisibility.Pixel);

		public TextureSampler textureSampler = new TextureSampler(ShaderVisibility.Pixel) {
			addressUVW = TextureAddressMode.Wrap,
			filter = Filter.MinimumMinMagMipPoint,
		};

		public LitShader() {

			lightBuffer = new StructuredBuffer<LightData>(16);
			lightingInfoBuffer = new ConstBuffer<LightingWorld>(1);


			rootSignature = CreateRootSignature(
				new DataSlot[] { projectionInfoDataSlot, transformInfoDataSlot, lightingWorldSlot, materialInfoSlot },
				new BufferSlot[] { lightInfoSlot },
				new TextureSlot[] { mainTexSlot, normalTexSlot },
				new TextureSampler[] { textureSampler }
			);
			pipelineState = CreatePipelineObject();

		}

		private PipelineState CreatePipelineObject() {

			InputElement[] inputLayout = new InputElement[] {
				new InputElement("SV_Position", 0, SharpDX.DXGI.Format.R32G32B32_Float, 0, 0), // 12 bytes
				new InputElement("COLOR", 0, SharpDX.DXGI.Format.R32G32B32A32_Float, 12, 0), // 16 bytes
				new InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, 28, 0), // 8 bytes
				new InputElement("NORMAL", 0, SharpDX.DXGI.Format.R32G32B32A32_Float, 36, 0), // 16 bytes
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
			ShaderBytecode pixelShader = CompileShader(".res/Shaders/Lit/LitPixelShader.hlsl", ShaderType.Pixel);



			GraphicsPipelineStateDescription pipelineStateDescription = new GraphicsPipelineStateDescription() {
				InputLayout = inputLayout,
				RootSignature = rootSignature,
				PrimitiveTopologyType = PrimitiveTopologyType.Triangle,
				RasterizerState = rasterStateDescription,
				DepthStencilState = depthStencilDesc,
				DepthStencilFormat = SharpDX.DXGI.Format.D32_Float,
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
			pipelineStateDescription.RenderTargetFormats[0] = SharpDX.DXGI.Format.R8G8B8A8_UNorm;

			return Graphics.device.CreateGraphicsPipelineState(pipelineStateDescription);

		}

		public static void SetLightingInfo(LightingWorld lightingInfo) {
			lightingInfoBuffer.Write(new LightingWorld[] { lightingInfo }, 0);
		}
		public static void SetLightData(LightData lightData, int bufferPos) {
			lightBuffer.Write(new LightData[] { lightData }, bufferPos);
		}

		public override void Render(Camera camera, SharpDX.Direct3D12.Resource renderTarget, DescriptorHeap rtvDescHeap, DescriptorHeap dsvDescHeap) {

			lightingWorldSlot.SetData(lightingInfoBuffer, 0);
			lightInfoSlot.SetBuffer(lightBuffer, 0);

			DefaultRender(camera, renderTarget, rtvDescHeap, dsvDescHeap, projectionInfoDataSlot, transformInfoDataSlot);

		}

		public override Material GetDefaultMaterial() {
			return new LitMaterial();
		}

	}

	

	

}
