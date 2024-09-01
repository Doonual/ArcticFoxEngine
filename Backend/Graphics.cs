using SharpDX.DXGI;
using ArcticFoxEngine.Input;

namespace ArcticFoxEngine {
	using ArcticFoxEngine.Backend;
	using ArcticFoxEngine.Debug;
	using CoolClassLibrary;
	using SharpDX;
	using SharpDX.Direct3D12;
	using SharpDX.Windows;
	using System.IO;

	public static class Graphics {

		public static bool isDebug = false;

		internal static Device device;

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

		internal static CommandAllocator cmdAllocator;
		internal static CommandQueue cmdQueue;
		internal static GraphicsCommandList cmdList;
		private static AutoResetEvent fenceEvent;
		private static Fence fence;
		private static int fenceValue;

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

				SetupCommandList();
				SetupSwapChain(width, height, refreshRate, cmdQueue);

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
		private static void SetupCommandList() {

			// Create the command list
			// Command lists are created in the recording state, but there is nothing
			// to record yet. The main loop expects it to be closed, so close it now.
			cmdAllocator = device.CreateCommandAllocator(CommandListType.Direct);
			cmdQueue = device.CreateCommandQueue(new CommandQueueDescription(CommandListType.Direct));
			cmdList = device.CreateCommandList(CommandListType.Direct, cmdAllocator, Backend.Render.GPU_Render.pipelineState);
			cmdList.Close();

			// Create synchronisation objects
			fence = device.CreateFence(0, FenceFlags.None);
			fenceValue = 1;
			// Create an event handle to use for frame synchronisation
			fenceEvent = new AutoResetEvent(false);

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
		
		internal enum ShaderType {
			Vertex,
			Geometry,
			Pixel,
		}
		/// <summary>
		/// Compiles the shader specified by the path
		/// </summary>
		/// <param name="path">Path to the shader code</param>
		/// <param name="type">The type of shader being compiled</param>
		/// <returns>The bytecode for that shader</returns>
		internal static ShaderBytecode CompileShader(string path, ShaderType type) {

			#region Changing root folder of #includes

			string rootPath = "";
			for (int i = path.Length - 1; i >= 0; i --) {
				if (path[i] == '/') {
					rootPath = new string(path.Take(i + 1).ToArray());
				}
			}

			string shaderCode = File.ReadAllText(path);
			string includeDirective = "#include \"";

			string includeEditedShaderCode = "";
			for (int i = 0; i < shaderCode.Length; i ++) {
				

				if (includeDirective.Length == 0) {
					includeEditedShaderCode += rootPath;
					includeDirective = "#include \"";
				}
				else {
					if (shaderCode[i] == includeDirective[0]) {
						includeDirective = new string(includeDirective.Skip(1).ToArray());
					}
					else {
						includeDirective = "#include \"";
					}
				}

				includeEditedShaderCode += shaderCode[i];

			}

			#endregion


			SharpDX.D3DCompiler.ShaderFlags flags = isDebug ? SharpDX.D3DCompiler.ShaderFlags.None : SharpDX.D3DCompiler.ShaderFlags.Debug;
			SharpDX.D3DCompiler.Include include = new StandardIncludeHandler();

			string entrypoint = "";
			string profile = "";

			switch (type) {

				case ShaderType.Vertex:
				entrypoint = "Vertex_Main";
				profile = "vs_5_0";
				break;

				case ShaderType.Geometry:
				entrypoint = "Geometry_Main";
				profile = "gs_5_0";
				break;

				case ShaderType.Pixel:
				entrypoint = "Pixel_Main";
				profile = "ps_5_0";
				break;

			}
			ShaderBytecode compiledShader = null;
			try {
				compiledShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.Compile(includeEditedShaderCode, entrypoint, profile, flags, SharpDX.D3DCompiler.EffectFlags.None, new SharpDX.Direct3D.ShaderMacro[0], include));
				switch (type) {
					case ShaderType.Vertex:
					Log.Success("Compiled vertex shader");
					break;

					case ShaderType.Geometry:
					Log.Success("Compiled geometry shader");
					break;

					case ShaderType.Pixel:
					Log.Success("Compiled pixel shader");
					break;
				}
				return compiledShader;
			}
			catch (Exception e) {
				switch (type) {
					case ShaderType.Vertex:
					Log.Error("Failed to compile vertex shader");
					break;

					case ShaderType.Geometry:
					Log.Error("Failed to compile geometry shader");
					break;

					case ShaderType.Pixel:
					Log.Error("Failed to compile pixel shader");
					break;
				}
				Log.Raw(e);
			}

			return null;
			


		}

		/// <summary>
		/// Blocks execution until the command list is free
		/// </summary>
		internal static void WaitForCmdList() {
			// WAITING FOR THE FRAME TO COMPLETE BEFORE CONTINUING IS NOT BEST PRACTICE. 
			// This is code implemented as such for simplicity. 

			int localFence = fenceValue;
			cmdQueue.Signal(fence, localFence);
			fenceValue++;

			// Wait until the previous frame is finished.
			if (fence.CompletedValue < localFence) {
				fence.SetEventOnCompletion(localFence, fenceEvent.SafeWaitHandle.DangerousGetHandle());
				fenceEvent.WaitOne();
			}


		}
		/// <summary>
		/// Shows the render target to the screen and swaps which resource is the render target
		/// </summary>
		internal static void Buffer() {
			WaitForCmdList();
			// Present the frame
			try {
				CheckHRESULT(swapChain.Present(1, 0));
				frameIndex = swapChain.CurrentBackBufferIndex;
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
			fence.Dispose();
			cmdAllocator.Dispose();
			cmdQueue.Dispose();
			cmdList.Dispose();

		}

	}

	internal class StandardIncludeHandler : CppObject, SharpDX.D3DCompiler.Include {
		public IDisposable Shadow { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		internal StandardIncludeHandler() : base(new IntPtr(1)) { }
		public void Close(Stream stream) { }
		public Stream Open(SharpDX.D3DCompiler.IncludeType type, string fileName, Stream parentStream) {
			Log.Info("Hi");
			throw new NotImplementedException();
		}
	}

}