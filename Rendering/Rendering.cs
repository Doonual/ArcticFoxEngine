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

		public static ConstBuffer<ProjectionInfo> projectionInfo;

		public static List<Shader> shaders;
		public static Texture[] textures;

		

		

		

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

			projectionInfo = new ConstBuffer<ProjectionInfo>(1);


			textures = new Texture[7];
			textures[0] = Texture.Cache.FindOrLoad(".res/Textures/white_pixel.png");
			textures[1] = Texture.Cache.FindOrLoad(".res/Textures/uv_512.png");
			textures[2] = Texture.Cache.FindOrLoad(".res/Textures/uv_blender.jpg");
			textures[3] = Texture.Cache.FindOrLoad(".res/Textures/tiger.png");
			textures[4] = Texture.Cache.FindOrLoad(".res/Textures/TestNormalMap.png");
			textures[5] = Texture.Cache.FindOrLoad(".res/Textures/BrickCol.png");
			textures[6] = Texture.Cache.FindOrLoad(".res/Textures/BrickNormal.png");



			shaders = new List<Shader>();
			shaders.Add(Shader.Cache.FindOrLoad(typeof(UnlitShader)));
			shaders.Add(Shader.Cache.FindOrLoad(typeof(LitShader)));
			shaders.Add(Shader.Cache.FindOrLoad(typeof(MandelbrotShader)));
			shaders.Add(Shader.Cache.FindOrLoad(typeof(SkyboxShader)));

			

		}

		public static Shader[] GetAllShaders() {
			return shaders.ToArray();
		}
		public static Shader GetShader(string name) {

			for (int i = 0; i < shaders.Count; i++) {
				if (shaders[i].name == name) {
					return shaders[i];
				}
			}

			return null;

		}


		internal static int ReserveDescriptorHeapSpace(int numDescriptors) {

			int spacePos = descriptorCopyPos;
			descriptorCopyPos += numDescriptors;
			return spacePos;

		}

		/// <summary>
		/// Renders a camera's view
		/// </summary>
		/// <param name="renderTarget">The render target resource to render to</param>
		/// <param name="rtvDescHeap">The descriptor heap containing the render target</param>
		/// <param name="dsvDescHeap">The descriptor heap containing the depth stencil</param>
		/// <param name="camera">The camera to render from</param>
		internal static void RenderScene(Resource renderTarget, DescriptorHeap rtvDescHeap, DescriptorHeap dsvDescHeap, Camera camera) {

			descriptorCopyPos = 0;

			camera.UpdateCameraInfoBuffer(projectionInfo);

			Graphics.WaitForCopyCommandQueue();
			Graphics.WaitForDirectCommandQueue();

			Graphics.ResetDirectCommandList(cmdList);

			cmdList.SetDescriptorHeaps(gpuDescriptorHeap);

			// Indicate that the back buffer will be used as a render target
			cmdList.ResourceBarrierTransition(renderTarget, ResourceStates.Present, ResourceStates.RenderTarget);

			// Set viewport and scissor rectancles
			cmdList.SetViewport(camera.viewport);
			cmdList.SetScissorRectangles(camera.scissorRect);

			// Set render target and depth stencil
			CpuDescriptorHandle rtvHandle = rtvDescHeap.CPUDescriptorHandleForHeapStart;
			CpuDescriptorHandle dsvHandle = dsvDescHeap.CPUDescriptorHandleForHeapStart;
			rtvHandle += Graphics.frameIndex * Graphics.rtvHeapIncrement;
			cmdList.SetRenderTargets(rtvHandle, dsvHandle);


			// Clear the render target and depth stencil
			cmdList.ClearRenderTargetView(rtvHandle, new Color4(0f, 0f, 0f, 1f), 0, null);
			cmdList.ClearDepthStencilView(dsvHandle, ClearFlags.FlagsDepth, 1f, 0);


			for (int i = 0; i < shaders.Count; i++) {



				Shader currentShader = shaders[i];
				GeometryInfo currentGeometryResources = currentShader.geometryResources;

				// Update the pipeline state and set this shaders root signature
				cmdList.PipelineState = currentShader.pipelineState;
				cmdList.SetGraphicsRootSignature(currentShader.rootSignature);

				currentShader.Render(camera, renderTarget, rtvDescHeap, dsvDescHeap);

			}

			// Indicate that the back buffer will now be used to present
			Rendering.cmdList.ResourceBarrierTransition(renderTarget, ResourceStates.RenderTarget, ResourceStates.Present);


			cmdList.Close();
			Graphics.ExecuteDirectCommandList(cmdList);

		}


		internal static void Dispose() {
			for (int i = 0; i < shaders.Count; i++) {
				shaders[i].Dispose();
			}
			cmdList.Dispose();

			for (int i = 0; i < textures.Length; i ++) {
				Texture.Cache.Release(textures[i]);
			}

		}


	}
}
