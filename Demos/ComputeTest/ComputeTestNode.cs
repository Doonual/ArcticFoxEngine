using ArcticFoxEngine.Compute;
using ArcticFoxEngine.Input;
using ArcticFoxEngine.Input.Bindings;
using ArcticFoxEngine.Nodes;
using CoolClassLibrary;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Demos.ComputeTest {
	public class ComputeTestNode : Node {

		ComputeShader computeShader;
		Texture testTexture;

		ButtonBinding updateButton;

		public ComputeTestNode() {

			computeShader = new ComputeShader("Demos/ComputeTest/gol_compute.hlsl");
			updateButton = new KeyboardButtonInput(KeyboardButtonInput.KeyboardButton.Space);

			testTexture = new Texture(1920, 1080, flags: SharpDX.Direct3D12.ResourceFlags.AllowUnorderedAccess);
			for (int i = 0; i < testTexture.width; i ++) {
				for (int n = 0; n < testTexture.height; n ++) {
					if (MathUtil.RandomChance(0.5f) == true) {
						testTexture.SetPixelBatch(new byte[] { 0xff, 0xff, 0xff, 0xff }, i, n);
					}
				}
			}
			testTexture.BatchSync();


		}

		public override void Render() {

			Graphics.Blit(testTexture, Graphics.GetActiveResource());

		}

		public override void Update() {
			
			if (updateButton.GetButton() == true) {
				computeShader.SetTexture(testTexture);
				computeShader.Dispatch();
			}

		}

		public override void DrawInspector() {
			if (ImGui.Button("Dispatch button") == true) {
				computeShader.SetTexture(testTexture);
				computeShader.Dispatch();
			}
		}

		

	}
}
