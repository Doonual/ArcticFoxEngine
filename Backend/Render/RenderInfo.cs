using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Backend.Render {
	internal struct RenderInfo {

		public Matrix projectionMatrix; // 64 bytes

		public int screenWidth;         // 4 bytes
		public int screenHeight;        // 4 bytes
		public float aspectRatio;      // 4 bytes

	};
}
