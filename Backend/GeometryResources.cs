using SharpDX;

namespace ArcticFoxEngine {
	using ArcticFoxEngine.Backend;
	using ArcticFoxEngine.Components;
	using CoolClassLibrary;
	using SharpDX.Direct3D12;
	using Swan;

	public class GeometryResources {

		internal List<MeshRenderer> meshFilters;
		
		internal int[] vertexGap;
		internal Resource vertexBuffer;
		internal VertexBufferView vertexBufferView;
		
		internal int[] indexGap;
		internal Resource indexBuffer;
		internal IndexBufferView indexBufferView;

		internal int[] objectGap;
		internal ConstBuffer<ObjectInfo> objectBuffer;

		GPU_Upload gpuUpload;

		
		public GeometryResources() {

			meshFilters = new List<MeshRenderer>();
			gpuUpload = new GPU_Upload();

			int vbElements = 2048;
			int ibElements = 2048;
			int obElements = 2048;

			// Create gap arrays
			vertexGap = new int[vbElements];
			for (int i = 0; i < vbElements; i++) {
				vertexGap[i] = -1;
			}
			indexGap = new int[ibElements];
			for (int i = 0; i < ibElements; i ++) {
				indexGap[i] = -1;
			}
			objectGap = new int[obElements];
			for (int i = 0; i < obElements; i++) {
				objectGap[i] = -1;
			}
			
			// Create buffer resources
			vertexBuffer = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, ResourceDescription.Buffer(vbElements * Utilities.SizeOf<Vertex>()), ResourceStates.GenericRead);
			vertexBufferView.BufferLocation = vertexBuffer.GPUVirtualAddress;
			vertexBufferView.StrideInBytes = Utilities.SizeOf<Vertex>();
			vertexBufferView.SizeInBytes = vbElements * Utilities.SizeOf<Vertex>();

			indexBuffer = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, ResourceDescription.Buffer(ibElements * sizeof(int)), ResourceStates.IndexBuffer);
			indexBufferView.BufferLocation = indexBuffer.GPUVirtualAddress;
			indexBufferView.SizeInBytes = ibElements * sizeof(int);
			indexBufferView.Format = SharpDX.DXGI.Format.R32_UInt;

			objectBuffer = new ConstBuffer<ObjectInfo>(obElements);
			objectBuffer.AddToDescriptorHeap(GraphicsResources.combinedDescriptorHeap, 1); // Offset by 1 to leave the render info

		}


		public (int vbStartIndex, int ibStartIndex, int obStartIndex) AddMesh(MeshRenderer meshFilter) {

			meshFilters.Add(meshFilter);

			// Find suitable space for mesh data insertion
			int vbStartIndex = FindGap(vertexGap, meshFilter.mesh.vertices.Length);
			int ibStartIndex = FindGap(indexGap, meshFilter.mesh.indices.Length);
			int obStartIndex = FindGap(objectGap, 1);

			// Shortcuts to the mesh data
			Vertex[] vertices = meshFilter.mesh.vertices;
			int[] indices = meshFilter.mesh.indices;
			ObjectInfo[] objectInfo = new ObjectInfo[] { meshFilter.GetObjectInfo() };

			// Mark each gap as being occupied by data
			for (int i = 0; i < vertices.Length; i ++) {
				vertexGap[i + vbStartIndex] = 0;
			}
			for (int i = 0; i < indices.Length; i ++) {
				indexGap[i + ibStartIndex] = 0;
			}
			for (int i = 0; i < objectInfo.Length; i ++) {
				objectGap[i + obStartIndex] = 0;
			}
			

			#region Upload data to GPU

			int[] indicesOffset = new int[indices.Length];
			for (int i = 0; i < indices.Length; i++) {
				indicesOffset[i] = indices[i];
			}
			
			int vertexUploadBufferSize = Utilities.SizeOf<Vertex>() * vertices.Length;
			gpuUpload.UploadContext(vertexUploadBufferSize);
			Utilities.Write(gpuUpload.cpuAddress, vertices, 0, vertices.Length);
			gpuUpload.commandList.CopyBufferRegion(vertexBuffer, vbStartIndex * Utilities.SizeOf<Vertex>(), gpuUpload.uploadBuffer, 0, vertexUploadBufferSize);
			gpuUpload.EndUpload();
			
			int indexUploadBufferSize = Utilities.SizeOf<int>() * indicesOffset.Length;
			gpuUpload.UploadContext(indexUploadBufferSize);
			Utilities.Write(gpuUpload.cpuAddress, indicesOffset, 0, indicesOffset.Length);
			gpuUpload.commandList.CopyBufferRegion(indexBuffer, ibStartIndex * Utilities.SizeOf<int>(), gpuUpload.uploadBuffer, 0, indexUploadBufferSize);
			gpuUpload.EndUpload();

			#endregion

			return (vbStartIndex, ibStartIndex, obStartIndex);
			
		}
		public void RemoveMesh(MeshRenderer meshFilter) {

			// When we remove a mesh, this will create an empty space
			// [X, X, X, X, X, X, X, X, X] to
			// [X, X, X, X, -, -, -, X, X]
			// this gap is not good.
			//
			// Make an array of ints of the same length of each of the vertex and index buffers
			// This array will store how many empty spaces there are until an occupied space
			// [X, X, X, X, -, -, -, X, X]
			// [0, 0, 0, 0, 3, 2, 1, 0, 0]
			// In the case where there are no more occupied spaces, the gap array is -1
			// [X, X, X, X, -, -, -, -, -]
			// [0, 0, 0, 0, -, -, -, -, -] (the full -1 isnt shown)
			//
			// Use this gap array to determine where to place new mesh data
			//
			// The gap array is calculated like this
			// When a chunk of data is removed:
			// 1. Start in the last cell in the gap array to be removed
			// 2. Check the value of the cell to the right:
			//		If the cell is outside the array or its value is -1
			//		set the value of the current cell to -1
			//		
			//		Otherwise set the value of the current cell to 1 +
			//		the value of the cell to the right
			// 3. Move one cell to the left
			// 4. Repeat steps 2 and 3 until you have reached an occupied space, or the start of the buffer
			//
			// This algorithm will give you an array of ints describing the size of each gap in memory.
			// When a gap is created the algorithm marks its size and position
			// When a gap joins multiple gaps together, it merges them

			meshFilters.Remove(meshFilter);


			// Find the start of the mesh within the buffers
			int vbStartIndex = meshFilter.vbStartIndex;
			int ibStartIndex = meshFilter.ibStartIndex;
			int obStartIndex = meshFilter.obStartIndex;

			int numVerts = meshFilter.mesh.vertices.Length;
			int numIndex = meshFilter.mesh.indices.Length;
			int numObjs = 1;

			// Mark the data as just removed but still to be propagated
			for (int i = 0; i < numVerts; i ++) {
				vertexGap[i + vbStartIndex] = -2;
			}
			for (int i = 0; i < numIndex; i ++) {
				indexGap[i + ibStartIndex] = -2;
			}
			for (int i = 0; i < numObjs; i++) {
				objectGap[i + obStartIndex] = -2;
			}

			// Propagate gaps
			for (int i = vbStartIndex + numVerts - 1; i >= 0; i --) {
				if (vertexGap[i] == 0) { break; }
				if (i + 1 == vertexGap.Length || vertexGap[i + 1] == -1) {
					vertexGap[i] = -1;
				}
				else {
					vertexGap[i] = vertexGap[i + 1] + 1;
				}
			}
			for (int i = ibStartIndex + numIndex - 1; i >= 0; i--) {
				if (indexGap[i] == 0) { break; }
				if (i + 1 == indexGap.Length || indexGap[i + 1] == -1) {
					indexGap[i] = -1;
				}
				else {
					indexGap[i] = indexGap[i + 1] + 1;
				}
			}
			for (int i = obStartIndex + numObjs - 1; i >= 0; i--) {
				if (objectGap[i] == 0) { break; }
				if (i + 1 == objectGap.Length || objectGap[i + 1] == -1) {
					objectGap[i] = -1;
				}
				else {
					objectGap[i] = objectGap[i + 1] + 1;
				}
			}


			// Im pretty sure I don't need to reset the memory occupied by the removed data
			// back to 0. The GPU will just overwrite it when it needs to.

		}
		public int FindGap(int[] gapArray, int width) {

			for (int i = 0; i < gapArray.Length;) {

				if (gapArray[i] >= width || gapArray[i] == -1) {
					return i;
				}
				i += gapArray[i] + 1;

			}
			return -1;

		}

		public void UpdateObjectInfoBuffer() {

			int maxObjectInfoIndex = -1;
			
			for (int i = 0; i < meshFilters.Count; i ++) {
				if (meshFilters[i].obStartIndex > maxObjectInfoIndex) {
					maxObjectInfoIndex = meshFilters[i].obStartIndex;
				}
			}
			if (maxObjectInfoIndex == -1) {
				return;
			}

			ObjectInfo[] gpuSendInfo = new ObjectInfo[maxObjectInfoIndex + 1];
			for (int i = 0; i < meshFilters.Count; i++) {
				MeshRenderer currentMesh = meshFilters[i];
				gpuSendInfo[currentMesh.obStartIndex] = currentMesh.GetObjectInfo();
			}
			
			objectBuffer.Write(gpuSendInfo, 0);

		}

		~GeometryResources() {

			vertexBuffer.Dispose();
			indexBuffer.Dispose();

		}

	}
}
