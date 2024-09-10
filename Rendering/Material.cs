using ArcticFoxEngine.Backend;
using SharpDX.Direct3D12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Rendering {
	public abstract class Material {

		public abstract void BindResources(RenderPipeline renderPipeline);

		public abstract void Debug();

	}
}
