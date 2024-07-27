using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Backend {

	// Input structs must be 256 byte aligned!!

	internal struct RenderInfo {

		public Matrix projectionMatrix;	// 64 bytes

		public int screenWidth;			// 4 bytes
		public int screenHeight;		// 4 bytes
		public float aspectRatio;       // 4 bytes

	};

	internal struct ObjectInfo {

		public Matrix transformationMatrix;	// 64 bytes


	};

}
