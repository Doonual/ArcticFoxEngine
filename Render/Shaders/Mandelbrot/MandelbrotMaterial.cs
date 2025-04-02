using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Render {

	public class MandelbrotMaterial : Material {

		MandelbrotShader.ViewportInfo viewportInfo;

		public MandelbrotMaterial() {
			viewportInfo = new MandelbrotShader.ViewportInfo();
		}

		public override void BindResources(Shader shader) {
			MandelbrotShader mandelbrotShader = (MandelbrotShader)shader;

			mandelbrotShader.viewportInfoBuffer.Write(new MandelbrotShader.ViewportInfo[] { viewportInfo }, 0);

			mandelbrotShader.viewportInfoSlot.SetData(mandelbrotShader.viewportInfoBuffer, 0);


		}

		public override void DrawInspectorGUI() {


			ImGui.DragFloat2("View center", ref viewportInfo.viewCenter, viewportInfo.zoom * 0.001f, -2f, 2f, null, ImGuiSliderFlags.NoRoundToFormat);


			ImGui.DragFloat("Zoom", ref viewportInfo.zoom, viewportInfo.zoom * 0.001f, 0.00000001f, 3f, null, ImGuiSliderFlags.NoRoundToFormat);
			ImGui.DragInt("Iterations", ref viewportInfo.numIterations, 0.1f, 1, 10000, null);


		}

	}

}
