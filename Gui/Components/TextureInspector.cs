using ArcticFoxEngine.ImGuiIntegration;
using CoolClassLibrary;
using ImGuiNET;

namespace ArcticFoxEngine.Debug.GUI_Components {
	public class TextureInspector {

		Texture texture;
		IntPtr texturePtr;

		Vector2 viewCentre = Vector2.zero;
		float textureSize = 1f;
		float zoom = 1f;
		float scrollVelocity = 0f;
		public bool addExtraPadding;

		Vector2 prevMousePos;

		public bool showViewOptions = true;
		public bool allowPan = true;

		Vector2 topLeftScreenPos = Vector2.zero;
		Vector2 bottomRightScreenPos = Vector2.zero;

		public Action<Vector2, Vector2> additionalDraws;

		public TextureInspector() {
			texture = null;
			textureSize = 1f;
		}

		public void SetTexture(Texture texture) {

			if (this.texture != null) {
				RenderImGui.DeRegisterTexture(texturePtr);
			}

			this.texture = texture;
			texturePtr = RenderImGui.RegisterTexture(texture);

			viewCentre.x = texture.width / 2;
			viewCentre.y = texture.height / 2;

		}

		Vector2 textureRectTL = Vector2.zero;
		Vector2 textureRectBR = Vector2.zero;

		public void Render() {

			if (texture == null) {
				ImGui.Text("Set a texture to begin");
				return;
			}

			float scrollbarWidth = ImGui.GetStyle().ScrollbarSize;
			float columnWidth = ImGui.GetColumnWidth();
			columnWidth = MathF.Pow(2, MathF.Floor(MathF.Log2(columnWidth)));



			// Calculate scroll positions
			float pixelScale = texture.width / (columnWidth * zoom);
			textureSize = columnWidth * zoom;
			Vector2 coordAtTopLeftCorner = new Vector2(viewCentre.x - texture.width / 2 / zoom, viewCentre.y - texture.height / 2 / zoom);
			Vector2 scrollPos = coordAtTopLeftCorner / pixelScale + Vector2.one * columnWidth / 2;
			scrollPos.x = MathF.Round(scrollPos.x);
			scrollPos.y = MathF.Round(scrollPos.y);
			ImGui.SetNextWindowScroll(scrollPos);

			// Mouse controls
			if (ImGui.GetIO().WantCaptureMouse == true && ImGui.IsMouseHoveringRect(textureRectTL, textureRectBR) == true && allowPan == true) {

				scrollVelocity += ImGui.GetIO().MouseWheel * 0.02f;
				scrollVelocity *= 0.9f;
				zoom *= MathF.Exp(scrollVelocity);
				zoom = MathF.Min(MathF.Max(1f, zoom), texture.width / 4);

				Vector2 currentMousePos = ImGui.GetMousePos();
				if (ImGui.IsMouseDown(ImGuiMouseButton.Middle) == true) {
					viewCentre.x -= (currentMousePos.x - prevMousePos.x) * pixelScale;
					viewCentre.y -= (currentMousePos.y - prevMousePos.y) * pixelScale;

					//viewCentre.x = MathF.Min(MathF.Max(0f, viewCentre.x), texture.width);
					//viewCentre.y = MathF.Min(MathF.Max(0f, viewCentre.y), texture.width);
				}
				prevMousePos = currentMousePos;

			}

			if (showViewOptions == true) {

				System.Numerics.Vector2 systemVec = viewCentre;
				
				if (ImGui.DragFloat("Zoom", ref zoom, 1f, 1f, texture.width / 4, null, ImGuiSliderFlags.Logarithmic) == true) {
					scrollVelocity = 0f;
				}
				ImGui.DragFloat2("View Centre", ref systemVec, 1, 0, texture.width - 1);
				ImGui.Checkbox("Allow pan outside texture", ref addExtraPadding);
				viewCentre = systemVec;

			}



			ImGuiWindowFlags flags = ImGuiWindowFlags.AlwaysHorizontalScrollbar | ImGuiWindowFlags.AlwaysVerticalScrollbar;
			// Start the child window
			textureRectTL = ImGui.GetCursorScreenPos();
			textureRectBR = textureRectTL + new Vector2(columnWidth + scrollbarWidth + 2, columnWidth + scrollbarWidth + 2);
			ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(1f, 1f));
			ImGui.BeginChild((uint)(GetHashCode() + " texture fame").GetHashCode(), new Vector2(columnWidth + scrollbarWidth + 2, columnWidth + scrollbarWidth + 2), true, flags);
			


			// Draw padding and window
			if (addExtraPadding == true) {
				ImGui.InvisibleButton("texture pad invis button", new Vector2(textureSize + columnWidth, textureSize + columnWidth));
				ImGui.SetCursorPos(new Vector2(columnWidth / 2f, columnWidth / 2f));
				//topLeftScreenPos += new Vector2(columnWidth / 2f, columnWidth / 2f);
				//bottomRightScreenPos += new Vector2(columnWidth / 2f, columnWidth / 2f);
			}

			topLeftScreenPos = (Vector2)ImGui.GetCursorScreenPos() - Vector2.one;
			bottomRightScreenPos = topLeftScreenPos + Vector2.one * (textureSize + 1);
			ImGui.Image(texturePtr, new Vector2(textureSize, textureSize));


			
			Vector2 newScrollPos = new Vector2(ImGui.GetScrollX(), ImGui.GetScrollY());
			
			if ((scrollPos - newScrollPos).GetLength() > 1.2f && false) {
				viewCentre = newScrollPos - Vector2.one * columnWidth / 2;
				viewCentre *= pixelScale;
				viewCentre += new Vector2(texture.width / 2 / zoom, texture.height / 2 / zoom);

			}

			if (additionalDraws != null) {
				additionalDraws(topLeftScreenPos, bottomRightScreenPos);
			}

			ImGui.EndChild();
			ImGui.PopStyleVar();


		}


	}
}
