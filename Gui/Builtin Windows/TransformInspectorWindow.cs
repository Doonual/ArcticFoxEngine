using ArcticFoxEngine.Nodes;

namespace ArcticFoxEngine.Gui.Builtin_Windows {

	internal class TransformInspectorWindow : GuiWindow {

		TransformInspectorGui transformInspectorGui;

		public TransformInspectorWindow(Transform targetTransform) {
			transformInspectorGui = new TransformInspectorGui(targetTransform);
		}

		public override void Render() {
			transformInspectorGui.DrawTransformInspector(true);
		}

	}
}
