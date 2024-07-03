using SharpDX;
using SharpDX.Direct3D12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ArcticFoxEngine.Graphics;

namespace ArcticFoxEngine {
	public static class Command {

		private static GraphicsCommandList commandList;
		private static CommandAllocator commandAllocator;
		private static CommandQueue commandQueue;

		internal static CommandQueue SetupCommand() {


			commandAllocator = Graphics.device.CreateCommandAllocator(CommandListType.Direct);

			CommandQueueDescription queueDesc = new CommandQueueDescription(CommandListType.Direct);
			commandQueue = Graphics.device.CreateCommandQueue(queueDesc);

			// Create the command list
			commandList = Graphics.device.CreateCommandList(CommandListType.Direct, commandAllocator, Graphics.pipelineState);
			// Command lists are created in the recording state, but there is nothing
			// to record yet. The main loop expects it to be closed, so close it now.
			commandList.Close();

			return commandQueue;

		}
		internal static CommandQueue GetCommandQueue() {
			return commandQueue;
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
			commandList.Reset(commandAllocator, Graphics.pipelineState);


			// Set necessary state
			commandList.SetGraphicsRootSignature(Graphics.rootSignature);

			commandList.SetDescriptorHeaps(1, new DescriptorHeap[] { GraphicsResources.mainCombinedDescriporHeap });
			commandList.SetGraphicsRootDescriptorTable(0, (GraphicsResources.mainCombinedDescriporHeap.GPUDescriptorHandleForHeapStart));

			commandList.SetViewport(Graphics.viewport);
			commandList.SetScissorRectangles(Graphics.scissorRect);




			// Indicate that the back buffer will be used as a render target
			commandList.ResourceBarrierTransition(GraphicsResources.renderTargets[Graphics.frameIndex], ResourceStates.Present, ResourceStates.RenderTarget);

			CpuDescriptorHandle rtvHandle = GraphicsResources.renderTargetViewHeap.CPUDescriptorHandleForHeapStart;
			rtvHandle += Graphics.frameIndex * GraphicsResources.renderTargetViewDescriptorSize;
			commandList.SetRenderTargets(rtvHandle, null);

			// Record commands
			commandList.ClearRenderTargetView(rtvHandle, new Color4(0f, 0f, 0f, 1), 0, null);



			commandList.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
			commandList.SetVertexBuffer(0, geometry.vertexBufferView);
			commandList.SetIndexBuffer(geometry.indexBufferView);
			commandList.DrawIndexedInstanced(geometry.indexData.Length, 1, 0, 0, 0);

			// Indicate that the back buffer will now be used to present
			commandList.ResourceBarrierTransition(GraphicsResources.renderTargets[Graphics.frameIndex], ResourceStates.RenderTarget, ResourceStates.Present);

			commandList.Close();

			#endregion

			commandQueue.ExecuteCommandList(commandList);

		}

		internal static void Dispose() {

			commandAllocator.Dispose();
			commandQueue.Dispose();
			commandList.Dispose();

		}

	}
}
