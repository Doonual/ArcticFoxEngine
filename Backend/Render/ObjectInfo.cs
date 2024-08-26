using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Backend.Render {

	// Input structs must be 256 byte aligned!!

	internal struct ObjectInfo {
		public Matrix transformationMatrix; // 64 bytes
	};
}
