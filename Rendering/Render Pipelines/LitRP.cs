
namespace ArcticFoxEngine.Rendering.Render_Pipelines {
	public class LitRenderPipeline : RenderPipeline {

		public override string name => "Lit";

		public LitRenderPipeline() {

			CreateDataSlot("Sun dir", SharpDX.Direct3D12.ShaderVisibility.Pixel);


			//Finalise();

		}

		public override Material GetDefaultMaterial() {
			return new LitMaterial();
		}

	}

	public class LitMaterial : Material {

		public override void BindResources(RenderPipeline renderPipeline) {
			throw new NotImplementedException();
		}

		public override void Debug() {
			throw new NotImplementedException();
		}
	}

}
