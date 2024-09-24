using SharpDX;
using SharpDX.Direct3D12;

namespace ArcticFoxEngine {


	/// <summary>
	/// A constant buffer on the GPU
	/// </summary>
	/// <typeparam name="T">The type of data contained in this buffer</typeparam>
	public class StructuredBuffer<T> where T : struct {

		bool disposed = true;

		internal DescriptorHeap descriptorHeap;
		private Resource structuredBuffer;
		private IntPtr structuredBufferPointer;

		public readonly int numElements;
		public readonly int stride;
		public readonly long size;


		/// <summary>
		/// Creates a new constant buffer
		/// </summary>
		/// <param name="numElements">The number of elements of type T the buffer can store</param>
		public StructuredBuffer(int numElements) {
			disposed = false;

			this.numElements = numElements;
			this.stride = Utilities.SizeOf<T>();
			size = stride * numElements;

			

			// Allocate memory on the heap for the constant buffer
			structuredBuffer = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(size * Utilities.SizeOf<T>()), ResourceStates.GenericRead);
			structuredBufferPointer = structuredBuffer.Map(0);

			
			DescriptorHeapDescription dhd = new DescriptorHeapDescription() {
				DescriptorCount = numElements,
				Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView,
			};
			descriptorHeap = Graphics.device.CreateDescriptorHeap(dhd);
			AddToDescriptorHeap(descriptorHeap);
			

		}

		/// <summary>
		/// Creates a constant buffer view on the descriptor heap of this constant buffer
		/// </summary>
		/// <param name="destDescriptorHeap">The descriptor heap to create the constant buffer view on</param>
		/// <param name="offset">The offset into the descriptor heap the constant buffer view will be created at</param>
		internal void AddToDescriptorHeap(DescriptorHeap destDescriptorHeap) {


			ShaderResourceViewDescription srvDesc = new ShaderResourceViewDescription() {
				Dimension = ShaderResourceViewDimension.Buffer,
				Format = SharpDX.DXGI.Format.Unknown,
				Shader4ComponentMapping = Texture.ComponentMapping(0, 1, 2, 3),
				Buffer = new ShaderResourceViewDescription.BufferResource() {
					ElementCount = (int)size,
					StructureByteStride = stride,
					Flags = BufferShaderResourceViewFlags.None,
				},
				
				
			};

			Graphics.device.CreateShaderResourceView(structuredBuffer, srvDesc, destDescriptorHeap.CPUDescriptorHandleForHeapStart);

		}

		/// <summary>
		/// Writes data to the constant buffer
		/// </summary>
		/// <param name="data">The data to be written to the constant buffer</param>
		/// <param name="offset">The the position of the 1st element</param>
		public void Write(T[] data, int offset) {
			// Writes the T[] array to the buffer, ensuring each element starts at a 256 byte aligned location
			for (int i = 0; i < data.Length; i++) {
				Utilities.Write(structuredBufferPointer + (offset + i) * stride, new T[] { data[i] }, 0, 1);
			}
		}



		public void Dispose() {
			if (disposed == true) { return; }
			disposed = true;

			structuredBuffer.Unmap(0);
			structuredBuffer.Dispose();
			descriptorHeap.Dispose();

		}
		~StructuredBuffer() {
			Dispose();
		}

	}
}
