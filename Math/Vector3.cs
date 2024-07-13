namespace ArcticFoxEngine {
	public struct Vector3 {

		public float x;
		public float y;
		public float z;

		public float this[int index] {

			get {

				switch (index) {
					case 0:
					return x;

					case 1:
					return y;

					case 2:
					return z;
				}
				throw new IndexOutOfRangeException();

			}
			set {
				switch (index) {

					case 0:
					x = value;
					break;

					case 1:
					y = value;
					break;

					case 2:
					z = value;
					break;

				}
				throw new IndexOutOfRangeException();
			}

		}

		#region Directions

		public static Vector3 Right {
			get {
				return new Vector3(1f, 0f, 0f);
			}
		}
		public static Vector3 Left {
			get {
				return new Vector3(-1f, 0f, 0f);
			}
		}
		public static Vector3 Up {
			get {
				return new Vector3(0f, 1f, 0f);
			}
		}
		public static Vector3 Down {
			get {
				return new Vector3(0f, -1f, 0f);
			}
		}
		public static Vector3 Forward {
			get {
				return new Vector3(0f, 0f, 1f);
			}
		}
		public static Vector3 Back {
			get {
				return new Vector3(0f, 0f, -1f);
			}
		}
		public static Vector3 Zero { 

			get {
				return new Vector3(0f, 0f, 0f);
			}
		}
		public static Vector3 One {
			get {
				return new Vector3(1f, 1f, 1f);
			}
		}

		#endregion

		public Vector3(float x, float y, float z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}
		public Vector3(System.Numerics.Vector3 vector3) {
			this.x = vector3.X;
			this.y = vector3.Y;
			this.z = vector3.Z;
		}

		public float Length() {
			return MathF.Sqrt(x * x + y * y + z * z);
		}
		public float SqrLength() {
			return x * x + y * y + z * z;
		}
		public Vector3 Normalize() {
			float length = this.Length();
			return new Vector3(x / length, y / length, z / length);
		}
		public Vector3 SetLength(float length) {
			return Normalize() * length;
		}


		#region Math

		public static float Dot(Vector3 a, Vector3 b) {
			return a.x * b.x + a.y * b.y + a.z * b.z;
		}
		public static Vector3 Cross(Vector3 a, Vector3 b) {
			return new Vector3(a.y * b.z - a.z * b.y, -a.x * b.z + a.z * b.x, a.x * b.y - a.y * b.x);
		}
		public static Vector3 operator *(Vector3 a, float b) {
			return new Vector3(a.x * b, a.y * b, a.z * b);
		}
		public static Vector3 operator *(float a, Vector3 b) {
			return new Vector3(b.x * a, b.y * a, b.z * a);
		}
		public static Vector3 operator *(Vector3 a, Vector3 b) {
			return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
		}
		public static Vector3 operator /(Vector3 a, float b) {
			return new Vector3(a.x / b, a.y / b, a.z / b);
		}
		public static Vector3 operator /(float a, Vector3 b) {
			return new Vector3(a / b.x, a / b.y, a / b.z);
		}
		public static Vector3 operator /(Vector3 a, Vector3 b) {
			return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
		}
		public static Vector3 operator +(Vector3 a, Vector3 b) {
			return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
		}
		public static Vector3 operator -(Vector3 a, Vector3 b) {
			return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
		}
		public static Vector3 operator -(Vector3 a) {
			return new Vector3(-a.x, -a.y, -a.z);
		}

		#endregion
		#region Implicit casts

		public static explicit operator SharpDX.Vector4(Vector3 d) {
			return new SharpDX.Vector4(d.x, d.y, d.z, 1f);
		}
		public static explicit operator Vector4(Vector3 d) {
			return new Vector4(d.x, d.y, d.z, 1f);
		}
		public static implicit operator SharpDX.Vector3(Vector3 d) {
			return new SharpDX.Vector3(d.x, d.y, d.z);
		}
		public static implicit operator System.Numerics.Vector3(Vector3 d) {
			return new System.Numerics.Vector3(d.x, d.y, d.z);
		}

		#endregion

		public override string ToString() {
			return "(" + x + ", " + y + ", " + z + ")";
		}

	}
}
