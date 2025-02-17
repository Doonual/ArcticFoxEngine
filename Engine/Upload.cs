using CoolClassLibrary;
using SharpDX.Direct3D12;
using SharpDX.DXGI;
using System.Runtime.InteropServices;
using Resource = SharpDX.Direct3D12.Resource;

namespace ArcticFoxEngine {


	internal static class Upload {

		private static bool disposed = true;

		// Upload resource, data is copied from this resource into the resources in default heaps
		static Resource uploadResource;

		static long uploadBytes;

		static GraphicsCommandList cmdList;

		/// <summary>
		/// Initialises all GPU_Upload
		/// </summary>
		internal static void Init() {
			if (disposed == false) { Log.Warn("Cannot initialise GPU_Upload, already initialised"); return; }
			disposed = false;

			// Initialise command queues and synchronisation

			cmdList = Graphics.CreateCopyCommandList();
			cmdList.Name = "Upload Command List";

			uploadResource = null;

		}


		/// <summary>
		/// Prepares the temporary buffer for uploading
		/// </summary>
		/// <param name="numBytes">The number of bytes to be uploaded</param>
		/// <returns>A pointer to the start of the temporary buffer. Use this to fill the temporary buffer with data</returns>
		internal static IntPtr BeginBufferUpload(long numBytes) {
			if (uploadResource != null) {
				Log.Error("Cannot begin buffer upload, upload not ready");
				return IntPtr.Zero;
			}
			if (numBytes <= 0) {
				Log.Error("Cannot begin buffer upload, numBytes cant be 0");
				return IntPtr.Zero;
			}

			uploadBytes = numBytes;
			uploadResource = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(uploadBytes), ResourceStates.GenericRead);
			return uploadResource.Map(0);

		}

		/// <summary>
		/// Copies the data from the temporary buffer into the specified default heap buffer
		/// </summary>
		/// <param name="dstBuffer">The default heap buffer to upload to</param>
		/// <param name="dstOffsetBytes">Location of the start of the data in the destination buffer</param>
		/// <param name="srcOffsetBytes">Location of the start of the data in the source buffer</param>
		internal static void EndBufferUpload(Resource dstBuffer, long dstOffsetBytes = 0, long srcOffsetBytes = 0) {

			Graphics.ResetCopyCommandList(cmdList);
			cmdList.CopyBufferRegion(dstBuffer, dstOffsetBytes, uploadResource, srcOffsetBytes, uploadBytes);
			cmdList.Close();

			Graphics.ExecuteCopyCommandList(cmdList);

			Graphics.WaitForCopyCommandQueue();
			uploadResource.Dispose();
			uploadResource = null;

		}

		/// <summary>
		/// Uploads data to a Texture2D resource
		/// </summary>
		/// <param name="dstTexture">The Texture2D resource to upload to</param>
		/// <param name="width">The width of the destination texture</param>
		/// <param name="height">The height of the destination texture</param>
		/// <param name="format">The pixel format of the destination texture</param>
		/// <param name="textureData">The data to upload to the texture</param>
		internal static void Texture2DUpload(Resource dstTexture, int width, int height, Format format, byte[] textureData) {
			if (uploadResource != null) { Log.Error("Cannot begin upload, upload not ready"); }

			Graphics.ResetCopyCommandList(cmdList);

			// Create a temporary Texture2D resource to fill with data, and then copy in to the dstTexture
			uploadResource = Graphics.device.CreateCommittedResource(new HeapProperties(CpuPageProperty.WriteBack, MemoryPool.L0), HeapFlags.None, ResourceDescription.Texture2D((SharpDX.DXGI.Format)format, width, height), ResourceStates.GenericRead);
			int texturePixelSize = format.SizeOfInBytes();


			GCHandle handle = GCHandle.Alloc(textureData, GCHandleType.Pinned);
			IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(textureData, 0);
			uploadResource.WriteToSubresource(0, null, ptr, texturePixelSize * width, textureData.Length);
			handle.Free();

			for (int i = 0; i < 500; i ++) {
				cmdList.CopyTextureRegion(new TextureCopyLocation(dstTexture, 0), 0, 0, 0, new TextureCopyLocation(uploadResource, 0), null);
			}
			
			cmdList.Close();

			Graphics.ExecuteCopyCommandList(cmdList);
			Graphics.WaitForCopyCommandQueue();
			
			uploadResource.Dispose();
			uploadResource = null;

		}

		/// <summary>
		/// Uploads single pixel data to a Texture2D resource
		/// </summary>
		/// <param name="dstTexture">The Texture2D resource to upload to</param>
		/// <param name="x">The X-coordinate to upload the pixel data into</param>
		/// <param name="y">The Y-coordinate to upload the pixel data into</param>
		/// <param name="format">The pixel format of the destination texture</param>
		/// <param name="textureData">The data to upload the texture</param>
		internal static void Texture2DPixelUpload(Resource dstTexture, int x, int y, Format format, byte[] textureData) {
			if (uploadResource != null) { Log.Error("Cannot begin upload, upload not ready"); }

			Graphics.ResetCopyCommandList(cmdList);

			// Create a temporary Texture2D resource to fill with data, and then copy in to the dstTexture
			uploadResource = Graphics.device.CreateCommittedResource(new HeapProperties(CpuPageProperty.WriteBack, MemoryPool.L0), HeapFlags.None, ResourceDescription.Texture2D((SharpDX.DXGI.Format)format, 1, 1), ResourceStates.GenericRead);
			int texturePixelSize = format.SizeOfInBytes();


			GCHandle handle = GCHandle.Alloc(textureData, GCHandleType.Pinned);
			IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(textureData, 0);
			uploadResource.WriteToSubresource(0, null, ptr, texturePixelSize * 1, textureData.Length);
			handle.Free();

			cmdList.CopyTextureRegion(new TextureCopyLocation(dstTexture, 0), x, y, 0, new TextureCopyLocation(uploadResource, 0), null);

			cmdList.Close();


			Graphics.ExecuteCopyCommandList(cmdList);

			Graphics.WaitForCopyCommandQueue();
			uploadResource.Dispose();
			uploadResource = null;

		}



		/// <summary>
		/// Disposes the resources held by GPU_Upload
		/// </summary>
		internal static void Dispose() {
			if (disposed == true) { Log.Warn("Cannot dispose GPU_Upload, not initialised"); return; }
			disposed = true;

			if (uploadResource != null) { uploadResource.Dispose(); }
			cmdList.Dispose();



		}

	}
}
