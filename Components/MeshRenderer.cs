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
		private Mesh loadedMesh;

		internal int vbStartIndex;
		internal int ibStartIndex;
		internal int obStartIndex;
		// May eventually move these into GeometryInfo

		Vector4 vertexColSet;

		public MeshRenderer() {
			loadedMesh = null;
		}


		public void SetMesh(Mesh mesh) {
			
			// If the object has a mesh already loaded, delete it
			if (loadedMesh != null) {
				UnloadMesh();
			}

			this.mesh = mesh;
			if (this.mesh == null) {
				this.mesh = Mesh.CreatePrimitive(Mesh.Primitive.Cube);
			}

			if (enabled == true) {
				LoadMesh();
			}
			

		}

		private void UnloadMesh() {
			gameObject.scene.mainGeometry.RemoveMesh(this);
			loadedMesh = null;
		}
		private void LoadMesh() {
			if (mesh == null) { return; }
			(vbStartIndex, ibStartIndex, obStartIndex) = gameObject.scene.mainGeometry.AddMesh(this);
			loadedMesh = mesh;
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

		public override void Debug() {

			base.Debug();

			System.Numerics.Vector3 vec3 = new System.Numerics.Vector3(vertexColSet.x, vertexColSet.y, vertexColSet.z);
			ImGui.ColorPicker3("Vertex Col", ref vec3);
			vertexColSet = new Vector4(vec3.X, vec3.Y, vec3.Z, 1f);

			ImGui.Text("Vertex Buffer Start Index: " + vbStartIndex);
			ImGui.Text("Index Buffer Start Index: " + ibStartIndex);
			ImGui.Text("Object Buffer Start Index: " + obStartIndex);

			if (ImGui.Button("Update") == true) {

				for (int i = 0; i < mesh.vertices.Length; i ++) {
					mesh.vertices[i].Color = vertexColSet;
				}
				SetMesh(mesh);

			}

		}


		internal override string debugName => "Mesh Filter";
		internal override string debugDescription => "Adds the mesh to the scene geometry";


	}
}
