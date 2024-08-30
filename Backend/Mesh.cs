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

					new Vertex() {position=new Vector3(-0.5f, -0.5f, -0.5f), color = new Color(0.0f, 0.0f, 0.0f), uv = new Vector2(0f, 1f)},
					new Vertex() {position=new Vector3(0.5f, -0.5f, -0.5f), color = new Color(1.0f, 0.0f, 0.0f), uv = new Vector2(1f, 1f)},
					new Vertex() {position=new Vector3(-0.5f, 0.5f, -0.5f), color = new Color(0.0f, 1.0f, 0.0f), uv = new Vector2(0f, 0f)},
					new Vertex() {position=new Vector3(0.5f, 0.5f, -0.5f), color = new Color(1.0f, 1.0f, 0.0f), uv = new Vector2(1f, 0f)},

					new Vertex() {position=new Vector3(-0.5f, -0.5f, -0.5f), color = new Color(0.0f, 0.0f, 0.0f), uv = new Vector2(0f, 1f)},
					new Vertex() {position=new Vector3(-0.5f, 0.5f, -0.5f), color = new Color(1.0f, 0.0f, 0.0f), uv = new Vector2(1f, 1f)},
					new Vertex() {position=new Vector3(-0.5f, -0.5f, 0.5f), color = new Color(0.0f, 1.0f, 0.0f), uv = new Vector2(0f, 0f)},
					new Vertex() {position=new Vector3(-0.5f, 0.5f, 0.5f), color = new Color(1.0f, 1.0f, 0.0f), uv = new Vector2(1f, 0f)},

					new Vertex() {position=new Vector3(-0.5f, -0.5f, -0.5f), color = new Color(0.0f, 0.0f, 0.0f), uv = new Vector2(0f, 1f)},
					new Vertex() {position=new Vector3(-0.5f, -0.5f, 0.5f), color = new Color(1.0f, 0.0f, 0.0f), uv = new Vector2(1f, 1f)},
					new Vertex() {position=new Vector3(0.5f, -0.5f, -0.5f), color = new Color(0.0f, 1.0f, 0.0f), uv = new Vector2(0f, 0f)},
					new Vertex() {position=new Vector3(0.5f, -0.5f, 0.5f), color = new Color(1.0f, 1.0f, 0.0f), uv = new Vector2(1f, 0f)},

					new Vertex() {position=new Vector3(0.5f, 0.5f, 0.5f), color = new Color(0.0f, 0.0f, 0.0f), uv = new Vector2(0f, 1f)},
					new Vertex() {position=new Vector3(-0.5f, 0.5f, 0.5f), color = new Color(1.0f, 0.0f, 0.0f), uv = new Vector2(1f, 1f)},
					new Vertex() {position=new Vector3(0.5f, -0.5f, 0.5f), color = new Color(0.0f, 1.0f, 0.0f), uv = new Vector2(0f, 0f)},
					new Vertex() {position=new Vector3(-0.5f, -0.5f, 0.5f), color = new Color(1.0f, 1.0f, 0.0f), uv = new Vector2(1f, 0f)},

					new Vertex() {position=new Vector3(0.5f, 0.5f, 0.5f), color = new Color(0.0f, 0.0f, 0.0f), uv = new Vector2(0f, 1f)},
					new Vertex() {position=new Vector3(0.5f, -0.5f, 0.5f), color = new Color(1.0f, 0.0f, 0.0f), uv = new Vector2(1f, 1f)},
					new Vertex() {position=new Vector3(0.5f, 0.5f, -0.5f), color = new Color(0.0f, 1.0f, 0.0f), uv = new Vector2(0f, 0f)},
					new Vertex() {position=new Vector3(0.5f, -0.5f, -0.5f), color = new Color(1.0f, 1.0f, 0.0f), uv = new Vector2(1f, 0f)},

					new Vertex() {position=new Vector3(0.5f, 0.5f, 0.5f), color = new Color(0.0f, 0.0f, 0.0f), uv = new Vector2(0f, 1f)},
					new Vertex() {position=new Vector3(0.5f, 0.5f, -0.5f), color = new Color(1.0f, 0.0f, 0.0f), uv = new Vector2(1f, 1f)},
					new Vertex() {position=new Vector3(-0.5f, 0.5f, 0.5f), color = new Color(0.0f, 1.0f, 0.0f), uv = new Vector2(0f, 0f)},
					new Vertex() {position=new Vector3(-0.5f, 0.5f, -0.5f), color = new Color(1.0f, 1.0f, 0.0f), uv = new Vector2(1f, 0f)},

				};
				indexData = new int[] {
					// Z+ Face
					0, 2, 1,
					2, 3, 1,

					4, 6, 5,
					6, 7, 5,

					8, 10, 9,
					10, 11, 9,

					12, 13, 14,
					14, 13, 15,

					16, 17, 18,
					18, 17, 19,

					20, 21, 22,
					22, 21, 23,
				};

				break;

				case Primitive.Quad:

				vertexData = new Vertex[] {
					new Vertex() {position = new Vector3(-1f, 0f, -1f), color = new Color(0, 0, 0), uv = new Vector2(0f, 0f)},
					new Vertex() {position = new Vector3(1f, 0f, -1f), color = new Color(255, 0, 0), uv = new Vector2(1f, 0f)},
					new Vertex() {position = new Vector3(-1f, 0f, 1f), color = new Color(0, 255, 0), uv = new Vector2(0f, 1f)},
					new Vertex() {position = new Vector3(1f, 0f, 1f), color = new Color(255, 255, 0), uv = new Vector2(1f, 1f)},
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
