using SharpDX.DXGI;
using System.Threading;
using System;

namespace ArcticFoxEngine {

	using SharpDX;
	using SharpDX.Direct3D12;
	using SharpDX.Windows;
	using System.Windows.Forms;

	public static class Graphics {

		public static bool debug = false;

		#region Pipeline objects

		const int swapChainFrameCount = 2;
		private static ViewportF viewport;
		private static Rectangle scissorRect;

		private static SwapChain3 swapChain;
		private static Device device;
		private static readonly Resource[] renderTargets = new Resource[swapChainFrameCount];
		private static RootSignature rootSignature;
		private static DescriptorHeap renderTargetViewHeap;
		private static PipelineState pipelineState;
		
		private static int rtvDescriptorSize;

		#endregion
		#region Command

		private static CommandAllocator commandAllocator;
		private static CommandQueue commandQueue;
		private static GraphicsCommandList mainRenderCMDList;

		#endregion
		#region Resources

		private static Vertex[] rawVertices;
		private static Resource vertexBuffer;
		private static VertexBufferView vertexBufferView;

		private static int[] rawIndices;
		private static Resource indexBuffer;
		private static IndexBufferView indexBufferView;

		#endregion
		#region Synchronisation objects

		private static int frameIndex;
		private static AutoResetEvent fenceEvent;

		private static Fence fence;
		private static int fenceValue;

		#endregion

		public struct Vertex {
			public Vector3 Position;
			public Vector4 Color;
		};

		// Initialise the graphics pipeline and all the graphics assets
		public static void Initialise(RenderForm form) {

			SetupRenderPipeline(form);
			LoadAssets();

		}
		private static void SetupRenderPipeline(RenderForm form) {

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

			// Create the graphics device and the swapChain
			device = new Device(null, SharpDX.Direct3D.FeatureLevel.Level_11_0);
			swapChain = CreateSwapChain(width, height, 60, form, device);
			commandAllocator = device.CreateCommandAllocator(CommandListType.Direct);

			// Create descriptor heaps
			// Describe and create a render target view (RTV) descriptor heap
			DescriptorHeapDescription rtvHeapDesc = new DescriptorHeapDescription() {
				DescriptorCount = swapChainFrameCount,
				Flags = DescriptorHeapFlags.None,
				Type = DescriptorHeapType.RenderTargetView
			};

			renderTargetViewHeap = device.CreateDescriptorHeap(rtvHeapDesc);
			rtvDescriptorSize = device.GetDescriptorHandleIncrementSize(DescriptorHeapType.RenderTargetView);

			// Create frame resources from swap chain frames
			CpuDescriptorHandle rtvHandle = renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
			for (int n = 0; n < swapChainFrameCount; n++) {
				renderTargets[n] = swapChain.GetBackBuffer<Resource>(n);
				device.CreateRenderTargetView(renderTargets[n], null, rtvHandle);
				rtvHandle += rtvDescriptorSize;
			}



		}
		private static void LoadAssets() {

			// Create an empty root signature
			RootSignatureDescription rootSignatureDesc = new RootSignatureDescription(RootSignatureFlags.AllowInputAssemblerInputLayout);
			rootSignature = device.CreateRootSignature(rootSignatureDesc.Serialize());

			// Create the pipeline state, which includes compiling and loading shaders

			#region Compiling shaders

			ShaderBytecode vertexShader;
			ShaderBytecode pixelShader;
			SharpDX.D3DCompiler.ShaderFlags flags = SharpDX.D3DCompiler.ShaderFlags.None;
			if (debug == true) {
				flags |= SharpDX.D3DCompiler.ShaderFlags.Debug;
			}

			string shader = File.ReadAllText("res/shaders.hlsl");
			SharpDX.D3DCompiler.Include include = new StandardIncludeHandler();

			vertexShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.Compile(shader, "Vertex_Main", "vs_5_0", flags, SharpDX.D3DCompiler.EffectFlags.None, new SharpDX.Direct3D.ShaderMacro[0], include));
			pixelShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.Compile(shader, "Pixel_Main", "ps_5_0", flags, SharpDX.D3DCompiler.EffectFlags.None, new SharpDX.Direct3D.ShaderMacro[0], include));

			#endregion

			// Input format
			InputElement[] inputElementDescs = new InputElement[] {
				new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
				new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 12, 0)
			};

			#region Setup graphics pileline state

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

			#endregion
			#region Create main render command list

			// Create the command list
			mainRenderCMDList = device.CreateCommandList(CommandListType.Direct, commandAllocator, pipelineState);
			// Command lists are created in the recording state, but there is nothing
			// to record yet. The main loop expects it to be closed, so close it now.
			mainRenderCMDList.Close();

			#endregion

			// Note: using upload heaps to transfer static data like vert buffers is not 
			// recommended. Every time the GPU needs it, the upload heap will be marshalled 
			// over. Please read up on Default Heap usage. An upload heap is used here for 
			// code simplicity and because there are very few verts to actually transfer.
			

			// Define the geometry for a triangle
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

			
			#region Synchronisation

			// Create synchronisation objects
			fence = device.CreateFence(0, FenceFlags.None);
			fenceValue = 1;

			// Create an event handle to use for frame synchronisation
			fenceEvent = new AutoResetEvent(false);

			#endregion

		}
		private static SwapChain3 CreateSwapChain(int width, int height, int refreshRate, RenderForm form, Device device) {

			SwapChain3 swapChain;

			using (Factory4 factory = new Factory4()) {

				// Describe and create the command queue
				CommandQueueDescription queueDesc = new CommandQueueDescription(CommandListType.Direct);
				commandQueue = device.CreateCommandQueue(queueDesc);

				// Describe and create the swap chain
				SwapChainDescription swapChainDesc = new SwapChainDescription() {
					BufferCount = swapChainFrameCount,
					ModeDescription = new ModeDescription(width, height, new Rational(refreshRate, 1), Format.R8G8B8A8_UNorm),
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

			return swapChain;

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
		
		// Instructions
		private static void PopulateCommandList() {
			
			// Command list allocators can only be reset when the associated 
			// command lists have finished execution on the GPU; apps should use 
			// fences to determine GPU execution progress
			commandAllocator.Reset();

			// However, when ExecuteCommandList() is called on a particular command 
			// list, that command list can then be reset at any time and must be before 
			// re-recording
			mainRenderCMDList.Reset(commandAllocator, pipelineState);


			// Set necessary state
			mainRenderCMDList.SetGraphicsRootSignature(rootSignature);
			mainRenderCMDList.SetViewport(viewport);
			mainRenderCMDList.SetScissorRectangles(scissorRect);

			// Indicate that the back buffer will be used as a render target
			mainRenderCMDList.ResourceBarrierTransition(renderTargets[frameIndex], ResourceStates.Present, ResourceStates.RenderTarget);

			CpuDescriptorHandle rtvHandle = renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
			rtvHandle += frameIndex * rtvDescriptorSize;
			mainRenderCMDList.SetRenderTargets(rtvHandle, null);

			// Record commands
			mainRenderCMDList.ClearRenderTargetView(rtvHandle, new Color4(0f, 0f, 0f, 1), 0, null);

			mainRenderCMDList.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
			mainRenderCMDList.SetIndexBuffer(indexBufferView);
			mainRenderCMDList.SetVertexBuffer(0, vertexBufferView);
			
			mainRenderCMDList.DrawIndexedInstanced(rawIndices.Length, 1, 0, 0, 0);

			// Indicate that the back buffer will now be used to present
			mainRenderCMDList.ResourceBarrierTransition(renderTargets[frameIndex], ResourceStates.RenderTarget, ResourceStates.Present);

			mainRenderCMDList.Close();
		}

		// Wait the previous command list to finish executing.
		internal static void WaitForPreviousFrame() {
			// WAITING FOR THE FRAME TO COMPLETE BEFORE CONTINUING IS NOT BEST PRACTICE. 
			// This is code implemented as such for simplicity. 

			int localFence = fenceValue;
			commandQueue.Signal(fence, localFence);
			fenceValue++;

			// Wait until the previous frame is finished.
			if (fence.CompletedValue < localFence) {
				fence.SetEventOnCompletion(localFence, fenceEvent.SafeWaitHandle.DangerousGetHandle());
				fenceEvent.WaitOne();
			}

			frameIndex = swapChain.CurrentBackBufferIndex;
		}

		public static void Render() {

			// Record all the commands we need to render the scene into the command list
			PopulateCommandList();

			// Execute the command list
			commandQueue.ExecuteCommandList(mainRenderCMDList);

			// Present the frame
			swapChain.Present(1, 0);


		}

		public static void Dispose() {


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
			mainRenderCMDList.Dispose();
			vertexBuffer.Dispose();
			indexBuffer.Dispose();
			fence.Dispose();
			swapChain.Dispose();
			device.Dispose();

		}

	}
}
