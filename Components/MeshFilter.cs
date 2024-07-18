using CoolClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Components {
	public class MeshFilter : Component {

		public Mesh mesh {
			get;
			private set;
		}

		

		bool meshAdded;

		public MeshFilter() {
			meshAdded = true;
		}
		public MeshFilter(Mesh.Primitive primitive) {
			meshAdded = false;
			SetMesh(Mesh.CreatePrimitive(primitive));
		}

		public void SetMesh(Mesh mesh) {
			this.mesh = mesh;
			meshAdded = false;
		}

		public override void Update() {
			
			if (meshAdded == false) {
				Log.Info("Adding Mesh Data");

				for (int i = 0; i < mesh.vertices.Length; i ++) {
					mesh.vertices[i].Position += gameObject.transform.position;
				}

				gameObject.scene.mainGeometry.AddMeshData(mesh.vertices, mesh.indices);
				meshAdded = true;
			}

		}

		internal override string debugName => "Mesh Filter";
		internal override string debugDescription => "Adds the mesh to the scene geometry";


	}
}
