using CoolClassLibrary;
using SharpDX.Direct3D12;
using Swan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Backend {
	internal class GPU_Upload {

		struct UploadFrame {

			public CommandAllocator commandAllocator;
			public GraphicsCommandList commandList;
			public Resource uploadBuffer;
			public IntPtr cpuAddress;
			public int fenceValue;
			public bool ready;

			public UploadFrame(GPU_Upload upload) {
				commandAllocator = Graphics.device.CreateCommandAllocator(CommandListType.Copy); ;
				commandList = Graphics.device.CreateCommandList(CommandListType.Copy, commandAllocator, null);
				commandList.Close();

				commandAllocator.Name = "Upload Command Allocator";
				commandList.Name = "Upload Command List";

				uploadBuffer = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(1024), ResourceStates.GenericRead);
				ready = true;
				cpuAddress = IntPtr.Zero;
				fenceValue = 0;
			}

			public void WaitAndReset(GPU_Upload upload) {

				if (upload.uploadFence.CompletedValue < upload.fenceValue) {
					upload.uploadFence.SetEventOnCompletion(upload.fenceValue, upload.fenceEvent.SafeWaitHandle.DangerousGetHandle());
					upload.fenceEvent.WaitOne();
				}

				uploadBuffer.Dispose();
				uploadBuffer = null;
				ready = true;
				cpuAddress = IntPtr.Zero;


			}

			public void Dispose(GPU_Upload upload) {
				WaitAndReset(upload);
				commandAllocator.Dispose();
				commandList.Dispose();
			}

		}


		int uploadFrameCount = 4;
		UploadFrame[] uploadFrames;

		CommandQueue uploadCommandQueue;
		Fence uploadFence;
		int fenceValue;
		AutoResetEvent fenceEvent;
		//Mutex frameMutex;

		int uploadFrameIndex;

		public GraphicsCommandList commandList;
		public IntPtr cpuAddress;
		public Resource uploadBuffer;

		

		public GPU_Upload() {

			uploadFrames = new UploadFrame[uploadFrameCount];
			for (int i = 0; i < uploadFrameCount; i++) {
				uploadFrames[i] = new UploadFrame(this);
			}

			CommandQueueDescription desc = new CommandQueueDescription();
			desc.Flags = CommandQueueFlags.None;
			desc.NodeMask = 0;
			desc.Priority = ((int)CommandQueuePriority.Normal);
			desc.Type = CommandListType.Copy;

			uploadCommandQueue = Graphics.device.CreateCommandQueue(desc);
			uploadCommandQueue.Name = "Upload Copy Queue";

			uploadFence = Graphics.device.CreateFence(0, FenceFlags.None);
			uploadFence.Name = "Upload Fence";

			fenceEvent = new AutoResetEvent(false);

		}

		public void UploadContext(int alignedSize) {


			uploadFrameIndex = GetAvailableUploadFrame();
			UploadFrame frame = uploadFrames[uploadFrameIndex];

			frame.ready = false;
			frame.uploadBuffer = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(alignedSize), ResourceStates.GenericRead);

			frame.cpuAddress = frame.uploadBuffer.Map(0);
			commandList = frame.commandList;
			uploadBuffer = frame.uploadBuffer;
			cpuAddress = frame.cpuAddress;

			frame.commandAllocator.Reset();
			frame.commandList.Reset(frame.commandAllocator, null);


		}
		public void EndUpload() {

			UploadFrame frame = uploadFrames[uploadFrameIndex];
			GraphicsCommandList commandList = frame.commandList;

			commandList.Close();
			uploadCommandQueue.ExecuteCommandList(commandList);
			fenceValue++;
			frame.fenceValue = fenceValue;
			uploadCommandQueue.Signal(uploadFence, frame.fenceValue);
			frame.WaitAndReset(this);

		}
		int GetAvailableUploadFrame() {
			// Iterates through the upload frames and returns the index of the first ready one

			int index = -1;
			int count = uploadFrameCount;

			for (int i = 0; i < uploadFrameCount; i ++) {
				if (uploadFrames[i].ready == true) {
					index = i;
					break;
				}
			}

			if (index == -1) {
				index = 0;
				while (uploadFrames[index].ready == false) {
					index += 1;
					index %= count;
					Thread.Yield();
				}
			}

			return index;

		}

		~GPU_Upload() {

			for (int i = 0; i < uploadFrameCount; i ++) {
				uploadFrames[i].Dispose(this);
			}

			if (fenceEvent != null) {
				fenceEvent.Close();
				fenceEvent.Dispose();
			}


			uploadCommandQueue.Dispose();
			uploadFence.Dispose();
			fenceValue = 0;

		}


	}
}
