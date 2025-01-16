using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Rendering {

	public class LitMaterial : Material {

		public LitShader.MaterialInfo materialInfo;
		public int textureID = 5;
		public int normalTextureID = 6;
		ConstBuffer<LitShader.MaterialInfo> lightingInfoBuffer;

		public LitMaterial() {

			materialInfo = new LitShader.MaterialInfo();
			lightingInfoBuffer = new ConstBuffer<LitShader.MaterialInfo>(1);


		}


		public override void BindResources(Shader shader) {
			LitShader litShader = (LitShader)shader;


			litShader.mainTexSlot.SetTexture(Rendering.textures[textureID]);
			litShader.normalTexSlot.SetTexture(Rendering.textures[normalTextureID]);


			lightingInfoBuffer.Write(new LitShader.MaterialInfo[] { materialInfo }, 0);
			litShader.materialInfoSlot.SetData(lightingInfoBuffer, 0);

		}



		public override void DrawInspectorGUI() {

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

			//ImGui.Separator();


		}
	}


}
