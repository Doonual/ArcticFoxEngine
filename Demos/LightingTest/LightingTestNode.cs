using ArcticFoxEngine.Nodes;
using ArcticFoxEngine.Nodes.Templates;
using ArcticFoxEngine.Rendering;

namespace ArcticFoxEngine.Demos.LightingTest {
	public class LightingTestNode : Node {

		public LightingTestNode() {

			CreateChild<LightingSystem>();
			MeshRenderer skybox = CreateChild<MeshRenderer>("Skybox");
			skybox.SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Quad));


			CameraController cameraTransform = CreateChild<CameraController>();
			cameraTransform.transform.localPosition = new Vector3(0f, 5f, -10f);
			cameraTransform.CreateChild<Camera>();


			MeshRenderer mainFloor = CreateChild<MeshRenderer>("Floor");
			mainFloor.SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));
			mainFloor.transform.localPosition = new Vector3(0f, -0.5f, 0f);
			mainFloor.transform.localScale = new Vector3(20f, 1f, 10f);

			MeshRenderer decor = CreateChild<MeshRenderer>("Box 1");
			decor.SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));
			decor.transform.localPosition = new Vector3(-8f, 2.5f, -3f);
			decor.transform.localScale = new Vector3(1f, 5f, 1f);


			decor = CreateChild<MeshRenderer>("Box 2");
			decor.SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));
			decor.transform.localPosition = new Vector3(-8f, 1.5f, 0f);
			decor.transform.localScale = new Vector3(1f, 3f, 1f);

			decor = CreateChild<MeshRenderer>("Box 3");
			decor.SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));
			decor.transform.localPosition = new Vector3(-8f, 0.5f, 3f);
			decor.transform.localScale = new Vector3(1f, 1f, 1f);

			decor = CreateChild<MeshRenderer>("Box 4");
			decor.SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));
			decor.transform.localPosition = new Vector3(0f, 1.5f, 0f);
			decor.transform.localScale = new Vector3(3f, 3f, 3f);


			Light lightNode = CreateChild<Light>("Light Object 1");
			lightNode.transform.localPosition = new Vector3(-2f, 0.6f, 0f);
			lightNode.strength = 5f;

			lightNode = CreateChild<Light>("Light Object 2");
			lightNode.transform.localPosition = new Vector3(2f, 0.6f, 0f);
			lightNode.strength = 5f;


			RenderPipeline litRP = Rendering.Rendering.GetRenderPipeline("Lit");
			List<MeshRenderer> allMeshRenderers = SearchNodeTreeDownAll<MeshRenderer>();
			for (int i = 0; i < allMeshRenderers.Count; i ++) {
				allMeshRenderers[i].SetRenderPipeline(litRP);
			}

			Enable();

		}

	}
}
