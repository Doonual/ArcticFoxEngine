using CoolClassLibrary;
using SharpDX.Direct3D12;
using SharpDX.DXGI;
using Resource = SharpDX.Direct3D12.Resource;
using Swan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Backend {


	internal static class GPU_Upload {

		static Resource uploadResource;

		static long uploadSize;

		static CommandQueue uploadCommandQueue;
		static CommandAllocator commandAllocator;
		static GraphicsCommandList commandList;

		static AutoResetEvent fenceEvent;
		static Fence uploadFence;
		static int fenceValue;


		static GPU_Upload() {

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

		internal static IntPtr BeginBufferUpload(long size) {
			if (uploadResource != null) { Log.Error("Cannot begin upload, upload not ready"); }
			uploadSize = size;

			uploadResource = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(uploadSize), ResourceStates.GenericRead);
			return uploadResource.Map(0);

		}
		internal static void EndBufferUpload(Resource dstBuffer, long dstOffset = 0, long srcOffset = 0) {

			commandAllocator.Reset();
			commandList.Reset(commandAllocator, null);
			commandList.CopyBufferRegion(dstBuffer, dstOffset, uploadResource, srcOffset, uploadSize);
			commandList.Close();

			uploadCommandQueue.ExecuteCommandList(commandList);
			fenceValue++;
			uploadCommandQueue.Signal(uploadFence, fenceValue);
			WaitAndReset();

		}

		internal static IntPtr BeginTextureUpload(int width, int height, short depth, Format format, ResourceDimension resourceDimension) {

			if (uploadResource != null) { Log.Error("Cannot begin upload, upload not ready"); }
			uploadSize = width * height * depth * format.SizeOfInBytes();

			ResourceDescription texResourceDesc = new ResourceDescription() {
				MipLevels = 1,
				Format = format,
				Width = width,
				Height = height,
				DepthOrArraySize = depth,
				Dimension = resourceDimension,
			};
			uploadResource = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, texResourceDesc, ResourceStates.GenericRead);
			return uploadResource.Map(0);
			
		}
		internal static void EndTextureUpload(Resource dstTexture, int dstX = 0, int dstY = 0, int dstZ = 0, ResourceRegion? region = null) {

			TextureCopyLocation dstLoc = new TextureCopyLocation(dstTexture, 0);
			TextureCopyLocation srcLoc = new TextureCopyLocation(uploadResource, 0);

			commandAllocator.Reset();
			commandList.Reset(commandAllocator, null);
			commandList.CopyTextureRegion(dstLoc, dstX, dstY, dstZ, srcLoc, region);
			commandList.Close();

			uploadCommandQueue.ExecuteCommandList(commandList);
			fenceValue++;
			uploadCommandQueue.Signal(uploadFence, fenceValue);
			WaitAndReset();

		}


		private static void WaitAndReset() {

			if (uploadFence.CompletedValue < fenceValue) {
				uploadFence.SetEventOnCompletion(fenceValue, fenceEvent.SafeWaitHandle.DangerousGetHandle());
				fenceEvent.WaitOne();
			}

			uploadResource.Dispose();
			uploadResource = null;

		}

		internal static void Dispose() {

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
