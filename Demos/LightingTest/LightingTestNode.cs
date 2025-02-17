using ArcticFoxEngine.Gui;
using ArcticFoxEngine.ImGuiIntegration;
using ArcticFoxEngine.Nodes;
using ArcticFoxEngine.Nodes.Templates;
using ArcticFoxEngine.Rendering;
using CoolClassLibrary;

namespace ArcticFoxEngine.Demos.LightingTest {
	public class LightingTestNode : Node {

		LitMaterial floorMaterial;
		LitMaterial decorMaterial;

		public LightingTestNode() {

			floorMaterial = new LitMaterial();
			floorMaterial.mainTexture = new Texture(".res/Textures/BrickCol.png");
			floorMaterial.normalTexture = new Texture(".res/Textures/BrickNormal.png");

			decorMaterial = new LitMaterial();
			decorMaterial.mainTexture = new Texture(".res/Textures/uv_blender.jpg");
			decorMaterial.normalTexture = new Texture(".res/Textures/TestNormalMap.png");

			CreateChild<LightingSystem>();
			MeshRenderer skybox = CreateChild<MeshRenderer>("Skybox");
			skybox.SetShader<SkyboxShader>();
			skybox.SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Quad));
			for (int i = 0; i < skybox.mesh.vertices.Length; i ++) {

				skybox.mesh.vertices[i].position.y = skybox.mesh.vertices[i].position.z;
				skybox.mesh.vertices[i].position.z = 0.5f;

			}
			skybox.UpdateMeshData();


			CameraController cameraTransform = CreateChild<CameraController>();
			cameraTransform.transform.localPosition = new Vector3(0f, 5f, -10f);
			cameraTransform.CreateChild<Camera>();


			MeshRenderer mainFloor = CreateChild<MeshRenderer>("Floor");
			mainFloor.SetShader<LitShader>();
			mainFloor.SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));
			mainFloor.material = floorMaterial;
			mainFloor.transform.localPosition = new Vector3(0f, -0.5f, 0f);
			mainFloor.transform.localScale = new Vector3(20f, 1f, 10f);

			MeshRenderer decor = CreateChild<MeshRenderer>("Box 1");
			decor.SetShader<LitShader>();
			decor.SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));
			decor.material = decorMaterial;
			decor.transform.localPosition = new Vector3(-8f, 2.5f, -3f);
			decor.transform.localScale = new Vector3(1f, 5f, 1f);


			decor = CreateChild<MeshRenderer>("Box 2");
			decor.SetShader<LitShader>();
			decor.SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));
			decor.material = decorMaterial;
			decor.transform.localPosition = new Vector3(-8f, 1.5f, 0f);
			decor.transform.localScale = new Vector3(1f, 3f, 1f);

			decor = CreateChild<MeshRenderer>("Box 3");
			decor.SetShader<LitShader>();
			decor.SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));
			decor.material = decorMaterial;
			decor.transform.localPosition = new Vector3(-8f, 0.5f, 3f);
			decor.transform.localScale = new Vector3(1f, 1f, 1f);

			decor = CreateChild<MeshRenderer>("Box 4");
			decor.SetShader<LitShader>();
			decor.SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));
			decor.material = decorMaterial;
			decor.transform.localPosition = new Vector3(0f, 1.5f, 0f);
			decor.transform.localScale = new Vector3(3f, 3f, 3f);


			Light lightNode = CreateChild<Light>("Light Object 1");
			lightNode.transform.localPosition = new Vector3(-2f, 0.6f, 0f);
			lightNode.strength = 5f;

			lightNode = CreateChild<Light>("Light Object 2");
			lightNode.transform.localPosition = new Vector3(2f, 0.6f, 0f);
			lightNode.strength = 5f;


			Enable();

		}

	}
}
