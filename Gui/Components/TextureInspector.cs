using ArcticFoxEngine.ImGuiIntegration;
using ImGuiNET;

namespace ArcticFoxEngine.Debug.GUI_Components {
	public class TextureInspector {

		Texture inspectTexture;
		IntPtr texturePtr;

		Vector2 viewCentre = Vector2.zero;
		float zoom = 1f;

		public TextureInspector() {
			inspectTexture = null;
			viewCentre = new Vector2(0f, 0f);
			zoom = 1f;

		}

		public void SetTexture(Texture texture) {

			if (inspectTexture != null) {
				RenderImGui.DeRegisterTexture(texturePtr);
			}

			inspectTexture = texture;

			texturePtr = RenderImGui.RegisterTexture(texture);

		}

		public void Render() {

			if (inspectTexture == null) {
				ImGui.Text("Set a texture to begin");
				return;
			}

			System.Numerics.Vector2 systemVec = (System.Numerics.Vector2)viewCentre;

			float columnWidth = ImGui.GetColumnWidth();
			columnWidth = MathF.Pow(2, MathF.Floor(MathF.Log2(columnWidth)));

			ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(1f, 1f));
			ImGui.BeginChild((uint)(GetHashCode() + " texture fame").GetHashCode(), new Vector2(columnWidth + 2, columnWidth + 2), true);
			

			ImGui.Image(texturePtr, new Vector2(columnWidth, columnWidth));

			
			ImGui.EndChild();
			ImGui.PopStyleVar();

			viewCentre = (Vector2)systemVec;

		}

	}
}
