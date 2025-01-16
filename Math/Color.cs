using CoolClassLibrary;
using SharpDX;

namespace ArcticFoxEngine {
	public struct Color {

		// Ranges between 0-255
		public byte r;
		public byte g;
		public byte b;
		public byte a;

		// Ranges between 0-359 for hue, between 0-255 for s and v
		public int h {
			get {

				float r_ = r / 255f;
				float g_ = g / 255f;
				float b_ = b / 255f;

				int cMax = Math.Max(Math.Max(r, g), b);
				int cMin = Math.Min(Math.Min(r, g), b);
				float delta = (cMax - cMin) / 255f;

				if (r == cMax) {
					int hueResult = (int)MathF.Round(60 * (((g_ - b_) / delta)));
					hueResult += hueResult < 0 ? 360 : 0;
					return hueResult;
				}
				if (g == cMax) {
					return (int)MathF.Round(60 * (((b_ - r_) / delta) + 2));
				}
				return (int)MathF.Round(60 * (((r_ - g_) / delta) + 4));
			}
			set {
				Color copyCol = FromHSV(value, s, v);
				r = copyCol.r;
				g = copyCol.g;
				b = copyCol.b;
			}
		}
		public int s {
			get {

				float cMax = Math.Max(Math.Max(r, g), b) / 255f;
				float cMin = Math.Min(Math.Min(r, g), b) / 255f;
				float delta = cMax - cMin;

				if (cMax == 0f) { return 0; }
				return (int)MathF.Round(255 * delta / cMax);

			}
			set {
				Color copyCol = FromHSV(h, value, v);
				r = copyCol.r;
				g = copyCol.g;
				b = copyCol.b;
			}
		}
		public int v {
			get {
				return Math.Max(Math.Max(r, g), b);
			}
			set {
				Color copyCol = FromHSV(h, s, value);
				r = copyCol.r;
				g = copyCol.g;
				b = copyCol.b;
			}
		}

		public Color() {
			r = 0x00;
			g = 0x00;
			b = 0x00;
			a = 0x00;
		}

		public Color(byte r, byte g, byte b, byte a) {
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}
		public Color(byte r, byte g, byte b) {
			this.r = r;
			this.g = g;
			this.b = b;
			a = 0xff;
		}

		public Color(float r, float g, float b, float a) {
			this.r = (byte)(r * 255f);
			this.g = (byte)(g * 255f);
			this.b = (byte)(b * 255f);
			this.a = (byte)(a * 255f);
		}
		public Color(float r, float g, float b) {
			this.r = (byte)MathF.Round(r * 255f);
			this.g = (byte)MathF.Round(g * 255f);
			this.b = (byte)MathF.Round(b * 255f);
			a = 0xff;
		}

		public Color(int r, int g, int b, int a) {
			this.r = (byte)r;
			this.g = (byte)g;
			this.b = (byte)b;
			this.a = (byte)a;
		}
		public Color(int r, int g, int b) {
			this.r = (byte)r;
			this.g = (byte)g;
			this.b = (byte)b;
			a = 0xff;
		}

		public static Color FromHSV(int h, int s, int v) {

			float h_ = (float)h % 360;
			float s_ = s / 255f;
			float v_ = v / 255f;

			float c = v_ * s_;
			float x = c * (1 - MathF.Abs(((h_ / 60f) % 2) - 1));
			float m = v_ - c;

			float r_ = 0f;
			float g_ = 0f;
			float b_ = 0f;

			if (h_ >= 0f && h_ < 60f) {
				r_ = c;
				g_ = x;
				b_ = 0f;
			}
			if (h_ >= 60f && h_ < 120f) {
				r_ = x;
				g_ = c;
				b_ = 0f;
			}
			if (h_ >= 120f && h_ < 180f) {
				r_ = 0f;
				g_ = c;
				b_ = x;
			}
			if (h_ >= 180f && h_ < 240f) {
				r_ = 0f;
				g_ = x;
				b_ = c;
			}
			if (h_ >= 240f && h_ < 300f) {
				r_ = x;
				g_ = 0f;
				b_ = c;
			}
			if (h_ >= 300f && h_ < 360f) {
				r_ = c;
				g_ = 0f;
				b_ = x;
			}

			return new Color((r_ + m), (g_ + m), (b_ + m));

		}

		public static Color black { get { return new Color(0, 0, 0); } }
		public static Color grey { get { return new Color(127, 127, 127); } }
		public static Color white { get { return new Color(255, 255, 255); } }

		public static Color red { get { return new Color(255, 0, 0); } }
		public static Color orange { get { return new Color(255, 127, 0); } }
		public static Color yellow { get { return new Color(255, 255, 0); } }
		public static Color lime { get { return new Color(127, 255, 0); } }
		public static Color green { get { return new Color(0, 255, 0); } }
		public static Color teal { get { return new Color(0, 255, 127); } }
		public static Color cyan { get { return new Color(0, 255, 255); } }
		public static Color aqua { get { return new Color(0, 127, 255); } }
		public static Color blue { get { return new Color(0, 0, 255); } }
		public static Color purple { get { return new Color(127, 0, 255); } }
		public static Color magenta { get { return new Color(255, 0, 255); } }
		public static Color pink { get { return new Color(255, 0, 127); } }

		public static implicit operator Vector4(Color col) {
			return new Vector4(col.r / 255f, col.g / 255f, col.b / 255f, col.a / 255f);
		}
		public static implicit operator Color(Vector4 vec) {
			return new Color(vec.x / 255f, vec.y / 255f, vec.z / 255f, vec.w / 255f);
		}
		public static implicit operator System.Drawing.Color(Color d) {
			return System.Drawing.Color.FromArgb(d.r, d.g, d.b);
		}

	}
}
