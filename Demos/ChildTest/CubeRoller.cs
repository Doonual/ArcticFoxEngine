using ArcticFoxEngine.Backend;
using ArcticFoxEngine.Components;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Testing.ChildTest {
	public class CubeRoller : Component {

		float rotationAngle;
		float rotationSpeed;


		float rotationMagnitude = 2f;


		public override void Start() {

			
			rotationAngle = 0f;
			rotationSpeed = 0.007f;
		}
		public override void Update() {

			rotationAngle += rotationSpeed;
			Vector2 rotationDirection = Vector2.Angle(rotationAngle, rotationMagnitude) * (float)Profiler.deltaTime;

			transform.rotation *= Quaternion.RotationYawPitchRoll(rotationDirection.x, rotationDirection.y, 0f);

		}

		public override void Debug() {
			base.Debug();
			ImGui.SliderAngle("Angle", ref rotationAngle);
			ImGui.SliderFloat("Omega", ref rotationSpeed, 0f, 0.015f, null);
		}

	}
}
