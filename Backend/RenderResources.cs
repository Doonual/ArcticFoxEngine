using SharpDX.DXGI;

namespace ArcticFoxEngine {

	using SharpDX.Direct3D12;
	using SharpDX;
	using static ArcticFoxEngine.Graphics;
	using ArcticFoxEngine.Backend;
	using CoolClassLibrary;

	internal static class RenderResources {


		internal static RootSignature rootSignature;

		// Combined descriptor heap for shader resource binding
		internal static DescriptorHeap combinedDescriptorHeap;
		internal static int combinedDescriptorHeapIncrement;
		internal static ConstBuffer<RenderInfo> renderInfo;

		// Render target view descriptor heap
		internal static DescriptorHeap renderTargetViewHeap;
		internal static readonly Resource[] renderTargets = new Resource[Graphics.swapChainFrameCount];
		internal static int renderTargetViewDescriptorSize;
		
		// Depth Stencil descriptor heap
		internal static DescriptorHeap depthStencilDescriptorHeap;
		internal static Resource depthStencilBuffer;

		// Main load for the GraphicsResources
		internal static void LoadResources(int renderWidth, int renderHeight) {

			#region Setup Render Target View (RTV) descriptor heaps and resources

			// Create a render target view (RTV) descriptor heap
			DescriptorHeapDescription rtvHeapDesc = new DescriptorHeapDescription() {
				DescriptorCount = Graphics.swapChainFrameCount,
				Flags = DescriptorHeapFlags.None,
				Type = DescriptorHeapType.RenderTargetView
			};
			renderTargetViewHeap = device.CreateDescriptorHeap(rtvHeapDesc);
			renderTargetViewDescriptorSize = device.GetDescriptorHandleIncrementSize(DescriptorHeapType.RenderTargetView);


			// Create frame resources from swap chain frames
			CpuDescriptorHandle rtvHandle = renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
			for (int n = 0; n < Graphics.swapChainFrameCount; n++) {
				renderTargets[n] = Graphics.swapChain.GetBackBuffer<Resource>(n);
				device.CreateRenderTargetView(renderTargets[n], null, rtvHandle);
				rtvHandle += renderTargetViewDescriptorSize;
			}

			#endregion
			#region Depth buffer setup

			DescriptorHeapDescription depthStencilHeapDescription = new DescriptorHeapDescription() {
				DescriptorCount = 1,
				Type = DescriptorHeapType.DepthStencilView,
				Flags = DescriptorHeapFlags.None
			};
			depthStencilDescriptorHeap = device.CreateDescriptorHeap(depthStencilHeapDescription);

			DepthStencilViewDescription depthStencilDesc = new DepthStencilViewDescription() {
				Format = Format.D32_Float,
				Dimension = DepthStencilViewDimension.Texture2D,
				Flags = DepthStencilViewFlags.None
			};
			ClearValue depthOptimizedClearValue = new ClearValue() {
				Format = Format.D32_Float,
				DepthStencil = new DepthStencilValue() { Depth = 1.0f, Stencil = 0 },
			};
			depthStencilBuffer = device.CreateCommittedResource(
				new HeapProperties(HeapType.Default),
				HeapFlags.None, ResourceDescription.Texture2D(Format.D32_Float, renderWidth, renderHeight, flags: ResourceFlags.AllowDepthStencil),
				ResourceStates.DepthWrite
			);
			depthStencilBuffer.Name = "Depth / Stencil Resource Heap";
			device.CreateDepthStencilView(depthStencilBuffer, depthStencilDesc, depthStencilDescriptorHeap.CPUDescriptorHandleForHeapStart);

			#endregion

			// Default Heap setup. Contains all the vertices and indices
			DescriptorHeapDescription mainCombinedDescriptorHeapDesc = new DescriptorHeapDescription() {
				DescriptorCount = 1 + 2048,
				Flags = DescriptorHeapFlags.ShaderVisible,
				Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView,
			};

			combinedDescriptorHeap = device.CreateDescriptorHeap(mainCombinedDescriptorHeapDesc);
			combinedDescriptorHeapIncrement = device.GetDescriptorHandleIncrementSize(DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);

			renderInfo = new ConstBuffer<RenderInfo>(1);
			renderInfo.AddToDescriptorHeap(combinedDescriptorHeap, 0);
			
			SetupRootSignature();

		}
		private static void SetupRootSignature() {
			
			// Basically what constants are you going to pass to the shaders
			// Create a root signature with one root argument
			RootParameter[] rootParameters = new RootParameter[] {

				new RootParameter(ShaderVisibility.All, new DescriptorRange() {
					RangeType = DescriptorRangeType.ConstantBufferView,
					BaseShaderRegister = 0,
					OffsetInDescriptorsFromTableStart = int.MinValue,
					DescriptorCount = 1
				}),
				
				new RootParameter(ShaderVisibility.All, new DescriptorRange() {
					RangeType = DescriptorRangeType.ConstantBufferView,
					BaseShaderRegister = 1,
					OffsetInDescriptorsFromTableStart = int.MinValue,
					DescriptorCount = 1
				})
				

			};

			RootSignatureDescription rootSignatureDesc = new RootSignatureDescription(RootSignatureFlags.AllowInputAssemblerInputLayout, rootParameters);
			rootSignature = device.CreateRootSignature(rootSignatureDesc.Serialize());

		}
		
		
		internal static void BindShaderResources(GraphicsCommandList destCmdList) {

			destCmdList.SetGraphicsRootSignature(rootSignature);
			destCmdList.SetDescriptorHeaps(1, new DescriptorHeap[] { combinedDescriptorHeap });
			destCmdList.SetGraphicsRootDescriptorTable(0, (combinedDescriptorHeap.GPUDescriptorHandleForHeapStart));

		}
		internal static void BindCurrentObject(GraphicsCommandList descCmdList, int obStartIndex) {

			GpuDescriptorHandle currentObjectHandle = combinedDescriptorHeap.GPUDescriptorHandleForHeapStart + (obStartIndex + 1) * combinedDescriptorHeapIncrement;
			descCmdList.SetGraphicsRootDescriptorTable(1, currentObjectHandle);

		}

		public static void Dispose() {
			foreach (Resource target in renderTargets) {
				target.Dispose();
			}
			renderTargetViewHeap.Dispose();

			depthStencilBuffer.Dispose();
			depthStencilDescriptorHeap.Dispose();

			rootSignature.Dispose();
		}

	}
}
