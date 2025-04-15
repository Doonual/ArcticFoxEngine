using CoolClassLibrary;
using SharpDX.Direct3D12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine {
	public abstract class GraphicsResource {

		internal abstract Resource GetResource();
		internal abstract int[] GetLength();
		internal virtual CpuDescriptorHandle GetSRVDescriptorLocation() {
			throw new NotImplementedException();
		}
		internal virtual CpuDescriptorHandle GetUAVDescriptorLocation() {
			throw new NotImplementedException();
		}
		internal virtual CpuDescriptorHandle GetCBVDescriptorLocation() {
			throw new NotImplementedException();
		}

	}
}
