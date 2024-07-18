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

			int vertexBufferSize = 2048;
			vertexData = new Vertex[vertexBufferSize];
			vertexBuffer = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, ResourceDescription.Buffer(vertexBufferSize), ResourceStates.GenericRead);
			vertexBufferView.BufferLocation = vertexBuffer.GPUVirtualAddress;
			vertexBufferView.StrideInBytes = Utilities.SizeOf<Vertex>();
			vertexBufferView.SizeInBytes = vertexBufferSize;

			int indexBufferSize = 2048;
			indexData = new int[indexBufferSize];
			indexBuffer = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, ResourceDescription.Buffer(indexBufferSize), ResourceStates.IndexBuffer);
			indexBufferView.BufferLocation = indexBuffer.GPUVirtualAddress;
			indexBufferView.SizeInBytes = indexBufferSize;
			indexBufferView.Format = SharpDX.DXGI.Format.R32_UInt;


		}

		public void AddMeshData(Vertex[] vertices, int[] indices) {

			int indicesStartIndex = vertexBufferWritePos;

			int[] indicesOffset = new int[indices.Length];
			for (int i = 0; i < indices.Length; i ++) {
				indicesOffset[i] = indices[i] + indicesStartIndex;
			}

			Array.Copy(vertices, 0, vertexData, vertexBufferWritePos, vertices.Length);
			Array.Copy(indicesOffset, 0, indexData, indexBufferWritePos, indicesOffset.Length);

			int vertexUploadBufferSize = Utilities.SizeOf<Vertex>() * vertices.Length;
			gpuUpload.UploadContext(vertexUploadBufferSize);
			Utilities.Write(gpuUpload.cpuAddress, vertices, 0, vertices.Length);
			gpuUpload.commandList.CopyBufferRegion(vertexBuffer, vertexBufferWritePos * Utilities.SizeOf<Vertex>(), gpuUpload.uploadBuffer, 0, vertexUploadBufferSize);
			gpuUpload.EndUpload();


			int indexUploadBufferSize = Utilities.SizeOf<int>() * indicesOffset.Length;
			gpuUpload.UploadContext(indexUploadBufferSize);
			Utilities.Write(gpuUpload.cpuAddress, indicesOffset, 0, indicesOffset.Length);
			gpuUpload.commandList.CopyBufferRegion(indexBuffer, indexBufferWritePos * Utilities.SizeOf<int>(), gpuUpload.uploadBuffer, 0, indexUploadBufferSize);
			gpuUpload.EndUpload();

			vertexBufferWritePos += vertices.Length;
			indexBufferWritePos += indicesOffset.Length;

		}

		~GeometryInfo() {

			vertexBuffer.Dispose();
			indexBuffer.Dispose();

		}

	}
}
