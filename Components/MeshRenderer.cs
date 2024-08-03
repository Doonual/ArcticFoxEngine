using ArcticFoxEngine.Backend;
using CoolClassLibrary;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Components {
	public class MeshRenderer : Component {

		public Mesh mesh { get; private set; }
		

		public MeshRenderer() {
			mesh = null;
		}

		public void SetMesh(Mesh mesh) {
			
			// If the object has a mesh already loaded, delete it
			if (this.mesh != null && enabled == true) {
				UnloadMesh();
			}

			this.mesh = mesh;
			if (this.mesh != null && enabled == true) {
				LoadMesh();
			}
			

		}

		private void LoadMesh() {
			if (mesh == null) { return; }
			bool meshAdded = gameObject.scene.mainGeometry.AddMesh(this);
			if (meshAdded == true) {
				Enable();
			}
			else {
				Disable();
			}
		}
		private void UnloadMesh() {
			gameObject.scene.mainGeometry.RemoveMesh(this);
			Disable();
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

			System.Numerics.Vector3 vec3 = new System.Numerics.Vector3(vertexColSet.x, vertexColSet.y, vertexColSet.z);
			ImGui.ColorPicker3("Vertex Col", ref vec3);
			vertexColSet = new Vector4(vec3.X, vec3.Y, vec3.Z, 1f);

			if (ImGui.Button("Update") == true) {

				for (int i = 0; i < mesh.vertices.Length; i ++) {
					mesh.vertices[i].Color = vertexColSet;
				}
				SetMesh(mesh);

			}

		}
		

	}
}
