using ArcticFoxEngine.Rendering;
using CoolClassLibrary;
using ImGuiNET;
using SharpDX;
using RectangleF = SharpDX.RectangleF;

namespace ArcticFoxEngine.Nodes {



	public class Camera : Node {

		internal override string description => "Renders the scene from the camera's point of view";
		internal override string nodeIconPath => ".res/NodeIcons/Camera.png";
		internal override string nodeIconPath32 => ".res/NodeIcons/Camera32.png";

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
		public float farPlane = 1000f;

		public float zoom = 1f;

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
			name = "Camera";

			viewportWidth = Screen.width;
			viewportHeight = Screen.height;

			Enable();
		}

		internal Matrix CalculateProjectionMatrix() {

			Matrix projectionMatrix = new Matrix();
			if (projectionType == ProjectionType.Perspective) {
				projectionMatrix = Matrix.PerspectiveFovLH(fov * MathF.PI / 180f, Screen.aspectRatio, nearPlane, farPlane);
			}
			if (projectionType == ProjectionType.Orthographic) {
				projectionMatrix = Matrix.OrthoLH(zoom * Screen.aspectRatio, zoom, nearPlane, farPlane);
			}

			Matrix cameraTransform = transform.worldMatrix.Invert();
			return cameraTransform * projectionMatrix;

		}

		public Vector3 WorldToCamera(Vector3 worldSpacePos) {

			Matrix projectionResult = Matrix.Translation(worldSpacePos) * CalculateProjectionMatrix();
			Vector3 cameraSpacePos = new Vector3(projectionResult.M30, projectionResult.M31, projectionResult.M32) / projectionResult.M33;
			return cameraSpacePos;

		}
		public Vector2 CameraToScreen(Vector3 cameraSpacePos) {

			Vector2 screenSpacePos = new Vector2(cameraSpacePos.x, cameraSpacePos.y);
			screenSpacePos *= new Vector2(0.9f, -0.5f);
			screenSpacePos *= new Vector2(Screen.height, Screen.height);
			screenSpacePos += new Vector2(Screen.width / 2f, Screen.height / 2f);
			return screenSpacePos;

		}
		public Vector2 WorldToScreen(Vector3 worldSpacePos) {

			Vector3 cameraSpacePos = WorldToCamera(worldSpacePos);
			Vector2 screenSpacePos = CameraToScreen(cameraSpacePos);
			return screenSpacePos;

		}

		public override void Debug() {

			base.Debug();
			ImGuiExtras.ComboEnum(ref projectionType);

			if (projectionType == ProjectionType.Perspective) {
				ImGui.SliderFloat("Fov", ref fov, 45f, 130f);
				ImGui.SliderFloat("Near plane", ref nearPlane, 0f, 1f, null, ImGuiSliderFlags.Logarithmic);
				ImGui.SliderFloat("Far plane", ref farPlane, 20f, 6000f, null, ImGuiSliderFlags.Logarithmic);
			}
			else {
				ImGui.SliderFloat("Zoom", ref zoom, 0.5f, 100f, null, ImGuiSliderFlags.Logarithmic);
			}


		}

		internal void UpdateCameraInfoBuffer(ConstBuffer<ProjectionInfo> projectionInfo) {

			ProjectionInfo info = new ProjectionInfo();
			info.projectionMatrix = projectionMatrix;
			info.screenWidth = renderWidth;
			info.screenHeight = renderHeight;
			info.aspectRatio = (float)renderWidth / renderHeight;
			projectionInfo.Write(new ProjectionInfo[] { info }, 0);

		}
		public override void Render() {
			Rendering.Rendering.RenderScene(Graphics.renderTargets[Graphics.frameIndex], Graphics.rtvHeap, Graphics.dsvHeap, this);
		}



	}

}
