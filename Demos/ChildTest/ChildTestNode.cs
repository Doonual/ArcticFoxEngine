using ArcticFoxEngine.Nodes;
using ArcticFoxEngine.Demos.SceneTest;
using ImGuiNET;
using CoolClassLibrary;
using ArcticFoxEngine.Render;

namespace ArcticFoxEngine.Demos.ChildTest {

	public class ChildTestNode : Node {

		Texture groundTexture;
		Texture cubeTexture;

		public ChildTestNode() {
			name = "Child Test";

			groundTexture = new Texture(".res/Textures/BrickCol.png");
			cubeTexture = new Texture(".res/Textures/uv_blender.jpg");

			CameraController cameraController = CreateChild<CameraController>();
			cameraController.CreateChild<Camera>();

			Node platformsNode = CreateChild<BaseNode>("Platforms");

			#region Block tower

			MeshRenderer blockPlatform = platformsNode.CreateChild<MeshRenderer>("Block tower platform");
			blockPlatform.SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));
			((UnlitMaterial)blockPlatform.material).mainTex = groundTexture;
			blockPlatform.transform.localPosition = new Vector3(0f, -0.5f, 0f);
			blockPlatform.transform.localScale = new Vector3(5f, 1f, 5f);

			MeshRenderer blockTowerA = CreateChild<MeshRenderer>("Block Tower A");
			blockTowerA.SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));
			((UnlitMaterial)blockTowerA.material).mainTex = cubeTexture;
			blockTowerA.transform.localPosition = new Vector3(0f, 0.5f, 0f);

			MeshRenderer blockTowerB = blockTowerA.CreateChild<MeshRenderer>("Block Tower B");
			blockTowerB.SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));
			((UnlitMaterial)blockTowerB.material).mainTex = cubeTexture;
			blockTowerB.transform.localPosition = new Vector3(0f, 2f, 0f);

			MeshRenderer blockTowerC = blockTowerB.CreateChild<MeshRenderer>("Block Tower C");
			blockTowerC.SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));
			((UnlitMaterial)blockTowerC.material).mainTex = cubeTexture;
			blockTowerC.transform.localPosition = new Vector3(0f, 2f, 0f);

			#endregion
			#region Cube Cube

			MeshRenderer cubePlatform = platformsNode.CreateChild<MeshRenderer>("Cube cube platform");
			cubePlatform.SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));
			((UnlitMaterial)cubePlatform.material).mainTex = groundTexture;
			cubePlatform.transform.localPosition = new Vector3(-7f, -0.5f, 1f);
			cubePlatform.transform.localScale = new Vector3(7f, 1f, 7f);

			CubeRoller cubeParent = CreateChild<CubeRoller>("Cube Cube");
			cubeParent.transform.localPosition = new Vector3(-7f, 4f, 1f);
			Vector3[] positions = new Vector3[] {
				new Vector3(-2f, -2f, -2f),
				new Vector3(2f, -2f, -2f),
				new Vector3(-2f, 2f, -2f),
				new Vector3(2f, 2f, -2f),
				new Vector3(-2f, -2f, 2f),
				new Vector3(2f, -2f, 2f),
				new Vector3(-2f, 2f, 2f),
				new Vector3(2f, 2f, 2f),
			};
			for (int i = 0; i < positions.Length; i++) {
				MeshRenderer cubeObj = cubeParent.CreateChild<MeshRenderer>("Cube #" + i);
				cubeObj.SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));
				((UnlitMaterial)cubeObj.material).mainTex = cubeTexture;
				cubeObj.transform.localPosition = positions[i];
			}

			#endregion

			TreeTester tester = CreateChild<TreeTester>();
			tester.transform.localPosition = new Vector3(8f / 2f + 3.5f, -0.5f, (8f / 2f) - 2.5f);
			tester.GenerateTree(8);
			
			Enable();
		}

		public override void DrawInspector() {
			if (ImGui.Button("Recurse") == true) {
				Recurse();
			}
			ImGui.Text("Enabled: " + enabled);
		}

		public void Recurse() {

			ChildTestNode nextChildTestNode = CreateChild<ChildTestNode>();
			nextChildTestNode.transform.localPosition = new Vector3(-5.25f, 0f, -6f);
			nextChildTestNode.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
			nextChildTestNode.GetChild(0).Disable();

		}


	}
}
