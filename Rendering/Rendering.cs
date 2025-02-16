 using ArcticFoxEngine.Nodes;

namespace ArcticFoxEngine.Rendering {
	using SharpDX;
	using SharpDX.Direct3D12;


	/// <summary>
	/// Encapsulates all the tasks required to render a GeometryResources instance
	/// </summary>
	public static class Rendering {

		public static GraphicsCommandList cmdList;
		public static DescriptorHeap gpuDescriptorHeap;
		private static int descriptorCopyPos;
		public static int descriptorHeapIncrement;

		public static Texture missingTexture;

		internal static void Init() {

			DescriptorHeapDescription descHeapDesc = new DescriptorHeapDescription() {
				DescriptorCount = 100000,
				Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView,
				Flags = DescriptorHeapFlags.ShaderVisible,
			};
			gpuDescriptorHeap = Graphics.device.CreateDescriptorHeap(descHeapDesc);
			descriptorCopyPos = 0;
			descriptorHeapIncrement = Graphics.device.GetDescriptorHandleIncrementSize(DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);

			cmdList = Graphics.CreateDirectCommandList();


			missingTexture = new Texture(64, 64);
			for (int x = 0; x < 64; x ++) {
				for (int y = 0; y < 64; y ++) {
					byte[] pixelData = new byte[] { 0x00, 0x00, 0x00, 0xff }; ;
					if ((x + y) % 2 == 0) {
						pixelData = new byte[] { 0xff, 0x00, 0xff, 0xff };
					}
					missingTexture.SetPixelBatch(pixelData, x, y);
				}
			}
			missingTexture.BatchSync();
		}

		/// <summary>
		/// Coppies descriptors from anywhere into the main descriptor heap for binding
		/// </summary>
		/// <param name="srcDescriptorHandle">The descriptor handle of the descriptor to be coppied</param>
		/// <param name="numDescriptors">The number of descriptors that are going to be coppied</param>
		/// <returns>The location of the coppied descriptor</returns>
		internal static GpuDescriptorHandle CopyDescriptorsIn(CpuDescriptorHandle srcDescriptorHandle, int numDescriptors) {

			int destinationDescriptorIndex = descriptorCopyPos;
			descriptorCopyPos += numDescriptors;

			// Copy the descriptors
			CpuDescriptorHandle destDescriptor = gpuDescriptorHeap.CPUDescriptorHandleForHeapStart + destinationDescriptorIndex * descriptorHeapIncrement;
			Graphics.device.CopyDescriptorsSimple(numDescriptors, destDescriptor, srcDescriptorHandle, DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);

			// Tell the dataslot where to find the descriptors
			return gpuDescriptorHeap.GPUDescriptorHandleForHeapStart + destinationDescriptorIndex * descriptorHeapIncrement;

		}

		/// <summary>
		/// Renders a camera's view
		/// </summary>
		/// <param name="renderTarget">The render target resource to render to</param>
		/// <param name="rtvDescHeap">The descriptor heap containing the render target</param>
		/// <param name="dsvDescHeap">The descriptor heap containing the depth stencil</param>
		/// <param name="camera">The camera to render from</param>
		internal static void RenderScene(Camera camera) {

			Graphics.WaitForCopyCommandQueue();
			Graphics.WaitForDirectCommandQueue();
			Graphics.ResetDirectCommandList(cmdList);

			descriptorCopyPos = 0;
			

			cmdList.SetDescriptorHeaps(gpuDescriptorHeap);

			// Indicate that the back buffer will be used as a render target
			//cmdList.ResourceBarrierTransition(camera.renderTexture.resource, ResourceStates.Common, ResourceStates.RenderTarget);

			// Set viewport and scissor rectancles
			cmdList.SetViewport(camera.viewport);
			cmdList.SetScissorRectangles(camera.scissorRect);

			// Set render target and depth stencil
			CpuDescriptorHandle rtvHandle = camera.rtvDescriptorHeap.CPUDescriptorHandleForHeapStart;
			CpuDescriptorHandle dsvHandle = camera.dsvDescriptorHeap.CPUDescriptorHandleForHeapStart;
			//rtvHandle += Graphics.frameIndex * Graphics.rtvHeapIncrement;
			cmdList.SetRenderTargets(rtvHandle, dsvHandle);


			// Clear the render target and depth stencil
			cmdList.ClearRenderTargetView(rtvHandle, new Color4(0f, 0f, 0f, 1f), 0, null);
			cmdList.ClearDepthStencilView(dsvHandle, ClearFlags.FlagsDepth, 1f, 0);


			List<Shader> shadersToRender = Shader.GetAllShaders();
			for (int i = 0; i < shadersToRender.Count; i++) {



				Shader currentShader = shadersToRender[i];
				GeometryInfo currentGeometryResources = currentShader.geometryResources;

				// Update the pipeline state and set this shaders root signature
				cmdList.PipelineState = currentShader.pipelineState;
				cmdList.SetGraphicsRootSignature(currentShader.rootSignature);

				currentShader.Render(camera, camera.renderTexture.resource, camera.rtvDescriptorHeap, camera.dsvDescriptorHeap);

			}

			// Indicate that the back buffer will now be used to present
			//cmdList.ResourceBarrierTransition(camera.renderTexture.resource, ResourceStates.RenderTarget, ResourceStates.Common);


			cmdList.Close();
			Graphics.ExecuteDirectCommandList(cmdList);

		}


		internal static void Dispose() {
			cmdList.Dispose();
			gpuDescriptorHeap.Dispose();
		}


	}
}
