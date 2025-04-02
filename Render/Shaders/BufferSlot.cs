using SharpDX.Direct3D12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Render {

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

			CpuDescriptorHandle srcDescriptor = buffer.descriptorHeap.CPUDescriptorHandleForHeapStart + srcOffset * Rendering.descriptorHeapIncrement;
			GpuDescriptorHandle destDescriptor = Rendering.CopyDescriptorsIn(srcDescriptor, buffer.numElements);

			Rendering.cmdList.SetGraphicsRootDescriptorTable(rootParameterIndex, destDescriptor);

		}


	}

}
