using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine {
	public struct Vector3 {

		public float x;
		public float y;
		public float z;

		#region Directions

		public static Vector3 right {
			get {
				return new Vector3(1f, 0f, 0f);
			}
		}
		public static Vector3 left {
			get {
				return new Vector3(-1f, 0f, 0f);
			}
		}
		public static Vector3 up {
			get {
				return new Vector3(0f, 1f, 0f);
			}
		}
		public static Vector3 down {
			get {
				return new Vector3(0f, -1f, 0f);
			}
		}
		public static Vector3 forward {
			get {
				return new Vector3(0f, 0f, 1f);
			}
		}
		public static Vector3 back {
			get {
				return new Vector3(0f, 0f, 1f);
			}
		}
		public static Vector3 zero {
			get {
				return new Vector3(0f, 0f, 0f);
			}
		}

		#endregion

		public Vector3(float x, float y, float z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public float GetLength() {
			return MathF.Sqrt(x * x + y * y + z * z);
		}
		public void SetLength(float length) {
			float currentLength = GetLength();
			x *= length / currentLength;
			y *= length / currentLength;
			z *= length / currentLength;
		}

		#region Math

		public static Vector3 operator *(Vector3 a, float b) {

			return new Vector3(a.x * b, a.y * b, a.z * b);

		}

		#endregion
		#region Implicit casts

		public static implicit operator SharpDX.Vector4(Vector3 d) {
			return new SharpDX.Vector4(d.x, d.y, d.z, 1f);
		}
		public static implicit operator SharpDX.Vector3(Vector3 d) {
			return new SharpDX.Vector3(d.x, d.y, d.z);
		}

		#endregion

	}
}
