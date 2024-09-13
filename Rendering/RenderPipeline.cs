using ArcticFoxEngine.Nodes;
using SharpDX;
using SharpDX.Direct3D12;
using SharpDX.DXGI;
using SixLabors.ImageSharp.Memory;
using System;
using Resource = SharpDX.Direct3D12.Resource;

namespace ArcticFoxEngine.Rendering {

	public abstract class RenderPipeline {

		public abstract string name { get; }

		public struct TextureSamplerOptions {

			public TextureSamplerOptions() {

				StaticSamplerDescription defaultOptions = new StaticSamplerDescription();

				addressU = defaultOptions.AddressU;
				addressV = defaultOptions.AddressV;
				addressW = defaultOptions.AddressW;

				borderCol = defaultOptions.BorderColor;
				comparisonFunc = defaultOptions.ComparisonFunc;
				filter = defaultOptions.Filter;
				maxAnisotropy = defaultOptions.MaxAnisotropy;

				maxLOD = defaultOptions.MaxLOD;
				minLOD = defaultOptions.MinLOD;
				mipLODBias = defaultOptions.MipLODBias;

			}

			public TextureAddressMode addressU;
			public TextureAddressMode addressV;
			public TextureAddressMode addressW;
			public TextureAddressMode addressUVW {
				set {
					addressU = value;
					addressV = value;
					addressW = value;
				}
			}

			public StaticBorderColor borderCol;
			public Comparison comparisonFunc;
			public Filter filter;
			public int maxAnisotropy;

			public float maxLOD;
			public float minLOD;
			public float mipLODBias;

		}

		struct DataSlot {
			public ShaderVisibility shaderVisibility;
			public int rootParameterIndex;

			public GpuDescriptorHandle currentDescriptorLocation;

		}
		struct TextureSlot {
			public ShaderVisibility shaderVisibility;
			public int rootParameterIndex;

			public GpuDescriptorHandle currentDescriptorLocation;
		}

		bool disposed = true;

		PipelineState pipelineState;
		RootSignature rootSignature;

		public GeometryInfo geometryResources;

		Dictionary<string, DataSlot> dataSlots;
		Dictionary<string, TextureSlot> textureSlots;


		List<RootParameter> rootParameters;
		List<StaticSamplerDescription> samplerDescriptions;

		
		public RenderPipeline() {

			geometryResources = new GeometryInfo();



			rootParameters = new List<RootParameter>();
			samplerDescriptions = new List<StaticSamplerDescription>();

			dataSlots = new Dictionary<string, DataSlot>();
			textureSlots = new Dictionary<string, TextureSlot>();

			CreateDataSlot("Camera info", ShaderVisibility.All);
			CreateDataSlot("Object info", ShaderVisibility.All);

		}
		public abstract Material GetDefaultMaterial();

		private void SetupPipeline(ShaderBytecode vertexShader, ShaderBytecode pixelShader, ShaderBytecode? geometryShader, RasterizerStateDescription? rasterState, DepthStencilStateDescription? depthState) {

			// Input format
			InputElement[] inputElementDescs = new InputElement[] {
				new InputElement("SV_Position", 0, Format.R32G32B32_Float, 0, 0),
				new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 12, 0),
				new InputElement("TEXCOORD", 0, Format.R32G32_Float, 28, 0),
				new InputElement("NORMAL", 0, Format.R32G32B32_Float, 36, 0),
			};

			RasterizerStateDescription actualRasterState;
			if (rasterState == null) {
				actualRasterState = RasterizerStateDescription.Default();
			}
			else {
				actualRasterState = (RasterizerStateDescription)rasterState;
			}

			DepthStencilStateDescription actualDepthState;
			if (depthState == null) {

				DepthStencilOperationDescription defaultStencilOp = new DepthStencilOperationDescription() {
					FailOperation = StencilOperation.Keep,
					DepthFailOperation = StencilOperation.Keep,
					PassOperation = StencilOperation.Keep,
					Comparison = Comparison.Always
				};
				actualDepthState = new DepthStencilStateDescription() {

					IsDepthEnabled = true,
					DepthWriteMask = DepthWriteMask.All,
					DepthComparison = Comparison.Less,

					IsStencilEnabled = false,
					StencilReadMask = 0xff,
					StencilWriteMask = 0xff,
					FrontFace = defaultStencilOp,
					BackFace = defaultStencilOp,

				};

			}
			else {
				actualDepthState = (DepthStencilStateDescription)depthState;
			}

			GraphicsPipelineStateDescription psonDesc = new GraphicsPipelineStateDescription() {

				InputLayout = new InputLayoutDescription(inputElementDescs),
				RootSignature = rootSignature,
				VertexShader = vertexShader,
				PixelShader = pixelShader,
				RasterizerState = actualRasterState,
				BlendState = BlendStateDescription.Default(),
				DepthStencilFormat = Format.D32_Float,
				DepthStencilState = actualDepthState,
				SampleMask = int.MaxValue,
				PrimitiveTopologyType = PrimitiveTopologyType.Triangle,
				RenderTargetCount = 1,
				Flags = PipelineStateFlags.None,
				SampleDescription = new SampleDescription(1, 0),
				StreamOutput = new StreamOutputDescription()

			};
			if (geometryShader != null) {
				psonDesc.GeometryShader = (ShaderBytecode)geometryShader;
			}

			psonDesc.RenderTargetFormats[0] = Format.R8G8B8A8_UNorm;
			pipelineState = Graphics.device.CreateGraphicsPipelineState(psonDesc);

		}

		public void CreateDataSlot(string name, ShaderVisibility shaderVisibility) {

			int numRootParameters = rootParameters.Count;
			int numDataSlots = dataSlots.Count;

			DataSlot newSlot = new DataSlot() {
				shaderVisibility = shaderVisibility,
				rootParameterIndex = numRootParameters,
			};
			dataSlots.Add(name, newSlot);

			// Create a new Root parameter for the buffer
			RootParameter newRootParam = new RootParameter(shaderVisibility, new DescriptorRange() {
				RangeType = DescriptorRangeType.ConstantBufferView,
				BaseShaderRegister = numDataSlots, // What index is this buffer out of all the buffers
				OffsetInDescriptorsFromTableStart = int.MinValue,
				DescriptorCount = 1,
			});
			rootParameters.Add(newRootParam);



		}
		public void CreateTextureSlot(string name, ShaderVisibility shaderVisibility) {

			int numRootParameters = rootParameters.Count;
			int numTextureSlots = textureSlots.Count;

			TextureSlot textureSlot = new TextureSlot() {
				shaderVisibility = shaderVisibility,
				rootParameterIndex = numRootParameters,
			};
			textureSlots.Add(name, textureSlot);

			RootParameter newRootParam = new RootParameter(shaderVisibility, new DescriptorRange() {
				RangeType = DescriptorRangeType.ShaderResourceView,
				BaseShaderRegister = numTextureSlots,
				OffsetInDescriptorsFromTableStart = int.MinValue,
				DescriptorCount = 1,
			});
			rootParameters.Add(newRootParam);

		}
		public void CreateTextureSampler(TextureSamplerOptions samplerOptions, ShaderVisibility shaderVisibility) {

			StaticSamplerDescription desc = new StaticSamplerDescription(shaderVisibility, samplerDescriptions.Count, 0);
			desc.AddressU = samplerOptions.addressU;
			desc.AddressV = samplerOptions.addressV;
			desc.AddressW = samplerOptions.addressW;

			desc.BorderColor = samplerOptions.borderCol;
			desc.ComparisonFunc = samplerOptions.comparisonFunc;
			desc.Filter = samplerOptions.filter;
			desc.MaxAnisotropy = samplerOptions.maxAnisotropy;

			desc.MaxLOD = samplerOptions.maxLOD;
			desc.MinLOD = samplerOptions.minLOD;
			desc.MipLODBias = samplerOptions.mipLODBias;

			samplerDescriptions.Add(desc);

		}


		public void SetDataSlot<T>(string name, ConstBuffer<T> buffer, int bufferIndex) where T : struct {

			// Copy the descriptors
			int destDescPos = Rendering.ReserveDescriptorHeapSpace(1);
			CpuDescriptorHandle destDescriptor = Rendering.gpuDescriptorHeap.CPUDescriptorHandleForHeapStart + destDescPos * Rendering.descriptorHeapIncrement;
			CpuDescriptorHandle srcDescriptor = buffer.descriptorHeap.CPUDescriptorHandleForHeapStart + bufferIndex * Rendering.descriptorHeapIncrement;
			Graphics.device.CopyDescriptorsSimple(1, destDescriptor, srcDescriptor, DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);

			// Tell the dataslot where to find the descriptors
			DataSlot currentDataSlot = dataSlots[name];
			currentDataSlot.currentDescriptorLocation = Rendering.gpuDescriptorHeap.GPUDescriptorHandleForHeapStart + destDescPos * Rendering.descriptorHeapIncrement;
			dataSlots[name] = currentDataSlot;

		}
		public void SetTextureSlot(string name, Texture texture) {

			// Copy the descriptors
			int destDescPos = Rendering.ReserveDescriptorHeapSpace(1);
			CpuDescriptorHandle destDescriptor = Rendering.gpuDescriptorHeap.CPUDescriptorHandleForHeapStart + destDescPos * Rendering.descriptorHeapIncrement;
			CpuDescriptorHandle srcDescriptor = texture.descriptorHeap.CPUDescriptorHandleForHeapStart;
			Graphics.device.CopyDescriptorsSimple(1, destDescriptor, srcDescriptor, DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);

			// Tell the dataslot where to find the descriptors
			TextureSlot currentTextureSlot = textureSlots[name];
			currentTextureSlot.currentDescriptorLocation = Rendering.gpuDescriptorHeap.GPUDescriptorHandleForHeapStart + destDescPos * Rendering.descriptorHeapIncrement;
			textureSlots[name] = currentTextureSlot;


		}



		public void Finalise(ShaderBytecode vertexShader, ShaderBytecode pixelShader, ShaderBytecode? geometryShader = null, RasterizerStateDescription? rasterState = null, DepthStencilStateDescription? depthState = null) {

			// Create root signature
			RootSignatureDescription rootSignatureDesc = new RootSignatureDescription(RootSignatureFlags.AllowInputAssemblerInputLayout, rootParameters.ToArray(), samplerDescriptions.ToArray());
			rootSignature = Graphics.device.CreateRootSignature(rootSignatureDesc.Serialize());


			SetupPipeline(vertexShader, pixelShader, geometryShader, rasterState, depthState);


		}

		public void Render(GeometryInfo geometry, Camera camera, Resource renderTarget, DescriptorHeap rtvDescHeap, DescriptorHeap dsvDescHeap, bool clearBackground = false) {

			Profiler.MetricBegin("Render setup");

			Rendering.cmdList.PipelineState = pipelineState;
			//Rendering.cmdList.SetDescriptorHeaps(Rendering.gpuDescriptorHeap);
			Rendering.cmdList.SetGraphicsRootSignature(rootSignature);
			
			
			SetDataSlot("Camera info", Rendering.renderInfo, 0);

			// Viewport and render target
			Rendering.cmdList.SetViewport(camera.viewport);
			Rendering.cmdList.SetScissorRectangles(camera.scissorRect);
			// Indicate that the back buffer will be used as a render target
			Rendering.cmdList.ResourceBarrierTransition(renderTarget, ResourceStates.Present, ResourceStates.RenderTarget);


			// Set render target and depth stencil
			CpuDescriptorHandle rtvHandle = rtvDescHeap.CPUDescriptorHandleForHeapStart;
			CpuDescriptorHandle dsvHandle = dsvDescHeap.CPUDescriptorHandleForHeapStart;
			rtvHandle += Graphics.frameIndex * Graphics.rtvHeapIncrement;
			Rendering.cmdList.SetRenderTargets(rtvHandle, dsvHandle);

			if (clearBackground == true) {
				Rendering.cmdList.ClearRenderTargetView(rtvHandle, new Color4(0f, 0f, 0f, 1f), 0, null);
				Rendering.cmdList.ClearDepthStencilView(dsvHandle, ClearFlags.FlagsDepth, 1f, 0);
			}



			// Set geometry
			Rendering.cmdList.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
			Rendering.cmdList.SetVertexBuffer(0, geometry.vertexBufferView);
			Rendering.cmdList.SetIndexBuffer(geometry.indexBufferView);


			// Render each mesh
			for (int i = 0; i < geometry.meshRenderers.Count; i++) {

				int indexCount = geometry.meshRenderers[i].mesh.indices.Length;
				(int vbStart, int ibStart, int obStart) = geometry.meshRendererPositions[i];


				SetDataSlot("Object info", geometryResources.objectBuffer, obStart);

				Material renderMaterial = geometry.meshRenderers[i].material;
				renderMaterial.BindResources(this);

				// Bind all data slots
				for (int b = 0; b < dataSlots.Count; b ++) {
					DataSlot dataSlot = dataSlots.ElementAt(b).Value;
					Rendering.cmdList.SetGraphicsRootDescriptorTable(dataSlot.rootParameterIndex, dataSlot.currentDescriptorLocation);
				}
				
				// Bind all texture slots
				for (int b = 0; b < textureSlots.Count; b++) {
					TextureSlot textureSlot = textureSlots.ElementAt(b).Value;
					Rendering.cmdList.SetGraphicsRootDescriptorTable(textureSlot.rootParameterIndex, textureSlot.currentDescriptorLocation);
				}

				Rendering.cmdList.DrawIndexedInstanced(indexCount, 1, ibStart, vbStart, vbStart);
			}



			// Indicate that the back buffer will now be used to present
			Rendering.cmdList.ResourceBarrierTransition(renderTarget, ResourceStates.RenderTarget, ResourceStates.Present);



			Profiler.MetricEnd();


		}

		~RenderPipeline() {
			Dispose();
		}
		public void Dispose() {
			if (disposed == true) { return; }
			disposed = true;

			pipelineState.Dispose();
			rootSignature.Dispose();

			geometryResources.Dispose();

		}


	}

}
