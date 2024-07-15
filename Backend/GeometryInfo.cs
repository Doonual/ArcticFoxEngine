using SharpDX;

namespace ArcticFoxEngine {
	using ArcticFoxEngine.Backend;
	using CoolClassLibrary;
	using SharpDX.Direct3D12;

	public class GeometryInfo {


		private int vertexBufferWritePos;
		internal Vertex[] vertexData;
		internal Resource vertexBuffer;
		internal VertexBufferView vertexBufferView;
		
		private int indexBufferWritePos;
		internal int[] indexData;
		internal Resource indexBuffer;
		internal IndexBufferView indexBufferView;

		GPU_Upload gpuUpload;

		public GeometryInfo() {

			gpuUpload = new GPU_Upload();

			int vertexBufferSize = 1024;
			vertexData = new Vertex[vertexBufferSize];
			vertexBuffer = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, ResourceDescription.Buffer(vertexBufferSize), ResourceStates.GenericRead);
			vertexBufferView.BufferLocation = vertexBuffer.GPUVirtualAddress;
			vertexBufferView.StrideInBytes = Utilities.SizeOf<Vertex>();
			vertexBufferView.SizeInBytes = vertexBufferSize;

			int indexBufferSize = 1024;
			indexData = new int[indexBufferSize];
			indexBuffer = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, ResourceDescription.Buffer(indexBufferSize), ResourceStates.IndexBuffer);
			indexBufferView.BufferLocation = indexBuffer.GPUVirtualAddress;
			indexBufferView.SizeInBytes = indexBufferSize;
			indexBufferView.Format = SharpDX.DXGI.Format.R32_UInt;


		}

		public void AddMeshData(Vertex[] vertices, int[] indices) {

			Array.Copy(vertices, 0, vertexData, vertexBufferWritePos, vertices.Length);
			Array.Copy(indices, 0, indexData, indexBufferWritePos, indices.Length);

			int sourceVertexBufferSize = Utilities.SizeOf<Vertex>() * vertices.Length;
			gpuUpload.UploadContext(sourceVertexBufferSize);
			Utilities.Write(gpuUpload.cpuAddress, vertices, 0, vertices.Length);
			gpuUpload.commandList.CopyBufferRegion(vertexBuffer, vertexBufferWritePos, gpuUpload.uploadBuffer, 0, sourceVertexBufferSize);
			gpuUpload.EndUpload();

			int sourceIndexBufferSize = Utilities.SizeOf<int>() * indices.Length;
			gpuUpload.UploadContext(sourceIndexBufferSize);
			Utilities.Write(gpuUpload.cpuAddress, indices, 0, indices.Length);
			gpuUpload.commandList.CopyBufferRegion(indexBuffer, indexBufferWritePos, gpuUpload.uploadBuffer, 0, sourceIndexBufferSize);
			gpuUpload.EndUpload();

			vertexBufferWritePos += vertices.Length;
			indexBufferWritePos += indices.Length;

		}

		~GeometryInfo() {

			vertexBuffer.Dispose();
			indexBuffer.Dispose();

		}

	}
}
