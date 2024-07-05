using CoolClassLibrary;

namespace ArcticFoxEngine {

	public struct Transform {

		public Vector3 position;
		public Quaternion rotation;
		public Vector3 scale;

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

		

	}

}
