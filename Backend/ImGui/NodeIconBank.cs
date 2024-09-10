using SixLabors.ImageSharp.PixelFormats;

namespace ArcticFoxEngine.ImGuiIntegration {
	internal static class NodeIconBank {

		static Dictionary<string, IntPtr> loadedTextures;

		internal static void Init() {

			loadedTextures = new Dictionary<string, IntPtr>();

		}
		internal static IntPtr LoadIcon(string path) {

			if (loadedTextures.ContainsKey(path) == false) {
				IntPtr id = RenderImGui.CreateImageTexture(SixLabors.ImageSharp.Image.Load<Rgba32>(path), SharpDX.DXGI.Format.R8G8B8A8_UNorm);
				loadedTextures.Add(path, id);
				return id;
			}

			return loadedTextures[path];

		}

	}
}
