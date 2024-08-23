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
			Quad,
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
					new Vertex() {Position=new Vector3(-0.5f, -0.5f, -0.5f), Color = new Color(0, 0, 0)},
					new Vertex() {Position=new Vector3(0.5f, -0.5f, -0.5f), Color = new Color(255, 0, 0)},
					new Vertex() {Position=new Vector3(-0.5f, 0.5f, -0.5f), Color = new Color(0, 255, 0)},
					new Vertex() {Position=new Vector3(0.5f, 0.5f, -0.5f), Color = new Color(255, 255, 0)},
					new Vertex() {Position=new Vector3(-0.5f, -0.5f, 0.5f), Color = new Color(0, 0, 255)},
					new Vertex() {Position=new Vector3(0.5f, -0.5f, 0.5f), Color = new Color(255, 0, 255)},
					new Vertex() {Position=new Vector3(-0.5f, 0.5f, 0.5f), Color = new Color(0, 255, 255)},
					new Vertex() {Position=new Vector3(0.5f, 0.5f, 0.5f), Color = new Color(255, 255, 255)},
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

				case Primitive.Quad:

				vertexData = new Vertex[] {
					new Vertex() {Position = new Vector3(-1f, 0f, -1f), Color = new Color(0, 0, 0)},
					new Vertex() {Position = new Vector3(1f, 0f, -1f), Color = new Color(255, 0, 0)},
					new Vertex() {Position = new Vector3(-1f, 0f, 1f), Color = new Color(0, 255, 0)},
					new Vertex() {Position = new Vector3(1f, 0f, 1f), Color = new Color(255, 255, 0)},
				};
				indexData = new int[] {
					0, 1, 2,
					2, 1, 3
				};

				break;


			}

			return new Mesh(vertexData, indexData);

		}

	}
}
