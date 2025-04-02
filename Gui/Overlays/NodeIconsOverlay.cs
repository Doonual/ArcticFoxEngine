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
	internal class NodeIconsOverlay : GuiOverlay {

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

			if (gizmoCamera != null) {
				// Start draw Gizmos layout tree
				DrawIconTree(Node.rootNode);
			}
			

			

		}

		internal static void DrawIconTree(Node node) {


			for (int i = 0; i < node.GetChildCount(); i ++) {
				DrawIconTree(node.GetChild(i));
			}

			if (node.transform.IsIdentity() == true) { return; }

			Vector3 cameraBoxPos = gizmoCamera.WorldToCamera(node.transform.worldPosition);
			Vector2 screenPos = gizmoCamera.WorldToScreen(node.transform.worldPosition);
			if (cameraBoxPos.z > 0f && cameraBoxPos.z < 1f) {
				DrawIcon(node, screenPos, circleRadius);
			}
			else {
				return;
			}

			for (int i = 0; i < node.GetChildCount(); i++) {
				if (node.GetChild(i).transform.IsIdentity() == false) {

					Vector3 childCameraBoxPos = gizmoCamera.WorldToCamera(node.GetChild(i).transform.worldPosition);
					Vector2 childScreenPos = gizmoCamera.WorldToScreen(node.GetChild(i).transform.worldPosition);

					if (childCameraBoxPos.z > 0f && childCameraBoxPos.z < 1f) {
						// Draw a line between this node and its child
						DrawDottedLine(screenPos, childScreenPos);

					}

				}
			}


			// After the Icon has been drawn


			List<Node> childrenWithoutTransforms = new List<Node>();
			for (int i = 0; i < node.GetChildCount(); i++) {
				if (node.GetChild(i).transform.IsIdentity() == true) {
					childrenWithoutTransforms.Add(node.GetChild(i));
				}
			}

			float childIconDistance = 3f;
			float childIconRadius = circleRadius / 2f;
			float iconRadialSize = 40f / 180f * MathF.PI;

			float startAngle = 0f;
			float endAngle = (childrenWithoutTransforms.Count() - 1) * iconRadialSize;

			startAngle -= endAngle / 2f;
			endAngle -= endAngle / 2f;

			for (int i = 0; i < childrenWithoutTransforms.Count(); i ++) {
				float currentAngle = startAngle + i * iconRadialSize + MathF.PI / 2f;
				Vector2 drawPos = screenPos + Vector2.Angle(currentAngle, circleRadius + childIconDistance + childIconRadius);
				DrawIcon(childrenWithoutTransforms[i], drawPos, childIconRadius);
			}


		}


		private static void DrawDottedLine(Vector2 screenPosA, Vector2 screenPosB) {
			float dotRadius = 4f;

			List<Vector2> dotPositions = new List<Vector2>();

			Vector2 stepVector = (screenPosB - screenPosA).SetLength(dotRadius * 4f);
			Vector2 currentDotPos = screenPosA + stepVector.SetLength(1f) * (dotRadius + circleRadius);

			for (int i = 0; i < 10000; i ++) {
				currentDotPos += stepVector;
				dotPositions.Add(currentDotPos);

				if ((currentDotPos - screenPosB).Length() < (stepVector.Length() + dotRadius + circleRadius)) {
					break;
				}

			}


			
			for (int i = 0; i < dotPositions.Count(); i ++) {
				ImGui.GetBackgroundDrawList().AddCircleFilled(dotPositions[i], dotRadius, circleNormalCol);
			}





		}
		private static void DrawIcon(Node node, Vector2 drawPos, float radius) {

			Vector2 screenPos = drawPos;


			

			// Setup window
			ImGui.SetNextWindowPos(screenPos - Vector2.one * radius);
			ImGui.SetNextWindowSize(Vector2.one * radius * 2);
			ImGuiWindowFlags windowFlags = ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoFocusOnAppearing;
			ImGui.Begin(node.GetHashCode() + " Gizmo click window", windowFlags);


			ImDrawListPtr bgDrawList = ImGui.GetBackgroundDrawList();
			uint circleCol = circleNormalCol;

			if (((Vector2)ImGui.GetMousePos() - screenPos).Length() <= radius) {
				circleCol = circleHoverCol;
				if (ImGui.IsMouseDown(ImGuiMouseButton.Left) == true) {
					circleCol = circlePressedCol;
				}
				if (ImGui.IsMouseReleased(ImGuiMouseButton.Left) == true) {

					GuiManager.OpenGuiWindow(new NodeInspectorWindow(node));

				}
				if (ImGui.IsMouseReleased(ImGuiMouseButton.Right) == true) {
					ImGui.OpenPopup(node.GetHashCode() + " gizmo context");
				}

			}

			if (ImGui.BeginPopup(node.GetHashCode() + " gizmo context") == true) {

				if (ImGui.MenuItem("Edit") == true) {
					NodeInspectorGui nodeInspectorGui = new NodeInspectorGui(node);
					GuiManager.OpenGuiWindow(new NodeInspectorWindow(node));
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

			bgDrawList.AddCircleFilled(screenPos, radius, circleCol);
			bgDrawList.AddImage(node.nodeIconId32, screenPos - Vector2.one * (radius - 2f), screenPos + Vector2.one * (radius - 2f));

			ImGui.End();

			


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
				DrawIcon(node, drawPos, circleRadius);
			}


		}


	}
}
