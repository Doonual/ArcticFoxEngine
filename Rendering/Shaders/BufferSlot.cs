using SharpDX.Direct3D12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Rendering {

	public class BufferSlot {

		public ShaderVisibility shaderVisibility;
		public int length;
		public int rootParameterIndex;
		public GpuDescriptorHandle currentDescriptorLocation;

		public BufferSlot(int length, ShaderVisibility visibility) {
			shaderVisibility = visibility;
			this.length = length;
			rootParameterIndex = -1;
			currentDescriptorLocation = new GpuDescriptorHandle();
		}

		public void SetBuffer<T>(StructuredBuffer<T> buffer, int srcOffset) where T : struct {

			// Copy the descriptors
			int destDescPos = Rendering.ReserveDescriptorHeapSpace(1);
			CpuDescriptorHandle destDescriptor = Rendering.gpuDescriptorHeap.CPUDescriptorHandleForHeapStart + destDescPos * Rendering.descriptorHeapIncrement;
			CpuDescriptorHandle srcDescriptor = buffer.descriptorHeap.CPUDescriptorHandleForHeapStart + srcOffset * Rendering.descriptorHeapIncrement;
			Graphics.device.CopyDescriptorsSimple(1, destDescriptor, srcDescriptor, DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);


			// Tell the bufferSlot where to find the descriptors
			currentDescriptorLocation = Rendering.gpuDescriptorHeap.GPUDescriptorHandleForHeapStart + destDescPos * Rendering.descriptorHeapIncrement;

			Rendering.cmdList.SetGraphicsRootDescriptorTable(rootParameterIndex, currentDescriptorLocation);

		}


	}

}
