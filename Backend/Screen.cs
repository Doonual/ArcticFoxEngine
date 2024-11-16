using SharpDX.Windows;

namespace ArcticFoxEngine {
	internal static class Screen {

		private static RenderForm window;

		internal static void Init(RenderForm window) {
			Screen.window = window;
		}


		internal static int width {
			get {
				return window.ClientSize.Width;
			}
		}
		internal static int height {
			get {
				return window.ClientSize.Height;
			}
		}

		internal static float aspectRatio {
			get {
				return (float)window.ClientSize.Width / window.ClientSize.Height;
			}
		}

	}
}
