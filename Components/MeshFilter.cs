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

		public void SetMesh(Mesh mesh) {
			this.mesh = mesh;
			meshAdded = false;
		}

		public override void Update() {
			
			if (meshAdded == false) {
				Log.Info("Adding Mesh Data");
				gameObject.scene.mainGeometry.AddMeshData(mesh.vertices, mesh.indices);
				meshAdded = true;
			}

		}

	}
}
