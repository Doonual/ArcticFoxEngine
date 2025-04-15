using SharpDX;
using SharpDX.Direct3D12;

namespace ArcticFoxEngine {


	/// <summary>
	/// A constant buffer on the GPU
	/// </summary>
	/// <typeparam name="T">The type of data contained in this buffer</typeparam>
	public class ConstantBuffer<T> : GraphicsResource where T : struct {

		bool disposed = true;

		private DescriptorHeap descriptorHeap;
		internal Resource resource;
		private IntPtr constantBufferPointer;

		public readonly int numElements;
		public readonly int stride;
		public readonly long size;


		/// <summary>
		/// Creates a new constant buffer
		/// </summary>
		/// <param name="numElements">The number of elements of type T the buffer can store</param>
		public ConstantBuffer(int numElements) {

			disposed = false;

			this.numElements = numElements;
			this.stride = (Utilities.SizeOf<T>() + 255) & ~255; // CB size is required to be 256-byte aligned
			size = stride * numElements;

			

			// Allocate memory on the heap for the constant buffer
			resource = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(size), ResourceStates.GenericRead);
			constantBufferPointer = resource.Map(0);

			PrepareDescriptorHeap();

		}

		private void PrepareDescriptorHeap() {

			DescriptorHeapDescription dhd = new DescriptorHeapDescription() {
				DescriptorCount = numElements,
				Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView,
			};
			descriptorHeap = Graphics.device.CreateDescriptorHeap(dhd);

			for (int i = 0; i < numElements; i++) {
				ConstantBufferViewDescription cbvDesc = new ConstantBufferViewDescription() {
					BufferLocation = resource.GPUVirtualAddress + stride * i,
					SizeInBytes = stride,
				};
				Graphics.device.CreateConstantBufferView(cbvDesc, descriptorHeap.CPUDescriptorHandleForHeapStart + i * Graphics.descriptorHeapIncrement);
			}

		}

		internal override Resource GetResource() {
			return resource;
		}
		internal override int[] GetLength() {
			return new int[] { numElements};
		}
		internal override CpuDescriptorHandle GetCBVDescriptorLocation() {
			return descriptorHeap.CPUDescriptorHandleForHeapStart;
		}

		/// <summary>
		/// Writes data to the constant buffer
		/// </summary>
		/// <param name="data">The data to be written to the constant buffer</param>
		/// <param name="offset">The the position of the 1st element</param>
		public void Write(T[] data, int offset) {
			// Writes the T[] array to the buffer, ensuring each element starts at a 256 byte aligned location
			for (int i = 0; i < data.Length; i++) {
				Utilities.Write(constantBufferPointer + (offset + i) * stride, new T[] { data[i] }, 0, 1);
			}
		}

		/// <summary>
		/// Writes data to the constant buffer
		/// </summary>
		/// <param name="data">The data to be written to the constant buffer</param>
		/// <param name="offset">The the position of the 1st element</param>
		public void Write(T data, int offset) {
			// Writes the T array to the buffer, ensuring each element starts at a 256 byte aligned location
			Write(new T[] { data }, offset);
		}



		public void Dispose() {
			if (disposed == true) { return; }
			disposed = true;

			if (resource.IsDisposed == false) {
				resource.Unmap(0);
			}
			resource.Dispose();
			descriptorHeap.Dispose();

		}
		~ConstantBuffer() {
			Dispose();
		}

	}
}
