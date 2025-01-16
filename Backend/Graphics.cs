using CoolClassLibrary;
using SharpDX;
using SharpDX.Direct3D12;
using SharpDX.DXGI;
using SharpDX.Windows;
using Device = SharpDX.Direct3D12.Device;
using Resource = SharpDX.Direct3D12.Resource;

namespace ArcticFoxEngine {

	public static class Graphics {

		public static bool isDebug = false;

		public static Device device { get; private set; }

		static RenderForm mainRenderForm;
		internal static SwapChain3 swapChain;
		internal const int swapChainFrameCount = 2;
		internal static int frameIndex;


		// Render target view descriptor heap
		internal static DescriptorHeap rtvHeap;
		internal static Resource[] renderTargets;
		internal static int rtvHeapIncrement;

		// Depth Stencil descriptor heap
		internal static DescriptorHeap dsvHeap;
		internal static Resource depthStencilBuffer;


		#region Command queue objects

		internal static CommandAllocator cmdAllocatorDirect;
		internal static CommandQueue cmdQueueDirect;
		private static Fence fenceDirect;
		private static long fenceValueDirect;

		internal static CommandAllocator cmdAllocatorCopy;
		internal static CommandQueue cmdQueueCopy;
		private static Fence fenceCopy;
		private static long fenceValueCopy;

		private static AutoResetEvent fenceEvent;

		#endregion


		// Main setup function
		// Combines all the individual steps to setting up rendering


		/// <summary>
		/// Initialises Graphics, this includes
		/// attaching the graphics device,
		/// setting up the command lists and synchronisation,
		/// settup up the swap chain and resources for rendering to the screen.
		/// </summary>
		/// <param name="form"></param>
		internal static void Init(RenderForm form) {

			if (isDebug == true) {
				// Enable the D3D12 debug layer
				DebugInterface.Get().EnableDebugLayer();
			}

			mainRenderForm = form;

			int width = form.ClientSize.Width;
			int height = form.ClientSize.Height;
			int refreshRate = 240;

			try {

				SetupDevice();

				// Create an event handle to use for frame synchronisation
				fenceEvent = new AutoResetEvent(false);
				SetupDirectCommandAllocator();
				SetupCopyCommandAllocator();

				SetupSwapChain(width, height, refreshRate, cmdQueueDirect);

				Log.Success("Initialised renderer");
			}
			catch (Exception e) {
				Log.Error("Failed to initialise renderer");
				Log.Raw(e);
			}



		}


		private static void SetupDevice() {
			// Create the graphics device
			device = new Device(null, SharpDX.Direct3D.FeatureLevel.Level_11_0);
		}
		
		
		// Command Queues
		
		private static void SetupDirectCommandAllocator() {

			// Create the command list
			// Command lists are created in the recording state, but there is nothing
			// to record yet. The main loop expects it to be closed, so close it now.
			cmdAllocatorDirect = device.CreateCommandAllocator(CommandListType.Direct);
			cmdQueueDirect = device.CreateCommandQueue(new CommandQueueDescription(CommandListType.Direct));
			

			// Create synchronisation objects
			fenceDirect = device.CreateFence(0, FenceFlags.None);
			fenceValueDirect = 1;

			// Give the fenceDirect a default completed value
			cmdQueueDirect.Signal(fenceDirect, fenceValueDirect);



		}
		internal static GraphicsCommandList CreateDirectCommandList() {
			GraphicsCommandList cmdList = device.CreateCommandList(CommandListType.Direct, cmdAllocatorDirect, null);
			cmdList.Close();
			return cmdList;
		}
		internal static void ResetDirectCommandList(GraphicsCommandList cmdList) {
			cmdList.Reset(cmdAllocatorDirect, null);
		}
		internal static void ExecuteDirectCommandList(GraphicsCommandList cmdList) {
			cmdQueueDirect.ExecuteCommandList(cmdList);

			fenceValueDirect++;
			cmdQueueDirect.Signal(fenceDirect, fenceValueDirect);

		}
		internal static void WaitForDirectCommandQueue() {
			WaitForFenceValue(fenceValueDirect, fenceDirect);
		}


		private static void SetupCopyCommandAllocator() {

			CommandQueueDescription desc = new CommandQueueDescription() {
				Flags = CommandQueueFlags.None,
				NodeMask = 0,
				Priority = ((int)CommandQueuePriority.Normal),
				Type = CommandListType.Copy,
			};
			cmdQueueCopy = device.CreateCommandQueue(desc);
			cmdQueueCopy.Name = "Copy Command Queue";

			cmdAllocatorCopy = device.CreateCommandAllocator(CommandListType.Copy);
			cmdAllocatorCopy.Name = "Copy Command Allocator";

			fenceCopy = device.CreateFence(0, FenceFlags.None);
			fenceCopy.Name = "Upload Fence";
			fenceValueDirect = 1;

			// Give the fenceDirect a default completed value
			cmdQueueCopy.Signal(fenceCopy, fenceValueDirect);

		}
		internal static GraphicsCommandList CreateCopyCommandList() {

			GraphicsCommandList cmdList = device.CreateCommandList(CommandListType.Copy, cmdAllocatorCopy, null);
			cmdList.Close();
			return cmdList;

		}
		internal static void ResetCopyCommandList(GraphicsCommandList cmdList) {
			cmdList.Reset(cmdAllocatorCopy, null);
		}
		internal static void ExecuteCopyCommandList(GraphicsCommandList cmdList) {
			cmdQueueCopy.ExecuteCommandList(cmdList);

			fenceValueCopy++;
			cmdQueueCopy.Signal(fenceCopy, fenceValueCopy);

		}
		internal static void WaitForCopyCommandQueue() {
			WaitForFenceValue(fenceValueCopy, fenceCopy);
		}

		internal static void WaitForFenceValue(long value, Fence fence) {
			
			if (fence.CompletedValue < value) {
				fence.SetEventOnCompletion(value, fenceEvent.SafeWaitHandle.DangerousGetHandle());
				fenceEvent.WaitOne();
			}

		}




		private static void SetupSwapChain(int width, int height, int refreshRate, CommandQueue commandQueue) {

			// Creating the swap chain
			using (Factory4 factory = new Factory4()) {

				// Describe and create the swap chain
				SwapChainDescription swapChainDesc = new SwapChainDescription() {
					BufferCount = swapChainFrameCount,
					ModeDescription = new ModeDescription(width, height, new Rational(refreshRate, 1), Format.R8G8B8A8_UNorm),
					Usage = Usage.RenderTargetOutput,
					SwapEffect = SwapEffect.FlipDiscard,
					OutputHandle = mainRenderForm.Handle,
					Flags = SwapChainFlags.None | SwapChainFlags.AllowModeSwitch,
					SampleDescription = new SampleDescription(1, 0),
					IsWindowed = true
				};
				SwapChain tempSwapChain = new SwapChain(factory, commandQueue, swapChainDesc);
				swapChain = tempSwapChain.QueryInterface<SwapChain3>();
				tempSwapChain.Dispose();
				frameIndex = swapChain.CurrentBackBufferIndex;

			}


			#region Setup RTV descripto heaps and resources

			// Create a render target view (RTV) descriptor heap
			DescriptorHeapDescription rtvHeapDesc = new DescriptorHeapDescription() {
				DescriptorCount = swapChainFrameCount,
				Flags = DescriptorHeapFlags.None,
				Type = DescriptorHeapType.RenderTargetView
			};
			rtvHeap = device.CreateDescriptorHeap(rtvHeapDesc);
			rtvHeapIncrement = device.GetDescriptorHandleIncrementSize(DescriptorHeapType.RenderTargetView);


			// Add render target resources to RTV descriptor heap
			CpuDescriptorHandle rtvHandle = rtvHeap.CPUDescriptorHandleForHeapStart;
			renderTargets = new Resource[swapChainFrameCount];
			for (int n = 0; n < swapChainFrameCount; n++) {
				renderTargets[n] = swapChain.GetBackBuffer<Resource>(n);
				device.CreateRenderTargetView(renderTargets[n], null, rtvHandle);
				rtvHandle += rtvHeapIncrement;
			}

			#endregion
			#region Setup DSV descriptor heap and resources

			DescriptorHeapDescription dsvHeapDesc = new DescriptorHeapDescription() {
				DescriptorCount = 1,
				Type = DescriptorHeapType.DepthStencilView,
				Flags = DescriptorHeapFlags.None
			};
			dsvHeap = device.CreateDescriptorHeap(dsvHeapDesc);

			DepthStencilViewDescription depthStencilDesc = new DepthStencilViewDescription() {
				Format = Format.D32_Float,
				Dimension = DepthStencilViewDimension.Texture2D,
				Flags = DepthStencilViewFlags.None
			};
			depthStencilBuffer = device.CreateCommittedResource(
				new HeapProperties(HeapType.Default),
				HeapFlags.None, ResourceDescription.Texture2D(Format.D32_Float, width, height, flags: ResourceFlags.AllowDepthStencil),
				ResourceStates.DepthWrite
			);
			depthStencilBuffer.Name = "Depth / Stencil Resource Heap";
			device.CreateDepthStencilView(depthStencilBuffer, depthStencilDesc, dsvHeap.CPUDescriptorHandleForHeapStart);

			#endregion


		}

		
		
		
		/// <summary>
		/// Shows the render target to the screen and swaps which resource is the render target
		/// </summary>
		internal static void Buffer() {
			// Present the frame
			try {
				CheckHRESULT(swapChain.Present(1, 0));
				frameIndex = swapChain.CurrentBackBufferIndex;

				cmdAllocatorCopy.Reset();
				cmdAllocatorDirect.Reset();

			}
			catch (Exception e) {
				CheckHRESULT(device.DeviceRemovedReason);
				Log.Error(e);
			}

		}

		/// <summary>
		/// Checks the HRESULT and throws an error if it is an error
		/// </summary>
		/// <param name="result">The HRESULT to check</param>
		internal static void CheckHRESULT(Result result) {
			// https://learn.microsoft.com/en-us/windows/win32/com/structure-of-com-error-codes

			uint resultCode = ((uint)result);
			uint severity = (resultCode >> 31);
			uint facility = ((resultCode >> 16) & 0b111111111111);
			uint code = resultCode & 0b1111111111111111;

			if (severity == 1) {
				Log.Error("DX12 Error: " + result);
				Thread.CurrentThread.Interrupt();
			}

		}

		/// <summary>
		/// Disposes all resources held by Graphics
		/// </summary>
		internal static void Dispose() {

			swapChain.Dispose();
			device.Dispose();

			fenceDirect.Dispose();
			cmdAllocatorDirect.Dispose();
			cmdQueueDirect.Dispose();

			fenceCopy.Dispose();
			cmdAllocatorCopy.Dispose();
			cmdQueueCopy.Dispose();

		}

	}

	internal class StandardIncludeHandler : CppObject, SharpDX.D3DCompiler.Include {
		public IDisposable Shadow { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		internal StandardIncludeHandler() : base(new IntPtr(1)) { }
		public void Close(Stream stream) { }
		public Stream Open(SharpDX.D3DCompiler.IncludeType type, string fileName, Stream parentStream) {
			throw new NotImplementedException();
		}
	}

}