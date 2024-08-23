using CoolClassLibrary;
using SharpDX;
using SharpDX.Direct3D12;

namespace ArcticFoxEngine.Backend {

	/// <summary>
	/// A constant buffer on the GPU
	/// </summary>
	/// <typeparam name="T">The type of data contained in this buffer</typeparam>
	internal class ConstBuffer<T> where T : struct {

		private Resource constantBuffer;
		private IntPtr constantBufferPointer;

		public readonly int numElements;
		public readonly int stride;
		public readonly long size;
		

		/// <summary>
		/// Creates a new constant buffer
		/// </summary>
		/// <param name="numElements">The number of elements of type T the buffer can store</param>
		internal ConstBuffer(int numElements) {

			this.numElements = numElements;
			this.stride = (Utilities.SizeOf<T>() + 255) & ~255; // CB size is required to be 256-byte aligned
			size = stride * numElements;
			// Allocate memory on the heap for the constant buffer
			constantBuffer = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(size), ResourceStates.GenericRead);

			// Initialise and map the constant buffers. We don't unmap this until the
			// app closes. Keeping things mapped for the lifetime of the resource is okay
			constantBufferPointer = constantBuffer.Map(0);

		}

		/// <summary>
		/// Creates a constant buffer view on the descriptor heap of this constant buffer
		/// </summary>
		/// <param name="destDescriptorHeap">The descriptor heap to create the constant buffer view on</param>
		/// <param name="offset">The offset into the descriptor heap the constant buffer view will be created at</param>
		internal void AddToDescriptorHeap(DescriptorHeap destDescriptorHeap, int offset) {

			for (int i = 0; i < numElements; i ++) {

				ConstantBufferViewDescription cbvDesc = new ConstantBufferViewDescription() {
					BufferLocation = constantBuffer.GPUVirtualAddress + stride * i,
					SizeInBytes = stride
				};
				Graphics.device.CreateConstantBufferView(cbvDesc, destDescriptorHeap.CPUDescriptorHandleForHeapStart + (offset + i) * RenderResources.combinedDescriptorHeapIncrement);

			}
			
		}

		/// <summary>
		/// Writes data to the constant buffer
		/// </summary>
		/// <param name="data">The data to be written to the constant buffer</param>
		/// <param name="offset">The the position of the 1st element</param>
		internal void Write(T[] data, int offset) {

			// Writes the T[] array to the buffer, ensuring each element starts at a 256 byte aligned location
			for (int i = 0; i < data.Length; i ++) {
				Utilities.Write(constantBufferPointer + (offset + i) * stride, new T[] { data[i] }, 0, 1);
			}
			
		}


		bool disposed = false;
		internal void Dispose() {
			if (disposed == true) { return; }
			disposed = true;
			constantBuffer.Unmap(0);
			constantBuffer.Dispose();
		}
		~ConstBuffer() {
			Dispose();
		}

	}
}
