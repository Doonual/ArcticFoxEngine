using ArcticFoxEngine.ImGuiIntegration;
using CoolClassLibrary;
using ImGuiNET;
using System.Xml.Serialization;

namespace ArcticFoxEngine.Gui.Components {
	public class TextureInspectorGui {

		Texture texture;

		Vector2 viewCentre = Vector2.zero;
		float textureSize = 1f;
		float zoom = 1f;
		float scrollVelocity = 0f;
		public bool addExtraPadding;

		Vector2 prevMousePos;

		public bool showViewOptions = false;
		public bool allowPan = true;
		public bool showScrollbars = true;

		Vector2 topLeftScreenPos = Vector2.zero;
		Vector2 bottomRightScreenPos = Vector2.zero;

		public Action<Vector2, Vector2> additionalDraws;

		public TextureInspectorGui() {
			texture = null;
			textureSize = 1f;
		}

		public void SetTexture(Texture texture) {

			this.texture = texture;

			viewCentre.x = texture.width / 2;
			viewCentre.y = texture.height / 2;

		}

		Vector2 textureRectTL = Vector2.zero;
		Vector2 textureRectBR = Vector2.zero;

		public void Render(bool enableInteraction = true) {

			if (texture == null || texture.disposed == true) {
				ImGui.Text("Set a texture to begin");
				return;
			}


			float columnWidth = ImGui.GetContentRegionAvail().x;
			if (showScrollbars == true) {
				columnWidth -= ImGui.GetStyle().ScrollbarSize;
			}
			columnWidth = MathF.Pow(2, MathF.Floor(MathF.Log2(columnWidth)));

			// Calculate scroll positions
			float pixelScale = texture.width / (columnWidth * zoom);
			textureSize = columnWidth * zoom;
			float aspectRatio = texture.width / (float)texture.height;

			Vector2 scrollPos = new Vector2(-10f, -10f);

			// Mouse controls
			if (enableInteraction && ImGui.IsMouseHoveringRect(textureRectTL, textureRectBR) == true && allowPan == true) {

				if (MathF.Abs(scrollVelocity) < 0.00001f) { scrollVelocity = 0f; }
				scrollVelocity += ImGui.GetIO().MouseWheel * 0.02f;
				scrollVelocity *= 0.9f;

				float deltaZoom = zoom;
				zoom *= MathF.Exp(scrollVelocity);
				zoom = MathF.Min(MathF.Max(1f, zoom), texture.width / 4);
				deltaZoom /= zoom;

				pixelScale = texture.width / (columnWidth * zoom);
				textureSize = columnWidth * zoom;

				Vector2 zoomCentre = ImGui.GetMousePos() - (textureRectTL + new Vector2(columnWidth, columnWidth / aspectRatio) / 2f);
				zoomCentre = zoomCentre * pixelScale + viewCentre;
				viewCentre = (viewCentre - zoomCentre) * deltaZoom + zoomCentre;




				Vector2 currentMousePos = ImGui.GetMousePos();
				if (ImGui.IsMouseDown(ImGuiMouseButton.Middle) == true) {
					viewCentre.x -= (currentMousePos.x - prevMousePos.x) * pixelScale;
					viewCentre.y -= (currentMousePos.y - prevMousePos.y) * pixelScale;
				}
				viewCentre.x = Math.Clamp(viewCentre.x, texture.width / zoom / 2f, texture.width - texture.width / zoom / 2f);
				viewCentre.y = Math.Clamp(viewCentre.y, texture.height / zoom / 2f, texture.height - texture.height / zoom / 2f);

				prevMousePos = currentMousePos;

				Vector2 coordAtTopLeftCorner = new Vector2(viewCentre.x - texture.width / 2 / zoom, viewCentre.y - texture.height / 2 / zoom);
				scrollPos = coordAtTopLeftCorner / pixelScale;
				if (addExtraPadding == true) {
					scrollPos += Vector2.one * columnWidth / 2;
				}
				ImGui.SetNextWindowScroll(new Vector2(MathF.Round(scrollPos.x), MathF.Round(scrollPos.y)));
				
			}

			
			if (showViewOptions == true) {

				if (ImGui.DragFloat("Zoom", ref zoom, 1f, 1f, texture.width / 4, null, ImGuiSliderFlags.Logarithmic) == true) {
					scrollVelocity = 0f;
				}
				ImGui.DragFloat2("View Centre", ref viewCentre, 1, 0, texture.width - 1);
				ImGui.Checkbox("Allow pan outside texture", ref addExtraPadding);

			}


			Vector2 imageChildWindowSize = new Vector2(1f, 1f / aspectRatio) * columnWidth; // Room for the image
			imageChildWindowSize += Vector2.one * 2; // 1 pixel border
			if (showScrollbars == true) {
				imageChildWindowSize += Vector2.one * ImGui.GetStyle().ScrollbarSize; // Scrollbars
			}

			// Start the child window
			textureRectTL = ImGui.GetCursorScreenPos();
			textureRectBR = textureRectTL + imageChildWindowSize;
			Vector2 textureCentre = (textureRectBR + textureRectTL) / 2f;
			ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(1f, 1f));
			ImGuiWindowFlags flags = ImGuiWindowFlags.AlwaysHorizontalScrollbar | ImGuiWindowFlags.AlwaysVerticalScrollbar;
			if (showScrollbars == false) {
				flags = ImGuiWindowFlags.NoScrollbar;
			}
			ImGui.BeginChild((uint)(GetHashCode() + " texture fame").GetHashCode(), textureRectBR - textureRectTL, true, flags);


			
			


			// Draw padding and window
			if (addExtraPadding == true) {
				ImGui.InvisibleButton("texture pad invis button", new Vector2(textureSize + columnWidth, (textureSize + columnWidth) / aspectRatio));
				ImGui.SetCursorPos(new Vector2(columnWidth / 2f, columnWidth / 2f / aspectRatio));
			}

			topLeftScreenPos = (Vector2)ImGui.GetCursorScreenPos() - Vector2.one;
			bottomRightScreenPos = topLeftScreenPos + new Vector2(1f, 1f / aspectRatio) * (textureSize + 1);
			ImGui.Image(texture.imGuiID, new Vector2(textureSize, textureSize / aspectRatio));


			if (additionalDraws != null) {
				additionalDraws(topLeftScreenPos, bottomRightScreenPos);
			}


			// Update viewCentre based on the window scroll
			Vector2 updatedScrollPos = new Vector2(ImGui.GetScrollX(), ImGui.GetScrollY());
			if (addExtraPadding == true) {
				updatedScrollPos -= Vector2.one * columnWidth / 2;
			}
			Vector2 updatedViewCentre = new Vector2(0f, 0f);
			updatedViewCentre.x = (updatedScrollPos.x * pixelScale) + (texture.width / 2 / zoom);
			updatedViewCentre.y = (updatedScrollPos.y * pixelScale) + (texture.height / 2 / zoom);

			if (scrollVelocity == 0f) {
				viewCentre = updatedViewCentre;
			}


			

			ImGui.EndChild();
			ImGui.PopStyleVar();

			

		}


	}
}
