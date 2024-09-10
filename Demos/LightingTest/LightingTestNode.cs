using ArcticFoxEngine.Nodes;

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
			decor.transformChild.scale = new Vector3(1f, 3f, 5f);

			Enable();

		}

	}
}
