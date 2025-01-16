using ArcticFoxEngine.Nodes;
using ImGuiNET;

namespace ArcticFoxEngine.Demos.ChildTest {
	public class CubeRoller : Node {

		float rotationAngle;
		float rotationSpeed;


		float rotationMagnitude = 2f;

		public CubeRoller() {
			name = "Cube Roller";

			rotationAngle = 0f;
			rotationSpeed = 0.007f;

			Enable();
		}

		public override void Update() {

			rotationAngle += rotationSpeed;
			Vector2 rotationDirection = Vector2.Angle(rotationAngle, rotationMagnitude) * (float)Profiler.deltaTime;

			transform.localRotation *= Quaternion.RotationYawPitchRoll(rotationDirection.x, rotationDirection.y, 0f);

		}

		public override void DrawInspector() {
			base.DrawInspector();
			ImGui.SliderAngle("Angle", ref rotationAngle);
			ImGui.SliderFloat("Omega", ref rotationSpeed, 0f, 0.015f, null);
		}

	}
}
