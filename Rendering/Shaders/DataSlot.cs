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

			CpuDescriptorHandle srcDescriptor = dataSource.descriptorHeap.CPUDescriptorHandleForHeapStart + sourceIndex * Render.descriptorHeapIncrement;
			GpuDescriptorHandle destDescriptor = Render.CopyDescriptorsIn(srcDescriptor, 1);

			Render.cmdList.SetGraphicsRootDescriptorTable(rootParameterIndex, destDescriptor);

		}

	}

}
