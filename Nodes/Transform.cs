using ImGuiNET;

namespace ArcticFoxEngine.Nodes {

	public class Transform : Node {

		internal override string description => "Gives an object a position, rotation and scale.";
		internal override string nodeIconPath => ".res/NodeIcons/Transform.png";

		public Vector3 position;
		public Quaternion rotation;
		public Vector3 scale;
		public Matrix transformationMatrix {
			get {
				return Matrix.Transformation(position, rotation, scale);
			}
		}
		public static Matrix CalculateFromNode(Node node) {

			Node searchForNextTf = node;
			Matrix accumTransform = Matrix.Identity;
			int numTransformsAccumed = 0;

			while (true) {
				if (searchForNextTf == null) {
					break;
				}
				Transform transformSib = searchForNextTf.transform;
				if (transformSib != null) {
					accumTransform = accumTransform * Matrix.Transformation(transformSib.position, transformSib.rotation, transformSib.scale);
					numTransformsAccumed += 1;
				}
				searchForNextTf = searchForNextTf.parentNode;
			}

			return accumTransform;

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
			name = "Transform";

			position = Vector3.Zero;
			rotation = Quaternion.Identity;
			scale = Vector3.One;

			Enable();

		}
		public void ResetTransform() {
			position = Vector3.Zero;
			rotation = Quaternion.Identity;
			scale = Vector3.One;
		}




		public override void Debug() {

			System.Numerics.Vector3 systemPos = (System.Numerics.Vector3)position;
			System.Numerics.Vector4 systemRot = new System.Numerics.Vector4(rotation.x, rotation.y, rotation.z, rotation.w);
			System.Numerics.Vector3 systemScale = (System.Numerics.Vector3)scale;

			ImGuiExtras.ItemWidthForText("Rotation"); ImGui.DragFloat3("Position", ref systemPos, 0.01f);
			ImGuiExtras.ItemWidthForText("Rotation"); ImGui.DragFloat4("Rotation", ref systemRot, 0.01f);
			ImGuiExtras.ItemWidthForText("Rotation"); ImGui.DragFloat3("Scale", ref systemScale, 0.01f);

			position = new Vector3(systemPos);
			rotation = new Quaternion(systemRot.X, systemRot.Y, systemRot.Z, systemRot.W);
			rotation.Normalize();
			scale = new Vector3(systemScale);

			if (ImGui.Button("Reset") == true) {
				ResetTransform();
			}

		}




	}

}
