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
	public static class GPU_Render {

		internal static RenderPipeline renderPipeline;
		internal static ConstBuffer<RenderInfo> renderInfo;

		internal static GeometryResources mainGeometry;

		internal static Texture[] textures;

		internal static void Init() {

			mainGeometry = new GeometryResources();
			renderInfo = new ConstBuffer<RenderInfo>(1);

			textures = new Texture[4];
			textures[0] = new Texture(".res/Textures/white_pixel.png");
			textures[1] = new Texture(".res/Textures/uv_512.png");
			textures[2] = new Texture(".res/Textures/uv_blender.jpg");
			textures[3] = new Texture(".res/Textures/tiger.png");

			ShaderBytecode vertexShader = Graphics.CompileShader(".res/VertexShader.hlsl", Graphics.ShaderType.Vertex);
			ShaderBytecode geometryShader = Graphics.CompileShader(".res/GeometryShader.hlsl", Graphics.ShaderType.Geometry);
			ShaderBytecode pixelShader = Graphics.CompileShader(".res/PixelShader.hlsl", Graphics.ShaderType.Pixel);

			renderPipeline = new RenderPipeline();

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


			renderPipeline.Finalise(vertexShader, pixelShader, geometryShader);

		}


		
		/// <summary>
		/// Renders a camera's view
		/// </summary>
		/// <param name="renderTarget">The render target resource to render to</param>
		/// <param name="rtvDescHeap">The descriptor heap containing the render target</param>
		/// <param name="dsvDescHeap">The descriptor heap containing the depth stencil</param>
		/// <param name="camera">The camera to render from</param>
		internal static void Render(Resource renderTarget, DescriptorHeap rtvDescHeap, DescriptorHeap dsvDescHeap, Camera camera) {


			mainGeometry.UpdateObjectInfoBuffer();
			camera.UpdateCameraInfoBuffer(renderInfo);
			renderPipeline.Render(mainGeometry, camera, renderTarget, rtvDescHeap, dsvDescHeap);


		}


		internal static void Dispose() {
			renderPipeline.Dispose();
		}


	}
}
