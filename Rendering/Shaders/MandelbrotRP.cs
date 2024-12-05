using ImGuiNET;
using SharpDX.Direct3D12;
using SharpDX.DXGI;

namespace ArcticFoxEngine.Rendering {

	public class MandelbrotRenderPipeline : Shader {

		public override string name => "Mandelbrot";

		public ConstBuffer<ViewportInfo> viewportInfoBuffer;

		public struct ViewportInfo {

			public Vector2 viewCenter = Vector2.zero;
			public float zoom = 1f;
			public int numIterations = 100;
			public bool doublePrecision = false;

			public ViewportInfo() {

			}

		};

		public DataSlot viewportInfoSlot = new DataSlot(ShaderVisibility.Pixel);

		public MandelbrotRenderPipeline() {

			viewportInfoBuffer = new ConstBuffer<ViewportInfo>(1);

			rootSignature = CreateRootSignature(
				new DataSlot[] {projectionInfoSlot, transformInfoSlot, viewportInfoSlot},
				new BufferSlot[] { },
				new TextureSlot[] { },
				new TextureSampler[] { }
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
			ShaderBytecode pixelShader = CompileShader(".res/Shaders/Mandelbrot/MandelbrotPixelShader.hlsl", ShaderType.Pixel);

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

			return new MandelbrotMaterial();

		}
	}

	public class MandelbrotMaterial : Material {

		MandelbrotRenderPipeline.ViewportInfo viewportInfo;

		public MandelbrotMaterial() {
			viewportInfo = new MandelbrotRenderPipeline.ViewportInfo();
		}

		public override void BindResources(Shader shader) {
			MandelbrotRenderPipeline mandelbrotShader = (MandelbrotRenderPipeline)shader;

			mandelbrotShader.viewportInfoBuffer.Write(new MandelbrotRenderPipeline.ViewportInfo[] { viewportInfo }, 0);

			mandelbrotShader.viewportInfoSlot.SetData(mandelbrotShader.viewportInfoBuffer, 0);
			

		}

		public override void Debug() {

			System.Numerics.Vector2 viewportInfoSys = viewportInfo.viewCenter;
			ImGui.DragFloat2("View center", ref viewportInfoSys, viewportInfo.zoom * 0.001f, -2f, 2f, null, ImGuiSliderFlags.NoRoundToFormat);
			viewportInfo.viewCenter = viewportInfoSys;


			ImGui.DragFloat("Zoom", ref viewportInfo.zoom, viewportInfo.zoom * 0.001f, 0.00000001f , 3f , null, ImGuiSliderFlags.NoRoundToFormat);
			ImGui.DragInt("Iterations", ref viewportInfo.numIterations, 0.1f, 1, 10000, null);
			ImGui.Checkbox("Double precision", ref viewportInfo.doublePrecision);


		}

	}

}
