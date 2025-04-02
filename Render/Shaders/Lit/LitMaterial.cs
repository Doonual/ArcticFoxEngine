using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Render {

	public class LitMaterial : Material {

		public struct MaterialInfo {

			public float normalStrength = 0.5f;
			public float textureScale = 1f;

			public MaterialInfo() {

			}

		}


		public MaterialInfo materialInfo;
		ConstBuffer<MaterialInfo> lightingInfoBuffer;
		public Texture mainTexture;
		public Texture normalTexture;
		

		public LitMaterial() {

			materialInfo = new MaterialInfo();
			lightingInfoBuffer = new ConstBuffer<MaterialInfo>(1);


		}


		public override void BindResources(Shader shader) {
			LitShader litShader = (LitShader)shader;


			litShader.mainTexSlot.SetTexture(mainTexture);
			litShader.normalTexSlot.SetTexture(normalTexture);


			lightingInfoBuffer.Write(new MaterialInfo[] { materialInfo }, 0);
			litShader.materialInfoSlot.SetData(lightingInfoBuffer, 0);

		}



		public override void DrawInspectorGUI() {

			string longestString = "Normal Texture ID";


			ImGui.Text("Albedo");
			//ImGuiExtras.ItemWidthForText(longestString);
			//ImGui.InputInt("Texture ID", ref textureID);
			ImGuiExtras.ItemWidthForText(longestString);
			ImGui.DragFloat("Texture scale", ref materialInfo.textureScale, 0.01f, 0.0001f, 20f, null, ImGuiSliderFlags.Logarithmic);

			ImGui.NewLine();

			ImGui.Text("Normal");
			//ImGuiExtras.ItemWidthForText(longestString);
			//ImGui.InputInt("Normal Texture ID", ref normalTextureID);
			ImGuiExtras.ItemWidthForText(longestString);
			ImGui.SliderFloat("Normal strength", ref materialInfo.normalStrength, -1f, 1f);

			//ImGui.Separator();


		}
	}


}
