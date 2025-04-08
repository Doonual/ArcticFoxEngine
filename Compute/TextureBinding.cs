using ArcticFoxEngine.Render;
using SharpDX.Direct3D12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Compute {
	internal class TextureBinding {

		internal int rootParameterIndex;
		internal Texture boundTexture;

		public TextureBinding(int rootParameterIndex) {

			this.rootParameterIndex = rootParameterIndex;
			boundTexture = null;

		}

		public void AssignTexture(Texture texture) {

			boundTexture = texture;

		}

		public void BindTexture(GraphicsCommandList cmdList) {

			GpuDescriptorHandle destGpuDescriptor = RenderEngine.CopyDescriptorsIn(boundTexture.GetUAVDescriptorLocation(), 1);
			cmdList.SetComputeRootDescriptorTable(rootParameterIndex, destGpuDescriptor);

		}

		public void ResourceTransitionToUA(GraphicsCommandList cmdList) {

			cmdList.ResourceBarrierTransition(boundTexture.resource, Texture.defaultState, ResourceStates.UnorderedAccess);

		}
		public void ResourceTransitionFromUA(GraphicsCommandList cmdList) {

			cmdList.ResourceBarrierTransition(boundTexture.resource, ResourceStates.UnorderedAccess, Texture.defaultState);

		}


	}
}
