using SharpDX.Direct3D12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Rendering {

	public class DataSlot {

		public ShaderVisibility shaderVisibility;
		public int rootParameterIndex;

		public GpuDescriptorHandle currentDescriptorLocation;

		public DataSlot(ShaderVisibility visibility) {
			shaderVisibility = visibility;
			rootParameterIndex = -1;
			currentDescriptorLocation = new GpuDescriptorHandle();
		}

		public void SetData<T>(ConstBuffer<T> dataSource, int sourceIndex) where T : struct {

			// Copy the descriptors
			int destDescPos = Rendering.ReserveDescriptorHeapSpace(1);
			CpuDescriptorHandle destDescriptor = Rendering.gpuDescriptorHeap.CPUDescriptorHandleForHeapStart + destDescPos * Rendering.descriptorHeapIncrement;
			CpuDescriptorHandle srcDescriptor = dataSource.descriptorHeap.CPUDescriptorHandleForHeapStart + sourceIndex * Rendering.descriptorHeapIncrement;
			Graphics.device.CopyDescriptorsSimple(1, destDescriptor, srcDescriptor, DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);

			// Tell the dataslot where to find the descriptors
			currentDescriptorLocation = Rendering.gpuDescriptorHeap.GPUDescriptorHandleForHeapStart + destDescPos * Rendering.descriptorHeapIncrement;

			Rendering.cmdList.SetGraphicsRootDescriptorTable(rootParameterIndex, currentDescriptorLocation);

		}

	}

}
