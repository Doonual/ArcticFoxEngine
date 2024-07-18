using SharpDX.DXGI;
using SharpDX.Windows;

namespace ArcticFoxEngine {
	internal static class Screen {

		private static RenderForm window;
		private static SwapChain3 swapChain;

		internal static void InitScreen(RenderForm window, SwapChain3 swapChain) {
			Screen.window = window;
			Screen.swapChain = swapChain;
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
