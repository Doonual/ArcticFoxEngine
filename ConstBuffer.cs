using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine {
	internal class ConstBuffer<T> where T : struct {

		private Resource constantBuffer;
		internal DescriptorHeap viewHeap;
		private IntPtr constantBufferPointer;
		private T constantBufferData;

		internal ConstBuffer(long width) {

			// Describe and create a constant buffer view (CBV) descriptor heap
			// Flags indicate that this descriptor heap can be bound to the pipeline
			// and that descriptors contained in it can be refrenced by a root table
			DescriptorHeapDescription cbvHeapDesc = new DescriptorHeapDescription() {
				DescriptorCount = 1,
				Flags = DescriptorHeapFlags.ShaderVisible,
				Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView
			};
			viewHeap = Graphics.device.CreateDescriptorHeap(cbvHeapDesc);


			constantBuffer = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(width), ResourceStates.GenericRead);

			// Describe and create a constant buffer view
			ConstantBufferViewDescription cbvDesc = new ConstantBufferViewDescription() {
				BufferLocation = constantBuffer.GPUVirtualAddress,
				SizeInBytes = (Utilities.SizeOf<T>() + 255) & ~255
			};
			Graphics.device.CreateConstantBufferView(cbvDesc, viewHeap.CPUDescriptorHandleForHeapStart);

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
