using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Backend.RenderImGui {
	internal static class NodeIconBank {

		static Dictionary<string, IntPtr> loadedTextures;

		internal static void Init() {

			loadedTextures = new Dictionary<string, IntPtr>();

		}
		internal static IntPtr LoadIcon(string path) {

			if (loadedTextures.ContainsKey(path) == false) {
                IntPtr id = ArcticFoxEngine.RenderImGui.CreateImageTexture(SixLabors.ImageSharp.Image.Load<Rgba32>(path), SharpDX.DXGI.Format.R8G8B8A8_UNorm);
				loadedTextures.Add(path, id);
				return id;
			}

			return loadedTextures[path];

		}

	}
}
