using ArcticFoxEngine.Nodes;
using ArcticFoxEngine.Rendering;

namespace ArcticFoxEngine.Demos.LightingTest {
	public class LightingTestNode : Node {

		public LightingTestNode() {

			CreateChild<LightingSystem>();
			CameraController cameraController = CreateChild<CameraController>();
			cameraController.transformChild.position = new Vector3(0f, 5f, -10f);

			Node mainFloor = CreateChild<Cube>();
			mainFloor.transformChild.position = new Vector3(0f, -0.5f, 0f);
			mainFloor.transformChild.scale = new Vector3(20f, 1f, 10f);

			Node decor;
			decor = CreateChild<Cube>();
			decor.transformChild.position = new Vector3(-8f, 2.5f, -3f);
			decor.transformChild.scale = new Vector3(1f, 5f, 1f);

			decor = CreateChild<Cube>();
			decor.transformChild.position = new Vector3(-8f, 1.5f, 0f);
			decor.transformChild.scale = new Vector3(1f, 3f, 1f);

			decor = CreateChild<Cube>();
			decor.transformChild.position = new Vector3(-8f, 0.5f, 3f);
			decor.transformChild.scale = new Vector3(1f, 1f, 1f);



			decor = CreateChild<Cube>();
			decor.transformChild.position = new Vector3(0f, 1.5f, 0f);
			decor.transformChild.scale = new Vector3(3f, 3f, 3f);


			Node lightNode = CreateChild<BaseNode>("Light Object");
			Transform lightTransform = lightNode.CreateChild<Transform>();
			lightTransform.position = new Vector3(-2f, 0.6f, 0f);
			Light testLight = lightNode.CreateChild<Light>();
			testLight.strength = 5f;

			lightNode = CreateChild<BaseNode>("Light Object");
			lightTransform = lightNode.CreateChild<Transform>();
			lightTransform.position = new Vector3(2f, 0.6f, 0f);
			testLight = lightNode.CreateChild<Light>();
			testLight.strength = 5f;


			RenderPipeline litRP = Rendering.Rendering.GetRenderPipeline("Lit");
			List<MeshRenderer> allMeshRenderers = SearchNodeTreeAll<MeshRenderer>();
			for (int i = 0; i < allMeshRenderers.Count; i ++) {
				allMeshRenderers[i].SetRenderPipeline(litRP);
			}

			Enable();

		}

	}
}
