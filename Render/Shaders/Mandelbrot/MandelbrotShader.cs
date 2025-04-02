using ArcticFoxEngine.Nodes;
using ImGuiNET;
using SharpDX.Direct3D12;
using SharpDX.DXGI;

namespace ArcticFoxEngine.Render {

	public class MandelbrotShader : Shader {

		public override string name => "Mandelbrot";

		public ConstBuffer<ViewportInfo> viewportInfoBuffer;

		public struct ViewportInfo {

			public Vector2 viewCenter = Vector2.zero;
			public float zoom = 1f;
			public int numIterations = 100;

			public ViewportInfo() {

			}

		};

		public DataSlot projectionInfoDataSlot = new DataSlot(ShaderVisibility.All);
		public DataSlot transformInfoDataSlot = new DataSlot(ShaderVisibility.All);
		public DataSlot viewportInfoSlot = new DataSlot(ShaderVisibility.Pixel);

		public MandelbrotShader() {

			viewportInfoBuffer = new ConstBuffer<ViewportInfo>(1);

			rootSignature = CreateRootSignature(
				new DataSlot[] { projectionInfoDataSlot, transformInfoDataSlot, viewportInfoSlot},
				new BufferSlot[] { },
				new TextureSlot[] { },
				new TextureSampler[] { }
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
			ShaderBytecode pixelShader = CompileShader(".res/Shaders/Mandelbrot/MandelbrotPixelShader.hlsl", ShaderType.Pixel);

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

		public override void Render(Camera camera, SharpDX.Direct3D12.Resource renderTarget, DescriptorHeap rtvDescHeap, DescriptorHeap dsvDescHeap) {

			DefaultRender(camera, renderTarget, rtvDescHeap, dsvDescHeap, projectionInfoDataSlot, transformInfoDataSlot);

		}

		public override Material GetDefaultMaterial() {

			return new MandelbrotMaterial();

		}
	}

	

}
