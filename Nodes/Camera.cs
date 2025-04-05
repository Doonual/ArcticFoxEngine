using ArcticFoxEngine.Render;
using CoolClassLibrary;
using ImGuiNET;
using SharpDX;
using SharpDX.Direct3D12;
using RectangleF = SharpDX.RectangleF;

namespace ArcticFoxEngine.Nodes {



	public class Camera : Node {

		internal override string nodeIconPath => ".res/NodeIcons/Camera.png";
		internal override string nodeIconPath32 => ".res/NodeIcons/Camera32.png";

		public Texture renderTexture;
		internal DescriptorHeap rtvDescriptorHeap;
		public Texture depthTexture;
		internal DescriptorHeap dsvDescriptorHeap;
		internal ConstBuffer<ProjectionInfo> projectionInfo;

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
				viewport.Width = renderTexture.width;
				viewport.Height = renderTexture.height;
				viewport.MaxDepth = 1f;
				return viewport;
			}
		}
		internal RectangleF scissorRect {
			get {
				RectangleF scissorRect = new RectangleF();
				scissorRect.Right = renderTexture.width;
				scissorRect.Bottom = renderTexture.height;
				return scissorRect;
			}
		}

		public enum ProjectionType {
			Perspective,
			Orthographic
		}

		public Camera() {
			name = "Camera";

			renderTexture = new Texture(MainWindow.width, MainWindow.height, format: Format.R8G8B8A8_UNorm, flags: ResourceFlags.AllowRenderTarget);
			renderTexture.name = "Camera Render Texture";
			depthTexture = new Texture(renderTexture.width, renderTexture.height, format: Format.D32_Float, flags: ResourceFlags.AllowDepthStencil);
			depthTexture.name = "Camera Depth Texture";
			projectionInfo = new ConstBuffer<ProjectionInfo>(1);

			// Create render target view descriptor heap and add the render texture to it
			DescriptorHeapDescription rtvHeapDesc = new DescriptorHeapDescription() {
				DescriptorCount = 1,
				Flags = DescriptorHeapFlags.None,
				Type = DescriptorHeapType.RenderTargetView
			};
			rtvDescriptorHeap = Graphics.device.CreateDescriptorHeap(rtvHeapDesc);
			Graphics.device.CreateRenderTargetView(renderTexture.resource, null, rtvDescriptorHeap.CPUDescriptorHandleForHeapStart);

			// Create depth stencil view descriptor heap and add the depth stencil texture to it
			DescriptorHeapDescription dsvHeapDesc = new DescriptorHeapDescription() {
				DescriptorCount = 1,
				Flags = DescriptorHeapFlags.None,
				Type = DescriptorHeapType.DepthStencilView,
			};
			dsvDescriptorHeap = Graphics.device.CreateDescriptorHeap(dsvHeapDesc);
			Graphics.device.CreateDepthStencilView(depthTexture.resource, null, dsvDescriptorHeap.CPUDescriptorHandleForHeapStart);


		}

		internal Matrix CalculateProjectionMatrix() {

			Matrix projectionMatrix = new Matrix();
			if (projectionType == ProjectionType.Perspective) {
				projectionMatrix = Matrix.PerspectiveFovLH(fov * MathF.PI / 180f, MainWindow.aspectRatio, nearPlane, farPlane);
			}
			if (projectionType == ProjectionType.Orthographic) {
				projectionMatrix = Matrix.OrthoLH(zoom * MainWindow.aspectRatio, zoom, nearPlane, farPlane);
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
			screenSpacePos *= new Vector2(MainWindow.width, MainWindow.height);
			screenSpacePos += new Vector2(MainWindow.width, MainWindow.height) / 2f;

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

			Vector2 normalizedScreenPos = screenSpacePos - new Vector2(MainWindow.width, MainWindow.height) / 2f;
			normalizedScreenPos = 2f * normalizedScreenPos / new Vector2(MainWindow.width, MainWindow.height);
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

		public override void Update() {

			ProjectionInfo info = new ProjectionInfo();
			info.projectionMatrix = fullMatrix;
			info.screenWidth = renderTexture.width;
			info.screenHeight = renderTexture.height;
			info.aspectRatio = (float)renderTexture.width / renderTexture.height;
			projectionInfo.Write(new ProjectionInfo[] { info }, 0);

		}

		public override void Render() {

			RenderEngine.RenderScene(this);
			Graphics.Blit(renderTexture, Graphics.GetActiveResource());

		}

		public override void Dispose() {
			renderTexture.Dispose();
			rtvDescriptorHeap.Dispose();
			depthTexture.Dispose();
			dsvDescriptorHeap.Dispose();
			projectionInfo.Dispose();
		}


	}

}
