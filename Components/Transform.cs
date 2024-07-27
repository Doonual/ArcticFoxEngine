using ImGuiNET;

namespace ArcticFoxEngine {

	public class Transform : Component {

		public Vector3 position;
		public Quaternion rotation;
		public Vector3 scale;

		float test = 0f;

		public Matrix transformationMatrix {
			get {
				return Matrix.Transformation(position, rotation, scale);
			}
		}

		#region Directions

		public Vector3 Right {
			get {
				return Matrix.RotationQuaternion(rotation).Invert() * Vector3.Right;
			}
		}
		public Vector3 Left {
			get {
				return Matrix.RotationQuaternion(rotation).Invert() * Vector3.Left;
			}
		}
		public Vector3 Up {
			get {
				return Matrix.RotationQuaternion(rotation).Invert() * Vector3.Up;
			}
		}
		public Vector3 Down {
			get {
				return Matrix.RotationQuaternion(rotation).Invert() * Vector3.Down;
			}
		}
		public Vector3 Forward {
			get {
				return Matrix.RotationQuaternion(rotation).Invert() * Vector3.Forward;
			}
		}
		public Vector3 Back {
			get {
				return Matrix.RotationQuaternion(rotation).Invert() * Vector3.Back;
			}
		}

		
		#endregion

		public Transform() {

			position = Vector3.Zero;
			rotation = Quaternion.Identity;
			scale = Vector3.One;
		}
		public void ResetTransform() {
			position = Vector3.Zero;
			rotation = Quaternion.Identity;
			scale = Vector3.One;
		}

		internal override string debugName => "Transform";

		internal override string debugDescription => "Gives an object a position, rotation and scale.";


		protected internal override void Debug() {

			base.Debug();

			System.Numerics.Vector3 systemPos = (System.Numerics.Vector3)position;
			System.Numerics.Vector4 systemRot = new System.Numerics.Vector4(rotation.x, rotation.y, rotation.z, rotation.w);
			System.Numerics.Vector3 systemScale = (System.Numerics.Vector3)scale;

			ImGui.DragFloat3("Position", ref systemPos, 0.01f);
			ImGui.DragFloat4("Rotation", ref systemRot, 0.01f);
			ImGui.DragFloat3("Scale", ref systemScale, 0.01f);

			position = new Vector3(systemPos);
			rotation = new Quaternion(systemRot.X, systemRot.Y, systemRot.Z, systemRot.W);
			scale = new Vector3(systemScale);

			if (ImGui.Button("Reset") == true) {
				ResetTransform();
			}

		}




	}

}
