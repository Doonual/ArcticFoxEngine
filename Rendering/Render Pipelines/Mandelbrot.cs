using ImGuiNET;
using SharpDX.Direct3D12;

namespace ArcticFoxEngine.Rendering {

	public class MandelbrotRenderPipeline : RenderPipeline {

		public override string name => "Mandelbrot";

		public ConstBuffer<ViewportInfo> viewportInfoBuffer;

		public struct ViewportInfo {

			public Vector2 viewCenter;
			public float zoom;

			public ViewportInfo() {
				viewCenter = Vector2.zero;
				zoom = 1f;
			}

		};

		public MandelbrotRenderPipeline() {

			viewportInfoBuffer = new ConstBuffer<ViewportInfo>(1);

			CreateDataSlot("Viewport info", ShaderVisibility.Pixel);
			BindBuffer(viewportInfoBuffer, ShaderVisibility.Pixel);


			ShaderBytecode vertexShader = Graphics.CompileShader(".res/Shaders/VertexShader.hlsl", Graphics.ShaderType.Vertex);
			ShaderBytecode geometryShader = Graphics.CompileShader(".res/Shaders/GeometryShader.hlsl", Graphics.ShaderType.Geometry);
			ShaderBytecode pixelShader = Graphics.CompileShader(".res/Shaders/MandelbrotPixelShader.hlsl", Graphics.ShaderType.Pixel);
			Finalise(vertexShader, pixelShader, geometryShader);

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

		public override void BindResources(RenderPipeline renderPipeline) {

			MandelbrotRenderPipeline mandelbrotRP = (MandelbrotRenderPipeline)renderPipeline;

			mandelbrotRP.viewportInfoBuffer.Write(new MandelbrotRenderPipeline.ViewportInfo[] { viewportInfo }, 0);

			renderPipeline.SetDataSlot("Viewport info", mandelbrotRP.viewportInfoBuffer, 0);

		}

		public override void Debug() {

			System.Numerics.Vector2 viewportInfoSys = viewportInfo.viewCenter;
			ImGui.DragFloat2("View center", ref viewportInfoSys, viewportInfo.zoom * 0.001f);
			ImGui.DragFloat("Zoom", ref viewportInfo.zoom, 0.001f, 0.00000001f , 3f , null, ImGuiSliderFlags.Logarithmic);
			viewportInfo.viewCenter = viewportInfoSys;

		}

	}

}
