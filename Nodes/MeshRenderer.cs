using ImGuiNET;

namespace ArcticFoxEngine.Nodes {

	using ArcticFoxEngine.Rendering;
	using CoolClassLibrary;

	public class MeshRenderer : Node {

		internal override string nodeIconPath => ".res/NodeIcons/MeshRenderer.png";
		internal override string nodeIconPath32 => ".res/NodeIcons/MeshRenderer32.png";

		private Shader shader;
		public Material material;

		public Mesh mesh { get; private set; }
		public int textureId = 2;


		private bool meshLoaded = false;


		public MeshRenderer() {
			name = "Mesh Renderer";

			mesh = null;
			SetShader(Rendering.GetShader("Unlit"));
			material = new UnlitMaterial();

			Enable();
		}

		public void SetShader(Shader shader) {


			if (mesh != null && enabled == true) {
				UnloadMesh();
			}

			this.shader = shader;
			material = shader.GetDefaultMaterial();

			if (mesh != null && enabled == true) {
				LoadMesh();
			}

		}
		public void SetMesh(Mesh mesh) {

			// If the object has a mesh already loaded, delete it
			if (this.mesh != null && globalEnabled == true) {
				UnloadMesh();
			}

			this.mesh = mesh;
			if (this.mesh != null && globalEnabled == true) {
				LoadMesh();
			}

		}


		private void LoadMesh() {
			if (mesh == null || meshLoaded == true || shader == null) { return; }
			bool meshAdded = shader.geometryResources.AddMesh(this);
			if (meshAdded == true) {
				meshLoaded = true;
			}
		}
		private void UnloadMesh() {
			if (meshLoaded == false || shader == null) { return; }
			shader.geometryResources.RemoveMesh(this);
			meshLoaded = false;
		}
		public void UpdateMeshData() {
			if (mesh == null || meshLoaded == false) { return; }
			shader.geometryResources.UpdateMeshData(this);
		}

		internal TransformInfo GetObjectInfo() {
			TransformInfo info = new TransformInfo();

			if (transform == null) {
				return info;
			}
			Matrix transformatrionMatrix = transform.worldMatrix;
			info.transformationMatrix = transformatrionMatrix;
			info.inverseTransformationMatrix = transformatrionMatrix.Invert();
			return info;
		}


		public override void OnDisable() {
			UnloadMesh();
		}
		public override void OnEnable() {
			LoadMesh();
		}

		Vector4 vertexColSet;

		private int shaderComboSelected = 0;
		public override void GuiEvent() {


			ImGui.TextWrapped("Renders the mesh to the scene geometry");

			Vector3 vec3 = new Vector3(vertexColSet.x, vertexColSet.y, vertexColSet.z);

			ImGuiExtras.ItemWidthForText("Vertex col");
			if (ImGui.ColorEdit3("Vertex col", ref vec3, ImGuiColorEditFlags.NoInputs) == true) {
				vertexColSet = new Vector4(vec3.x, vec3.y, vec3.z, 1f);
				for (int i = 0; i < mesh.vertices.Length; i++) {
					mesh.vertices[i].color = vertexColSet;
				}
				UpdateMeshData();
			}


			// Shader combo
			Shader[] shaders = Rendering.GetAllShaders();
			string[] shaderNames = new string[shaders.Length];
			for (int i = 0; i < shaders.Length; i++) {
				shaderNames[i] = shaders[i].name;
				if (shader == shaders[i]) {
					shaderComboSelected = i;
				}
			}

			ImGuiExtras.ItemWidthForText("Shader");
			if (ImGui.Combo("Shader", ref shaderComboSelected, shaderNames, shaders.Length) == true) {
				SetShader(shaders[shaderComboSelected]);
			}

			ImGui.Text("Material Settings");
			if (material != null) {
				material.Debug();
			}

			


		}


	}
}
