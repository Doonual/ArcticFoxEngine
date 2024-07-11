using SharpDX;
using RectangleF = SharpDX.RectangleF;
using ImGuiNET;

namespace ArcticFoxEngine {
	public class Camera {

		public float viewportWidth;
		public float viewportHeight;

		public Transform transform = new Transform();

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

		public Camera(float fov, ProjectionType projectionType) {

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

			Matrix cameraTransform = transform.transformationMatrix.Invert();
			return cameraTransform * projectionMatrix;
			
		}

		public void Debug() {

			ImGui.Begin("Camera");
			ImGui.SliderFloat("Fov", ref fov, 45f, 130f);
			ImGui.SliderFloat("Near plane", ref nearPlane, 0f, 1f);
			ImGui.SliderFloat("Far plane", ref farPlane, 50f, 1000f);

		}

	}

	

}
