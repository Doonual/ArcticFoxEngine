using SharpDX;

namespace ArcticFoxEngine.Backend.Render {
	using ArcticFoxEngine.Backend;
	using ArcticFoxEngine.Nodes;
	using ArcticFoxEngine.Debug;
	using CoolClassLibrary;
	using SharpDX.Direct3D12;
	using SixLabors.ImageSharp.PixelFormats;
	using SixLabors.ImageSharp;
	using Swan;
	using System.IO;

	/// <summary>
	/// Contains and controls all the resources needed for rendering geometry
	/// This is the vertex buffer, index buffers and the object buffers
	/// </summary>
	public class GeometryResources {

		internal static int kbPerBuffer = 2048;

		internal static int numVbElements = kbPerBuffer * 1024 / Utilities.SizeOf<Vertex>();
		internal static int numIbElements = kbPerBuffer * 1024 / sizeof(int);
		internal static int numObElements = 128 * 1024 / Utilities.SizeOf<ObjectInfo>();

		internal List<MeshRenderer> meshRenderers;
		internal List<(int vbStart, int ibStart, int obStart)> meshRendererPositions;
		
		internal int[] vertexGap;
		internal Resource vertexBuffer;
		internal VertexBufferView vertexBufferView;
		
		internal int[] indexGap;
		internal Resource indexBuffer;
		internal IndexBufferView indexBufferView;

		internal int[] objectGap;
		internal ConstBuffer<ObjectInfo> objectBuffer;


		/// <summary>
		/// Creates a new instance GeometryResources
		/// </summary>
		public GeometryResources() {

			meshRenderers = new List<MeshRenderer>();
			meshRendererPositions = new List<(int vbStart, int ibStart, int obStart)>();

			// Create gap arrays
			vertexGap = new int[numVbElements];
			for (int i = 0; i < numVbElements; i++) {
				vertexGap[i] = -1;
			}
			indexGap = new int[numIbElements];
			for (int i = 0; i < numIbElements; i ++) {
				indexGap[i] = -1;
			}
			objectGap = new int[numObElements];
			for (int i = 0; i < numObElements; i++) {
				objectGap[i] = -1;
			}
			
			// Create buffer resources
			vertexBuffer = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, ResourceDescription.Buffer(numVbElements * Utilities.SizeOf<Vertex>()), ResourceStates.GenericRead);
			vertexBufferView.BufferLocation = vertexBuffer.GPUVirtualAddress;
			vertexBufferView.StrideInBytes = Utilities.SizeOf<Vertex>();
			vertexBufferView.SizeInBytes = numVbElements * Utilities.SizeOf<Vertex>();

			indexBuffer = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, ResourceDescription.Buffer(numIbElements * sizeof(int)), ResourceStates.IndexBuffer);
			indexBufferView.BufferLocation = indexBuffer.GPUVirtualAddress;
			indexBufferView.SizeInBytes = numIbElements * sizeof(int);
			indexBufferView.Format = SharpDX.DXGI.Format.R32_UInt;

			objectBuffer = new ConstBuffer<ObjectInfo>(numObElements);

		}

		/// <summary>
		/// Adds a mesh to GeometryResources
		/// </summary>
		/// <param name="meshRenderer">The mesh renderer to be added</param>
		/// <returns>Whether the mesh was added successfully</returns>
		public bool AddMesh(MeshRenderer meshRenderer) {
			if (meshRenderer.mesh == null) { Log.Error("Failed to add mesh renderer, no mesh"); return false; }

			// Find suitable space for mesh data insertion
			int vbStartIndex = FindGap(vertexGap, meshRenderer.mesh.vertices.Length);
			int ibStartIndex = FindGap(indexGap, meshRenderer.mesh.indices.Length);
			int obStartIndex = FindGap(objectGap, 1);

			// If there is no suitable space for mesh data
			if (vbStartIndex == -1 || ibStartIndex == -1 || obStartIndex == -1) {
				if (vbStartIndex == -1) {
					Log.Warn("Failed to add mesh vertex data to vertex buffer, not enough room");
				}
				if (ibStartIndex == -1) {
					Log.Warn("Failed to add mesh index data to index buffer, not enough room");
				}
				if (obStartIndex == -1) {
					Log.Warn("Failed to add mesh object data to index object, not enough room");
				}
				return false;

			}

			meshRenderers.Add(meshRenderer);
			meshRendererPositions.Add((vbStartIndex, ibStartIndex, obStartIndex));

			// Shortcuts to the mesh data
			Vertex[] vertices = meshRenderer.mesh.vertices;
			int[] indices = meshRenderer.mesh.indices;
			ObjectInfo[] objectInfo = new ObjectInfo[] { meshRenderer.GetObjectInfo() };

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

			IntPtr writePos;
			

			int vertexUploadBufferSize = Utilities.SizeOf<Vertex>() * vertices.Length;
			writePos = GPU_Upload.BeginBufferUpload(vertexUploadBufferSize);
			Utilities.Write(writePos, vertices, 0, vertices.Length);
			GPU_Upload.EndBufferUpload(vertexBuffer, dstOffsetBytes: vbStartIndex * Utilities.SizeOf<Vertex>(), srcOffsetBytes: 0);

			int indexUploadBufferSize = Utilities.SizeOf<int>() * indicesOffset.Length;
			writePos = GPU_Upload.BeginBufferUpload(indexUploadBufferSize);
			Utilities.Write(writePos, indicesOffset, 0, indicesOffset.Length);
			GPU_Upload.EndBufferUpload(indexBuffer, dstOffsetBytes: ibStartIndex * Utilities.SizeOf<int>(), srcOffsetBytes: 0);

			#endregion

			return true;
			
		}
		
		/// <summary>
		/// Removes a mesh from GeometryResources
		/// </summary>
		/// <param name="meshRenderer">The mesh renderer to be removed</param>
		public void RemoveMesh(MeshRenderer meshRenderer) {

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




			// Find the start of the mesh within the buffers

			int meshRendererListIndex = -1;
			for (int i = 0; i < meshRenderers.Count; i ++) {
				if (meshRenderers[i] == meshRenderer) {
					meshRendererListIndex = i;
					break;
				}
			}
			if (meshRendererListIndex == -1) {
				Log.Warn("Failed to remove mesh, mesh not added");
				return;
			}

			

			int vbStartIndex = meshRendererPositions[meshRendererListIndex].vbStart;
			int ibStartIndex = meshRendererPositions[meshRendererListIndex].ibStart;
			int obStartIndex = meshRendererPositions[meshRendererListIndex].obStart;

			int numVerts = meshRenderer.mesh.vertices.Length;
			int numIndex = meshRenderer.mesh.indices.Length;
			int numObjs = 1;

			meshRenderers.RemoveAt(meshRendererListIndex);
			meshRendererPositions.RemoveAt(meshRendererListIndex);

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
		
		/// <summary>
		/// Applies updates to per-vertex data
		/// Do not use to add verticies or tris!!!
		/// </summary>
		/// <param name="meshRenderer">The Mesh Renderer containing the mesh to be updated</param>
		public void UpdateMeshData(MeshRenderer meshRenderer) {

			int meshRendererIndex = -1;
			for (int i = 0; i < meshRenderers.Count; i ++) {
				if (meshRenderers[i] == meshRenderer) {
					meshRendererIndex = i;
					break;
				}
			}
			if (meshRendererIndex == -1) {
				Log.Warn("Mesh renderer not found, ignoring");
				return;
			}

			long vertexBufferStartPos = meshRendererPositions[meshRendererIndex].vbStart;

			long numBytes = meshRenderer.mesh.vertices.Length * Utilities.SizeOf<Vertex>();
			IntPtr writePos = GPU_Upload.BeginBufferUpload(numBytes);
			Utilities.Write(writePos, meshRenderer.mesh.vertices, 0, meshRenderer.mesh.vertices.Length);
			GPU_Upload.EndBufferUpload(vertexBuffer, dstOffsetBytes: vertexBufferStartPos * Utilities.SizeOf<Vertex>());

		}

		private int FindGap(int[] gapArray, int width) {

			int foundSpot = -1;
			for (int i = 0; i < gapArray.Length;) {

				if (gapArray[i] >= width || gapArray[i] == -1) {
					foundSpot = i;
					break;
				}
				i += gapArray[i] + 1;

			}

			if (gapArray.Length - foundSpot < width) {
				return -1;
			}

			return foundSpot;

		}

		/// <summary>
		/// Updates the object data buffer for all mesh renderers added to this GeometryResources
		/// </summary>
		internal void UpdateObjectInfoBuffer() {

			int maxObjectInfoIndex = -1;

			for (int i = 0; i < meshRenderers.Count; i ++) {
				if (meshRendererPositions[i].obStart > maxObjectInfoIndex) {
					maxObjectInfoIndex = meshRendererPositions[i].obStart;
				}
			}
			if (maxObjectInfoIndex == -1) {
				return;
			}

			ObjectInfo[] gpuSendInfo = new ObjectInfo[maxObjectInfoIndex + 1];
			for (int i = 0; i < meshRenderers.Count; i++) {
				MeshRenderer currentMesh = meshRenderers[i];
				gpuSendInfo[meshRendererPositions[i].obStart] = currentMesh.GetObjectInfo();
			}

			objectBuffer.Write(gpuSendInfo, 0);

		}

		bool disposed = false;
		internal void Dispose() {
			if (disposed == true) { return; }
			disposed = true;
			vertexBuffer.Dispose();
			indexBuffer.Dispose();
			objectBuffer.Dispose();
		}
		~GeometryResources() {
			Dispose();
		}

	}
}
