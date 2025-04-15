using ArcticFoxEngine.Render;
using SharpDX.Direct3D12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Compute {
	internal class ConstantBufferBinding {

		internal int registerIndex;
		internal int rootParameterIndex;
		internal GraphicsResource boundResource;

		internal ConstantBufferBinding(int registerIndex) {
			this.registerIndex = registerIndex;
			boundResource = null;
		}

		internal void AssignRootParameterIndex(int rootParameterIndex) {
			this.rootParameterIndex = rootParameterIndex;
		}

		internal void AssignResource(GraphicsResource resource) {
			boundResource = resource;
		}

		internal void BindResource(GraphicsCommandList cmdList) {

			GpuDescriptorHandle destGpuDescriptor = RenderEngine.CopyDescriptorsIn(boundResource.GetCBVDescriptorLocation(), boundResource.GetLength()[0]);
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
