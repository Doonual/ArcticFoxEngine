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

		private bool compute;

		public BufferSlot(int length, ShaderVisibility visibility, bool compute = false) {
			shaderVisibility = visibility;
			this.length = length;
			rootParameterIndex = -1;
			currentDescriptorLocation = new GpuDescriptorHandle();
			this.compute = compute;
		}

		public void SetBuffer<T>(StructuredBuffer<T> buffer, int srcOffset) where T : struct {

			CpuDescriptorHandle srcDescriptor = buffer.descriptorHeap.CPUDescriptorHandleForHeapStart + srcOffset * Graphics.descriptorHeapIncrement;
			GpuDescriptorHandle destDescriptor = RenderEngine.CopyDescriptorsIn(srcDescriptor, buffer.numElements);

			if (compute == false) {
				RenderEngine.cmdList.SetGraphicsRootDescriptorTable(rootParameterIndex, destDescriptor);
			}
			else {
				
			}
			

		}


	}

}
