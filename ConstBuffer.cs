using SharpDX;
using SharpDX.Direct3D12;

namespace ArcticFoxEngine {
	internal class ConstBuffer<T> where T : struct {

		private Resource constantBuffer;
		private IntPtr constantBufferPointer;
		private T constantBufferData;

		internal ConstBuffer(long width, DescriptorHeap parentHeap) {

			// Allocate memory on the heap for the constant buffer
			constantBuffer = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(1024 * 64), ResourceStates.GenericRead);

			// Describe and create a descriptor of type constant buffer view which refrences the constant buffer
			ConstantBufferViewDescription cbvDesc = new ConstantBufferViewDescription() {
				BufferLocation = constantBuffer.GPUVirtualAddress,
				SizeInBytes = (Utilities.SizeOf<T>() + 255) & ~255 // CB size is required to be 256-byte aligned
			};
			Graphics.device.CreateConstantBufferView(cbvDesc, GraphicsResources.mainCombinedDescriporHeap.CPUDescriptorHandleForHeapStart);

			// Initialise and map the constant buffers. We don't unmap this until the
			// app closes. Keeping things mapped for the lifetime of the resource is okay
			constantBufferPointer = constantBuffer.Map(0);
			Utilities.Write(constantBufferPointer, ref constantBufferData);

		}

		internal void WriteToBuffer(T data) {
			constantBufferData = data;
			Utilities.Write(constantBufferPointer, ref constantBufferData);
		}

		internal T ReadFromBufferNOGPU() {
			// This method does not read anything from the gpu

			return constantBufferData;
		}

	}
}
