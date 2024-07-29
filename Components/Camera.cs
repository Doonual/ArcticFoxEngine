using SharpDX;
using RectangleF = SharpDX.RectangleF;
using ImGuiNET;
using ArcticFoxEngine.Backend;

namespace ArcticFoxEngine {
	public class Camera : Component {

		public int renderWidth { 
			get {
				return Screen.width;
			}
		}
		public int renderHeight {
			get {
				return Screen.height;
			}
		}

		public float viewportWidth;
		public float viewportHeight;

		public float fov = 100f;

		public float nearPlane = 0.01f;
		public float farPlane = 100f;

		public ProjectionType projectionType = ProjectionType.Perspective;

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

		public Camera() {

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

			Matrix cameraTransform = gameObject.transform.transformationMatrix.Invert();
			return cameraTransform * projectionMatrix;
			
		}

		internal override string debugName => "Camera";
		internal override string debugDescription => "Renders the scene from the camera's point of view";

		public override void Debug() {

			base.Debug();
			ImGui.SliderFloat("Fov", ref fov, 45f, 130f);
			ImGui.SliderFloat("Near plane", ref nearPlane, 0f, 1f);
			ImGui.SliderFloat("Far plane", ref farPlane, 50f, 1000f);

		}

		public override void OnRender() {
			GPU_Render.ExecuteMainRender(this, gameObject.scene.mainGeometry);
		}

		internal void WriteCameraInfoBuffer(ConstBuffer<RenderInfo> buffer) {

			RenderInfo info = new RenderInfo();
			info.projectionMatrix = projectionMatrix;
			info.screenWidth = renderWidth;
			info.screenHeight = renderHeight;
			info.aspectRatio = (float)renderWidth / renderHeight;
			buffer.Write(new RenderInfo[] { info }, 0);

		}


	}

}
