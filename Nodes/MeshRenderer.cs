using ArcticFoxEngine.Backend;
using CoolClassLibrary;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Nodes {

	using ArcticFoxEngine.Backend.Render;
	
	public class MeshRenderer : Node {

		internal override string debugName => "Mesh Renderer";
		internal override string debugDescription => "Renders the mesh to the scene geometry";
		internal override string nodeIconPath => ".res/NodeIcons/MeshRenderer.png";

		private string renderPipeline;
		public Mesh mesh { get; private set; }
		public int textureId = 2;


		private bool meshLoaded = false;


		public MeshRenderer() : base() {
			mesh = null;
			renderPipeline = "normal";
			SetName("Mesh Renderer");
			Enable();
		}

		public void SetRenderPipeline(string renderPipeline) {

			if (mesh != null && enabled == true) {
				UnloadMesh(this.renderPipeline);
			}

			this.renderPipeline = renderPipeline;

			if (mesh != null && enabled == true) {
				LoadMesh(renderPipeline);
			}

		}
		public void SetMesh(Mesh mesh) {

			// If the object has a mesh already loaded, delete it
			bool prevEnabled = enabled;
			if (this.mesh != null && enabled == true) {
				UnloadMesh(renderPipeline);
			}
			
			this.mesh = mesh;
			if (this.mesh != null && enabled == true) {
				LoadMesh(renderPipeline);
			}
			
		}
		
		private void LoadMesh(string renderPipeline) {
			if (mesh == null || meshLoaded == true) { return; }
			bool meshAdded = GPU_Render.renderPipelines[renderPipeline].geometryResources.AddMesh(this);
			if (meshAdded == true) {
				meshLoaded = true;
			}
		}
		private void UnloadMesh(string renderPipeline) {
			if (meshLoaded == false) { return; }
			GPU_Render.renderPipelines[renderPipeline].geometryResources.RemoveMesh(this);
			meshLoaded = false;
		}
		public void UpdateMeshData() {
			if (mesh == null || meshLoaded == false) { return; }
			GPU_Render.renderPipelines[renderPipeline].geometryResources.UpdateMeshData(this);
		}

		internal ObjectInfo GetObjectInfo() {
			ObjectInfo info = new ObjectInfo();
			info.transformationMatrix = Transform.CalculateFromNode(this); ;
			return info;
		}


		public override void OnDisable() {
			UnloadMesh(renderPipeline);
		}
		public override void OnEnable() {
			LoadMesh(renderPipeline);
		}

		Vector4 vertexColSet;

		private int renderPipelineComboSelected = 0;
		public override void Debug() {

			// Render Pipeline combo
			string[] renderPipelines = GPU_Render.renderPipelines.Keys.ToArray();
			ImGuiExtras.ItemWidthForText("Render pipeline");
			ImGui.Combo("Render pipeline", ref renderPipelineComboSelected, renderPipelines, renderPipelines.Length);
			if (renderPipelines[renderPipelineComboSelected] != renderPipeline) {
				SetRenderPipeline(renderPipelines[renderPipelineComboSelected]);
			}

			// Texture ID input
			ImGuiExtras.ItemWidthForText("Render pipeline");
			ImGui.InputInt("Texture ID", ref textureId);

			System.Numerics.Vector3 vec3 = new System.Numerics.Vector3(vertexColSet.x, vertexColSet.y, vertexColSet.z);

			ImGuiExtras.ItemWidthForText("Vertex col");
			if (ImGui.ColorEdit3("Vertex col", ref vec3, ImGuiColorEditFlags.NoInputs) == true) {
				vertexColSet = new Vector4(vec3.X, vec3.Y, vec3.Z, 1f);
				for (int i = 0; i < mesh.vertices.Length; i++) {
					mesh.vertices[i].color = vertexColSet;
				}
				UpdateMeshData();
			}


			
			


		}
		

	}
}
