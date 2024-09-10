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

		static int texUploadWidth;
		static int texUploadHeight;
		static Format texUploadFormat;

		static CommandQueue uploadCommandQueue;
		static CommandAllocator commandAllocator;
		static GraphicsCommandList commandList;

		static AutoResetEvent fenceEvent;
		static Fence uploadFence;
		static int fenceValue;

		/// <summary>
		/// Initialises all GPU_Upload
		/// </summary>
		internal static void Init() {
			if (disposed == false) { Log.Warn("Cannot initialise GPU_Upload, already initialised"); return; }
			disposed = false;

			// Initialise command queues and synchronisation

			CommandQueueDescription desc = new CommandQueueDescription() {
				Flags = CommandQueueFlags.None,
				NodeMask = 0,
				Priority = ((int)CommandQueuePriority.Normal),
				Type = CommandListType.Copy,
			};
			uploadCommandQueue = Graphics.device.CreateCommandQueue(desc);
			uploadCommandQueue.Name = "Upload Copy Queue";

			commandAllocator = Graphics.device.CreateCommandAllocator(CommandListType.Copy);
			commandAllocator.Name = "Upload Command Allocator";

			commandList = Graphics.device.CreateCommandList(CommandListType.Copy, commandAllocator, null);
			commandList.Close();
			commandList.Name = "Upload Command List";

			uploadFence = Graphics.device.CreateFence(0, FenceFlags.None);
			uploadFence.Name = "Upload Fence";
			fenceEvent = new AutoResetEvent(false);

			uploadResource = null;

		}


		/// <summary>
		/// Prepares the temporary buffer for uploading
		/// </summary>
		/// <param name="numBytes">The number of bytes to be uploaded</param>
		/// <returns>A pointer to the start of the temporary buffer. Use this to fill the temporary buffer with data</returns>
		internal static IntPtr BeginBufferUpload(long numBytes) {
			if (uploadResource != null) { Log.Error("Cannot begin buffer upload, upload not ready"); }

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

			commandAllocator.Reset();
			commandList.Reset(commandAllocator, null);
			commandList.CopyBufferRegion(dstBuffer, dstOffsetBytes, uploadResource, srcOffsetBytes, uploadBytes);
			commandList.Close();

			uploadCommandQueue.ExecuteCommandList(commandList);
			fenceValue++;
			uploadCommandQueue.Signal(uploadFence, fenceValue);
			WaitForCmdList();

		}

		/// <summary>
		/// Prepares the temporary texture for uploading
		/// </summary>
		/// <param name="numBytes">The number of bytes to be uploaded</param>
		/// <returns>A pointer to the start of the temporary texture. Use this to fill the temporary texture with data</returns>
		internal static void Texture2DUpload(Resource dstTexture, int width, int height, Format format, byte[] textureData) {
			if (uploadResource != null) { Log.Error("Cannot begin upload, upload not ready"); }

			commandAllocator.Reset();
			commandList.Reset(commandAllocator, null);

			uploadResource = Graphics.device.CreateCommittedResource(new HeapProperties(CpuPageProperty.WriteBack, MemoryPool.L0), HeapFlags.None, ResourceDescription.Texture2D(format, width, height), ResourceStates.GenericRead);
			int texturePixelSize = format.SizeOfInBytes();

			texUploadWidth = width;
			texUploadHeight = height;
			texUploadFormat = format;


			GCHandle handle = GCHandle.Alloc(textureData, GCHandleType.Pinned);
			IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(textureData, 0);
			uploadResource.WriteToSubresource(0, null, ptr, texturePixelSize * width, textureData.Length);
			handle.Free();

			commandList.CopyTextureRegion(new TextureCopyLocation(dstTexture, 0), 0, 0, 0, new TextureCopyLocation(uploadResource, 0), null);

			commandList.Close();


			uploadCommandQueue.ExecuteCommandList(commandList);
			fenceValue++;
			uploadCommandQueue.Signal(uploadFence, fenceValue);

			WaitForCmdList();

		}


		private static void WaitForCmdList() {

			if (uploadFence.CompletedValue < fenceValue) {
				uploadFence.SetEventOnCompletion(fenceValue, fenceEvent.SafeWaitHandle.DangerousGetHandle());
				fenceEvent.WaitOne();
			}

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
			uploadCommandQueue.Dispose();
			commandAllocator.Dispose();
			commandList.Dispose();

			if (fenceEvent != null) {
				fenceEvent.Close();
				fenceEvent.Dispose();
			}



			uploadFence.Dispose();
			fenceValue = 0;

		}

	}
}
