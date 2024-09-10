using ArcticFoxEngine.Nodes;

namespace ArcticFoxEngine.Rendering {

	using SharpDX.Direct3D12;

	/// <summary>
	/// Encapsulates all the tasks required to render a GeometryResources instance
	/// </summary>
	public static class Rendering {

		internal static ConstBuffer<RenderInfo> renderInfo;
		internal static Texture[] textures;

		private static List<RenderPipeline> renderPipelines;
		public static RenderPipeline rpUnlit { get { return renderPipelines[0]; } }
		public static RenderPipeline rpWireframe { get { return renderPipelines[1]; } }
		public static RenderPipeline rpMandelbrot { get { return renderPipelines[2]; } }

		internal static void Init() {


			renderInfo = new ConstBuffer<RenderInfo>(1);

			textures = new Texture[4];
			textures[0] = new Texture(".res/Textures/white_pixel.png");
			textures[1] = new Texture(".res/Textures/uv_512.png");
			textures[2] = new Texture(".res/Textures/uv_blender.jpg");
			textures[3] = new Texture(".res/Textures/tiger.png");

			renderPipelines = new List<RenderPipeline>();
			renderPipelines.Add(new UnlitRenderPipeline());
			renderPipelines.Add(new MandelbrotRenderPipeline());


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

		/// <summary>
		/// Renders a camera's view
		/// </summary>
		/// <param name="renderTarget">The render target resource to render to</param>
		/// <param name="rtvDescHeap">The descriptor heap containing the render target</param>
		/// <param name="dsvDescHeap">The descriptor heap containing the depth stencil</param>
		/// <param name="camera">The camera to render from</param>
		internal static void RenderScene(Resource renderTarget, DescriptorHeap rtvDescHeap, DescriptorHeap dsvDescHeap, Camera camera) {

			camera.UpdateCameraInfoBuffer(renderInfo);

			Graphics.cmdAllocator.Reset();
			for (int i = 0; i < renderPipelines.Count; i++) {



				RenderPipeline currentRenderPipeline = renderPipelines[i];
				GeometryInfo currentGeometryResources = currentRenderPipeline.geometryResources;

				Profiler.MetricBegin("Render Pipeline " + i);

				currentGeometryResources.UpdateObjectInfoBuffer();
				currentRenderPipeline.Render(currentGeometryResources, camera, renderTarget, rtvDescHeap, dsvDescHeap, i == 0);

				Profiler.MetricEnd();

			}

		}


		internal static void Dispose() {
			for (int i = 0; i < renderPipelines.Count; i++) {
				renderPipelines[i].Dispose();
			}

		}


	}
}
