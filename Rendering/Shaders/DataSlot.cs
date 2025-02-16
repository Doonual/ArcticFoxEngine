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

			CpuDescriptorHandle srcDescriptor = dataSource.descriptorHeap.CPUDescriptorHandleForHeapStart + sourceIndex * Rendering.descriptorHeapIncrement;
			GpuDescriptorHandle destDescriptor = Rendering.CopyDescriptorsIn(srcDescriptor, 1);

			Rendering.cmdList.SetGraphicsRootDescriptorTable(rootParameterIndex, destDescriptor);

		}

	}

}
