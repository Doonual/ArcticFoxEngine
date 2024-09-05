using ArcticFoxEngine.Backend;
using ArcticFoxEngine.Nodes;
using ArcticFoxEngine.Debug;
using ArcticFoxEngine;
using CoolClassLibrary;
using SharpDX;

using SharpDX.DXGI;
using SharpDX.Direct3D12;

namespace ArcticFoxEngine.Backend.Render {

	using SharpDX.Direct3D12;

	/// <summary>
	/// Encapsulates all the tasks required to render a GeometryResources instance
	/// </summary>
	public static class GPU_Render {

		private static bool disposed = true;
		
		internal static PipelineState pipelineState;
		internal static RootSignature rootSignature;
		internal static DescriptorHeap descHeap;

		internal static int dh_renderInfoStart = 0;
		internal static int dh_textureDataStart = 1;
		internal static int dh_objectDataStart = 16;

		// Descriptor heap usage
		// [0]			Render info
		// [1, 16]		Texture data
		// [16, 2063]	Object data

		internal static int descHeapIncrement;
		internal static ConstBuffer<RenderInfo> renderInfo;

		internal static GeometryResources mainGeometry;

		/// <summary>
		/// Initialises the main render pipeline
		/// Includes creating the root signature and descriptor heap
		/// As well as setting up the rendering pipeline
		/// </summary>
		/// <param name="renderWidth">The width of the render target</param>
		/// <param name="renderHeight">The height of the render target</param>
		internal static void Init(int renderWidth, int renderHeight) {
			if (disposed == false) { Log.Warn("Cannot initialise GPU_Render, already initialised"); return; }
			disposed = false;

			LoadResources(renderWidth, renderHeight);
			pipelineState = null;
			SetupPipeline();

			mainGeometry = new GeometryResources();

		}


		private static void SetupPipeline() {

			if (pipelineState != null) { pipelineState.Dispose(); }


			ShaderBytecode vertexShader = Graphics.CompileShader(".res/VertexShader.hlsl", Graphics.ShaderType.Vertex);
			ShaderBytecode geometryShader = Graphics.CompileShader(".res/GeometryShader.hlsl", Graphics.ShaderType.Geometry);
			ShaderBytecode pixelShader = Graphics.CompileShader(".res/PixelShader.hlsl", Graphics.ShaderType.Pixel);

			// Input format
			InputElement[] inputElementDescs = new InputElement[] {
				new InputElement("SV_Position", 0, Format.R32G32B32_Float, 0, 0),
				new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 12, 0),
				new InputElement("TEXCOORD", 0, Format.R32G32_Float, 28, 0),
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

			RasterizerStateDescription rasterState = RasterizerStateDescription.Default();

			GraphicsPipelineStateDescription psonDesc = new GraphicsPipelineStateDescription() {

				InputLayout = new InputLayoutDescription(inputElementDescs),
				RootSignature = rootSignature,
				VertexShader = vertexShader,
				GeometryShader = geometryShader,
				PixelShader = pixelShader,
				RasterizerState = rasterState,
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
			pipelineState = Graphics.device.CreateGraphicsPipelineState(psonDesc);


		}
		private static void LoadResources(int renderWidth, int renderHeight) {

			#region Create root signature

			// Basically what constants are you going to pass to the shaders
			// Create a root signature with one root argument
			RootParameter[] rootParameters = new RootParameter[] {

				new RootParameter(ShaderVisibility.All, new DescriptorRange() {
					RangeType = DescriptorRangeType.ConstantBufferView,
					BaseShaderRegister = 0,
					OffsetInDescriptorsFromTableStart = int.MinValue,
					DescriptorCount = 1,
				}),
				new RootParameter(ShaderVisibility.All, new DescriptorRange() {
					RangeType = DescriptorRangeType.ConstantBufferView,
					BaseShaderRegister = 1,
					OffsetInDescriptorsFromTableStart = int.MinValue,
					DescriptorCount = 1,
				}),
				new RootParameter(ShaderVisibility.Pixel, new DescriptorRange() {
					RangeType = DescriptorRangeType.ShaderResourceView,
					BaseShaderRegister = 0,
					OffsetInDescriptorsFromTableStart = int.MinValue,
					DescriptorCount = 1,
				}),

			};

			StaticSamplerDescription[] staticSamplerDescription = new StaticSamplerDescription[] {
				new StaticSamplerDescription(ShaderVisibility.Pixel, 0, 0) {
					Filter = Filter.MinimumMinMagMipPoint,
					AddressUVW = TextureAddressMode.Wrap,
				}
			};

			RootSignatureDescription rootSignatureDesc = new RootSignatureDescription(RootSignatureFlags.AllowInputAssemblerInputLayout, rootParameters, staticSamplerDescription);
			rootSignature = Graphics.device.CreateRootSignature(rootSignatureDesc.Serialize());

			#endregion
			#region Create main combined descriptor heap

			// Default Heap setup. Contains all the vertices and indices
			DescriptorHeapDescription mainCombinedDescriptorHeapDesc = new DescriptorHeapDescription() {
				DescriptorCount = 1 + 16 + 2048,
				Flags = DescriptorHeapFlags.ShaderVisible,
				Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView,
			};

			descHeap = Graphics.device.CreateDescriptorHeap(mainCombinedDescriptorHeapDesc);
			descHeapIncrement = Graphics.device.GetDescriptorHandleIncrementSize(DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);

			#endregion

			renderInfo = new ConstBuffer<RenderInfo>(1);
			renderInfo.AddToDescriptorHeap(descHeap, dh_renderInfoStart);

		}

		/// <summary>
		/// Renders a camera's view
		/// </summary>
		/// <param name="renderTarget">The render target resource to render to</param>
		/// <param name="rtvDescHeap">The descriptor heap containing the render target</param>
		/// <param name="dsvDescHeap">The descriptor heap containing the depth stencil</param>
		/// <param name="camera">The camera to render from</param>
		internal static void Render(Resource renderTarget, DescriptorHeap rtvDescHeap, DescriptorHeap dsvDescHeap, Camera camera) {

			GeometryResources geometry = mainGeometry;

			Profiler.MetricBegin("Render setup");

			Graphics.cmdAllocator.Reset();
			Graphics.cmdList.Reset(Graphics.cmdAllocator, pipelineState);
			GraphicsCommandList cmdList = Graphics.cmdList;

			cmdList.SetGraphicsRootSignature(rootSignature);
			cmdList.SetDescriptorHeaps(1, new DescriptorHeap[] { descHeap });
			cmdList.SetGraphicsRootDescriptorTable(0, (descHeap.GPUDescriptorHandleForHeapStart + dh_renderInfoStart * descHeapIncrement));	// Set camera info


			// Bind shader resources
			camera.UpdateCameraInfoBuffer(renderInfo);
			geometry.UpdateObjectInfoBuffer();


			// Viewport and render target
			cmdList.SetViewport(camera.viewport);
			cmdList.SetScissorRectangles(camera.scissorRect);
			// Indicate that the back buffer will be used as a render target
			cmdList.ResourceBarrierTransition(renderTarget, ResourceStates.Present, ResourceStates.RenderTarget);


			// Set render target and depth stencil
			CpuDescriptorHandle rtvHandle = rtvDescHeap.CPUDescriptorHandleForHeapStart;
			CpuDescriptorHandle dsvHandle = dsvDescHeap.CPUDescriptorHandleForHeapStart;
			rtvHandle += Graphics.frameIndex * Graphics.rtvHeapIncrement;
			cmdList.SetRenderTargets(rtvHandle, dsvHandle);
			cmdList.ClearRenderTargetView(rtvHandle, new Color4(0f, 0f, 0f, 1f), 0, null);
			cmdList.ClearDepthStencilView(dsvHandle, ClearFlags.FlagsDepth, 1f, 0);

			
			// Set geometry
			cmdList.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
			cmdList.SetVertexBuffer(0, geometry.vertexBufferView);
			cmdList.SetIndexBuffer(geometry.indexBufferView);

			
			// Render each mesh
			for (int i = 0; i < geometry.meshRenderers.Count; i ++) {
				int indexCount = geometry.meshRenderers[i].mesh.indices.Length;
				(int vbStart, int ibStart, int obStart) = geometry.meshRendererPositions[i];


				GpuDescriptorHandle currentObjectHandle = descHeap.GPUDescriptorHandleForHeapStart + (obStart + dh_objectDataStart) * descHeapIncrement;
				GpuDescriptorHandle currentTextureHandle = descHeap.GPUDescriptorHandleForHeapStart + (geometry.meshRenderers[i].textureId + dh_textureDataStart) * descHeapIncrement;

				cmdList.SetGraphicsRootDescriptorTable(1, currentObjectHandle);
				cmdList.SetGraphicsRootDescriptorTable(2, currentTextureHandle);

				cmdList.DrawIndexedInstanced(indexCount, 1, ibStart, vbStart, vbStart);
			}
			

			// Indicate that the back buffer will now be used to present
			cmdList.ResourceBarrierTransition(renderTarget, ResourceStates.RenderTarget, ResourceStates.Present);
			
			
			cmdList.Close();

			Profiler.MetricEnd();
			Profiler.MetricBegin("Render");
			Graphics.cmdQueue.ExecuteCommandList(cmdList);
			Profiler.MetricEnd();
			

		}


		internal static void Dispose() {
			if (disposed == true) { Log.Warn("Cannot dispose GPU_Render, not initialised"); return; }
			disposed = true;

			pipelineState.Dispose();
			descHeap.Dispose();
			
		}


	}
}
