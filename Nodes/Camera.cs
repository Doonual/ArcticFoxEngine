using SharpDX;
using RectangleF = SharpDX.RectangleF;
using ImGuiNET;
using ArcticFoxEngine.Backend;

namespace ArcticFoxEngine {

	using ArcticFoxEngine.Backend.Render;
	using CoolClassLibrary;

	public class Camera : Node {

		internal override string debugName => "Camera";
		internal override string debugDescription => "Renders the scene from the camera's point of view";
		internal override string nodeIconPath => ".res/NodeIcons/Camera.png";

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

		public Camera() : base() {

			viewportWidth = Screen.width;
			viewportHeight = Screen.height;

			SetName("Camera");
			Enable();
		}

		private Matrix CalculateProjectionMatrix() {

			Matrix projectionMatrix = new Matrix();
			if (projectionType == ProjectionType.Perspective) {
				projectionMatrix = Matrix.PerspectiveFovLH(fov * MathF.PI / 180f, Screen.aspectRatio, nearPlane, farPlane);
			}
			if (projectionType == ProjectionType.Orthographic) {
				projectionMatrix = Matrix.OrthoLH(zoom * Screen.aspectRatio, zoom, nearPlane, farPlane);
			}

			Matrix cameraTransform = Transform.CalculateFromNode(this).Invert();
			return cameraTransform * projectionMatrix;
			
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

		internal void UpdateCameraInfoBuffer(ConstBuffer<RenderInfo> renderInfo) {

			RenderInfo info = new RenderInfo();
			info.projectionMatrix = projectionMatrix;
			info.screenWidth = renderWidth;
			info.screenHeight = renderHeight;
			info.aspectRatio = (float)renderWidth / renderHeight;
			renderInfo.Write(new RenderInfo[] { info }, 0);

		}
		public override void Render() {
			GPU_Render.Render(Graphics.renderTargets[Graphics.frameIndex], Graphics.rtvHeap, Graphics.dsvHeap, this);
		}


	}

}
