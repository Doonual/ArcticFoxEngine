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
	
	public class NodeInspectorGui {

		Node targetNode;
		bool nodeInspectorExpanded; // Whether the inspector should be collapsed or expanded
		float nodeGuiTotalHeight; // Height of the inspector window, to get the child window the correct size

		List<NodeInspectorGui> childInspectorGuis;
		TransformInspectorGui transformInspectorGui;

		public NodeInspectorGui(Node targetNode) {
			this.targetNode = targetNode;
			nodeInspectorExpanded = true;
			nodeGuiTotalHeight = 0f;

			childInspectorGuis = new List<NodeInspectorGui>();
			for (int i = 0; i < targetNode.GetChildCount(); i ++) {
				NodeInspectorGui newNodeInspector = new NodeInspectorGui(targetNode.GetChild(i));
				newNodeInspector.nodeInspectorExpanded = false;
				childInspectorGuis.Add(newNodeInspector);
			}

			transformInspectorGui = new TransformInspectorGui(targetNode.transform);

		}

		public void DrawNodeInspector(bool skipMenuBar) {

			nodeInspectorExpanded |= skipMenuBar;

			#region Update the childInspectorGuis if the child nodes of the target node has changed

			bool targetNodeChangedChildren = false;
			if (childInspectorGuis.Count() != targetNode.GetChildCount()) {
				targetNodeChangedChildren = true;
			}
			else {
				for (int i = 0; i < childInspectorGuis.Count(); i ++) {
					if (childInspectorGuis[i].targetNode.GetHashCode() != targetNode.GetChild(i).GetHashCode()) {
						targetNodeChangedChildren = true;
						break;
					}
				}
			}
			if (targetNodeChangedChildren == true) {
				childInspectorGuis.Clear();
				childInspectorGuis = new List<NodeInspectorGui>();
				for (int i = 0; i < targetNode.GetChildCount(); i++) {
					childInspectorGuis.Add(new NodeInspectorGui(targetNode.GetChild(i)));
				}
			}

			#endregion

			Vector2 defaultWindowPaddingSize = ImGui.GetStyle().WindowPadding;


			// No window padding style
			bool pushWindowPadding = !nodeInspectorExpanded;
			if (pushWindowPadding == true) {
				ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0f, 0f));
			}

			if (skipMenuBar == false) {

				

				// Begin the child
				ImGuiWindowFlags flags = ImGuiWindowFlags.None;
				if (skipMenuBar == false) {
					flags |= ImGuiWindowFlags.MenuBar;
				}
				ImGui.BeginChild((uint)(GetHashCode() + " parameters").GetHashCode(), new Vector2(0f, nodeGuiTotalHeight), true, flags);

				// Draw the node icon and name in menu bar
				ImGui.BeginMenuBar();
				ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 2f);
				if (Node.DrawImageButtonTextButtonGui(targetNode.nodeIconId, targetNode.name, GetHashCode() + "debug menu bar", 16f, out uint buttonID) == true) {
					nodeInspectorExpanded = !nodeInspectorExpanded;
				}

				// Draw the button popup
				ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, defaultWindowPaddingSize);
				if (ImGui.BeginPopupContextItem(buttonID.ToString("X")) == true) {

					string collapseExpandMenuText = "Expand";
					if (nodeInspectorExpanded == true) {
						collapseExpandMenuText = "Collapse";
					}
					if (ImGui.MenuItem(collapseExpandMenuText) == true) {
						nodeInspectorExpanded = !nodeInspectorExpanded;
					}

					if (ImGui.MenuItem("Pop out") == true) {

						NodeInspectorGui popOutNodeInspector = new NodeInspectorGui(targetNode);
						GuiManager.OpenWindow(targetNode.name, () => { popOutNodeInspector.DrawNodeInspector(true); });

					}


					targetNode.DrawContextMenuGui();

					ImGui.EndPopup();
				}
				ImGui.PopStyleVar();

				ImGui.EndMenuBar();

			}
			

			// Inspector
			if (nodeInspectorExpanded == true) {
				transformInspectorGui.DrawTransformInspector(false);
				targetNode.GuiEvent();

				if (childInspectorGuis.Count() != 0) {
					ImGui.SeparatorText("Children");
				}
				for (int i = 0; i < childInspectorGuis.Count(); i++) {
					childInspectorGuis[i].DrawNodeInspector(false);
				}
			}


			
			if (skipMenuBar == false) {
				// Update the height of this window for next frame
				nodeGuiTotalHeight = ImGui.GetCursorPosY() + ImGui.GetStyle().WindowPadding.y;
				ImGui.EndChild();
			}
			

			// No window padding style
			if (pushWindowPadding == true) {
				ImGui.PopStyleVar();
			}



		}

	}
}
