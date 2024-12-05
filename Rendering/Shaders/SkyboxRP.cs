using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Rendering.Render_Pipelines {
	internal class SkyboxRP : Shader {

		public SkyboxRP() {




			
		}

		public override string name => "Skybox";

		public override Material GetDefaultMaterial() {
			return new SkyboxMaterial();
		}

	}

	public class SkyboxMaterial : Material {

		public override void BindResources(Shader renderPipeline) {
			
		}

		public override void Debug() {
			
		}
	}
}
