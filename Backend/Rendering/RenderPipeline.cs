using ArcticFoxEngine.Backend.Render;
using SharpDX;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Backend {
	using ArcticFoxEngine.Nodes;
	using SharpDX.Direct3D12;

	public class RenderPipeline {

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
		public struct BufferBinding {
			public Action<DescriptorHeap> addToDescHeap;
			public int descStartIndex;
			public GpuDescriptorHandle descHeapStartPos;
			public Func<int, int> perObjectIndexSelection;
			public int rootParameterIndex;
		}
		public struct TextureSlot {
			public Func<int, int> perObjectIndexSelection;
			public int rootParameterIndex;
		}
		public struct TextureBinding {
			public Action<DescriptorHeap> addToDescHeap;
			public int descStartIndex;
			public GpuDescriptorHandle descHeapStartPos;
		}

		bool disposed = true;

		PipelineState pipelineState;
		RootSignature rootSignature;
		DescriptorHeap descriptorHeap;

		int descriptorHeapIncrement;

		List<RootParameter> rootParameters;
		List<StaticSamplerDescription> samplerDescriptions;

		List<BufferBinding> boundBuffers;
		List<TextureBinding> boundTextures;
		List<TextureSlot> textureSlots;

		
		int requiredDescriptorHeapSize;

		public RenderPipeline() {

			rootParameters = new List<RootParameter>();
			samplerDescriptions = new List<StaticSamplerDescription>();
			requiredDescriptorHeapSize = 0;

			boundBuffers = new List<BufferBinding>();
			boundTextures = new List<TextureBinding>();
			textureSlots = new List<TextureSlot>();

			
		}
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

		public void BindBuffer<T>(ConstBuffer<T> buffer, ShaderVisibility shaderVisibility, Func<int, int> perObjectIndexSelection) where T : struct {

			BufferBinding bufferBinding = new BufferBinding();

			int currentDhPos = requiredDescriptorHeapSize;

			// After we know how big the descriptor heap needs to be, add this to it
			// Each element in each buffer takes up 1 descriptor slot, starting at the accumulated position from all the other buffers
			bufferBinding.addToDescHeap = (DescriptorHeap descriptorHeap) => { buffer.AddToDescriptorHeap(descriptorHeap, currentDhPos);};
			bufferBinding.descStartIndex = requiredDescriptorHeapSize;
			bufferBinding.rootParameterIndex = rootParameters.Count;

			// Every time an object is to be rendered, how do we choose what element of each buffer to bind?
			// Might be different for different render pipelines so leave this up to the user
			bufferBinding.perObjectIndexSelection = perObjectIndexSelection;

		


			// Create a new Root parameter for the buffer
			RootParameter newRootParam = new RootParameter(shaderVisibility, new DescriptorRange() {
				RangeType = DescriptorRangeType.ConstantBufferView,
				BaseShaderRegister = boundBuffers.Count, // What index is this buffer out of all the buffers
				OffsetInDescriptorsFromTableStart = int.MinValue,
				DescriptorCount = 1,
			});
			rootParameters.Add(newRootParam);

			requiredDescriptorHeapSize += buffer.numElements;

			boundBuffers.Add(bufferBinding);

		}

		public void CreateTextureSlot(ShaderVisibility shaderVisibility, Func<int, int> perObjectIndexSelection) {
		
			TextureSlot textureSlot = new TextureSlot();
			textureSlot.perObjectIndexSelection = perObjectIndexSelection;
			textureSlot.rootParameterIndex = rootParameters.Count;
			

			RootParameter newRootParam = new RootParameter(shaderVisibility, new DescriptorRange() {
				RangeType = DescriptorRangeType.ShaderResourceView,
				BaseShaderRegister = textureSlots.Count,
				OffsetInDescriptorsFromTableStart = int.MinValue,
				DescriptorCount = 1,
			});
			rootParameters.Add(newRootParam);

			textureSlots.Add(textureSlot);

		}
		public void BindTexture(Texture texture, ShaderVisibility shaderVisibility) {

			int currentDhPos = requiredDescriptorHeapSize;

			TextureBinding textureBinding = new TextureBinding();
			textureBinding.addToDescHeap = (DescriptorHeap descriptorHeap) => { texture.AddToDescriptorHeap(descriptorHeap, currentDhPos); };
			textureBinding.descStartIndex = requiredDescriptorHeapSize;

			boundTextures.Add(textureBinding);
			requiredDescriptorHeapSize += 1;

		}
		public void BindTextureSampler(TextureSamplerOptions samplerOptions, ShaderVisibility shaderVisibility) {

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

		

		public void Finalise(ShaderBytecode vertexShader, ShaderBytecode pixelShader, ShaderBytecode? geometryShader = null, RasterizerStateDescription? rasterState = null, DepthStencilStateDescription? depthState = null) {

			#region Create root signature

			RootSignatureDescription rootSignatureDesc = new RootSignatureDescription(RootSignatureFlags.AllowInputAssemblerInputLayout, rootParameters.ToArray(), samplerDescriptions.ToArray());
			rootSignature = Graphics.device.CreateRootSignature(rootSignatureDesc.Serialize());

			#endregion
			#region Create main combined descriptor heap

			DescriptorHeapDescription mainCombinedDescriptorHeapDesc = new DescriptorHeapDescription() {
				DescriptorCount = requiredDescriptorHeapSize,
				Flags = DescriptorHeapFlags.ShaderVisible,
				Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView,
			};

			descriptorHeap = Graphics.device.CreateDescriptorHeap(mainCombinedDescriptorHeapDesc);
			descriptorHeapIncrement = Graphics.device.GetDescriptorHandleIncrementSize(DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);



			#endregion

			for (int i = 0; i < boundBuffers.Count; i ++) {

				BufferBinding currentBufferBinding = boundBuffers[i];

				currentBufferBinding.addToDescHeap(descriptorHeap);
				currentBufferBinding.descHeapStartPos = descriptorHeap.GPUDescriptorHandleForHeapStart + currentBufferBinding.descStartIndex * descriptorHeapIncrement;

				boundBuffers[i] = currentBufferBinding;

			}
			for (int i = 0; i < boundTextures.Count; i++) {

				TextureBinding currentTextureBinding = boundTextures[i];
				currentTextureBinding.addToDescHeap(descriptorHeap);
				currentTextureBinding.descHeapStartPos = descriptorHeap.GPUDescriptorHandleForHeapStart + currentTextureBinding.descStartIndex * descriptorHeapIncrement;
				boundTextures[i] = currentTextureBinding;

			}


			SetupPipeline(vertexShader, pixelShader, geometryShader, rasterState, depthState);


		}

		public void Render(GeometryInfo geometry, Camera camera, Resource renderTarget, DescriptorHeap rtvDescHeap, DescriptorHeap dsvDescHeap, bool clearBackground = false) {

			Profiler.MetricBegin("Render setup");


			GraphicsCommandList cmdList = Graphics.CreateGraphicsCommandList(pipelineState);

			cmdList.SetGraphicsRootSignature(rootSignature);
			cmdList.SetDescriptorHeaps(1, new DescriptorHeap[] { descriptorHeap });


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

			if (clearBackground == true) {
				cmdList.ClearRenderTargetView(rtvHandle, new Color4(0f, 0f, 0f, 1f), 0, null);
				cmdList.ClearDepthStencilView(dsvHandle, ClearFlags.FlagsDepth, 1f, 0);
			}
			


			// Set geometry
			cmdList.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
			cmdList.SetVertexBuffer(0, geometry.vertexBufferView);
			cmdList.SetIndexBuffer(geometry.indexBufferView);


			// Render each mesh
			for (int i = 0; i < geometry.meshRenderers.Count; i++) {
				int indexCount = geometry.meshRenderers[i].mesh.indices.Length;
				(int vbStart, int ibStart, int obStart) = geometry.meshRendererPositions[i];

				for (int b = 0; b < boundBuffers.Count; b ++) {
					GpuDescriptorHandle descHandle = boundBuffers[b].descHeapStartPos + boundBuffers[b].perObjectIndexSelection(i) * descriptorHeapIncrement;
					cmdList.SetGraphicsRootDescriptorTable(boundBuffers[b].rootParameterIndex, descHandle);
				}
				for (int b = 0; b < textureSlots.Count; b++) {

					TextureBinding boundTexture = boundTextures[textureSlots[b].perObjectIndexSelection(i)];

					GpuDescriptorHandle descHandle = boundTexture.descHeapStartPos;
					cmdList.SetGraphicsRootDescriptorTable(textureSlots[b].rootParameterIndex, descHandle);
				}

				cmdList.DrawIndexedInstanced(indexCount, 1, ibStart, vbStart, vbStart);
			}



			// Indicate that the back buffer will now be used to present
			cmdList.ResourceBarrierTransition(renderTarget, ResourceStates.RenderTarget, ResourceStates.Present);


			cmdList.Close();

			Profiler.MetricEnd();
			Profiler.MetricBegin("Render");
			Graphics.SubmitGraphicsCommandList(cmdList);
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
			descriptorHeap.Dispose();

		}


	}

}
