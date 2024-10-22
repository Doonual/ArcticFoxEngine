using ArcticFoxEngine.Nodes;
using CoolClassLibrary;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ArcticFoxEngine.Debug {
	internal class NodeGizmosOverlay : GuiOverlay {

		internal override string name => "Gizmos";

		static Camera gizmoCamera = null;
		static List<(Node, bool)> floatingEditWindows;

		static float circleRadius = 18f;
		static float fanRadiusStep = 52f;

		static float nodeLinkThickness = 4f;
		static uint nodeLinkCol = ImGui.ColorConvertFloat4ToU32(new Vector4(0.3f, 0.3f, 0.3f, 0.5f));
		
		static uint circleNormalCol = ImGui.ColorConvertFloat4ToU32(new Vector4(0.3f, 0.3f, 0.3f, 0.5f));
		static uint circleHoverCol = ImGui.ColorConvertFloat4ToU32(new Vector4(0.3f, 0.3f, 0.3f, 0.7f));
		static uint circlePressedCol = ImGui.ColorConvertFloat4ToU32(new Vector4(0.3f, 0.3f, 0.3f, 0.9f));


		internal override void Render() {

			if (floatingEditWindows == null) {
				floatingEditWindows = new List<(Node, bool)>();
			}

			if (Node.rootNode == null) { return; }
			if (gizmoCamera == null) {
				gizmoCamera = Node.rootNode.SearchNodeTreeDown<Camera>();
			}

			// Start debug Gizmos layout tree
			DebugGizmosLayout(Node.rootNode, Vector2.zero, Vector2.zero, 0f, 0, MathF.PI * 2f, true);

			// Render floating windows
			for (int i = floatingEditWindows.Count - 1; i >= 0; i --) {
				if (floatingEditWindows[i].Item2 == true) {
					floatingEditWindows[i] = (floatingEditWindows[i].Item1, false);
					ImGui.SetNextWindowPos(ImGui.GetMousePos());
					ImGui.SetNextWindowSize(new Vector2(400f, 0f));
				}

				bool windowOpen = true;
				ImGui.Begin(floatingEditWindows[i].Item1.name + " edit ##" + floatingEditWindows[i].Item1.GetHashCode(), ref windowOpen, ImGuiWindowFlags.None);
				if (ImGui.TreeNode("Transform") == true) {
					floatingEditWindows[i].Item1.transform.Debug();
					ImGui.TreePop();
				}
				
				floatingEditWindows[i].Item1.Debug();
				ImGui.End();

				if (windowOpen == false) {
					floatingEditWindows.Remove(floatingEditWindows[i]);
				}
			}

		}

		internal static void OpenFloatingNodeEdit(Node node) {
			for (int i = 0; i < floatingEditWindows.Count; i ++) {
				if (floatingEditWindows[i].Item1 == node) {
					floatingEditWindows[i] = (node, true);
					return;
				}
			}
			floatingEditWindows.Add((node, true));
		}

		internal static void DebugGizmosLayout(Node node, Vector2 prevDrawPos, Vector2 fanCentre, float fanAngle, int fanDepth, float fanRange, bool gizmoInView) {

			float fanRadius = fanDepth * fanRadiusStep;
			Vector2 drawPos = fanCentre + Vector2.Angle(fanAngle, fanRadius);

			// If the node has a position, use its position for drawing the gizmo.
			// Unless it's parent is the root node, use the origin for drawing the gizmo
			if (node.transform.localPosition.SqrLength() > 0.01f || (node.parentNode != null && node.parentNode == Node.rootNode)) {

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

				// If this node is fanning off it's parent node.

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
				DebugGizmosLayout(node.GetChild(i), drawPos, fanCentre, angle, fanDepth + 1, fanRange / Math.Max(2, node.GetChildCount()), gizmoInView);
				ImGui.PopID();

			}

			if (node != Node.rootNode && gizmoInView == true) {
				DrawGizmoIcon(node, drawPos);
			}
			

		}

		private static void DrawGizmoIcon(Node node, Vector2 drawPos) {


			Vector2 screenPos = drawPos;
			float circleRadius = 18f;

			// Setup window
			ImGui.SetNextWindowPos(screenPos - Vector2.one * circleRadius);
			ImGui.SetNextWindowSize(Vector2.one * circleRadius * 2);
			ImGuiWindowFlags windowFlags = ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoBackground;
			ImGui.Begin(node.GetHashCode() + " Gizmo click window", windowFlags);


			ImDrawListPtr bgDrawList = ImGui.GetBackgroundDrawList();
			uint circleCol = circleNormalCol;

			if (((Vector2)ImGui.GetMousePos() - screenPos).GetLength() <= circleRadius) {
				circleCol = circleHoverCol;
				if (ImGui.IsMouseDown(ImGuiMouseButton.Left) == true || ImGui.IsMouseDown(ImGuiMouseButton.Left) == true) {
					circleCol = circlePressedCol;
				}
				if (ImGui.IsMouseReleased(ImGuiMouseButton.Left) == true) {
					OpenFloatingNodeEdit(node);
				}
				if (ImGui.IsMouseReleased(ImGuiMouseButton.Right) == true) {
					ImGui.OpenPopup(node.GetHashCode() + " gizmo context");
				}

			}

			if (ImGui.BeginPopup(node.GetHashCode() + " gizmo context") == true) {

				node.DebugContextMenu();
				

				ImGui.EndPopup();
			}

			bgDrawList.AddCircleFilled(screenPos, circleRadius, circleCol);
			bgDrawList.AddImage(node.nodeIconId32, screenPos - Vector2.one * 16f, screenPos + Vector2.one * 16f);

			ImGui.End();

			


		}



	}
}
