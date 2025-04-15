using ArcticFoxEngine.Render;
using SharpDX.Direct3D12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Compute {
	internal class UnorderedAccessBinding {

		internal int registerIndex;
		internal int rootParameterIndex;
		internal GraphicsResource boundResource;

		public UnorderedAccessBinding(int registerIndex) {
			this.registerIndex = registerIndex;
			
			boundResource = null;
		}
		public void AssignRootParameterIndex(int rootParameterIndex) {
			this.rootParameterIndex = rootParameterIndex;
		}

		public void AssignResource(GraphicsResource resource) {
			boundResource = resource;
		}

		public void BindResource(GraphicsCommandList cmdList) {

			GpuDescriptorHandle destGpuDescriptor = RenderEngine.CopyDescriptorsIn(boundResource.GetUAVDescriptorLocation(), 1);
			cmdList.SetComputeRootDescriptorTable(rootParameterIndex, destGpuDescriptor);

		}

		public void ResourceTransitionToUA(GraphicsCommandList cmdList) {
			cmdList.ResourceBarrierTransition(boundResource.GetResource(), Texture.defaultState, ResourceStates.UnorderedAccess);
		}
		public void ResourceTransitionFromUA(GraphicsCommandList cmdList) {
			cmdList.ResourceBarrierTransition(boundResource.GetResource(), ResourceStates.UnorderedAccess, Texture.defaultState);
		}


	}
}
