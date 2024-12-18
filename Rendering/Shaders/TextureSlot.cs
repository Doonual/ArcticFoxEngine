using SharpDX.Direct3D12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Rendering {

	public class TextureSlot {

		public ShaderVisibility shaderVisibility;
		public int rootParameterIndex;

		public GpuDescriptorHandle currentDescriptorLocation;

		public TextureSlot(ShaderVisibility shaderVisibility) {

			this.shaderVisibility = shaderVisibility;
			rootParameterIndex = -1;
			currentDescriptorLocation = new GpuDescriptorHandle();

		}

		public void SetTexture(Texture texture) {

			// Copy the descriptors
			int destDescPos = Rendering.ReserveDescriptorHeapSpace(1);
			CpuDescriptorHandle destDescriptor = Rendering.gpuDescriptorHeap.CPUDescriptorHandleForHeapStart + destDescPos * Rendering.descriptorHeapIncrement;
			CpuDescriptorHandle srcDescriptor = texture.descriptorHeap.CPUDescriptorHandleForHeapStart;
			Graphics.device.CopyDescriptorsSimple(1, destDescriptor, srcDescriptor, DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);

			// Tell the dataslot where to find the descriptors
			currentDescriptorLocation = Rendering.gpuDescriptorHeap.GPUDescriptorHandleForHeapStart + destDescPos * Rendering.descriptorHeapIncrement;

			Rendering.cmdList.SetGraphicsRootDescriptorTable(rootParameterIndex, currentDescriptorLocation);

		}

	}

}
