using ArcticFoxEngine.Nodes;

namespace ArcticFoxEngine.Rendering {
	using SharpDX.Direct3D12;

	/// <summary>
	/// Encapsulates all the tasks required to render a GeometryResources instance
	/// </summary>
	public static class Rendering {

		internal static GraphicsCommandList cmdList;
		internal static DescriptorHeap gpuDescriptorHeap;
		private static int descriptorCopyPos;
		internal static int descriptorHeapIncrement;

		private static List<RenderPipeline> renderPipelines;
		public static RenderPipeline rpUnlit { get { return renderPipelines[0]; } }
		public static RenderPipeline rpLit { get { return renderPipelines[1]; } }
		public static RenderPipeline rpMandelbrot { get { return renderPipelines[2]; } }

		internal static ConstBuffer<RenderInfo> renderInfo;
		internal static Texture[] textures;

		

		

		

		internal static void Init() {

			DescriptorHeapDescription descHeapDesc = new DescriptorHeapDescription() {
				DescriptorCount = 100000,
				Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView,
				Flags = DescriptorHeapFlags.ShaderVisible,
			};
			gpuDescriptorHeap = Graphics.device.CreateDescriptorHeap(descHeapDesc);
			descriptorCopyPos = 0;
			descriptorHeapIncrement = Graphics.device.GetDescriptorHandleIncrementSize(DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);


			cmdList = Graphics.CreateGraphicsCommandList();
			cmdList.Close();

			renderInfo = new ConstBuffer<RenderInfo>(1);

			textures = new Texture[7];
			textures[0] = new Texture(".res/Textures/white_pixel.png");
			textures[1] = new Texture(".res/Textures/uv_512.png");
			textures[2] = new Texture(".res/Textures/uv_blender.jpg");
			textures[3] = new Texture(".res/Textures/tiger.png");
			textures[4] = new Texture(".res/Textures/TestNormalMap.png");
			textures[5] = new Texture(".res/Textures/BrickCol.png");
			textures[6] = new Texture(".res/Textures/BrickNormal.png");


			renderPipelines = new List<RenderPipeline>();
			renderPipelines.Add(new UnlitRenderPipeline());
			renderPipelines.Add(new LitRenderPipeline());
			//renderPipelines.Add(new MandelbrotRenderPipeline());

			

		}

		public static RenderPipeline[] GetAllRenderPipelines() {
			return renderPipelines.ToArray();
		}
		public static RenderPipeline GetRenderPipeline(string name) {

			for (int i = 0; i < renderPipelines.Count; i++) {
				if (renderPipelines[i].name == name) {
					return renderPipelines[i];
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

			camera.UpdateCameraInfoBuffer(renderInfo);

			Graphics.cmdAllocator.Reset();
			cmdList.Reset(Graphics.cmdAllocator, null);
			cmdList.SetDescriptorHeaps(gpuDescriptorHeap);

			for (int i = 0; i < renderPipelines.Count; i++) {



				RenderPipeline currentRenderPipeline = renderPipelines[i];
				GeometryInfo currentGeometryResources = currentRenderPipeline.geometryResources;

				Profiler.MetricBegin("Render Pipeline " + i);

				currentGeometryResources.UpdateObjectInfoBuffer();
				currentRenderPipeline.Render(currentGeometryResources, camera, renderTarget, rtvDescHeap, dsvDescHeap, i == 0);

				Profiler.MetricEnd();

			}


			cmdList.Close();
			Graphics.SubmitGraphicsCommandList(cmdList);

		}


		internal static void Dispose() {
			for (int i = 0; i < renderPipelines.Count; i++) {
				renderPipelines[i].Dispose();
			}
			cmdList.Dispose();

		}


	}
}
