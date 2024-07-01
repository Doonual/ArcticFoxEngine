using SharpDX.DXGI;
using System.Threading;
using System;

namespace ArcticFoxEngine {
	using CoolClassLibrary;
	using SharpDX;
	using SharpDX.Direct3D12;
	using SharpDX.Windows;
	using Swan;
	using System.IO;
	using System.Windows.Forms;
	using static System.Windows.Forms.VisualStyles.VisualStyleElement;

	public static class Graphics {

		public static bool debug = false;

		static RenderForm mainRenderForm;

		#region Pipeline objects

		internal const int swapChainFrameCount = 2;
		internal static ViewportF viewport;
		internal static Rectangle scissorRect;

		private static SwapChain3 swapChain;
		internal static Device device;
		internal static readonly Resource[] renderTargets = new Resource[swapChainFrameCount];
		internal static RootSignature rootSignature;
		internal static PipelineState pipelineState;

		internal static DescriptorHeap renderTargetViewHeap;
		internal static int renderTargetViewDescriptorSize;

		#endregion

		#region Resources

		private static Vertex[] rawVertices;
		private static Resource vertexBuffer;
		private static VertexBufferView vertexBufferView;

		private static int[] rawIndices;
		private static Resource indexBuffer;
		private static IndexBufferView indexBufferView;

		internal static ConstBuffer<ConstantBuffer> offsetBuffer;

		#endregion
		#region Synchronisation objects

		internal static int frameIndex;
		private static AutoResetEvent fenceEvent;

		private static Fence fence;
		private static int fenceValue;

		#endregion

		public struct Vertex {
			public Vector3 Position;
			public Vector4 Color;
		};
		internal struct ConstantBuffer {
			public Vector4 Offset;
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
			SetupRenderAssets(width, height, refreshRate, Command.GetCommandQueue());

			SetupRootSignature();

			ShaderBytecode vertexShader = CompileShader("res/shaders.hlsl", ShaderType.Vertex);
			ShaderBytecode pixelShader = CompileShader("res/shaders.hlsl", ShaderType.Pixel);
			SetupPipeline(vertexShader, pixelShader);


			offsetBuffer = new ConstBuffer<ConstantBuffer>(1024 * 64);

			Vertex[] triangleVertices = new Vertex[] {
				new Vertex() {Position=new Vector3(-1f, -1f, 0.0f), Color = new Vector4(1.0f, 0.0f, 0.0f, 1.0f)},
				new Vertex() {Position=new Vector3(-1f, 1f, 0.0f), Color = new Vector4(0.0f, 0.0f, 1.0f, 1.0f)},
				new Vertex() {Position=new Vector3(1f, -1f, 0.0f), Color = new Vector4(0.0f, 1.0f, 0.0f, 1.0f)},
				new Vertex() {Position=new Vector3(1f, 1f, 0.0f), Color = new Vector4(1.0f, 1.0f, 0.0f, 1.0f)},
			};
			int[] triangleIndices = new int[] {
				0, 1, 2,
				2, 1, 3
			};
			SetVertexIndexBuffers(triangleVertices, triangleIndices);


			SetupSynchronisation();

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
		private static void SetupRenderAssets(int width, int height, int refreshRate, CommandQueue commandQueue) {

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



			// Create a render target view (RTV) descriptor heap
			DescriptorHeapDescription rtvHeapDesc = new DescriptorHeapDescription() {
				DescriptorCount = swapChainFrameCount,
				Flags = DescriptorHeapFlags.None,
				Type = DescriptorHeapType.RenderTargetView
			};
			renderTargetViewHeap = device.CreateDescriptorHeap(rtvHeapDesc);
			renderTargetViewDescriptorSize = device.GetDescriptorHandleIncrementSize(DescriptorHeapType.RenderTargetView);


			// Create frame resources from swap chain frames
			CpuDescriptorHandle rtvHandle = renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
			for (int n = 0; n < swapChainFrameCount; n++) {
				renderTargets[n] = swapChain.GetBackBuffer<Resource>(n);
				device.CreateRenderTargetView(renderTargets[n], null, rtvHandle);
				rtvHandle += renderTargetViewDescriptorSize;
			}

		}

		private static void SetupRootSignature() {
			// Basically what constants are you going to pass to the shaders

			// Create a root signature with one constant buffer
			RootParameter[] rootParameters = new RootParameter[] {

				new RootParameter(ShaderVisibility.Pixel, new DescriptorRange() {
					RangeType = DescriptorRangeType.ConstantBufferView,
					BaseShaderRegister = 0,
					OffsetInDescriptorsFromTableStart = int.MinValue,
					DescriptorCount = 1
				})

			};

			RootSignatureDescription rootSignatureDesc = new RootSignatureDescription(RootSignatureFlags.AllowInputAssemblerInputLayout, rootParameters);
			rootSignature = device.CreateRootSignature(rootSignatureDesc.Serialize());

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

		// Manage resource and memory
		internal static Resource CreateGPUResource(int size) {

			Resource res = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(size), ResourceStates.GenericRead);
			return res;

		}
		internal static void WriteGPUResource<T>(Resource resource, T[] data, int offset) where T : struct {

			// Copy the triangle data to the vertex buffer
			IntPtr pDataBegin = resource.Map(0);
			Utilities.Write(pDataBegin, data, offset, data.Length);
			resource.Unmap(0);

		}
		public static void SetVertexIndexBuffers(Vertex[] vertices, int[] indices) {

			// Note: using upload heaps to transfer static data like vert buffers is not 
			// recommended. Every time the GPU needs it, the upload heap will be marshalled 
			// over. Please read up on Default Heap usage. An upload heap is used here for 
			// code simplicity and because there are very few verts to actually transfer.


			rawVertices = vertices;
			rawIndices = indices;

			if (vertexBuffer != null) {
				vertexBuffer.Dispose();
			}
			if (indexBuffer != null) {
				indexBuffer.Dispose();
			}

			int vertexBufferSize = Utilities.SizeOf(vertices);
			vertexBuffer = CreateGPUResource(vertexBufferSize);
			WriteGPUResource(vertexBuffer, vertices, 0);

			vertexBufferView.BufferLocation = vertexBuffer.GPUVirtualAddress;
			vertexBufferView.StrideInBytes = Utilities.SizeOf<Vertex>();
			vertexBufferView.SizeInBytes = vertexBufferSize;

			int indexBufferSize = Utilities.SizeOf(indices);
			indexBuffer = CreateGPUResource(indexBufferSize);
			WriteGPUResource(indexBuffer, indices, 0);

			indexBufferView.BufferLocation = indexBuffer.GPUVirtualAddress;
			indexBufferView.SizeInBytes = indexBufferSize;
			indexBufferView.Format = Format.R32_UInt;


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

		static float time = 0f;

		public static void Render() {

			// Execute MainRender
			Command.ExecuteMainRender(vertexBufferView, indexBufferView, rawIndices.Length);

			// Present the frame
			swapChain.Present(1, 0);

			ConstantBuffer bufferData = new ConstantBuffer();
			bufferData.Offset.X = MathF.Cos(time * 0.5f) * 0.2f;
			bufferData.Offset.Y = MathF.Sin(time * 0.5f) * 0.2f;
			offsetBuffer.WriteToBuffer(bufferData);
			

			time += 1f / 60f;

		}

		public static void Dispose() {


			// Wait for the GPU to be done with all resources.
			WaitForPreviousFrame();

			foreach (Resource target in renderTargets) {
				target.Dispose();
			}
			rootSignature.Dispose();
			renderTargetViewHeap.Dispose();
			pipelineState.Dispose();
			vertexBuffer.Dispose();
			indexBuffer.Dispose();
			fence.Dispose();
			swapChain.Dispose();
			device.Dispose();
			Command.Dispose();

		}

	}
}
