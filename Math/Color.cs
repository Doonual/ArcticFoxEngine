using CoolClassLibrary;
using SharpDX;

namespace ArcticFoxEngine {
	public struct Color {

		public float r;
		public float g;
		public float b;
		public float a;

		public Color() {
			r = 0f;
			g = 0f;
			b = 0f;
			a = 0f;
		}

		public Color(byte r, byte g, byte b, byte a) {
			this.r = r / 255f;
			this.g = g / 255f;
			this.b = b / 255f;
			this.a = a / 255f;
		}
		public Color(byte r, byte g, byte b) {
			this.r = r / 255f;
			this.g = g / 255f;
			this.b = b / 255f;
			a = 1f;
		}

		public Color(float r, float g, float b, float a) {
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}
		public Color(float r, float g, float b) {
			this.r = r;
			this.g = g;
			this.b = b;
			a = 255f;
		}

		public Color(int r, int g, int b, int a) {
			this.r = r / 255f;
			this.g = g / 255f;
			this.b = b / 255f;
			this.a = a / 255f;
		}
		public Color(int r, int g, int b) {
			this.r = r / 255f;
			this.g = g / 255f;
			this.b = b / 255f;
			a = 1f;
		}

		/// <summary>
		/// Creates a color object based on the HSV color model
		/// </summary>
		/// <param name="h">Hue [0-359]</param>
		/// <param name="s">Saturation [0-255]</param>
		/// <param name="v">Value [0-255]</param>
		/// <returns></returns>
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
			return System.Drawing.Color.FromArgb((int)MathF.Round(d.r * 255), (int)MathF.Round(d.g * 255), (int)MathF.Round(d.b * 255));
		}

	}
}
