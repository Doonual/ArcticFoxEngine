using ArcticFoxEngine.Backend;
using ArcticFoxEngine.Components;
using ArcticFoxEngine.Debug;
using ClickableTransparentOverlay;
using CoolClassLibrary;
using SharpDX;
using SharpDX.Direct3D12;

namespace ArcticFoxEngine {

	/// <summary>
	/// Encapsulates all the tasks required to render a GeometryResources instance
	/// </summary>
	public static class GPU_Render {

		
		private static CommandAllocator cmdAllocator;

		internal static CommandQueue cmdQueue;
		private static GraphicsCommandList cmdList;

		/// <summary>
		/// Initialises all the command queue objects
		/// </summary>
		internal static void Init() {

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

		/// <summary>
		/// Renders the geometry
		/// </summary>
		/// <param name="camera">The camera used to render the geometry</param>
		/// <param name="geometry">The geometry to be rendered</param>
		public static void Render(Camera camera, GeometryResources geometry) {

			// Command list allocators can only be reset when the associated 
			// command lists have finished execution on the GPU; apps should use 
			// fences to determine GPU execution progress
			//
			// However, when ExecuteCommandList() is called on a particular command 
			// list, that command list can then be reset at any time and must be before 
			// re-recording
			
			#region Normal rendering

			Profiler.MetricBegin();
			cmdAllocator.Reset();
			cmdList.Reset(cmdAllocator, Graphics.pipelineState);


			// Bind shader resources


			camera.WriteCameraInfoBuffer();
			geometry.WriteObjectInfoBuffer();
			RenderResources.BindShaderResources(cmdList);



			// Viewport and render target
			cmdList.SetViewport(camera.viewport);
			cmdList.SetScissorRectangles(camera.scissorRect);
			// Indicate that the back buffer will be used as a render target
			cmdList.ResourceBarrierTransition(RenderResources.renderTargets[Graphics.frameIndex], ResourceStates.Present, ResourceStates.RenderTarget);

			

			// Set render target and depth stencil
			CpuDescriptorHandle rtvHandle = RenderResources.renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
			CpuDescriptorHandle dsvHandle = RenderResources.depthStencilDescriptorHeap.CPUDescriptorHandleForHeapStart;
			rtvHandle += Graphics.frameIndex * RenderResources.renderTargetViewDescriptorSize;
			cmdList.SetRenderTargets(rtvHandle, dsvHandle);
			cmdList.ClearRenderTargetView(rtvHandle, new Color4(0f, 0f, 0f, 1f), 0, null);
			cmdList.ClearDepthStencilView(dsvHandle, ClearFlags.FlagsDepth, 1f, 0);

			


			

			// Set geometry
			cmdList.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
			cmdList.SetVertexBuffer(0, geometry.vertexBufferView);
			cmdList.SetIndexBuffer(geometry.indexBufferView);

			
			for (int i = 0; i < geometry.meshRenderers.Count; i ++) {

				int indexCount = geometry.meshRenderers[i].mesh.indices.Length;
				(int vbStart, int ibStart, int obStart) = geometry.meshRendererPositions[i];

				RenderResources.BindCurrentObject(cmdList, obStart);
				cmdList.DrawIndexedInstanced(indexCount, 1, ibStart, vbStart, vbStart);

			}
			

			// Indicate that the back buffer will now be used to present
			cmdList.ResourceBarrierTransition(RenderResources.renderTargets[Graphics.frameIndex], ResourceStates.RenderTarget, ResourceStates.Present);
			
			
			cmdList.Close();

			Profiler.MetricEnd("Render setup");
			Profiler.MetricBegin();
			cmdQueue.ExecuteCommandList(cmdList);
			Profiler.MetricEnd("Render");

			#endregion

			


			// IMGui Render
			#region ImGui render

			if (Overlay.render != false) {
				Graphics.WaitForPreviousFrame();
				cmdAllocator.Reset();
				cmdList.Reset(cmdAllocator, Overlay.mainOverlay.renderer.pipelineState);

				// Indicate that the back buffer will be used as a render target
				cmdList.ResourceBarrierTransition(RenderResources.renderTargets[Graphics.frameIndex], ResourceStates.Present, ResourceStates.RenderTarget);



				// Set render target and depth stencil
				rtvHandle = RenderResources.renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
				dsvHandle = RenderResources.depthStencilDescriptorHeap.CPUDescriptorHandleForHeapStart;
				rtvHandle += Graphics.frameIndex * RenderResources.renderTargetViewDescriptorSize;
				cmdList.SetRenderTargets(rtvHandle, dsvHandle);
				cmdList.ClearDepthStencilView(dsvHandle, ClearFlags.FlagsDepth, 1f, 0);


			
				Overlay.mainOverlay.OneLoop(Profiler.deltaTime, cmdList);
			


				cmdList.ResourceBarrierTransition(RenderResources.renderTargets[Graphics.frameIndex], ResourceStates.RenderTarget, ResourceStates.Present);


				cmdList.Close();
				cmdQueue.ExecuteCommandList(cmdList);

			}

			#endregion

		}


		internal static void Dispose() {

			cmdAllocator.Dispose();
			cmdQueue.Dispose();
			cmdList.Dispose();

		}

	}
}
