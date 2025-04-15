using ArcticFoxEngine.Compute;
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
		public static int descriptorHeapIncrement;

		static RenderForm mainRenderForm;
		internal static SwapChain3 swapChain;
		internal static Resource[] swapchainResources;
		internal const int swapChainFrameCount = 2;
		internal static int frameIndex;

		public static Texture mainTexture;
		private static ComputeShader alphaBlendShader;

		#region Command queue objects

		internal static CommandAllocator cmdAllocatorDirect;
		internal static CommandQueue cmdQueueDirect;
		internal static GraphicsCommandList directCmdList;
		private static Fence fenceDirect;
		private static long fenceValueDirect;

		internal static CommandAllocator cmdAllocatorCopy;
		internal static CommandQueue cmdQueueCopy;
		internal static GraphicsCommandList copyCmdList;
		private static Fence fenceCopy;
		private static long fenceValueCopy;

		internal static CommandAllocator cmdAllocatorCompute;
		internal static CommandQueue cmdQueueCompute;
		internal static GraphicsCommandList computeCmdList;
		private static Fence fenceCompute;
		private static long fenceValueCompute;

		private static AutoResetEvent fenceEvent;

		#endregion


		/// <summary>
		/// Initialises Graphics, this includes
		/// attaching the graphics device,
		/// setting up the command lists and synchronisation,
		/// settup up the swap chain and resources for rendering to the screen.
		/// </summary>
		/// <param name="form"></param>
		internal static void Init(RenderForm form, bool isDebug) {

			Graphics.isDebug = isDebug;

			if (isDebug == true) {
				// Enable the D3D12 debug layer
				DebugInterface.Get().EnableDebugLayer();
				Log.Info("Enabled D3D12 Debugging Layer");
			}

			mainRenderForm = form;

			int width = form.ClientSize.Width;
			int height = form.ClientSize.Height;
			int refreshRate = 240;

			//try {

				SetupDevice();
				

				// Create an event handle to use for frame synchronisation
				fenceEvent = new AutoResetEvent(false);
				SetupDirectCommandAllocator();
				SetupCopyCommandAllocator();
				SetupComputeCommandAllocator();

				SetupSwapChain(width, height, refreshRate, cmdQueueDirect);

				alphaBlendShader = new ComputeShader(".res/ComputeShaders/alpha_blend.hlsl", "main");

				Log.Success("Initialised renderer");
			//}
			//catch (Exception e) {
			//	Log.Error("Failed to initialise renderer");
			//	Log.Raw(e);
			//}



		}


		private static void SetupDevice() {
			// Create the graphics device
			device = new Device(null, SharpDX.Direct3D.FeatureLevel.Level_11_0);

			descriptorHeapIncrement = Graphics.device.GetDescriptorHandleIncrementSize(DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);
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
			fenceValueDirect = 0;

			// Give the fenceDirect a default completed value
			fenceDirect.Signal(fenceValueDirect);

			directCmdList = CreateDirectCommandList();


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
			fenceValueCopy = 0;

			// Give the fenceDirect a default completed value
			fenceCopy.Signal(fenceValueCopy);

			copyCmdList = CreateCopyCommandList();


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

		private static void SetupComputeCommandAllocator() {

			// Create the command list
			// Command lists are created in the recording state, but there is nothing
			// to record yet. The main loop expects it to be closed, so close it now.
			cmdAllocatorCompute = device.CreateCommandAllocator(CommandListType.Compute);
			cmdQueueCompute = device.CreateCommandQueue(new CommandQueueDescription(CommandListType.Compute));


			// Create synchronisation objects
			fenceCompute = device.CreateFence(0, FenceFlags.None);
			fenceValueCompute = 0;

			// Give the fenceCompute a default completed value
			fenceCompute.Signal(fenceValueCompute);

			computeCmdList = CreateComputeCommandList();

		}
		internal static GraphicsCommandList CreateComputeCommandList() {
			GraphicsCommandList cmdList = device.CreateCommandList(CommandListType.Compute, cmdAllocatorCompute, null);
			cmdList.Close();
			return cmdList;
		}
		internal static void ResetComputeCommandList(GraphicsCommandList cmdList) {
			cmdList.Reset(cmdAllocatorCompute, null);
		}
		internal static void ExecuteComputeCommandList(GraphicsCommandList cmdList) {

			cmdQueueCompute.ExecuteCommandList(cmdList);
			fenceValueCompute++;
			cmdQueueCompute.Signal(fenceCompute, fenceValueCompute);

		}
		internal static void WaitForComputeCommandQueue() {
			WaitForFenceValue(fenceValueCompute, fenceCompute);
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
					ModeDescription = new ModeDescription(width, height, new Rational(refreshRate, 1), SharpDX.DXGI.Format.R8G8B8A8_UNorm),
					Usage = Usage.RenderTargetOutput,
					SwapEffect = SwapEffect.FlipDiscard,
					OutputHandle = mainRenderForm.Handle,
					Flags = SwapChainFlags.AllowModeSwitch,
					SampleDescription = new SampleDescription(1, 0),
					IsWindowed = true
				};
				SwapChain tempSwapChain = new SwapChain(factory, commandQueue, swapChainDesc);
				swapChain = tempSwapChain.QueryInterface<SwapChain3>();
				tempSwapChain.Dispose();
				frameIndex = swapChain.CurrentBackBufferIndex;

			}


			// Grab the swapchain resources
			swapchainResources = new Resource[swapChainFrameCount];
			for (int n = 0; n < swapChainFrameCount; n++) {
				swapchainResources[n] = swapChain.GetBackBuffer<Resource>(n);
			}

			mainTexture = new Texture(width, height, flags: ResourceFlags.AllowUnorderedAccess);


		}


		
		/// <summary>
		/// Shows the render target to the screen and swaps which resource is the render target
		/// </summary>
		internal static void Buffer() {

			
			BlitTexture(mainTexture, GetActiveResource());

			Graphics.WaitForDirectCommandQueue();
			Graphics.WaitForComputeCommandQueue();

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

		public static Resource GetActiveResource() {
			return swapchainResources[frameIndex];
		}

		public static void BlitTexture(Resource src, Resource dst, int xOffset = 0, int yOffset = 0, int zOffset = 0) {

			WaitForDirectCommandQueue();
			ResetDirectCommandList(directCmdList);

			TextureCopyLocation srcLocation = new TextureCopyLocation(src, 0);
			TextureCopyLocation dstLocation = new TextureCopyLocation(dst, 0);

			directCmdList.CopyTextureRegion(dstLocation, xOffset, yOffset, zOffset, srcLocation, null);

			directCmdList.Close();
			ExecuteDirectCommandList(directCmdList);

		}
		public static void BlitTexture(Texture src, Resource dst, int xOffset = 0, int yOffset = 0, int zOffset = 0) {
			BlitTexture(src.resource, dst, xOffset, yOffset, zOffset);
		}
		public static void BlitTexture(Resource src, Texture dst, int xOffset = 0, int yOffset = 0, int zOffset = 0) {
			BlitTexture(src, dst.resource, xOffset, yOffset, zOffset);
		}
		public static void BlitTexture(Texture src, Texture dst, int xOffset = 0, int yOffset = 0, int zOffset = 0) {
			BlitTexture(src.resource, dst.resource, xOffset, yOffset, zOffset);
		}

		public static void BlitBuffer(Resource src, Resource dst, int numBytes, int srcOffset = 0, int dstOffset = 0) {

			WaitForDirectCommandQueue();
			ResetDirectCommandList(directCmdList);

			directCmdList.CopyBufferRegion(dst, dstOffset, src, srcOffset, numBytes);

			directCmdList.Close();
			ExecuteDirectCommandList(directCmdList);

		}
		public static void BlitBuffer<T>(StructuredBuffer<T> src, Resource dst, int numBytes, int srcOffset = 0, int dstOffset = 0) where T : struct {
			BlitBuffer(src.resource, dst, numBytes, srcOffset, dstOffset);
		}
		public static void BlitBuffer<T>(Resource src, StructuredBuffer<T> dst, int numBytes, int srcOffset = 0, int dstOffset = 0) where T : struct {
			BlitBuffer(src, dst.resource, numBytes, srcOffset, dstOffset);
		}
		public static void BlitBuffer<T>(StructuredBuffer<T> src, StructuredBuffer<T> dst, int numBytes, int srcOffset = 0, int dstOffset = 0) where T : struct {
			BlitBuffer(src.resource, dst.resource, numBytes, srcOffset, dstOffset);
		}



		public static void AlphaBlendTextures(Texture underTexture, Texture overTexture, Texture resultTexture) {

			alphaBlendShader.SetTexture("underTexture", underTexture);
			alphaBlendShader.SetTexture("overTexture", overTexture);
			alphaBlendShader.SetTexture("resultTexture", resultTexture);
			alphaBlendShader.Dispatch("main", (int)MathF.Ceiling(resultTexture.width / 8f), (int)MathF.Ceiling(resultTexture.height / 8f), 1);

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