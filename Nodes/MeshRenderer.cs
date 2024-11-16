using ImGuiNET;

namespace ArcticFoxEngine.Nodes {

	using ArcticFoxEngine.Rendering;

	public class MeshRenderer : Node {

		internal override string description => "Renders the mesh to the scene geometry";
		internal override string nodeIconPath => ".res/NodeIcons/MeshRenderer.png";
		internal override string nodeIconPath32 => ".res/NodeIcons/MeshRenderer32.png";

		private RenderPipeline renderPipeline;
		public Material material;

		public Mesh mesh { get; private set; }
		public int textureId = 2;


		private bool meshLoaded = false;


		public MeshRenderer() {
			name = "Mesh Renderer";

			mesh = null;
			SetRenderPipeline(Rendering.GetRenderPipeline("Unlit"));
			material = new UnlitMaterial();

			Enable();
		}

		public void SetRenderPipeline(RenderPipeline renderPipeline) {


			if (mesh != null && enabled == true) {
				UnloadMesh();
			}

			this.renderPipeline = renderPipeline;
			material = renderPipeline.GetDefaultMaterial();

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
			if (mesh == null || meshLoaded == true || renderPipeline == null) { return; }
			bool meshAdded = renderPipeline.geometryResources.AddMesh(this);
			if (meshAdded == true) {
				meshLoaded = true;
			}
		}
		private void UnloadMesh() {
			if (meshLoaded == false || renderPipeline == null) { return; }
			renderPipeline.geometryResources.RemoveMesh(this);
			meshLoaded = false;
		}
		public void UpdateMeshData() {
			if (mesh == null || meshLoaded == false) { return; }
			renderPipeline.geometryResources.UpdateMeshData(this);
		}

		internal ObjectInfo GetObjectInfo() {
			ObjectInfo info = new ObjectInfo();

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

		private int renderPipelineComboSelected = 0;
		public override void Debug() {




			System.Numerics.Vector3 vec3 = new System.Numerics.Vector3(vertexColSet.x, vertexColSet.y, vertexColSet.z);

			ImGuiExtras.ItemWidthForText("Vertex col");
			if (ImGui.ColorEdit3("Vertex col", ref vec3, ImGuiColorEditFlags.NoInputs) == true) {
				vertexColSet = new Vector4(vec3.X, vec3.Y, vec3.Z, 1f);
				for (int i = 0; i < mesh.vertices.Length; i++) {
					mesh.vertices[i].color = vertexColSet;
				}
				UpdateMeshData();
			}


			// Render Pipeline combo
			RenderPipeline[] renderPipelines = Rendering.GetAllRenderPipelines();
			string[] pipelineNames = new string[renderPipelines.Length];
			for (int i = 0; i < renderPipelines.Length; i++) {
				pipelineNames[i] = renderPipelines[i].name;
				if (renderPipeline == renderPipelines[i]) {
					renderPipelineComboSelected = i;
				}
			}

			ImGuiExtras.ItemWidthForText("Render pipeline");
			if (ImGui.Combo("Render pipeline", ref renderPipelineComboSelected, pipelineNames, renderPipelines.Length) == true) {
				SetRenderPipeline(renderPipelines[renderPipelineComboSelected]);
			}

			ImGui.Text("Material Settings");
			if (material != null) {
				material.Debug();
			}

			


		}


	}
}
