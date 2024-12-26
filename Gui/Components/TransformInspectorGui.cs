using ArcticFoxEngine.Gui;
using ArcticFoxEngine.Nodes;
using CoolClassLibrary;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ArcticFoxEngine.Gui.Builtin_Windows {
	
	public class TransformInspectorGui {

		Transform targetTransform;
		bool transformInspectorExpanded; // Whether the inspector should be collapsed or expanded
		float transformGuiTotalHeight; // Height of the inspector window, to get the child window the correct size

		public TransformInspectorGui(Transform targetTransform) {
			this.targetTransform = targetTransform;
			transformInspectorExpanded = false;
			transformGuiTotalHeight = 0f;
		}

		public void DrawTransformInspector(bool skipMenuBar) {

			transformInspectorExpanded |= skipMenuBar;

			Vector2 defaultWindowPaddingSize = ImGui.GetStyle().WindowPadding;


			// No window padding style
			bool pushWindowPadding = !transformInspectorExpanded;
			if (pushWindowPadding == true) {
				ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0f, 0f));
			}

			if (skipMenuBar == false) {

				

				// Begin the child
				ImGuiWindowFlags flags = ImGuiWindowFlags.None;
				if (skipMenuBar == false) {
					flags |= ImGuiWindowFlags.MenuBar;
				}
				ImGui.BeginChild((uint)(GetHashCode() + " parameters").GetHashCode(), new Vector2(0f, transformGuiTotalHeight), true, flags);

				// Draw the node icon and name in menu bar
				ImGui.BeginMenuBar();
				ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 2f);
				if (Node.DrawImageButtonTextButtonGui(Transform.transformIconPtr, "Transform", GetHashCode() + "debug menu bar", 16f, out uint buttonID) == true) {
					transformInspectorExpanded = !transformInspectorExpanded;
				}

				// Draw the button popup
				ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, defaultWindowPaddingSize);
				if (ImGui.BeginPopupContextItem(buttonID.ToString("X")) == true) {

					if (ImGui.MenuItem("Pop out") == true) {

						TransformInspectorGui popOutNodeInspector = new TransformInspectorGui(targetTransform);
						GuiManager.OpenWindow("Transform", () => { popOutNodeInspector.DrawTransformInspector(true); });

					}

					string collapseExpandMenuText = "Expand";
					if (transformInspectorExpanded == true) {
						collapseExpandMenuText = "Collapse";
					}
					if (ImGui.MenuItem(collapseExpandMenuText) == true) {
						transformInspectorExpanded = !transformInspectorExpanded;
					}


					ImGui.EndPopup();
				}
				ImGui.PopStyleVar();

				ImGui.EndMenuBar();

			}
			

			// Inspector
			if (transformInspectorExpanded == true) {
				targetTransform.DrawTransformGui();
			}


			
			if (skipMenuBar == false) {
				// Update the height of this window for next frame
				transformGuiTotalHeight = ImGui.GetCursorPosY() + ImGui.GetStyle().WindowPadding.y;
				ImGui.EndChild();
			}
			

			// No window padding style
			if (pushWindowPadding == true) {
				ImGui.PopStyleVar();
			}



		}

	}
}
