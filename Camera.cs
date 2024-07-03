using CoolClassLibrary;
using SharpDX;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine {
	public class Camera {

		internal Matrix projectionMatrix;

		public enum Projection {

			Perspective,
			Orthographic

		}

		public Camera(Vector3 position, float fov, Projection projectionType) {

			if (projectionType == Projection.Perspective) {
				projectionMatrix = CreateProjectionMatrix(position, fov);
			}

			Log.Info("Camera matrix: " + projectionMatrix);

		}

		public static Matrix CreateProjectionMatrix(Vector3 position, float fov) {

			Matrix mat = Matrix.PerspectiveFovRH(fov, Screen.aspectRatio, 0.03f, 1000f);
			mat = Matrix.LookAtRH(position, Vector3.zero, Vector3.up);
			
			for (int i = 0; i < 16; i ++) {
				mat[i] = (float)i / 16f;
			}
			

			return mat;

		}

		float add = 0f;
		public void Test() {

			for (int i = 0; i < 16; i++) {
				projectionMatrix[i] = ((float)i / 16f + add) % 1f;
			}

			add += 0.1f / 60f;

		}


	}
}
