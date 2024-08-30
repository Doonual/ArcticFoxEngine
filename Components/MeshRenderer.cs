using ArcticFoxEngine.Backend;
using CoolClassLibrary;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Components {

	using ArcticFoxEngine.Backend.Render;
	
	public class MeshRenderer : Component {

		public Mesh mesh { get; private set; }
		private bool meshLoaded = false;

		public MeshRenderer() {
			mesh = null;
		}

		public void SetMesh(Mesh mesh) {

			// If the object has a mesh already loaded, delete it
			bool prevEnabled = enabled;
			if (this.mesh != null && enabled == true) {
				UnloadMesh();
			}
			
			this.mesh = mesh;
			if (this.mesh != null && enabled == true) {
				LoadMesh();
			}
			

		}

		private void LoadMesh() {
			if (mesh == null || meshLoaded == true) { return; }
			
			bool meshAdded = gameObject.scene.mainGeometry.AddMesh(this);
			if (meshAdded == true) {
				meshLoaded = true;
				//Enable();
			}
			else {
				//Disable();
			}
		}
		private void UnloadMesh() {
			if (meshLoaded == false) { return; }
			gameObject.scene.mainGeometry.RemoveMesh(this);
			meshLoaded = false;
			//Disable();
		}

		internal ObjectInfo GetObjectInfo() {
			ObjectInfo info = new ObjectInfo();
			info.transformationMatrix = gameObject.transform.transformationMatrix;
			return info;
		}


		public override void OnDisable() {
			UnloadMesh();
		}
		public override void OnEnable() {
			LoadMesh();
		}

		Vector4 vertexColSet;
		internal override string debugName => "Mesh Filter";
		internal override string debugDescription => "Adds the mesh to the scene geometry";
		public override void Debug() {

			base.Debug();

			//ImGui.BeginListBox("Verticies");

			bool changed = false;

			for (int i = 0; i < mesh.vertices.Length; i ++) {
				if (ImGui.TreeNode("Vertex #" + i) == true) {

					

					System.Numerics.Vector2 sysVec2;
					System.Numerics.Vector3 sysVec3;

					sysVec3 = mesh.vertices[i].position;
					changed |= ImGui.SliderFloat3("Position", ref sysVec3, -2f, 2f);
					mesh.vertices[i].position = sysVec3;

					sysVec2 = mesh.vertices[i].uv;
					changed |= ImGui.SliderFloat2("UV", ref sysVec2, -2f, 2f);
					mesh.vertices[i].uv = sysVec2;

				}
			}

			if (changed == true) {
				SetMesh(mesh);
			}

			//ImGui.EndListBox();

			System.Numerics.Vector3 vec3 = new System.Numerics.Vector3(vertexColSet.x, vertexColSet.y, vertexColSet.z);
			ImGui.ColorPicker3("Vertex Col", ref vec3);
			vertexColSet = new Vector4(vec3.X, vec3.Y, vec3.Z, 1f);

			if (ImGui.Button("Update") == true) {

				for (int i = 0; i < mesh.vertices.Length; i ++) {
					mesh.vertices[i].color = vertexColSet;
				}
				SetMesh(mesh);

			}

		}
		

	}
}
