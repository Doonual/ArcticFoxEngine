using ArcticFoxEngine.Backend;
using CoolClassLibrary;
using SharpDX;
using SharpDX.Direct3D12;

namespace ArcticFoxEngine {
	public static class Command {

		
		private static CommandAllocator commandAllocator;

		internal static CommandQueue mainRenderCommandQueue;
		private static GraphicsCommandList mainRenderCommandList;

		internal static CommandQueue SetupCommand() {


			commandAllocator = Graphics.device.CreateCommandAllocator(CommandListType.Direct);


			mainRenderCommandQueue = Graphics.device.CreateCommandQueue(new CommandQueueDescription(CommandListType.Direct));

			// Create the command list
			mainRenderCommandList = Graphics.device.CreateCommandList(CommandListType.Direct, commandAllocator, Graphics.pipelineState);
			// Command lists are created in the recording state, but there is nothing
			// to record yet. The main loop expects it to be closed, so close it now.
			mainRenderCommandList.Close();


			return mainRenderCommandQueue;

		}
		internal static CommandQueue GetCommandQueue() {
			return mainRenderCommandQueue;
		}


		public static void ExecuteMainRender(Camera camera, GeometryInfo geometry) {

			GraphicsResources.UpdateShaderInfo(camera);

			#region Setup commandlist

			// Command list allocators can only be reset when the associated 
			// command lists have finished execution on the GPU; apps should use 
			// fences to determine GPU execution progress
			commandAllocator.Reset();

			// However, when ExecuteCommandList() is called on a particular command 
			// list, that command list can then be reset at any time and must be before 
			// re-recording
			mainRenderCommandList.Reset(commandAllocator, Graphics.pipelineState);


			// Set necessary state
			mainRenderCommandList.SetGraphicsRootSignature(Graphics.rootSignature);

			mainRenderCommandList.SetDescriptorHeaps(1, new DescriptorHeap[] { GraphicsResources.mainCombinedDescriporHeap });
			mainRenderCommandList.SetGraphicsRootDescriptorTable(0, (GraphicsResources.mainCombinedDescriporHeap.GPUDescriptorHandleForHeapStart));

			mainRenderCommandList.SetViewport(camera.viewport);
			mainRenderCommandList.SetScissorRectangles(camera.scissorRect);



			// Indicate that the back buffer will be used as a render target
			mainRenderCommandList.ResourceBarrierTransition(GraphicsResources.renderTargets[Graphics.frameIndex], ResourceStates.Present, ResourceStates.RenderTarget);

			CpuDescriptorHandle rtvHandle = GraphicsResources.renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
			CpuDescriptorHandle dsvHandle = GraphicsResources.depthStencilDescriptorHeap.CPUDescriptorHandleForHeapStart;
			rtvHandle += Graphics.frameIndex * GraphicsResources.renderTargetViewDescriptorSize;
			mainRenderCommandList.SetRenderTargets(rtvHandle, dsvHandle);

			// Record commands
			mainRenderCommandList.ClearRenderTargetView(rtvHandle, new Color4(0f, 0f, 0f, 1), 0, null);
			mainRenderCommandList.ClearDepthStencilView(dsvHandle, ClearFlags.FlagsDepth, 1f, 0);


			mainRenderCommandList.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
			mainRenderCommandList.SetVertexBuffer(0, geometry.vertexBufferView);
			mainRenderCommandList.SetIndexBuffer(geometry.indexBufferView);
			mainRenderCommandList.DrawIndexedInstanced(geometry.indexData.Length, 1, 0, 0, 0);
			//mainRenderCommandList.DrawInstanced(3, 1, 1, 0);

			// Indicate that the back buffer will now be used to present
			mainRenderCommandList.ResourceBarrierTransition(GraphicsResources.renderTargets[Graphics.frameIndex], ResourceStates.RenderTarget, ResourceStates.Present);
			mainRenderCommandList.Close();

			#endregion

			mainRenderCommandQueue.ExecuteCommandList(mainRenderCommandList);


			long cpuTimestamp;
			long gpuTimestamp;
			Command.mainRenderCommandQueue.GetClockCalibration(out gpuTimestamp, out cpuTimestamp);
			GPU_Profiler.UpdateGpuTimestamp(gpuTimestamp, Command.mainRenderCommandQueue.TimestampFrequency);



		}


		internal static void Dispose() {

			commandAllocator.Dispose();
			mainRenderCommandQueue.Dispose();
			mainRenderCommandList.Dispose();

		}

	}
}
