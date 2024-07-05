using SharpDX;

namespace ArcticFoxEngine {

	using SharpDX.Direct3D12;

	public class GeometryInfo {


		internal Vertex[] vertexData;
		internal Resource vertexBuffer;
		internal VertexBufferView vertexBufferView;

		internal int[] indexData;
		internal Resource indexBuffer;
		internal IndexBufferView indexBufferView;


		public GeometryInfo(Vertex[] vertexData, int[] indexData) {

			this.vertexData = vertexData;
			this.indexData = indexData;


			if (vertexBuffer != null) {
				vertexBuffer.Dispose();
			}
			if (indexBuffer != null) {
				indexBuffer.Dispose();
			}

			IntPtr pDataBegin;

			// Note: using upload heaps to transfer static data like vert buffers is not 
			// recommended. Every time the GPU needs it, the upload heap will be marshalled 
			// over. Please read up on Default Heap usage. An upload heap is used here for 
			// code simplicity and because there are very few verts to actually transfer.
			int vertexBufferSize = Utilities.SizeOf(vertexData);
			vertexBuffer = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(vertexBufferSize), ResourceStates.GenericRead);

			pDataBegin = vertexBuffer.Map(0);
			Utilities.Write(pDataBegin, vertexData, 0, vertexData.Length);
			vertexBuffer.Unmap(0);

			vertexBufferView.BufferLocation = vertexBuffer.GPUVirtualAddress;
			vertexBufferView.StrideInBytes = Utilities.SizeOf<Vertex>();
			vertexBufferView.SizeInBytes = vertexBufferSize;

			int indexBufferSize = Utilities.SizeOf(indexData);
			indexBuffer = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(indexBufferSize), ResourceStates.GenericRead);
			pDataBegin = indexBuffer.Map(0);
			Utilities.Write(pDataBegin, indexData, 0, indexData.Length);
			indexBuffer.Unmap(0);

			indexBufferView.BufferLocation = indexBuffer.GPUVirtualAddress;
			indexBufferView.SizeInBytes = indexBufferSize;
			indexBufferView.Format = SharpDX.DXGI.Format.R32_UInt;


		}

		~GeometryInfo() {

			vertexBuffer.Dispose();
			indexBuffer.Dispose();

		}

	}
}
