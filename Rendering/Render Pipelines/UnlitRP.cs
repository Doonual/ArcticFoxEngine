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

		public Texture mainTex;
		

		public override void BindResources(RenderPipeline renderPipeline) {

			if (mainTex == null) {
				renderPipeline.SetTextureSlot("Main tex", Rendering.textures[0]);
			}
			else {
				renderPipeline.SetTextureSlot("Main tex", mainTex);
			}
			

		}

		int setTextureId = 0;
		public override void Debug() {


			if (ImGui.InputInt("Texture ID", ref setTextureId) == true) {
				mainTex = Rendering.textures[setTextureId];
			}

		}

	}

}
