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

		public float GetLength() {
			return MathF.Sqrt(x * x + y * y);
		}
		public void SetLength(float length) {
			float currentLength = GetLength();
			x *= length / currentLength;
			y *= length / currentLength;
		}

		#region Math

		public static Vector2 operator *(Vector2 a, float b) {
			return new Vector2(a.x * b, a.y * b);
		}
		public static Vector2 operator *(Vector2 a, Vector2 b) {
			return new Vector2(a.x * b.x, a.y * b.y);
		}
		public static Vector2 operator /(Vector2 a, float b) {
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

		#endregion

		public override string ToString() {
			return "(" + x + ", " + y + ")";
		}

	}
}
