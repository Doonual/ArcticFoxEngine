using SharpDX.DXGI;

namespace ArcticFoxEngine {

	using CoolClassLibrary;
	using SharpDX;
	using SharpDX.Direct3D12;
	using SharpDX.Windows;
	using System.IO;
	using System.Runtime.InteropServices;

	public static class Graphics {

		public static bool debug = false;

		static RenderForm mainRenderForm;

		#region Pipeline objects

		internal static ViewportF viewport;
		internal static Rectangle scissorRect;

		private static SwapChain3 swapChain;
		internal static Device device;
		
		internal static RootSignature rootSignature;
		internal static PipelineState pipelineState;

		internal const int swapChainFrameCount = 2;

		#endregion
		#region Synchronisation objects

		internal static int frameIndex;
		private static AutoResetEvent fenceEvent;

		private static Fence fence;
		private static int fenceValue;

		#endregion

		
		internal struct ShaderInfo {

			public Matrix projectionMatrix;
			public Matrix worldToCameraMatrix;

			public int screenWidth;
			public int screenHeight;
			public float aspectRatio;

		};

		
		// Main setup function
		// Combines all the individual steps to setting up rendering
		public static void SetupRenderer(RenderForm form) {

			if (debug == true) {
				// Enable the D3D12 debug layer
				DebugInterface.Get().EnableDebugLayer();
			}

			mainRenderForm = form;

			int width = form.ClientSize.Width;
			int height = form.ClientSize.Height;
			int refreshRate = 60;

			SetupViewport(width, height);
			SetupDevice(width, height, refreshRate);

			Command.SetupCommand();
			SetupSwapChain(width, height, refreshRate, Command.GetCommandQueue());

			GraphicsResources.SetupResources(device, swapChain);
			GraphicsResources.SetupRootSignature();

			ShaderBytecode vertexShader = CompileShader("res/shaders.hlsl", ShaderType.Vertex);
			ShaderBytecode pixelShader = CompileShader("res/shaders.hlsl", ShaderType.Pixel);
			SetupPipeline(vertexShader, pixelShader);

			SetupSynchronisation();
			LinkClasses();

		}


		private static void SetupViewport(int viewportWidth, int viewportHeight) {
			viewport.Width = viewportWidth;
			viewport.Height = viewportHeight;
			viewport.MaxDepth = 1.0f;

			scissorRect.Right = viewportWidth;
			scissorRect.Bottom = viewportHeight;
		}
		private static void SetupDevice(int swapchainWidth, int swapchainHeight, int refreshRate) {

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
					//Flags = SwapChainFlags.None,
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
				new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 12, 0)
			};


			GraphicsPipelineStateDescription psonDesc = new GraphicsPipelineStateDescription() {

				InputLayout = new InputLayoutDescription(inputElementDescs),
				RootSignature = rootSignature,
				VertexShader = vertexShader,
				PixelShader = pixelShader,
				RasterizerState = RasterizerStateDescription.Default(),
				BlendState = BlendStateDescription.Default(),
				DepthStencilFormat = SharpDX.DXGI.Format.D32_Float,
				DepthStencilState = new DepthStencilStateDescription() { IsDepthEnabled = false, IsStencilEnabled = false },
				SampleMask = int.MaxValue,
				PrimitiveTopologyType = PrimitiveTopologyType.Triangle,
				RenderTargetCount = 1,
				Flags = PipelineStateFlags.None,
				SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
				StreamOutput = new StreamOutputDescription()

			};
			psonDesc.RenderTargetFormats[0] = SharpDX.DXGI.Format.R8G8B8A8_UNorm;
			pipelineState = device.CreateGraphicsPipelineState(psonDesc);

		}

		private static void SetupSynchronisation() {

			// Create synchronisation objects
			fence = device.CreateFence(0, FenceFlags.None);
			fenceValue = 1;

			// Create an event handle to use for frame synchronisation
			fenceEvent = new AutoResetEvent(false);

		}
		
		
		private static void LinkClasses() {
			Screen.LinkRenderForm(mainRenderForm);
		}

		internal enum ShaderType {
			Vertex,
			Pixel
		}
		internal static ShaderBytecode CompileShader(string path, ShaderType type) {

			string shaderCode = File.ReadAllText(path);

			SharpDX.D3DCompiler.ShaderFlags flags = debug ? SharpDX.D3DCompiler.ShaderFlags.None : SharpDX.D3DCompiler.ShaderFlags.Debug;
			SharpDX.D3DCompiler.Include include = new StandardIncludeHandler();

			switch (type) {

				case ShaderType.Vertex:
				return new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.Compile(shaderCode, "Vertex_Main", "vs_5_0", flags, SharpDX.D3DCompiler.EffectFlags.None, new SharpDX.Direct3D.ShaderMacro[0], include));

				case ShaderType.Pixel:
				return new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.Compile(shaderCode, "Pixel_Main", "ps_5_0", flags, SharpDX.D3DCompiler.EffectFlags.None, new SharpDX.Direct3D.ShaderMacro[0], include));

				default:
				return null;


			}

			
		}

		internal static void WriteGPUResource<T>(Resource resource, T[] data, int offset) where T : struct {

			// Copy the triangle data to the vertex buffer
			IntPtr pDataBegin = resource.Map(0);
			Utilities.Write(pDataBegin, data, offset, data.Length);
			resource.Unmap(0);

		}
		

		// Wait the previous command list to finish executing.
		internal static void WaitForPreviousFrame() {
			// WAITING FOR THE FRAME TO COMPLETE BEFORE CONTINUING IS NOT BEST PRACTICE. 
			// This is code implemented as such for simplicity. 

			int localFence = fenceValue;
			Command.GetCommandQueue().Signal(fence, localFence);
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
			swapChain.Present(1, 0);
			
		}

		public static void Dispose() {


			// Wait for the GPU to be done with all resources.
			WaitForPreviousFrame();

			GraphicsResources.Dispose();
			rootSignature.Dispose();
			pipelineState.Dispose();
			fence.Dispose();
			swapChain.Dispose();
			device.Dispose();
			Command.Dispose();

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