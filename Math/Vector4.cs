namespace ArcticFoxEngine {
	public struct Vector4 {

		public float x;
		public float y;
		public float z;
		public float w;

		public float this[int index] {

			get {

				switch (index) {
					case 0:
					return x;

					case 1:
					return y;

					case 2:
					return z;

					case 3:
					return w;
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

					case 3:
					w = value;
					break;
				}
				throw new IndexOutOfRangeException();
			}

		}

		#region Directions

		public static Vector4 right {
			get {
				return new Vector4(1f, 0f, 0f, 1f);
			}
		}
		public static Vector4 left {
			get {
				return new Vector4(-1f, 0f, 0f, 1f);
			}
		}
		public static Vector4 up {
			get {
				return new Vector4(0f, 1f, 0f, 1f);
			}
		}
		public static Vector4 down {
			get {
				return new Vector4(0f, -1f, 0f, 1f);
			}
		}
		public static Vector4 forward {
			get {
				return new Vector4(0f, 0f, 1f, 1f);
			}
		}
		public static Vector4 back {
			get {
				return new Vector4(0f, 0f, -1f, 1f);
			}
		}
		public static Vector4 zero { 

			get
{
				return new Vector4(0f, 0f, 0f, 1f);
			}
		}
		public static Vector4 one {
			get {
				return new Vector4(1f, 1f, 1f, 1f);
			}
		}

		#endregion

		public Vector4(float x, float y, float z, float w) {
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public float Length() {
			return MathF.Sqrt(x * x + y * y + z * z + w * w);
		}
		public Vector4 Normalize() {
			float length = this.Length();
			return new Vector4(x / length, y / length, z / length, w / length);
		}
		public Vector4 SetLength(float length) {
			return Normalize() * length;
		}
		public Vector4 Round() {
			return new Vector4(MathF.Round(x), MathF.Round(y), MathF.Round(z), MathF.Round(w));
		}


		#region Math

		public static float Dot(Vector4 a, Vector4 b) {
			return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		}

		public static Vector4 operator *(Vector4 a, float b) {
			return new Vector4(a.x * b, a.y * b, a.z * b, a.w * b);
		}
		public static Vector4 operator *(float a, Vector4 b) {
			return new Vector4(b.x * a, b.y * a, b.z * a, b.w * a);
		}
		public static Vector4 operator *(Vector4 a, Vector4 b) {
			return new Vector4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
		}
		public static Vector4 operator /(Vector4 a, float b) {
			return new Vector4(a.x / b, a.y / b, a.z / b, a.w / b);
		}
		public static Vector4 operator /(float a, Vector4 b) {
			return new Vector4(a / b.x, a / b.y, a / b.z, a / b.w);
		}

		public static Vector4 operator /(Vector4 a, Vector4 b) {
			return new Vector4(a.x / b.x, a.y / b.y, a.z / b.z, a.w / b.w);
		}
		public static Vector4 operator +(Vector4 a, Vector4 b) {
			return new Vector4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
		}
		public static Vector4 operator -(Vector4 a, Vector4 b) {
			return new Vector4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
		}
		public static Vector4 operator -(Vector4 a) {
			return new Vector4(-a.x, -a.y, -a.z, -a.w);
		}

		#endregion
		#region Implicit casts

		public static implicit operator SharpDX.Vector4(Vector4 d) {
			return new SharpDX.Vector4(d.x, d.y, d.z, d.w);
		}
		public static implicit operator Vector3(Vector4 d) {
			return new Vector3(d.x, d.y, d.z);
		}
		public static implicit operator System.Numerics.Vector4(Vector4 d) {
			return new System.Numerics.Vector4(d.x, d.y, d.z, d.w);
		}
		public static implicit operator Vector4(System.Numerics.Vector4 d) {
			return new Vector4(d.X, d.Y, d.Z, d.W);
		}

		#endregion

		public override string ToString() {
			return "(" + x + ", " + y + ", " + z + ", " + w + ")";
		}

	}
}
