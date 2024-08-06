using SharpDX.DXGI;
using ArcticFoxEngine.Input;

namespace ArcticFoxEngine {
	using ArcticFoxEngine.Backend;
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

		
		internal static PipelineState pipelineState;

		internal static int frameIndex;
		private static AutoResetEvent fenceEvent;

		private static Fence fence;
		private static int fenceValue;
		
		// Main setup function
		// Combines all the individual steps to setting up rendering
		public static void SetupRenderer(RenderForm form) {

			if (isDebug == true) {
				// Enable the D3D12 debug layer
				DebugInterface.Get().EnableDebugLayer();
			}

			mainRenderForm = form;

			int width = form.ClientSize.Width;
			int height = form.ClientSize.Height;
			int refreshRate = 60;

			try {
				SetupDevice();

				GPU_Render.SetupCommand();
				SetupSwapChain(width, height, refreshRate, GPU_Render.GetCommandQueue());
				
				RenderResources.LoadResources(width, height);

				ShaderBytecode vertexShader = CompileShader(".res/shaders.hlsl", ShaderType.Vertex);
				ShaderBytecode pixelShader = CompileShader(".res/shaders.hlsl", ShaderType.Pixel);
				SetupPipeline(vertexShader, pixelShader);

				SetupSynchronisation();

				Screen.InitScreen(mainRenderForm, swapChain);
				InputManager.InitInput();

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

		}

		private static void SetupPipeline(ShaderBytecode vertexShader, ShaderBytecode pixelShader) {

			// Input format
			InputElement[] inputElementDescs = new InputElement[] {
				new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
				new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 12, 0),
			};

			DepthStencilOperationDescription defaultStencilOp = new DepthStencilOperationDescription() {
				FailOperation = StencilOperation.Keep,
				DepthFailOperation = StencilOperation.Keep,
				PassOperation = StencilOperation.Keep,
				Comparison = Comparison.Always
			};
			DepthStencilStateDescription depthState = new DepthStencilStateDescription() {

				IsDepthEnabled = true,
				DepthWriteMask = DepthWriteMask.All,
				DepthComparison = Comparison.Less,

				IsStencilEnabled = false,
				StencilReadMask = 0xff,
				StencilWriteMask = 0xff,
				FrontFace = defaultStencilOp,
				BackFace = defaultStencilOp,

			};

			GraphicsPipelineStateDescription psonDesc = new GraphicsPipelineStateDescription() {

				InputLayout = new InputLayoutDescription(inputElementDescs),
				RootSignature = RenderResources.rootSignature,
				VertexShader = vertexShader,
				PixelShader = pixelShader,
				RasterizerState = RasterizerStateDescription.Default(),
				BlendState = BlendStateDescription.Default(),
				DepthStencilFormat = Format.D32_Float,
				DepthStencilState = depthState,
				SampleMask = int.MaxValue,
				PrimitiveTopologyType = PrimitiveTopologyType.Triangle,
				RenderTargetCount = 1,
				Flags = PipelineStateFlags.None,
				SampleDescription = new SampleDescription(1, 0),
				StreamOutput = new StreamOutputDescription()

			};
			psonDesc.RenderTargetFormats[0] = Format.R8G8B8A8_UNorm;
			pipelineState = device.CreateGraphicsPipelineState(psonDesc);

		}

		private static void SetupSynchronisation() {

			// Create synchronisation objects
			fence = device.CreateFence(0, FenceFlags.None);
			fenceValue = 1;

			// Create an event handle to use for frame synchronisation
			fenceEvent = new AutoResetEvent(false);

		}

		internal enum ShaderType {
			Vertex,
			Pixel
		}
		internal static ShaderBytecode CompileShader(string path, ShaderType type) {


			string shaderCode = File.ReadAllText(path);

			SharpDX.D3DCompiler.ShaderFlags flags = isDebug ? SharpDX.D3DCompiler.ShaderFlags.None : SharpDX.D3DCompiler.ShaderFlags.Debug;
			SharpDX.D3DCompiler.Include include = new StandardIncludeHandler();

			string entrypoint = "";
			string profile = "";

			switch (type) {

				case ShaderType.Vertex:
				entrypoint = "Vertex_Main";
				profile = "vs_5_0";
				break;

				case ShaderType.Pixel:
				entrypoint = "Pixel_Main";
				profile = "ps_5_0";
				break;

			}
			ShaderBytecode compiledShader = null;
			try {
				compiledShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.Compile(shaderCode, entrypoint, profile, flags, SharpDX.D3DCompiler.EffectFlags.None, new SharpDX.Direct3D.ShaderMacro[0], include));
				switch (type) {
					case ShaderType.Vertex:
					Log.Success("Compiled vertex shader");
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

					case ShaderType.Pixel:
					Log.Error("Failed to compile pixel shader");
					break;
				}
				Log.Raw(e);
			}

			return null;
			


		}
		

		// Wait the previous command list to finish executing.
		internal static void WaitForPreviousFrame() {
			// WAITING FOR THE FRAME TO COMPLETE BEFORE CONTINUING IS NOT BEST PRACTICE. 
			// This is code implemented as such for simplicity. 

			int localFence = fenceValue;
			GPU_Render.GetCommandQueue().Signal(fence, localFence);
			fenceValue++;

			// Wait until the previous frame is finished.
			if (fence.CompletedValue < localFence) {
				fence.SetEventOnCompletion(localFence, fenceEvent.SafeWaitHandle.DangerousGetHandle());
				fenceEvent.WaitOne();
			}

			frameIndex = swapChain.CurrentBackBufferIndex;
		}

		public static void Buffer() {

			// Present the frame
			try {
				CheckHRESULT(swapChain.Present(1, 0));
			}
			catch (Exception e) {
				CheckHRESULT(device.DeviceRemovedReason);
				Log.Error(e);
			}

		}

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

		public static void Dispose() {


			// Wait for the GPU to be done with all resources.
			WaitForPreviousFrame();

			RenderResources.Dispose();
			pipelineState.Dispose();
			fence.Dispose();
			swapChain.Dispose();
			device.Dispose();
			GPU_Render.Dispose();

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