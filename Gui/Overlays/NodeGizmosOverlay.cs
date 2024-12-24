using ArcticFoxEngine.Gui.Builtin_Windows;
using ArcticFoxEngine.Nodes;
using CoolClassLibrary;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ArcticFoxEngine.Gui {
	internal class NodeGizmosOverlay : GuiOverlay {

		internal override string name => "Gizmos";

		static Camera gizmoCamera = null;
		static List<Node> openNodes; // List of nodes that have their children exposed

		static float circleRadius = 18f;
		static float fanRadiusStep = 52f;

		static float nodeLinkThickness = 4f;
		static uint nodeLinkCol = ImGui.ColorConvertFloat4ToU32(new Vector4(0.3f, 0.3f, 0.3f, 0.5f));
		
		static uint circleNormalCol = ImGui.ColorConvertFloat4ToU32(new Vector4(0.3f, 0.3f, 0.3f, 0.5f));
		static uint circleHoverCol = ImGui.ColorConvertFloat4ToU32(new Vector4(0.3f, 0.3f, 0.3f, 0.7f));
		static uint circlePressedCol = ImGui.ColorConvertFloat4ToU32(new Vector4(0.3f, 0.3f, 0.3f, 0.9f));


		internal override void Render() {

			if (openNodes == null) {
				openNodes = new List<Node>();
			}

			if (Node.rootNode == null) { return; }
			if (gizmoCamera == null || gizmoCamera.disposed == true) {
				gizmoCamera = Node.rootNode.SearchNodeTreeDown<Camera>();
			}

			// Start draw Gizmos layout tree
			DrawGizmoIconsInWorld(Node.rootNode, Vector2.zero, Vector2.zero, 0f, 0, MathF.PI * 2f, true, true);

			

		}


		// This has a lot of parameters :(
		// Fix this later
		internal static void DrawGizmoIconsInWorld(Node node, Vector2 prevDrawPos, Vector2 fanCentre, float fanAngle, int fanDepth, float fanRange, bool gizmoInView, bool skipDrawing) {


			float fanRadius = fanDepth * fanRadiusStep;
			Vector2 drawPos = fanCentre + Vector2.Angle(fanAngle, fanRadius);

			// If the node has a position, use its position for drawing the gizmo.
			// Unless it's parent is the root node, use the origin for drawing the gizmo
			if (node.transform.localPosition.SqrLength() > 0.01f) {
				// This node has its own position, render it using that position
				skipDrawing = false;
				Vector3 cameraSpacePos = gizmoCamera.WorldToCamera(node.transform.worldPosition);
				if (cameraSpacePos.z < 0.01f || cameraSpacePos.z > 1f) {
					gizmoInView = false;
				}
				else {
					gizmoInView = true;
				}
				drawPos = gizmoCamera.CameraToScreen(cameraSpacePos);
				fanCentre = drawPos;
				fanRange = MathF.PI * 2f;
				fanDepth = 0;

			}
			else {

				// This node does not have a position. Fan it off it's parent
				if (gizmoInView == true) {
					Vector2 lineDirection = (drawPos - prevDrawPos).SetLength(1f);
					ImGui.GetBackgroundDrawList().AddLine(drawPos - lineDirection * (circleRadius), prevDrawPos + lineDirection * (circleRadius), nodeLinkCol, nodeLinkThickness);
				}

			}

			

			float angleStart = -fanRange / 2f;
			float angleEnd = +fanRange / 2f;

			for (int i = 0; i < node.GetChildCount(); i++) {

				
				float angle = MathUtil.Map(i + 0.5f, 0, node.GetChildCount(), angleStart, angleEnd) + fanAngle;

				ImGui.PushID(node.GetChild(i).GetHashCode() + " gizmo rendering");
				DrawGizmoIconsInWorld(node.GetChild(i), drawPos, fanCentre, angle, fanDepth + 1, fanRange / Math.Max(2, node.GetChildCount()), gizmoInView, skipDrawing);
				ImGui.PopID();

			}

			if (node != Node.rootNode && skipDrawing == false && gizmoInView == true) {
				DrawGizmoIcon(node, drawPos);
			}
			

		}

		private static void DrawGizmoIcon(Node node, Vector2 drawPos) {

			float circleRadius = 18f;
			Vector2 screenPos = drawPos;


			

			// Setup window
			ImGui.SetNextWindowPos(screenPos - Vector2.one * circleRadius);
			ImGui.SetNextWindowSize(Vector2.one * circleRadius * 2);
			ImGuiWindowFlags windowFlags = ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoFocusOnAppearing;
			ImGui.Begin(node.GetHashCode() + " Gizmo click window", windowFlags);


			ImDrawListPtr bgDrawList = ImGui.GetBackgroundDrawList();
			uint circleCol = circleNormalCol;

			if (((Vector2)ImGui.GetMousePos() - screenPos).GetLength() <= circleRadius) {
				circleCol = circleHoverCol;
				if (ImGui.IsMouseDown(ImGuiMouseButton.Left) == true) {
					circleCol = circlePressedCol;
				}
				if (ImGui.IsMouseReleased(ImGuiMouseButton.Left) == true) {

					NodeInspectorGui nodeInspectorGui = new NodeInspectorGui(node);
					GuiManager.OpenWindow(node.name, () => { nodeInspectorGui.DrawNodeInspector(true); });

				}
				if (ImGui.IsMouseReleased(ImGuiMouseButton.Right) == true) {
					ImGui.OpenPopup(node.GetHashCode() + " gizmo context");
				}

			}

			if (ImGui.BeginPopup(node.GetHashCode() + " gizmo context") == true) {

				if (ImGui.MenuItem("Edit") == true) {
					NodeInspectorGui nodeInspectorGui = new NodeInspectorGui(node);
					GuiManager.OpenWindow(node.name, () => { nodeInspectorGui.DrawNodeInspector(true); });
				}
				if (ImGui.MenuItem("Reveal in node tree") == true) {

					SceneWindow.selectedNode = node;


					Node parentChain = node.parentNode;
					while (parentChain != null) {
						parentChain.nodeOpen = true;
						parentChain = parentChain.parentNode;
					}

				}

				node.DrawContextMenuGui();
				

				ImGui.EndPopup();
			}

			bgDrawList.AddCircleFilled(screenPos, circleRadius, circleCol);
			bgDrawList.AddImage(node.nodeIconId32, screenPos - Vector2.one * 16f, screenPos + Vector2.one * 16f);

			ImGui.End();

			


		}



	}
}
