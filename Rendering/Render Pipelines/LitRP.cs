using CoolClassLibrary;
using ImGuiNET;
using SharpDX.Direct3D12;

namespace ArcticFoxEngine.Rendering {
	
	public class LitRenderPipeline : RenderPipeline {

		public struct LightingWorld {

			public Vector3 sunDir;
			public float sunStrength;
			public float ambientLight;

			public LightingWorld() {
				sunDir = new Vector3(-0.25f, -0.5f, 0.4f).Normalize();
				sunStrength = 0.8f;
				ambientLight = 0.2f;
			}

		}
		public struct LightData {

			public Vector4 pos;
			public Vector3 col;
			public float strength;

		}

		static ConstBuffer<LightingWorld> lightingInfoBuffer;
		static StructuredBuffer<LightData> lightBuffer;

		public override string name => "Lit";

		public LitRenderPipeline() {

			lightBuffer = new StructuredBuffer<LightData>(16);
			lightingInfoBuffer = new ConstBuffer<LightingWorld>(1);

			CreateDataSlot("Lighting world", ShaderVisibility.Pixel);
			CreateDataSlot("Material info", ShaderVisibility.Pixel);
			
			CreateTextureSlot("Main tex", ShaderVisibility.Pixel);
			CreateTextureSlot("Normal tex", ShaderVisibility.Pixel);
			CreateBufferSlot("Light info", lightBuffer.numElements, ShaderVisibility.Pixel);

			TextureSamplerOptions textureSamplerOptions = new TextureSamplerOptions() {
				addressUVW = TextureAddressMode.Wrap,
				filter = Filter.MinimumMinMagMipPoint,
			};
			CreateTextureSampler(textureSamplerOptions, ShaderVisibility.Pixel);

			

			ShaderBytecode vertexShader = Graphics.CompileShader(".res/Shaders/VertexShader.hlsl", Graphics.ShaderType.Vertex);
			ShaderBytecode geometryShader = Graphics.CompileShader(".res/Shaders/GeometryShader.hlsl", Graphics.ShaderType.Geometry);
			ShaderBytecode pixelShader = Graphics.CompileShader(".res/Shaders/Lit/LitPixelShader.hlsl", Graphics.ShaderType.Pixel);

			Finalise(vertexShader, pixelShader, geometryShader);

		}
		public static void SetLightingInfo(LightingWorld lightingInfo) {
			lightingInfoBuffer.Write(new LightingWorld[] { lightingInfo }, 0);
		}
		public static void SetLightData(LightData lightData, int bufferPos) {
			lightBuffer.Write(new LightData[] { lightData }, bufferPos);
		}

		protected override void SetGlobalData() {
			SetDataSlot("Lighting world", lightingInfoBuffer, 0);
			SetBufferSlot("Light info", lightBuffer, 0);
		}

		public override Material GetDefaultMaterial() {
			return new LitMaterial();
		}

	}

	public struct MaterialInfo {

		public float normalStrength = 0.5f;
		public float textureScale = 1f;

		public MaterialInfo() {

		}

	}

	public class LitMaterial : Material {

		public MaterialInfo materialInfo;
		public int textureID = 5;
		public int normalTextureID = 6;
		ConstBuffer<MaterialInfo> lightingInfoBuffer;

		public LitMaterial() {

			materialInfo = new MaterialInfo();
			lightingInfoBuffer = new ConstBuffer<MaterialInfo>(1);
			

		}


		public override void BindResources(RenderPipeline renderPipeline) {

			renderPipeline.SetTextureSlot("Main tex", Rendering.textures[textureID]);
			renderPipeline.SetTextureSlot("Normal tex", Rendering.textures[normalTextureID]);


			lightingInfoBuffer.Write(new MaterialInfo[] { materialInfo }, 0);
			renderPipeline.SetDataSlot("Material info", lightingInfoBuffer, 0);

		}

		public override void Debug() {

			string longestString = "Normal Texture ID";

			ImGui.Text("Albedo");
			ImGuiExtras.ItemWidthForText(longestString);
			ImGui.InputInt("Texture ID", ref textureID);
			ImGuiExtras.ItemWidthForText(longestString);
			ImGui.DragFloat("Texture scale", ref materialInfo.textureScale, 0.01f, 0.0001f, 20f, null, ImGuiSliderFlags.Logarithmic);

			ImGui.NewLine();

			ImGui.Text("Normal");
			ImGuiExtras.ItemWidthForText(longestString);
			ImGui.InputInt("Normal Texture ID", ref normalTextureID);
			ImGuiExtras.ItemWidthForText(longestString);
			ImGui.SliderFloat("Normal strength", ref materialInfo.normalStrength, -1f, 1f);

		}
	}

}
