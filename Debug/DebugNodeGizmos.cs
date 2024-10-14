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
	internal class DebugNodeGizmos : DebugOverlay {

		internal override string name => "Gizmos";

		static Camera gizmoCamera = null;
		static List<(Node, bool)> floatingEditWindows;

		

		internal override void Render() {

			if (floatingEditWindows == null) {
				floatingEditWindows = new List<(Node, bool)>();
			}

			if (Node.rootNode == null) { return; }
			if (gizmoCamera == null) {
				gizmoCamera = Node.rootNode.SearchNodeTreeDown<Camera>();
			}
			DebugGizmosLayout(Node.rootNode, new Vector2(-40f, -40f), 0f, new Vector2(-40f, -40f));
			for (int i = floatingEditWindows.Count - 1; i >= 0; i --) {
				if (floatingEditWindows[i].Item2 == true) {
					floatingEditWindows[i] = (floatingEditWindows[i].Item1, false);
					ImGui.SetNextWindowPos(ImGui.GetMousePos());
					ImGui.SetNextWindowSize(new Vector2(400f, 0f));
				}

				bool windowOpen = true;
				ImGui.Begin(floatingEditWindows[i].Item1.name + " edit ##" + floatingEditWindows[i].Item1.GetHashCode(), ref windowOpen, ImGuiWindowFlags.None);
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

		internal static void DebugGizmosLayout(Node node, Vector2 drawPos, float drawAngle, Vector2 prevScreenPos) {


			bool fullFan = false;
			if (node.GetType() == typeof(Transform) || true) {

				Vector3 cameraSpacePos = gizmoCamera.WorldToCamera(node.transform.worldPosition);
				if (cameraSpacePos.z < 0.01f || cameraSpacePos.z > 1f) {
					return;
				}
				drawPos = gizmoCamera.CameraToScreen(cameraSpacePos);
				fullFan = true;

			}
			else {
				uint col = ImGui.ColorConvertFloat4ToU32(new Vector4(0.3f, 0.3f, 0.3f, 0.5f));
				Vector2 perpandicular = prevScreenPos - drawPos;
				perpandicular = new Vector2(perpandicular.y, -perpandicular.x);
				perpandicular = perpandicular.SetLength(18f);
				ImGui.GetBackgroundDrawList().AddLine(drawPos + perpandicular, prevScreenPos + perpandicular, col);
				ImGui.GetBackgroundDrawList().AddLine(drawPos - perpandicular, prevScreenPos - perpandicular, col);
			}

			

			float angleStart = drawAngle - MathF.PI / 2f;
			float angleEnd = drawAngle + MathF.PI / 2f;

			if (fullFan == true) {
				angleStart = MathF.PI / 2f;
				angleEnd = -MathF.PI * 3f / 2f;
			}

			for (int i = 0; i < node.GetChildCount(); i++) {

				
				float angle = MathUtil.Map(i, 0, node.GetChildCount(), angleStart, angleEnd);
				Vector2 childOffset = Vector2.Angle(angle, 36f);

				
				
				Vector2 childPos = drawPos + childOffset;
				

				


				ImGui.PushID(node.GetChild(i).GetHashCode() + " gizmo rendering");
				DebugGizmosLayout(node.GetChild(i), drawPos + childOffset, angle, drawPos);
				ImGui.PopID();

			}

			DrawGizmoIcon(node, drawPos);

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
			Vector4 circleCol = new Vector4(0.3f, 0.3f, 0.3f, 0.5f);
			if (((Vector2)ImGui.GetMousePos() - screenPos).GetLength() <= circleRadius) {
				circleCol = new Vector4(0.3f, 0.3f, 0.3f, 0.7f);
				if (ImGui.IsMouseDown(ImGuiMouseButton.Left) == true || ImGui.IsMouseDown(ImGuiMouseButton.Left) == true) {
					circleCol = new Vector4(0.3f, 0.3f, 0.3f, 0.9f);
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

			bgDrawList.AddCircle(screenPos, circleRadius, ImGui.ColorConvertFloat4ToU32(circleCol));
			bgDrawList.AddImage(node.nodeIconId32, screenPos - Vector2.one * 16f, screenPos + Vector2.one * 16f);

			ImGui.End();

			


		}



	}
}
