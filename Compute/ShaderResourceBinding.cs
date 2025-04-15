using ArcticFoxEngine.Render;
using SharpDX.Direct3D12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Compute {
	internal class ShaderResourceBinding {

		internal int registerIndex;
		internal int rootParameterIndex;
		internal GraphicsResource boundResource;

		public ShaderResourceBinding(int registerIndex) {
			this.registerIndex = registerIndex;
			boundResource = null;
		}

		public void AssignRootParameterIndex(int rootParameterIndex) {
			this.rootParameterIndex = rootParameterIndex;
		}

		public void AssignResource(GraphicsResource texture) {
			boundResource = texture;
		}

		public void BindResource(GraphicsCommandList cmdList) {

			GpuDescriptorHandle destGpuDescriptor = RenderEngine.CopyDescriptorsIn(boundResource.GetSRVDescriptorLocation(), 1);
			cmdList.SetComputeRootDescriptorTable(rootParameterIndex, destGpuDescriptor);

		}

		internal void ResourceTransitionToGenericRead(GraphicsCommandList cmdList) {
			cmdList.ResourceBarrierTransition(boundResource.GetResource(), Texture.defaultState, ResourceStates.GenericRead);
		}
		internal void ResourceTransitionFromGenericRead(GraphicsCommandList cmdList) {
			cmdList.ResourceBarrierTransition(boundResource.GetResource(), ResourceStates.GenericRead, Texture.defaultState);
		}

	}
}
