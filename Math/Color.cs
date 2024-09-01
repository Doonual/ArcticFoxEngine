using SharpDX;

namespace ArcticFoxEngine {
	public struct Color {

		public byte r;
		public byte g;
		public byte b;

		public byte a;

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
