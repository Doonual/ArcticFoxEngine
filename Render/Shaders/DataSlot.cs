using SharpDX.Direct3D12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Render {

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

			CpuDescriptorHandle srcDescriptor = dataSource.descriptorHeap.CPUDescriptorHandleForHeapStart + sourceIndex * Graphics.descriptorHeapIncrement;
			GpuDescriptorHandle destDescriptor = RenderEngine.CopyDescriptorsIn(srcDescriptor, 1);

			RenderEngine.cmdList.SetGraphicsRootDescriptorTable(rootParameterIndex, destDescriptor);

		}

	}

}
