using ArcticFoxEngine.Gui.Components;
using ImGuiNET;

namespace ArcticFoxEngine.Gui.Tools {

	[GuiWindowOptions("Texture Inspector", allowMultipleWindows: true)]
	public class TextureInspectorWindow : GuiWindow {

		private static List<Texture> allTextures;

		public Texture texture;
		private TextureInspectorGui textureInspector;
		public bool cinematicMode;

		public TextureInspectorWindow() {

			texture = ArcticFoxEngine.Render.RenderEngine.missingTexture;
			textureInspector = new TextureInspectorGui();
			textureInspector.SetTexture(texture);

			cinematicMode = false;

		}


		static TextureInspectorWindow() {
			allTextures = new List<Texture>();
		}
		internal static void RegisterTexture(Texture texture) {
			allTextures.Add(texture);
		}
		internal static void DeRegisterTexture(Texture texture) {
			allTextures.Remove(texture);
		}


		float cinematicModeButtonSizeX;
		public override void Render() {

			if (cinematicMode == true) {
				CinematicRender();
			}
			else {
				NonCinematicRender();
			}

		}


		Vector2 maximumDrawCoord = Vector2.one * 1000f;
		private void NonCinematicRender() {
			textureInspector.showScrollbars = true;

			float titleBarSize = ImGui.GetFontSize() + ImGui.GetStyle().FramePadding.y * 2f;
			ImGui.SetNextWindowSize(maximumDrawCoord + new Vector2(ImGui.GetStyle().WindowPadding.x * 2f, ImGui.GetStyle().WindowPadding.y));
			ImGui.Begin("Texture Inspector##" + GetHashCode(), ref open, ImGuiWindowFlags.NoScrollbar);


			ImGui.Columns(2);
			ImGui.SetColumnWidth(0, 300f);


			ImGui.SeparatorText("Loaded Textures: " + allTextures.Count);
			ImGui.BeginChildFrame((uint)("Texture list child" + GetHashCode()).GetHashCode(), ImGui.GetContentRegionAvail(), ImGuiWindowFlags.NoBackground);
			for (int i = 0; i < allTextures.Count; i++) {
				Texture currentTexture = allTextures[i];
				string name = allTextures[i].name;

				if (ImGui.MenuItem(name, texture != currentTexture) == true) {
					texture = currentTexture;
					textureInspector.SetTexture(texture);
				}
			}
			ImGui.EndChildFrame();
			ImGui.NextColumn();

			if (texture.disposed == true) {
				texture = ArcticFoxEngine.Render.RenderEngine.missingTexture;
				textureInspector.SetTexture(texture);
			}
			textureInspector.additionalDraws = null;
			textureInspector.Render();

			maximumDrawCoord.x = ImGui.GetItemRectMax().x - ImGui.GetWindowPos().x;


			ImGui.Checkbox("Cinematic mode", ref cinematicMode);
			maximumDrawCoord.y = ImGui.GetItemRectMax().y - ImGui.GetWindowPos().y;

			ImGui.SameLine(ImGui.GetContentRegionAvail().x - cinematicModeButtonSizeX);

			ImGui.Text(texture.width + "x" + texture.height + " | " + texture.format);
			cinematicModeButtonSizeX = ImGui.GetItemRectSize().x;

			ImGui.Columns();
			ImGui.End();

		}

		Vector2 textureInspectorSize = Vector2.one * 1000f;
		Vector2 cinematicModeButtonSize = Vector2.one * -100f;
		private void CinematicRender() {
			textureInspector.showScrollbars = false;

			float titleBarSize = ImGui.GetFontSize() + ImGui.GetStyle().FramePadding.y * 2f;

			ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.zero);
			ImGui.SetNextWindowSize(textureInspectorSize + Vector2.up * titleBarSize);
			ImGui.Begin("Texture Inspector##" + GetHashCode(), ref open, ImGuiWindowFlags.NoScrollbar);

			textureInspector.additionalDraws = DrawCinematicModeCinematicToggle;

			textureInspector.Render(ImGui.IsWindowFocused());
			textureInspectorSize = ImGui.GetItemRectSize();

			ImGui.End();
			ImGui.PopStyleVar();

		}
		private void DrawCinematicModeCinematicToggle(Vector2 screenTopLeft, Vector2 screenBottomRight) {
			Vector2 windowTL = screenTopLeft - ImGui.GetWindowPos();
			Vector2 windowBR = screenBottomRight - ImGui.GetWindowPos();


			ImGui.SetCursorPos(new Vector2(windowTL.x, windowBR.y - cinematicModeButtonSize.y));
			if (cinematicModeButtonSize.x < 0f || ImGui.IsMouseHoveringRect(ImGui.GetCursorScreenPos(), ImGui.GetCursorScreenPos() + cinematicModeButtonSize)) {
				ImGui.Checkbox("Cinematic mode##" + GetHashCode(), ref cinematicMode);
				cinematicModeButtonSize = ImGui.GetItemRectSize();
			}


		}
	}
}
