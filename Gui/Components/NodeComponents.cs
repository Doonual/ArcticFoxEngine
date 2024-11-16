using ImGuiNET;


namespace ArcticFoxEngine.Gui.Components {
	public static class NodeGuiComponents {

		public static bool ImageButtonTextButton(IntPtr imageId, string text, string id, float height) {

			// Tell the image and text to have no padding
			ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0f, 0f));
			ImGui.BeginChild((uint)(id.GetHashCode()), new Vector2(0f, height), false);

			// Make button colourless and transparent
			ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.3f, 0.3f, 0.3f, 0.0f));
			ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.3f, 0.3f, 0.3f, 0.3f));
			ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.3f, 0.3f, 0.3f, 0.5f));

			// Draw the actual button
			Vector2 buttonPos = ImGui.GetCursorPos();
			bool buttonResult = ImGui.Button("##" + id + "actual button", new Vector2(-1f, -1f));

			// Draw image button
			ImGui.SetCursorPos(buttonPos);
			ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(0f, 0f));
			ImGui.ImageButton(id + "image button", imageId, new Vector2(height, height));
			ImGui.PopStyleVar();

			ImGui.SameLine();

			// Draw the text
			float textDownOffset = height / 2f - ImGui.GetTextLineHeight() / 2f;
			ImGui.SetCursorPosY(ImGui.GetCursorPosY() + textDownOffset);
			ImGui.Text(text);


			ImGui.PopStyleColor(3);


			ImGui.EndChild();
			ImGui.PopStyleVar();

			return buttonResult;

		}

	}
}
