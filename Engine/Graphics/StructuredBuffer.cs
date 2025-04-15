using SharpDX;
using SharpDX.Direct3D12;

namespace ArcticFoxEngine {


	/// <summary>
	/// A constant buffer on the GPU
	/// </summary>
	/// <typeparam name="T">The type of data contained in this buffer</typeparam>
	public class StructuredBuffer<T> : GraphicsResource where T : struct {

		bool disposed = true;

		internal DescriptorHeap descriptorHeap;
		internal Resource resource;

		bool allowUnorderedAccess;
		public readonly int numElements;
		public readonly int stride;
		public readonly long size;


		/// <summary>
		/// Creates a new constant buffer
		/// </summary>
		/// <param name="numElements">The number of elements of type T the buffer can store</param>
		public StructuredBuffer(int numElements, ResourceFlags flags = ResourceFlags.None) {
			disposed = false;
			this.numElements = numElements;
			this.stride = Utilities.SizeOf<T>();
			size = stride * numElements;

			allowUnorderedAccess = false;
			if ((flags & ResourceFlags.AllowUnorderedAccess) != 0) {
				allowUnorderedAccess = true;
			}


			// Allocate memory on the heap for the constant buffer
			resource = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, ResourceDescription.Buffer(size, flags), ResourceStates.GenericRead);
			
			DescriptorHeapDescription dhd = new DescriptorHeapDescription() {
				DescriptorCount = numElements * 2,
				Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView,
			};
			descriptorHeap = Graphics.device.CreateDescriptorHeap(dhd);
			PrepareDescriptorHeap();
			

		}

		/// <summary>
		/// Creates a constant buffer view on the descriptor heap of this constant buffer
		/// </summary>
		/// <param name="destDescriptorHeap">The descriptor heap to create the constant buffer view on</param>
		/// <param name="offset">The offset into the descriptor heap the constant buffer view will be created at</param>
		private void PrepareDescriptorHeap() {


			ShaderResourceViewDescription srvDesc = new ShaderResourceViewDescription() {
				Dimension = ShaderResourceViewDimension.Buffer,
				Format = SharpDX.DXGI.Format.Unknown,
				Shader4ComponentMapping = Texture.ComponentMapping(0, 1, 2, 3),
				Buffer = new ShaderResourceViewDescription.BufferResource() {
					ElementCount = (int)numElements,
					StructureByteStride = stride,
					Flags = BufferShaderResourceViewFlags.None,
				},

			};
			Graphics.device.CreateShaderResourceView(resource, srvDesc, descriptorHeap.CPUDescriptorHandleForHeapStart);

			if (allowUnorderedAccess == true) {

				UnorderedAccessViewDescription uavDesc = new UnorderedAccessViewDescription() {
					Dimension = UnorderedAccessViewDimension.Buffer,
					Format = SharpDX.DXGI.Format.Unknown,
					Buffer = new UnorderedAccessViewDescription.BufferResource() {
						ElementCount = (int)numElements,
						StructureByteStride = stride,
						Flags = BufferUnorderedAccessViewFlags.None,
					},
				};
				Graphics.device.CreateUnorderedAccessView(resource, null, uavDesc, descriptorHeap.CPUDescriptorHandleForHeapStart + Graphics.descriptorHeapIncrement);

			}


			

		}

		internal override Resource GetResource() {
			return resource;
		}
		internal override int[] GetLength() {
			return new int[] { numElements };
		}
		internal override CpuDescriptorHandle GetSRVDescriptorLocation() {
			return descriptorHeap.CPUDescriptorHandleForHeapStart;
		}
		internal override CpuDescriptorHandle GetUAVDescriptorLocation() {
			return descriptorHeap.CPUDescriptorHandleForHeapStart + Graphics.descriptorHeapIncrement;
		}


		/// <summary>
		/// Writes data to the constant buffer
		/// </summary>
		/// <param name="data">The data to be written to the constant buffer</param>
		/// <param name="offset">The the position of the 1st element</param>
		public void Write(T[] data, int offset) {

			Resource uploadResource = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(data.Length * Utilities.SizeOf<T>()), ResourceStates.GenericRead);
			IntPtr structuredBufferPointer = uploadResource.Map(0);

			// Writes the T[] array to the buffer, ensuring each element starts at a 256 byte aligned location
			Utilities.Write(structuredBufferPointer, data, offset, data.Length);

			Graphics.BlitBuffer(uploadResource, resource, data.Length * Utilities.SizeOf<T>(), 0, offset);
			Graphics.WaitForDirectCommandQueue();
			resource.Unmap(0);
			uploadResource.Dispose();

		}

		public void Dispose() {
			if (disposed == true) { return; }
			disposed = true;

			resource.Unmap(0);
			resource.Dispose();
			descriptorHeap.Dispose();

		}
		~StructuredBuffer() {
			Dispose();
		}

	}
}
