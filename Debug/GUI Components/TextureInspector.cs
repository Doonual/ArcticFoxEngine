using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Debug.GUI_Components {
	public class TextureInspector {

		Texture inspectTexture;
		Vector2 viewCenter;
		float zoom;

		public TextureInspector() {
			inspectTexture = null;
			viewCenter = new Vector2(0f, 0f);
			zoom = 1f;

		}

		public void SetTexture(Texture texture) {

			inspectTexture = texture;

		}

		public void Render() {

			

		}

	}
}
