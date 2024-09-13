using ImGuiNET;
using SharpDX.Direct3D12;

namespace ArcticFoxEngine.Rendering {
	public class UnlitRenderPipeline : RenderPipeline {

		public override string name => "Unlit";

		public UnlitRenderPipeline() {

			CreateTextureSlot("Main tex", ShaderVisibility.Pixel);

			TextureSamplerOptions textureSamplerOptions = new TextureSamplerOptions() {
				addressUVW = TextureAddressMode.Wrap,
				filter = Filter.MinimumMinMagMipPoint,
			};
			CreateTextureSampler(textureSamplerOptions, ShaderVisibility.Pixel);

			ShaderBytecode vertexShader = Graphics.CompileShader(".res/Shaders/VertexShader.hlsl", Graphics.ShaderType.Vertex);
			ShaderBytecode geometryShader = Graphics.CompileShader(".res/Shaders/GeometryShader.hlsl", Graphics.ShaderType.Geometry);
			ShaderBytecode pixelShader = Graphics.CompileShader(".res/Shaders/Unlit/PixelShader.hlsl", Graphics.ShaderType.Pixel);

			Finalise(vertexShader, pixelShader, geometryShader);

		}

		public override Material GetDefaultMaterial() {
			return new UnlitMaterial();
		}
	}

	public class UnlitMaterial : Material {

		int textureId = 1;

		public override void BindResources(RenderPipeline renderPipeline) {

			renderPipeline.SetTextureSlot("Main tex", Rendering.textures[textureId]);

		}

		public override void Debug() {

			ImGui.InputInt("Texture ID", ref textureId);

		}

	}

}
