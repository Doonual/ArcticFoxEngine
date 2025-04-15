namespace ArcticFoxEngine {
	public class Mesh {

		public Vertex[] vertices;
		public int[] indices;

		public enum Primitive {
			Cube,
			Quad,
			Cylinder,
		}

		/// <summary>
		/// Creates a new mesh
		/// </summary>
		/// <param name="vertices">The verticies of the mesh</param>
		/// <param name="indices">The indices of the mesh</param>
		public Mesh(Vertex[] vertices, int[] indices) {
			this.vertices = vertices;
			this.indices = indices;
		}

		/// <summary>
		/// Creates a mesh of the specified primitive
		/// </summary>
		/// <param name="primitive">The type of primitive to create</param>
		/// <returns>The mesh of the specified primitive</returns>
		public static Mesh CreatePrimitive(Primitive primitive) {


			Vertex[] vertexData = null;
			int[] indexData = null;

			switch (primitive) {

				case Primitive.Cube:

				vertexData = new Vertex[] {

					new Vertex() {position=new Vector3(-0.5f, -0.5f, -0.5f), color = new Color(255.0f, 255.0f, 255.0f), uv = new Vector2(0f, 1f), normal = new Vector3(0f, 0f, -1f)},
					new Vertex() {position=new Vector3(0.5f, -0.5f, -0.5f), color = new Color(255.0f, 255.0f, 255.0f), uv = new Vector2(1f, 1f), normal = new Vector3(0f, 0f, -1f)},
					new Vertex() {position=new Vector3(-0.5f, 0.5f, -0.5f), color = new Color(255.0f, 255.0f, 255.0f), uv = new Vector2(0f, 0f), normal = new Vector3(0f, 0f, -1f)},
					new Vertex() {position=new Vector3(0.5f, 0.5f, -0.5f), color = new Color(255.0f, 255.0f, 255.0f), uv = new Vector2(1f, 0f), normal = new Vector3(0f, 0f, -1f)},

					new Vertex() {position=new Vector3(-0.5f, -0.5f, -0.5f), color = new Color(255.0f, 255.0f, 255.0f), uv = new Vector2(0f, 1f), normal = new Vector3(-1f, 0f, 0f)},
					new Vertex() {position=new Vector3(-0.5f, 0.5f, -0.5f), color = new Color(255.0f, 255.0f, 255.0f), uv = new Vector2(1f, 1f), normal = new Vector3(-1f, 0f, 0f)},
					new Vertex() {position=new Vector3(-0.5f, -0.5f, 0.5f), color = new Color(255.0f, 255.0f, 255.0f), uv = new Vector2(0f, 0f), normal = new Vector3(-1f, 0f, 0f)},
					new Vertex() {position=new Vector3(-0.5f, 0.5f, 0.5f), color = new Color(255.0f, 255.0f, 255.0f), uv = new Vector2(1f, 0f), normal = new Vector3(-1f, 0f, 0f)},

					new Vertex() {position=new Vector3(-0.5f, -0.5f, -0.5f), color = new Color(255.0f, 255.0f, 255.0f), uv = new Vector2(0f, 1f), normal = new Vector3(0f, -1f, 0f)},
					new Vertex() {position=new Vector3(-0.5f, -0.5f, 0.5f), color = new Color(255.0f, 255.0f, 255.0f), uv = new Vector2(1f, 1f), normal = new Vector3(0f, -1f, 0f)},
					new Vertex() {position=new Vector3(0.5f, -0.5f, -0.5f), color = new Color(255.0f, 255.0f, 255.0f), uv = new Vector2(0f, 0f), normal = new Vector3(0f, -1f, 0f)},
					new Vertex() {position=new Vector3(0.5f, -0.5f, 0.5f), color = new Color(255.0f, 255.0f, 255.0f), uv = new Vector2(1f, 0f), normal = new Vector3(0f, -1f, 0f)},

					new Vertex() {position=new Vector3(0.5f, 0.5f, 0.5f), color = new Color(255.0f, 255.0f, 255.0f), uv = new Vector2(0f, 0f), normal = new Vector3(0f, 0f, 1f)},
					new Vertex() {position=new Vector3(-0.5f, 0.5f, 0.5f), color = new Color(255.0f, 255.0f, 255.0f), uv = new Vector2(1f, 0f), normal = new Vector3(0f, 0f, 1f)},
					new Vertex() {position=new Vector3(0.5f, -0.5f, 0.5f), color = new Color(255.0f, 255.0f, 255.0f), uv = new Vector2(0f, 1f), normal = new Vector3(0f, 0f, 1f)},
					new Vertex() {position=new Vector3(-0.5f, -0.5f, 0.5f), color = new Color(255.0f, 255.0f, 255.0f), uv = new Vector2(1f, 1f), normal = new Vector3(0f, 0f, 1f)},

					new Vertex() {position=new Vector3(0.5f, 0.5f, 0.5f), color = new Color(255.0f, 255.0f, 255.0f), uv = new Vector2(0f, 0f), normal = new Vector3(1f, 0f, 0f)},
					new Vertex() {position=new Vector3(0.5f, -0.5f, 0.5f), color = new Color(255.0f, 255.0f, 255.0f), uv = new Vector2(1f, 0f), normal = new Vector3(1f, 0f, 0f)},
					new Vertex() {position=new Vector3(0.5f, 0.5f, -0.5f), color = new Color(255.0f, 255.0f, 255.0f), uv = new Vector2(0f, 1f), normal = new Vector3(1f, 0f, 0f)},
					new Vertex() {position=new Vector3(0.5f, -0.5f, -0.5f), color = new Color(255.0f, 255.0f, 255.0f), uv = new Vector2(1f, 1f), normal = new Vector3(1f, 0f, 0f)},

					new Vertex() {position=new Vector3(0.5f, 0.5f, 0.5f), color = new Color(255.0f, 255.0f, 255.0f), uv = new Vector2(0f, 0f), normal = new Vector3(0f, 1f, 0f)},
					new Vertex() {position=new Vector3(0.5f, 0.5f, -0.5f), color = new Color(255.0f, 255.0f, 255.0f), uv = new Vector2(1f, 0f), normal = new Vector3(0f, 1f, 0f)},
					new Vertex() {position=new Vector3(-0.5f, 0.5f, 0.5f), color = new Color(255.0f, 255.0f, 255.0f), uv = new Vector2(0f, 1f), normal = new Vector3(0f, 1f, 0f)},
					new Vertex() {position=new Vector3(-0.5f, 0.5f, -0.5f), color = new Color(255.0f, 255.0f, 255.0f), uv = new Vector2(1f, 1f), normal = new Vector3(0f, 1f, 0f)},

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
					new Vertex() {position = new Vector3(-1f, 0f, -1f), color = new Color(255, 255, 255), uv = new Vector2(0f, 0f), normal = new Vector3(0f, 1f, 0f)},
					new Vertex() {position = new Vector3(1f, 0f, -1f), color = new Color(255, 255, 255), uv = new Vector2(1f, 0f), normal = new Vector3(0f, 1f, 0f)},
					new Vertex() {position = new Vector3(-1f, 0f, 1f), color = new Color(255, 255, 255), uv = new Vector2(0f, 1f), normal = new Vector3(0f, 1f, 0f)},
					new Vertex() {position = new Vector3(1f, 0f, 1f), color = new Color(255, 255, 255), uv = new Vector2(1f, 1f), normal = new Vector3(0f, 1f, 0f)},
				};
				indexData = new int[] {
					0, 1, 2,
					2, 1, 3
				};

				break;
				case Primitive.Cylinder:

				int segments = 16;

				List<Vertex> bottomStripVerts = new List<Vertex>();
				List<Vertex> topStripVerts = new List<Vertex>();

				List<Vertex> topCircleVerts = new List<Vertex>();
				List<Vertex> bottomCircleVerts = new List<Vertex>();


				Vertex topCircleCenter = new Vertex();
				topCircleCenter.position = new Vector3(0f, 0.5f, 0f);
				topCircleCenter.normal = new Vector3(0f, 1f, 0f);
				topCircleCenter.color = Color.white;
				topCircleCenter.uv = Vector2.one / 2f;

				Vertex bottomCircleCenter = new Vertex();
				bottomCircleCenter.position = new Vector3(0f, -0.5f, 0f);
				bottomCircleCenter.normal = new Vector3(0f, -1f, 0f);
				bottomCircleCenter.color = Color.white;
				bottomCircleCenter.uv = Vector2.one / 2f;

				for (int i = 0; i < segments + 1; i ++) {

					Vector2 circleVec = Vector2.Angle((float)i / segments * MathF.PI * 2f, 0.5f);

					Vertex bottomStripVert = new Vertex();
					bottomStripVert.position = new Vector3(circleVec.x, -0.5f, circleVec.y);
					bottomStripVert.normal = new Vector3(circleVec.x, 0f, circleVec.y);
					bottomStripVert.color = Color.white;
					bottomStripVert.uv = new Vector2(4f * (float)i / segments, 0f);
					bottomStripVerts.Add(bottomStripVert);

					Vertex bottomCircleVert = new Vertex();
					bottomCircleVert.position = new Vector3(circleVec.x, -0.5f, circleVec.y);
					bottomCircleVert.normal = new Vector3(0f, -1f, 0f);
					bottomCircleVert.color = Color.white;
					bottomCircleVert.uv = circleVec + Vector2.one / 2f;
					bottomCircleVerts.Add(bottomCircleVert);



					Vertex topStripVert = new Vertex();
					topStripVert.position = new Vector3(circleVec.x, 0.5f, circleVec.y);
					topStripVert.normal = new Vector3(circleVec.x, 0f, circleVec.y);
					topStripVert.color = Color.white;
					topStripVert.uv = new Vector2(4f * (float)i / segments, 1f);
					topStripVerts.Add(topStripVert);

					Vertex topCircleVert = new Vertex();
					topCircleVert.position = new Vector3(circleVec.x, 0.5f, circleVec.y);
					topCircleVert.normal = new Vector3(0f, 1f, 0f);
					topCircleVert.color = Color.white;
					topCircleVert.uv = circleVec + Vector2.one / 2;
					topCircleVerts.Add(topCircleVert);

				}

				List<int> stripTris = new List<int>();
				for (int i = 0; i < segments; i ++) {

					stripTris.Add(i);
					stripTris.Add(i + (segments + 1));
					stripTris.Add((i + 1) % (segments + 1));

					stripTris.Add((i + 1) % (segments + 1));
					stripTris.Add(i + (segments + 1));
					stripTris.Add((i + 1) % (segments + 1) + (segments + 1));
					
				}

				List<int> topCircleTris = new List<int>();
				int circleIndexStart = (segments + 1) * 2 + 1;
				for (int i = 0; i < segments; i ++) {
					topCircleTris.Add(i + circleIndexStart);
					topCircleTris.Add(circleIndexStart - 1);
					topCircleTris.Add((i + 1) % segments + circleIndexStart);
				}

				List<int> bottomCircleTris = new List<int>();
				circleIndexStart = (segments + 1) * 2 + 1 + segments + 2;
				for (int i = 0; i < segments; i++) {
					bottomCircleTris.Add(i + circleIndexStart);
					bottomCircleTris.Add((i + 1) % segments + circleIndexStart);
					bottomCircleTris.Add(circleIndexStart - 1);
				}


				List<Vertex> totalVerts = new List<Vertex>();
				totalVerts.AddRange(bottomStripVerts);
				totalVerts.AddRange(topStripVerts);
				totalVerts.Add(topCircleCenter);
				totalVerts.AddRange(topCircleVerts);
				totalVerts.Add(bottomCircleCenter);
				totalVerts.AddRange(bottomCircleVerts);


				List<int> totalTris = new List<int>();
				totalTris.AddRange(stripTris);
				totalTris.AddRange(topCircleTris);
				totalTris.AddRange(bottomCircleTris);

				vertexData = totalVerts.ToArray();
				indexData = totalTris.ToArray();

				break;

			}

			return new Mesh(vertexData, indexData);

		}

		public void SetColor(Color color) {
			for (int i = 0; i < vertices.Length; i ++) {
				vertices[i].color = color;
			}
		}

	}
}
