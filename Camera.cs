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
		internal Matrix transformationMatrix;

		public enum Projection {

			Perspective,
			Orthographic

		}

		public Camera(Vector3 position, float fov, Projection projectionType) {

			if (projectionType == Projection.Perspective) {
				projectionMatrix = CreateProjectionMatrix(fov);
				transformationMatrix = CreateTransformationMatrix(position, Quaternion.Identity, Vector3.one);
			}

		}

		public static Matrix CreateProjectionMatrix(float fov) {
			float nearPlane = 0.3f;
			float farPlane = 100f;

			Matrix mat = new Matrix(
				1f / (Screen.aspectRatio * MathF.Tan(fov / 2f)), 0f, 0f, 0f,
				0f, 1f / MathF.Tan(fov / 2f), 0f, 0f,
				0f, 0f, -(farPlane + nearPlane) / (farPlane - nearPlane), -(2 * farPlane * nearPlane) / (farPlane - nearPlane),
				0f, 0f, -1f, 0f
			);

			mat = Matrix.PerspectiveFovRH(fov, Screen.aspectRatio, nearPlane, farPlane);
			return mat;
		}

		public static Matrix CreateTransformationMatrix(Vector3 position, Quaternion rotation, Vector3 scale) {
			Matrix mat = Matrix.Transformation(Vector3.zero, Quaternion.Identity, scale, Vector3.zero, rotation, position);
			//mat.Invert();
			return mat;
		}

		float angle = 0f;
		public void Test() {

			angle += 0.003f;
			//angle %= 2f * MathF.PI;
			Vector3 pos = new Vector3(MathF.Cos(angle), 0f, MathF.Sin(angle)) * 5f;

			float fov = 110f;

			projectionMatrix = CreateProjectionMatrix(fov * MathF.PI / 180);
			transformationMatrix = CreateTransformationMatrix(Vector3.back * 2.5f, Quaternion.RotationYawPitchRoll(angle, angle * 0.7f, 0f), Vector3.one);

		}


	}

	

}
