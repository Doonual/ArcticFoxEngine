using SharpDX.Windows;

namespace ArcticFoxEngine {
	public static class Screen {

		private static RenderForm window;

		internal static void Init(RenderForm window) {
			Screen.window = window;
		}


		public static int width {
			get {
				return window.ClientSize.Width;
			}
		}
		public static int height {
			get {
				return window.ClientSize.Height;
			}
		}

		public static float aspectRatio {
			get {
				return (float)window.ClientSize.Width / window.ClientSize.Height;
			}
		}

	}
}
