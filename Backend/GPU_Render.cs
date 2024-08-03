using ArcticFoxEngine.Backend;
using ArcticFoxEngine.Components;
using ArcticFoxEngine.Debug;
using CoolClassLibrary;
using SharpDX;
using SharpDX.Direct3D12;

namespace ArcticFoxEngine {
	public static class GPU_Render {

		
		private static CommandAllocator cmdAllocator;

		internal static CommandQueue cmdQueue;
		private static GraphicsCommandList cmdList;

		internal static void SetupCommand() {

			// Create the command list
			// Command lists are created in the recording state, but there is nothing
			// to record yet. The main loop expects it to be closed, so close it now.

			cmdAllocator = Graphics.device.CreateCommandAllocator(CommandListType.Direct);
			cmdQueue = Graphics.device.CreateCommandQueue(new CommandQueueDescription(CommandListType.Direct));
			cmdList = Graphics.device.CreateCommandList(CommandListType.Direct, cmdAllocator, Graphics.pipelineState);
			cmdList.Close();


		}
		internal static CommandQueue GetCommandQueue() {
			return cmdQueue;
		}


		public static void Render(Camera camera, GeometryResources geometry) {

			// Command list allocators can only be reset when the associated 
			// command lists have finished execution on the GPU; apps should use 
			// fences to determine GPU execution progress
			//
			// However, when ExecuteCommandList() is called on a particular command 
			// list, that command list can then be reset at any time and must be before 
			// re-recording
			cmdAllocator.Reset();
			cmdList.Reset(cmdAllocator, Graphics.pipelineState);

			// Bind shader resources

			
			camera.WriteCameraInfoBuffer(GraphicsResources.renderInfo);
			geometry.UpdateObjectInfoBuffer();
			GraphicsResources.BindShaderResources(cmdList);

			

			// Viewport and render target
			cmdList.SetViewport(camera.viewport);
			cmdList.SetScissorRectangles(camera.scissorRect);
			// Indicate that the back buffer will be used as a render target
			cmdList.ResourceBarrierTransition(GraphicsResources.renderTargets[Graphics.frameIndex], ResourceStates.Present, ResourceStates.RenderTarget);
			// Set render target and depth stencil
			CpuDescriptorHandle rtvHandle = GraphicsResources.renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
			CpuDescriptorHandle dsvHandle = GraphicsResources.depthStencilDescriptorHeap.CPUDescriptorHandleForHeapStart;
			rtvHandle += Graphics.frameIndex * GraphicsResources.renderTargetViewDescriptorSize;
			cmdList.SetRenderTargets(rtvHandle, dsvHandle);
			cmdList.ClearRenderTargetView(rtvHandle, new Color4(0f, 0f, 0f, 1), 0, null);
			cmdList.ClearDepthStencilView(dsvHandle, ClearFlags.FlagsDepth, 1f, 0);

			// Set geometry
			cmdList.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
			cmdList.SetVertexBuffer(0, geometry.vertexBufferView);
			cmdList.SetIndexBuffer(geometry.indexBufferView);

			
			for (int i = 0; i < geometry.meshRenderers.Count; i ++) {

				MeshRenderer renderMesh = geometry.meshRenderers[i];
				int indexCount = renderMesh.mesh.indices.Length;

				GpuDescriptorHandle startOfObjectBufferPos = GraphicsResources.combinedDescriptorHeap.GPUDescriptorHandleForHeapStart + GraphicsResources.combinedDescriptorHeapIncrement;
				startOfObjectBufferPos += geometry.meshRendererPositions[i].obStart * GraphicsResources.combinedDescriptorHeapIncrement;
				cmdList.SetGraphicsRootDescriptorTable(1, startOfObjectBufferPos);
				cmdList.DrawIndexedInstanced(renderMesh.mesh.indices.Length, 1, geometry.meshRendererPositions[i].ibStart, geometry.meshRendererPositions[i].vbStart, geometry.meshRendererPositions[i].vbStart);

			}

			// Indicate that the back buffer will now be used to present
			cmdList.ResourceBarrierTransition(GraphicsResources.renderTargets[Graphics.frameIndex], ResourceStates.RenderTarget, ResourceStates.Present);
			
			
			cmdList.Close();
			cmdQueue.ExecuteCommandList(cmdList);


		}


		internal static void Dispose() {

			cmdAllocator.Dispose();
			cmdQueue.Dispose();
			cmdList.Dispose();

		}

	}
}
