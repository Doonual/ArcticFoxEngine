using ArcticFoxEngine.Rendering;
using CoolClassLibrary;
using ImGuiNET;
using SharpDX;
using RectangleF = SharpDX.RectangleF;

namespace ArcticFoxEngine.Nodes {



	public class Camera : Node {

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

		public Matrix fullMatrix {
			get {
				return CalculateFullMatrix();
			}
		}
		public Matrix projectionMatrix {
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

			return projectionMatrix;

		}
		internal Matrix CalculateFullMatrix() {

			Matrix projectionMatrix = CalculateProjectionMatrix();
			Matrix cameraTransform = transform.worldMatrix.Invert();
			return cameraTransform * projectionMatrix;

		}

		/// <summary>
		/// Calculates the camera space position of a point in world space
		/// </summary>
		/// <param name="worldSpacePos">The point in world space</param>
		/// <returns>The point in camera space</returns>
		public Vector3 WorldToCamera(Vector3 worldSpacePos) {

			Matrix projectionResult = Matrix.Translation(worldSpacePos) * CalculateFullMatrix();
			Vector3 cameraSpacePos = new Vector3(projectionResult.M30, projectionResult.M31, projectionResult.M32) / projectionResult.M33;
			return cameraSpacePos;

		}
		/// <summary>
		/// Calculates the screen space position of a point in camera space
		/// </summary>
		/// <param name="cameraSpacePos">The camera space position</param>
		/// <returns>The screen space position</returns>
		public Vector2 CameraToScreen(Vector3 cameraSpacePos) {

			Vector2 screenSpacePos = new Vector2(cameraSpacePos.x, -cameraSpacePos.y) / 2f;
			screenSpacePos *= new Vector2(Screen.width, Screen.height);
			screenSpacePos += new Vector2(Screen.width, Screen.height) / 2f;

			return screenSpacePos;

		}
		/// <summary>
		/// Calculates the screen space position of a point in world space
		/// </summary>
		/// <param name="worldSpacePos"></param>
		/// <returns>The screen space position</returns>
		public Vector2 WorldToScreen(Vector3 worldSpacePos) {

			Vector3 cameraSpacePos = WorldToCamera(worldSpacePos);
			Vector2 screenSpacePos = CameraToScreen(cameraSpacePos);
			return screenSpacePos;

		}

		/// <summary>
		/// Calculates the camera space position of a pixel coordinate. The camera space position ranges from -1 to 1 on the x, y, z axis
		/// </summary>
		/// <param name="screenSpacePos"></param>
		/// <returns>Camera space position. Ranging from -1 to 1 on the x, y, z axis</returns>
		public Vector3 ScreenToCamera(Vector2 screenSpacePos, float depth = 1f) {

			Vector2 normalizedScreenPos = screenSpacePos - new Vector2(Screen.width, Screen.height) / 2f;
			normalizedScreenPos = 2f * normalizedScreenPos / new Vector2(Screen.width, Screen.height);
			Vector3 cameraPos = new Vector3(normalizedScreenPos.x, -normalizedScreenPos.y, depth);
			return cameraPos;

		}
		/// <summary>
		/// Calculates the world space position of a point in camera space.
		/// </summary>
		/// <param name="cameraSpacePos">The position of the point in camera space</param>
		/// <returns>The world space position of the point</returns>
		public Vector3 CameraToWorld(Vector3 cameraSpacePos) {
			
			Vector4 lhs = new Vector4(cameraSpacePos.x, cameraSpacePos.y, cameraSpacePos.z, 1f);
			Vector3 worldSpacePos = transform.worldMatrix * (projectionMatrix.Invert() * lhs);
			return worldSpacePos;
		}
		/// <summary>
		/// Calculates the World space position of a point in camera space
		/// </summary>
		/// <param name="screenSpacePos"></param>
		/// <returns></returns>
		public Vector3 ScreenToWorld(Vector2 screenSpacePos, float depth = 1f) {
			return CameraToWorld(ScreenToCamera(screenSpacePos, depth));
		}

		public override void DrawInspector() {

			ImGui.TextWrapped("Renders the scene from the camera's point of view");
			
			ImGuiExtras.ComboEnum(ref projectionType);

			if (projectionType == ProjectionType.Perspective) {
				ImGui.SliderFloat("Fov", ref fov, 45f, 130f);
				ImGui.SliderFloat("Near plane", ref nearPlane, 0f, 1f, null, ImGuiSliderFlags.Logarithmic);
				ImGui.SliderFloat("Far plane", ref farPlane, 20f, 6000f, null, ImGuiSliderFlags.Logarithmic);
			}
			else {
				ImGui.SliderFloat("Zoom", ref zoom, 0.5f, 100f, null, ImGuiSliderFlags.Logarithmic);
			}

			ImGui.Text("Projection matrix");
			Vector4 row;

			row = projectionMatrix.Row0;
			ImGui.InputFloat4("Row 0", ref row);
			row = projectionMatrix.Row1;
			ImGui.InputFloat4("Row 1", ref row);
			row = projectionMatrix.Row2;
			ImGui.InputFloat4("Row 2", ref row);
			row = projectionMatrix.Row3;
			ImGui.InputFloat4("Row 3", ref row);


		}

		internal void UpdateCameraInfoBuffer(ConstBuffer<ProjectionInfo> projectionInfo) {

			ProjectionInfo info = new ProjectionInfo();
			info.projectionMatrix = fullMatrix;
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
