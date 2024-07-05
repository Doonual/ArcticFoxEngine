using SharpDX;
using RectangleF = SharpDX.RectangleF;

namespace ArcticFoxEngine {
	public class Camera {

		public float viewportWidth;
		public float viewportHeight;

		public Vector3 position;
		public Quaternion rotation;

		public float fov;

		public float nearPlane = 0.3f;
		public float farPlane = 100f;

		public ProjectionType projectionType;

		internal Matrix projectionMatrix {
			get {
				return CalculateProjectionMatrix();
			}
		}

		internal ViewportF viewport {
			get {
				ViewportF viewport = new ViewportF();
				viewport.Width = viewportWidth;
				viewport.Height = viewportHeight;
				viewport.MaxDepth = 1f;
				return viewport;
			}
		}
		internal RectangleF scissorRect {
			get {
				RectangleF scissorRect = new RectangleF();
				scissorRect.Right = viewportWidth;
				scissorRect.Bottom = viewportHeight;
				return scissorRect;
			}
		}

		public enum ProjectionType {
			Perspective,
			Orthographic
		}

		public Camera(Vector3 position, Quaternion rotation, float fov, ProjectionType projectionType) {

			this.position = position;
			this.rotation = rotation;
			this.fov = fov;
			this.projectionType = projectionType;
			viewportWidth = Screen.width;
			viewportHeight = Screen.height;

		}

		private Matrix CalculateProjectionMatrix() {

			Matrix projectionMatrix = new Matrix();
			if (projectionType == ProjectionType.Perspective) {
				projectionMatrix = Matrix.PerspectiveFovLH(fov * MathF.PI / 180f, Screen.aspectRatio, nearPlane, farPlane);
			}
			if (projectionType == ProjectionType.Orthographic) {
				projectionMatrix = Matrix.OrthoRH(Screen.aspectRatio, 1f, nearPlane, farPlane);
			}

			Matrix cameraTransform = Matrix.Transformation(Vector3.zero, Quaternion.Identity, Vector3.one, Vector3.zero, rotation, position);
			cameraTransform.Invert();

			return cameraTransform * projectionMatrix;
			
		}

	}

	

}
