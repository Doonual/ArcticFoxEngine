using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine {
	public class Mesh {

		public Vertex[] vertices;
		public int[] indices;

		public enum Primitive {

			Cube,

		}

		public Mesh(Vertex[] vertices, int[] indices) {
			this.vertices = vertices;
			this.indices = indices;
		}

		public static Mesh CreatePrimitive(Primitive primitive) {

			Vertex[] vertexData = null;
			int[] indexData = null;

			switch (primitive) {

				case Primitive.Cube:

				vertexData = new Vertex[] {
					new Vertex() {Position=new Vector3(-1f, -1f, -1f), Color = new Color(0.0f, 0.0f, 0.0f)},
					new Vertex() {Position=new Vector3(1f, -1f, -1f), Color = new Color(1.0f, 0.0f, 0.0f)},
					new Vertex() {Position=new Vector3(-1f, 1f, -1f), Color = new Color(0.0f, 1.0f, 0.0f)},
					new Vertex() {Position=new Vector3(1f, 1f, -1f), Color = new Color(1.0f, 1.0f, 0.0f)},
					new Vertex() {Position=new Vector3(-1f, -1f, 1f), Color = new Color(0.0f, 0.0f, 1.0f)},
					new Vertex() {Position=new Vector3(1f, -1f, 1f), Color = new Color(1.0f, 0.0f, 1.0f)},
					new Vertex() {Position=new Vector3(-1f, 1f, 1f), Color = new Color(0.0f, 1.0f, 1.0f)},
					new Vertex() {Position=new Vector3(1f, 1f, 1f), Color = new Color(1.0f, 1.0f, 1.0f)},
				};
				indexData = new int[] {
					// Z+ Face
					0, 2, 1,
					2, 3, 1,
					4, 6, 0,
					6, 2, 0,
					5, 7, 4,
					7, 6, 4,
					1, 3, 5,
					3, 7, 5,
					2, 6, 3,
					6, 7, 3,
					0, 1, 5,
					5, 4, 0
				};

				break;


			}

			return new Mesh(vertexData, indexData);

		}

	}
}
