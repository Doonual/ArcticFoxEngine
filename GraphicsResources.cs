using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine {

	using SharpDX.Direct3D12;
	using SharpDX;
	using System.IO;
	using static ArcticFoxEngine.Graphics;

	internal static class GraphicsResources {

		internal static DescriptorHeap mainCombinedDescriporHeap;
		
		internal static DescriptorHeap renderTargetViewHeap;
		internal static int renderTargetViewDescriptorSize;
		internal static readonly Resource[] renderTargets = new Resource[Graphics.swapChainFrameCount];

		internal static ConstBuffer<ShaderInfo> shaderInfo;

		internal static void SetupResources(Device device, SwapChain3 swapChain) {

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
				renderTargets[n] = swapChain.GetBackBuffer<Resource>(n);
				device.CreateRenderTargetView(renderTargets[n], null, rtvHandle);
				rtvHandle += renderTargetViewDescriptorSize;
			}

			#endregion


			DescriptorHeapDescription mainCombinedDescriptorHeapDesc = new DescriptorHeapDescription() {
				DescriptorCount = 1,
				Flags = DescriptorHeapFlags.ShaderVisible,
				Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView
			};
			mainCombinedDescriporHeap = Graphics.device.CreateDescriptorHeap(mainCombinedDescriptorHeapDesc);


			shaderInfo = new ConstBuffer<ShaderInfo>(Utilities.SizeOf<ShaderInfo>(), mainCombinedDescriporHeap);

		}
		internal static void UpdateShaderInfo(Camera camera) {

			ShaderInfo shaderInfoData = new ShaderInfo();
			shaderInfoData.screenWidth = Screen.width;
			shaderInfoData.screenHeight = Screen.height;
			shaderInfoData.aspectRatio = (float)Screen.width / Screen.height;
			shaderInfoData.cameraInfo = camera.projectionMatrix;
			shaderInfo.WriteToBuffer(shaderInfoData);

		}

		internal static void SetupRootSignature() {
			
			// Basically what constants are you going to pass to the shaders
			// Create a root signature with one root argument
			RootParameter[] rootParameters = new RootParameter[] {

				new RootParameter(ShaderVisibility.All, new DescriptorRange() {
					RangeType = DescriptorRangeType.ConstantBufferView,
					BaseShaderRegister = 0,
					OffsetInDescriptorsFromTableStart = int.MinValue,
					DescriptorCount = 1
				})

			};

			RootSignatureDescription rootSignatureDesc = new RootSignatureDescription(RootSignatureFlags.AllowInputAssemblerInputLayout, rootParameters);
			rootSignature = device.CreateRootSignature(rootSignatureDesc.Serialize());

		}


		public static void Dispose() {
			foreach (Resource target in renderTargets) {
				target.Dispose();
			}
			renderTargetViewHeap.Dispose();
		}

	}
}
