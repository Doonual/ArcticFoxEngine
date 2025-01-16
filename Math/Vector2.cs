namespace ArcticFoxEngine {
	public struct Vector2 {

		public float x;
		public float y;

		#region Directions

		public static Vector2 right {
			get {
				return new Vector2(1f, 0f);
			}
		}
		public static Vector2 left {
			get {
				return new Vector2(-1f, 0f);
			}
		}
		public static Vector2 up {
			get {
				return new Vector2(0f, 1f);
			}
		}
		public static Vector2 down {
			get {
				return new Vector2(0f, -1f);
			}
		}
		public static Vector2 zero { 

			get
{
				return new Vector2(0f, 0f);
			}
		}
		public static Vector2 one {
			get {
				return new Vector2(1f, 1f);
			}
		}

		#endregion

		public Vector2(float x, float y) {
			this.x = x;
			this.y = y;
		}

		public float Length() {
			return MathF.Sqrt(x * x + y * y);
		}
		public float SqrLength() {
			return x * x + y * y;
		}
		public Vector2 SetLength(float length) {
			float currentLength = Length();
			return new Vector2(x * length / currentLength, y * length / currentLength);
		}
		public Vector2 Round() {
			return new Vector2(MathF.Round(x), MathF.Round(y));
		}


		public static Vector2 Angle(float theta, float magnitude) {
			return new Vector2(MathF.Cos(theta), MathF.Sin(theta)) * magnitude;
		}

		#region Math

		public static Vector2 operator *(Vector2 a, float b) {
			return new Vector2(a.x * b, a.y * b);
		}
		public static Vector2 operator *(float b, Vector2 a) {
			return new Vector2(a.x * b, a.y * b);
		}
		public static Vector2 operator *(Vector2 a, Vector2 b) {
			return new Vector2(a.x * b.x, a.y * b.y);
		}
		public static Vector2 operator /(Vector2 a, float b) {
			return new Vector2(a.x / b, a.y / b);
		}
		public static Vector2 operator /(float b, Vector2 a) {
			return new Vector2(a.x / b, a.y / b);
		}
		public static Vector2 operator /(Vector2 a, Vector2 b) {
			return new Vector2(a.x / b.x, a.y / b.y);
		}
		public static Vector2 operator +(Vector2 a, Vector2 b) {
			return new Vector2(a.x + b.x, a.y + b.y);
		}
		public static Vector2 operator -(Vector2 a, Vector2 b) {
			return new Vector2(a.x - b.x, a.y - b.y);
		}
		public static Vector2 operator -(Vector2 a) {
			return new Vector2(-a.x, -a.y);
		}

		public static float Dot(Vector2 a, Vector2 b) {
			return a.x * b.x + a.y * b.y;
		}

		#endregion
		#region Implicit casts

		public static implicit operator SharpDX.Vector2(Vector2 d) {
			return new SharpDX.Vector2(d.x, d.y);
		}
		public static implicit operator Vector3(Vector2 d) {
			return new Vector3(d.x, d.y, 0f);
		}
		public static implicit operator System.Numerics.Vector2(Vector2 d) {
			return new System.Numerics.Vector2(d.x, d.y);
		}
		public static implicit operator Vector2(System.Numerics.Vector2 d) {
			return new Vector2(d.X, d.Y);
		}

		#endregion

		public override string ToString() {
			return "(" + x + ", " + y + ")";
		}

	}
}
