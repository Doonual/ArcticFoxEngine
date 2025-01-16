using ArcticFoxEngine.Nodes;
using CoolClassLibrary;
using SharpDX;
using SharpDX.Direct3D12;


namespace ArcticFoxEngine.Rendering {

	/// <summary>
	/// Contains and controls all the resources needed for rendering geometry
	/// This is the vertex buffer, index buffers and the transform buffers
	/// </summary>
	public class GeometryInfo {

		internal static int kbPerBuffer = 128 * 1024;

		internal static int numVertexBufferElements = kbPerBuffer * 1024 / Utilities.SizeOf<Vertex>();
		internal static int numIndexBufferElements = kbPerBuffer * 1024 / sizeof(int);
		internal static int numTransformBufferElements = kbPerBuffer * 1024 / Utilities.SizeOf<TransformInfo>();

		public List<MeshRenderer> meshRenderers;
		public List<(int vertexBufferStart, int indexBufferStart, int transformBufferStart)> meshRendererPositions;

		public int[] vertexGap;
		public Resource vertexBuffer;
		public VertexBufferView vertexBufferView;

		public int[] indexGap;
		public Resource indexBuffer;
		public IndexBufferView indexBufferView;

		public int[] transformGap;
		public ConstBuffer<TransformInfo> transformBuffer;


		/// <summary>
		/// Creates a new instance GeometryResources
		/// </summary>
		public GeometryInfo() {

			meshRenderers = new List<MeshRenderer>();
			meshRendererPositions = new List<(int vertexBufferStart, int indexBufferStart, int transformBufferStart)>();

			Log.Info("VB Size: " + numVertexBufferElements);
			Log.Info("IB Size: " + numIndexBufferElements);
			Log.Info("TB Size: " + numTransformBufferElements);

			// Create gap arrays
			vertexGap = new int[numVertexBufferElements];
			for (int i = 0; i < numVertexBufferElements; i++) {
				vertexGap[i] = -1;
			}
			indexGap = new int[numIndexBufferElements];
			for (int i = 0; i < numIndexBufferElements; i++) {
				indexGap[i] = -1;
			}
			transformGap = new int[numTransformBufferElements];
			for (int i = 0; i < numTransformBufferElements; i++) {
				transformGap[i] = -1;
			}

			// Create buffer resources
			vertexBuffer = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, ResourceDescription.Buffer(numVertexBufferElements * Utilities.SizeOf<Vertex>()), ResourceStates.GenericRead);
			vertexBufferView.BufferLocation = vertexBuffer.GPUVirtualAddress;
			vertexBufferView.StrideInBytes = Utilities.SizeOf<Vertex>();
			vertexBufferView.SizeInBytes = numVertexBufferElements * Utilities.SizeOf<Vertex>();

			indexBuffer = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, ResourceDescription.Buffer(numIndexBufferElements * sizeof(int)), ResourceStates.IndexBuffer);
			indexBufferView.BufferLocation = indexBuffer.GPUVirtualAddress;
			indexBufferView.SizeInBytes = numIndexBufferElements * sizeof(int);
			indexBufferView.Format = SharpDX.DXGI.Format.R32_UInt;

			transformBuffer = new ConstBuffer<TransformInfo>(numTransformBufferElements);

		}

		/// <summary>
		/// Adds a mesh to GeometryResources
		/// </summary>
		/// <param name="meshRenderer">The mesh renderer to be added</param>
		/// <returns>Whether the mesh was added successfully</returns>
		public bool AddMesh(MeshRenderer meshRenderer) {
			if (meshRenderer.mesh == null) { Log.Error("Failed to add mesh renderer, no mesh"); return false; }

			// Find suitable space for mesh data insertion
			int vertexBufferStartIndex = FindGap(vertexGap, meshRenderer.mesh.vertices.Length);
			int indexBufferStartIndex = FindGap(indexGap, meshRenderer.mesh.indices.Length);
			int transformBufferStartIndex = FindGap(transformGap, 1);

			// If there is no suitable space for mesh data
			if (vertexBufferStartIndex == -1 || indexBufferStartIndex == -1 || transformBufferStartIndex == -1) {
				if (vertexBufferStartIndex == -1) {
					Log.Warn("Failed to add mesh vertex data to vertex buffer, not enough room");
				}
				if (indexBufferStartIndex == -1) {
					Log.Warn("Failed to add mesh index data to index buffer, not enough room");
				}
				if (transformBufferStartIndex == -1) {
					Log.Warn("Failed to add mesh transform data to transform buffer, not enough room");
				}
				return false;

			}

			meshRenderers.Add(meshRenderer);
			meshRendererPositions.Add((vertexBufferStartIndex, indexBufferStartIndex, transformBufferStartIndex));

			// Shortcuts to the mesh data
			Vertex[] vertices = meshRenderer.mesh.vertices;
			int[] indices = meshRenderer.mesh.indices;
			TransformInfo[] transformInfo = new TransformInfo[] { meshRenderer.GetObjectInfo() };

			// Mark each gap as being occupied by data
			for (int i = 0; i < vertices.Length; i++) {
				vertexGap[i + vertexBufferStartIndex] = 0;
			}
			for (int i = 0; i < indices.Length; i++) {
				indexGap[i + indexBufferStartIndex] = 0;
			}
			for (int i = 0; i < transformInfo.Length; i++) {
				transformGap[i + transformBufferStartIndex] = 0;
			}


			#region Upload data to GPU

			int[] indicesOffset = new int[indices.Length];
			for (int i = 0; i < indices.Length; i++) {
				indicesOffset[i] = indices[i];
			}

			IntPtr writePos;


			int vertexUploadBufferSize = Utilities.SizeOf<Vertex>() * vertices.Length;
			writePos = Upload.BeginBufferUpload(vertexUploadBufferSize);
			Utilities.Write(writePos, vertices, 0, vertices.Length);
			Upload.EndBufferUpload(vertexBuffer, dstOffsetBytes: vertexBufferStartIndex * Utilities.SizeOf<Vertex>(), srcOffsetBytes: 0);

			int indexUploadBufferSize = Utilities.SizeOf<int>() * indicesOffset.Length;
			writePos = Upload.BeginBufferUpload(indexUploadBufferSize);
			Utilities.Write(writePos, indicesOffset, 0, indicesOffset.Length);
			Upload.EndBufferUpload(indexBuffer, dstOffsetBytes: indexBufferStartIndex * Utilities.SizeOf<int>(), srcOffsetBytes: 0);

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
			for (int i = 0; i < meshRenderers.Count; i++) {
				if (meshRenderers[i] == meshRenderer) {
					meshRendererListIndex = i;
					break;
				}
			}
			if (meshRendererListIndex == -1) {
				Log.Warn("Failed to remove mesh, mesh not added");
				return;
			}



			int vertexBufferStartIndex = meshRendererPositions[meshRendererListIndex].vertexBufferStart;
			int indexBufferStartIndex = meshRendererPositions[meshRendererListIndex].indexBufferStart;
			int transformBufferStartIndex = meshRendererPositions[meshRendererListIndex].transformBufferStart;

			int numVerts = meshRenderer.mesh.vertices.Length;
			int numIndex = meshRenderer.mesh.indices.Length;
			int numObjs = 1;

			meshRenderers.RemoveAt(meshRendererListIndex);
			meshRendererPositions.RemoveAt(meshRendererListIndex);

			// Mark the data as just removed but still to be propagated
			for (int i = 0; i < numVerts; i++) {
				vertexGap[i + vertexBufferStartIndex] = -2;
			}
			for (int i = 0; i < numIndex; i++) {
				indexGap[i + indexBufferStartIndex] = -2;
			}
			for (int i = 0; i < numObjs; i++) {
				transformGap[i + transformBufferStartIndex] = -2;
			}

			// Propagate gaps
			for (int i = vertexBufferStartIndex + numVerts - 1; i >= 0; i--) {
				if (vertexGap[i] == 0) { break; }
				if (i + 1 == vertexGap.Length || vertexGap[i + 1] == -1) {
					vertexGap[i] = -1;
				}
				else {
					vertexGap[i] = vertexGap[i + 1] + 1;
				}
			}
			for (int i = indexBufferStartIndex + numIndex - 1; i >= 0; i--) {
				if (indexGap[i] == 0) { break; }
				if (i + 1 == indexGap.Length || indexGap[i + 1] == -1) {
					indexGap[i] = -1;
				}
				else {
					indexGap[i] = indexGap[i + 1] + 1;
				}
			}
			for (int i = transformBufferStartIndex + numObjs - 1; i >= 0; i--) {
				if (transformGap[i] == 0) { break; }
				if (i + 1 == transformGap.Length || transformGap[i + 1] == -1) {
					transformGap[i] = -1;
				}
				else {
					transformGap[i] = transformGap[i + 1] + 1;
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
			for (int i = 0; i < meshRenderers.Count; i++) {
				if (meshRenderers[i] == meshRenderer) {
					meshRendererIndex = i;
					break;
				}
			}
			if (meshRendererIndex == -1) {
				Log.Warn("Mesh renderer not found, ignoring");
				return;
			}

			long vertexBufferStartPos = meshRendererPositions[meshRendererIndex].vertexBufferStart;

			long numBytes = meshRenderer.mesh.vertices.Length * Utilities.SizeOf<Vertex>();
			IntPtr writePos = Upload.BeginBufferUpload(numBytes);
			Utilities.Write(writePos, meshRenderer.mesh.vertices, 0, meshRenderer.mesh.vertices.Length);
			Upload.EndBufferUpload(vertexBuffer, dstOffsetBytes: vertexBufferStartPos * Utilities.SizeOf<Vertex>());

		}

		public int GetMeshPosInVertexBuffer(int index) {
			return meshRendererPositions[index].vertexBufferStart;
		}
		public int GetMeshPosInIndexBuffer(int index) {
			return meshRendererPositions[index].indexBufferStart;
		}
		public int GetMeshPosInObjectBuffer(int index) {
			return meshRendererPositions[index].transformBufferStart;
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
		/// Updates the transform data buffer for all mesh renderers added to this GeometryResources
		/// </summary>
		public void UpdateObjectInfoBuffer() {

			int maxObjectInfoIndex = -1;

			for (int i = 0; i < meshRenderers.Count; i++) {
				if (meshRendererPositions[i].transformBufferStart > maxObjectInfoIndex) {
					maxObjectInfoIndex = meshRendererPositions[i].transformBufferStart;
				}
			}
			if (maxObjectInfoIndex == -1) {
				return;
			}

			TransformInfo[] gpuSendInfo = new TransformInfo[maxObjectInfoIndex + 1];
			for (int i = 0; i < meshRenderers.Count; i++) {
				MeshRenderer currentMesh = meshRenderers[i];
				gpuSendInfo[meshRendererPositions[i].transformBufferStart] = currentMesh.GetObjectInfo();
			}

			transformBuffer.Write(gpuSendInfo, 0);

		}

		bool disposed = false;
		internal void Dispose() {
			if (disposed == true) { return; }
			disposed = true;
			vertexBuffer.Dispose();
			indexBuffer.Dispose();
			transformBuffer.Dispose();
		}
		~GeometryInfo() {
			Dispose();
		}

	}
}
