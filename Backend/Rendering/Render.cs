using ArcticFoxEngine.Backend;
using ArcticFoxEngine.Nodes;
using ArcticFoxEngine.Debug;
using ArcticFoxEngine;
using CoolClassLibrary;
using SharpDX;

using SharpDX.DXGI;
using SharpDX.Direct3D12;

namespace ArcticFoxEngine.Backend.Render {

	using SharpDX.Direct3D12;

	/// <summary>
	/// Encapsulates all the tasks required to render a GeometryResources instance
	/// </summary>
	public static class Render {

		internal static ConstBuffer<RenderInfo> renderInfo;
		internal static Texture[] textures;

		internal static Dictionary<string, (RenderPipeline renderPipeline, GeometryInfo geometryResources)> renderPipelines;


		internal static void Init() {

			
			renderInfo = new ConstBuffer<RenderInfo>(1);

			textures = new Texture[4];
			textures[0] = new Texture(".res/Textures/white_pixel.png");
			textures[1] = new Texture(".res/Textures/uv_512.png");
			textures[2] = new Texture(".res/Textures/uv_blender.jpg");
			textures[3] = new Texture(".res/Textures/tiger.png");

			renderPipelines = new Dictionary<string, (RenderPipeline renderPipeline, GeometryInfo geometryResources)>();

			SetupMainRP();
			SetupWireframeRP();
			SetupMandelbrotRP();

		}

		private static void SetupMainRP() {


			GeometryInfo mainGeometry = new GeometryInfo();
			RenderPipeline renderPipeline = new RenderPipeline();

			renderPipeline.BindBuffer(renderInfo, ShaderVisibility.All, (int mrIndex) => { return 0; });
			renderPipeline.BindBuffer(mainGeometry.objectBuffer, ShaderVisibility.All, (int mrIndex) => { return mainGeometry.meshRendererPositions[mrIndex].obStart; });

			RenderPipeline.TextureSamplerOptions textureSamplerOptions = new RenderPipeline.TextureSamplerOptions() {
				addressUVW = TextureAddressMode.Wrap,
				filter = Filter.MinimumMinMagMipPoint,
			};
			renderPipeline.BindTextureSampler(textureSamplerOptions, ShaderVisibility.Pixel);

			renderPipeline.CreateTextureSlot(ShaderVisibility.Pixel, (int mrIndex) => { return mainGeometry.meshRenderers[mrIndex].textureId; });
			renderPipeline.BindTexture(textures[0], ShaderVisibility.Pixel);
			renderPipeline.BindTexture(textures[1], ShaderVisibility.Pixel);
			renderPipeline.BindTexture(textures[2], ShaderVisibility.Pixel);
			renderPipeline.BindTexture(textures[3], ShaderVisibility.Pixel);

			ShaderBytecode vertexShader = Graphics.CompileShader(".res/Shaders/VertexShader.hlsl", Graphics.ShaderType.Vertex);
			ShaderBytecode geometryShader = Graphics.CompileShader(".res/Shaders/GeometryShader.hlsl", Graphics.ShaderType.Geometry);
			ShaderBytecode pixelShader = Graphics.CompileShader(".res/Shaders/PixelShader.hlsl", Graphics.ShaderType.Pixel);

			renderPipeline.Finalise(vertexShader, pixelShader, geometryShader);

			renderPipelines.Add("normal", (renderPipeline, mainGeometry));

		}
		private static void SetupWireframeRP() {


			GeometryInfo mainGeometry = new GeometryInfo();
			RenderPipeline renderPipeline = new RenderPipeline();

			renderPipeline.BindBuffer(renderInfo, ShaderVisibility.All, (int mrIndex) => { return 0; });
			renderPipeline.BindBuffer(mainGeometry.objectBuffer, ShaderVisibility.All, (int mrIndex) => { return mainGeometry.meshRendererPositions[mrIndex].obStart; });

			RenderPipeline.TextureSamplerOptions textureSamplerOptions = new RenderPipeline.TextureSamplerOptions() {
				addressUVW = TextureAddressMode.Wrap,
				filter = Filter.MinimumMinMagMipPoint,
			};
			renderPipeline.BindTextureSampler(textureSamplerOptions, ShaderVisibility.Pixel);

			renderPipeline.CreateTextureSlot(ShaderVisibility.Pixel, (int mrIndex) => { return mainGeometry.meshRenderers[mrIndex].textureId; });
			renderPipeline.BindTexture(textures[0], ShaderVisibility.Pixel);
			renderPipeline.BindTexture(textures[1], ShaderVisibility.Pixel);
			renderPipeline.BindTexture(textures[2], ShaderVisibility.Pixel);
			renderPipeline.BindTexture(textures[3], ShaderVisibility.Pixel);

			ShaderBytecode vertexShader = Graphics.CompileShader(".res/Shaders/VertexShader.hlsl", Graphics.ShaderType.Vertex);
			ShaderBytecode geometryShader = Graphics.CompileShader(".res/Shaders/GeometryShader.hlsl", Graphics.ShaderType.Geometry);
			ShaderBytecode pixelShader = Graphics.CompileShader(".res/Shaders/PixelShader.hlsl", Graphics.ShaderType.Pixel);

			RasterizerStateDescription rasterState = RasterizerStateDescription.Default();
			rasterState.FillMode = FillMode.Wireframe;

			renderPipeline.Finalise(vertexShader, pixelShader, geometryShader, rasterState: rasterState);

			renderPipelines.Add("wireframe", (renderPipeline, mainGeometry));

		}
		private static void SetupMandelbrotRP() {

			GeometryInfo mainGeometry = new GeometryInfo();
			RenderPipeline renderPipeline = new RenderPipeline();

			renderPipeline.BindBuffer(renderInfo, ShaderVisibility.All, (int mrIndex) => { return 0; });
			renderPipeline.BindBuffer(mainGeometry.objectBuffer, ShaderVisibility.All, (int mrIndex) => { return mainGeometry.meshRendererPositions[mrIndex].obStart; });

			RenderPipeline.TextureSamplerOptions textureSamplerOptions = new RenderPipeline.TextureSamplerOptions() {
				addressUVW = TextureAddressMode.Wrap,
				filter = Filter.MinimumMinMagMipPoint,
			};
			renderPipeline.BindTextureSampler(textureSamplerOptions, ShaderVisibility.Pixel);

			renderPipeline.CreateTextureSlot(ShaderVisibility.Pixel, (int mrIndex) => { return mainGeometry.meshRenderers[mrIndex].textureId; });
			renderPipeline.BindTexture(textures[0], ShaderVisibility.Pixel);
			renderPipeline.BindTexture(textures[1], ShaderVisibility.Pixel);
			renderPipeline.BindTexture(textures[2], ShaderVisibility.Pixel);
			renderPipeline.BindTexture(textures[3], ShaderVisibility.Pixel);

			ShaderBytecode vertexShader = Graphics.CompileShader(".res/Shaders/VertexShader.hlsl", Graphics.ShaderType.Vertex);
			ShaderBytecode geometryShader = Graphics.CompileShader(".res/Shaders/GeometryShader.hlsl", Graphics.ShaderType.Geometry);
			ShaderBytecode pixelShader = Graphics.CompileShader(".res/Shaders/mandelbrot_shader.hlsl", Graphics.ShaderType.Pixel);

			renderPipeline.Finalise(vertexShader, pixelShader, geometryShader);

			renderPipelines.Add("mandelbrot", (renderPipeline, mainGeometry));

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
			for (int i = 0; i < renderPipelines.Count; i ++) {



				RenderPipeline currentRenderPipeline = renderPipelines.ElementAt(i).Value.Item1;
				GeometryInfo currentGeometryResources = renderPipelines.ElementAt(i).Value.Item2;

				Profiler.MetricBegin(renderPipelines.ElementAt(i).Key + " RP");

				currentGeometryResources.UpdateObjectInfoBuffer();
				currentRenderPipeline.Render(currentGeometryResources, camera, renderTarget, rtvDescHeap, dsvDescHeap, i == 0);

				Profiler.MetricEnd();

			}

			Graphics.ExecuteCommandLists();

		}


		internal static void Dispose() {
			for (int i = 0; i < renderPipelines.Count; i++) {
				RenderPipeline currentRenderPipeline = renderPipelines.ElementAt(i).Value.Item1;
				GeometryInfo currentGeometryResources = renderPipelines.ElementAt(i).Value.Item2;
				currentRenderPipeline.Dispose();
				currentGeometryResources.Dispose();
			}

		}


	}
}
