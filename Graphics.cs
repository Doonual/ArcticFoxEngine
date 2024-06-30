using SharpDX.DXGI;
using System.Threading;
using System;
using CoolClassLibrary;

namespace Engine {

	using SharpDX;
	using SharpDX.Direct3D12;
	using SharpDX.Windows;

	internal class HelloTriangle : IDisposable {

		public static bool debug = false;

		const int frameCount = 2;
		private ViewportF viewport;
		private Rectangle scissorRect;

		// Pipeline objects
		private SwapChain3 swapChain;
		private Device device;
		private readonly Resource[] renderTargets = new Resource[frameCount];
		private CommandAllocator commandAllocator;
		private CommandQueue commandQueue;
		private RootSignature rootSignature;
		private DescriptorHeap renderTargetViewHeap;
		private PipelineState pipelineState;
		private GraphicsCommandList commandList;
		private int rtvDescriptorSize;

		// App resources
		Resource vertexBuffer;
		VertexBufferView vertexBufferView;

		// Synchronisation objects
		private int frameIndex;
		private AutoResetEvent fenceEvent;

		private Fence fence;
		private int fenceValue;

		struct Vertex {
			public Vector3 Position;
			public Vector4 Color;
		};

		/// <summary>
		/// Initialise pipeline and assets
		/// </summary>
		/// <param name="form">The form</param>
		public void Initialise(RenderForm form) {

			LoadPipeline(form);
			LoadAssets();

		}

		private void LoadPipeline(RenderForm form) {

			int width = form.ClientSize.Width;
			int height = form.ClientSize.Height;

			viewport.Width = width;
			viewport.Height = height;
			viewport.MaxDepth = 1.0f;

			scissorRect.Right = width;
			scissorRect.Bottom = height;

			if (debug == true) {
				// Enable the D3D12 debug layer
				DebugInterface.Get().EnableDebugLayer();
			}
			

			device = new Device(null, SharpDX.Direct3D.FeatureLevel.Level_11_0);
			using (Factory4 factory = new Factory4()) {

				// Describe and create the command queue
				CommandQueueDescription queueDesc = new CommandQueueDescription(CommandListType.Direct);
				commandQueue = device.CreateCommandQueue(queueDesc);

				// Describe and create the swap chain
				SwapChainDescription swapChainDesc = new SwapChainDescription() {
					BufferCount = frameCount,
					ModeDescription = new ModeDescription(width, height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
					Usage = Usage.RenderTargetOutput,
					SwapEffect = SwapEffect.FlipDiscard,
					OutputHandle = form.Handle,
					//Flags = SwapChainFlags.None,
					SampleDescription = new SampleDescription(1, 0),
					IsWindowed = true
				};
				SwapChain tempSwapChain = new SwapChain(factory, commandQueue, swapChainDesc);
				swapChain = tempSwapChain.QueryInterface<SwapChain3>();
				tempSwapChain.Dispose();
				frameIndex = swapChain.CurrentBackBufferIndex;

			}

			// Create descriptor heaps
			// Describe and create a render target view (RTV) descriptor heap
			DescriptorHeapDescription rtvHeapDesc = new DescriptorHeapDescription() {
				DescriptorCount = frameCount,
				Flags = DescriptorHeapFlags.None,
				Type = DescriptorHeapType.RenderTargetView
			};

			renderTargetViewHeap = device.CreateDescriptorHeap(rtvHeapDesc);
			rtvDescriptorSize = device.GetDescriptorHandleIncrementSize(DescriptorHeapType.RenderTargetView);

			// Create frame resources
			CpuDescriptorHandle rtvHandle = renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
			for (int n = 0; n < frameCount; n++) {
				renderTargets[n] = swapChain.GetBackBuffer<Resource>(n);
				device.CreateRenderTargetView(renderTargets[n], null, rtvHandle);
				rtvHandle += rtvDescriptorSize;
			}

			commandAllocator = device.CreateCommandAllocator(CommandListType.Direct);

		}

		private void LoadAssets() {

			// Create an empty root signature
			RootSignatureDescription rootSignatureDesc = new RootSignatureDescription(RootSignatureFlags.AllowInputAssemblerInputLayout);
			rootSignature = device.CreateRootSignature(rootSignatureDesc.Serialize());

			// Create the pipeline state, which includes compiling and loading shaders

			ShaderBytecode vertexShader;
			ShaderBytecode pixelShader;

			if (debug == true) {
				vertexShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile("shaders.hlsl", "VSMain", "vs_5_0", SharpDX.D3DCompiler.ShaderFlags.Debug));
				pixelShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile("shaders.hlsl", "PSMain", "ps_5_0", SharpDX.D3DCompiler.ShaderFlags.Debug));
			}
			else {
				vertexShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile("shaders.hlsl", "VSMain", "vs_5_0"));
				pixelShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile("shaders.hlsl", "PSMain", "ps_5_0"));
			}


			InputElement[] inputElementDescs = new InputElement[] {
				new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
				new InputElement("COLOR",0,Format.R32G32B32A32_Float,12,0)
			};
			GraphicsPipelineStateDescription psonDesc = new GraphicsPipelineStateDescription() {

				InputLayout = new InputLayoutDescription(inputElementDescs),
				RootSignature = rootSignature,
				VertexShader = vertexShader,
				PixelShader = pixelShader,
				RasterizerState = RasterizerStateDescription.Default(),
				BlendState = BlendStateDescription.Default(),
				DepthStencilFormat = SharpDX.DXGI.Format.D32_Float,
				DepthStencilState = new DepthStencilStateDescription() { IsDepthEnabled = false, IsStencilEnabled = false},
				SampleMask = int.MaxValue,
				PrimitiveTopologyType = PrimitiveTopologyType.Triangle,
				RenderTargetCount = 1,
				Flags = PipelineStateFlags.None,
				SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
				StreamOutput = new StreamOutputDescription()

			};
			psonDesc.RenderTargetFormats[0] = SharpDX.DXGI.Format.R8G8B8A8_UNorm;
			pipelineState = device.CreateGraphicsPipelineState(psonDesc);

			// Create the command list
			commandList = device.CreateCommandList(CommandListType.Direct, commandAllocator, pipelineState);

			// Create the vertex buffer
			float aspectRatio = viewport.Width / viewport.Height;

			// Define the geometry for a triangle
			Vertex[] triangleVertices = new Vertex[] {
				new Vertex() {Position=new Vector3(-1f, 1f, 0.0f), Color = new Vector4(1.0f, 0.0f, 0.0f, 1.0f)},
				new Vertex() {Position=new Vector3(1f, -1f, 0.0f), Color = new Vector4(0.0f, 1.0f, 0.0f, 1.0f)},
				new Vertex() {Position=new Vector3(-1f, -1f, 0.0f), Color = new Vector4(0.0f, 0.0f, 1.0f, 1.0f)},

				new Vertex() {Position=new Vector3(-1f, 1f, 0.0f), Color = new Vector4(1.0f, 0.0f, 0.0f, 1.0f)},
				new Vertex() {Position=new Vector3(1f, 1f, 0.0f), Color = new Vector4(1.0f, 1.0f, 0.0f, 1.0f)},
				new Vertex() {Position=new Vector3(1f, -1f, 0.0f), Color = new Vector4(0.0f, 1.0f, 0.0f, 1.0f)},
			};
			int vertexBufferSize = Utilities.SizeOf(triangleVertices);

			// Note: using upload heaps to transfer static data like vert buffers is not 
			// recommended. Every time the GPU needs it, the upload heap will be marshalled 
			// over. Please read up on Default Heap usage. An upload heap is used here for 
			// code simplicity and because there are very few verts to actually transfer.
			vertexBuffer = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(vertexBufferSize), ResourceStates.GenericRead);

			// Copy the triangle data to the vertex buffer
			IntPtr pVertexDataBegin = vertexBuffer.Map(0);
			Utilities.Write(pVertexDataBegin, triangleVertices, 0, triangleVertices.Length);
			vertexBuffer.Unmap(0);

			// Initialize the vertex buffer view.
			vertexBufferView = new VertexBufferView();
			vertexBufferView.BufferLocation = vertexBuffer.GPUVirtualAddress;
			vertexBufferView.StrideInBytes = Utilities.SizeOf<Vertex>();
			vertexBufferView.SizeInBytes = vertexBufferSize;

			// Command lists are created in the recording state, but there is nothing
			// to record yet. The main loop expects it to be closed, so close it now.
			commandList.Close();

			// Create synchronisation objects
			fence = device.CreateFence(0, FenceFlags.None);
			fenceValue = 1;

			// Create an event handle to use for frame synchronisation
			fenceEvent = new AutoResetEvent(false);

		}

		private void PopulateCommandList() {
			
			// Command list allocators can only be reset when the associated 
			// command lists have finished execution on the GPU; apps should use 
			// fences to determine GPU execution progress.
			commandAllocator.Reset();

			// However, when ExecuteCommandList() is called on a particular command 
			// list, that command list can then be reset at any time and must be before 
			// re-recording.
			commandList.Reset(commandAllocator, pipelineState);


			// Set necessary state.
			commandList.SetGraphicsRootSignature(rootSignature);
			commandList.SetViewport(viewport);
			commandList.SetScissorRectangles(scissorRect);

			// Indicate that the back buffer will be used as a render target.
			commandList.ResourceBarrierTransition(renderTargets[frameIndex], ResourceStates.Present, ResourceStates.RenderTarget);

			CpuDescriptorHandle rtvHandle = renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
			rtvHandle += frameIndex * rtvDescriptorSize;
			commandList.SetRenderTargets(rtvHandle, null);

			// Record commands.
			commandList.ClearRenderTargetView(rtvHandle, new Color4(0f, 0f, 0f, 1), 0, null);

			commandList.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
			commandList.SetVertexBuffer(0, vertexBufferView);
			commandList.DrawInstanced(3, 1, 0, 0);
			commandList.DrawInstanced(3, 1, 3, 0);

			// Indicate that the back buffer will now be used to present.
			commandList.ResourceBarrierTransition(renderTargets[frameIndex], ResourceStates.RenderTarget, ResourceStates.Present);

			commandList.Close();
		}

		/// <summary> 
		/// Wait the previous command list to finish executing. 
		/// </summary> 
		private void WaitForPreviousFrame() {
			// WAITING FOR THE FRAME TO COMPLETE BEFORE CONTINUING IS NOT BEST PRACTICE. 
			// This is code implemented as such for simplicity. 

			int localFence = fenceValue;
			commandQueue.Signal(this.fence, localFence);
			fenceValue++;

			// Wait until the previous frame is finished.
			if (this.fence.CompletedValue < localFence) {
				this.fence.SetEventOnCompletion(localFence, fenceEvent.SafeWaitHandle.DangerousGetHandle());
				fenceEvent.WaitOne();
			}

			frameIndex = swapChain.CurrentBackBufferIndex;
		}


		public void Update() { }
		public void Render() {

			// Record all the commands we need to render the scene into the command list
			PopulateCommandList();

			// Execute the command list
			commandQueue.ExecuteCommandList(commandList);

			// Present the frame
			swapChain.Present(1, 0);

			WaitForPreviousFrame();


		}

		public void Dispose() {


			// Wait for the GPU to be done with all resources.
			WaitForPreviousFrame();

			foreach (Resource target in renderTargets) {
				target.Dispose();
			}
			commandAllocator.Dispose();
			commandQueue.Dispose();
			rootSignature.Dispose();
			renderTargetViewHeap.Dispose();
			pipelineState.Dispose();
			commandList.Dispose();
			vertexBuffer.Dispose();
			fence.Dispose();
			swapChain.Dispose();
			device.Dispose();

		}

	}
}
