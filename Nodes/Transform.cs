using ArcticFoxEngine.Gui;
using ArcticFoxEngine.ImGuiIntegration;
using ImGuiNET;
using System.Security.Cryptography.Xml;
using System.Xml.Linq;

namespace ArcticFoxEngine.Nodes {

	public class Transform {

		//internal override string description => "Gives an object a position, rotation and scale.";
		//internal override string nodeIconPath => ".res/NodeIcons/Transform.png";
		//internal override string nodeIconPath32 => ".res/NodeIcons/Transform32.png";

		Node containedNode;

		public Vector3 localPosition;
		public Quaternion localRotation;
		public Vector3 localScale;
		public Matrix localMatrix {
			get {
				return Matrix.Transformation(localPosition, localRotation, localScale);
			}
		}

		public Vector3 worldPosition {
			get {
				return worldMatrix.Row3;
			}
		}
		public Matrix worldMatrix {
			get {

				if (containedNode.parentNode == null) {
					return localMatrix;
				}
				return localMatrix * containedNode.parentNode.transform.worldMatrix;

			}
		}
		public Quaternion worldRotation {
			get {

				if (containedNode.parentNode == null) {
					return localRotation;
				}
				return localRotation * containedNode.parentNode.transform.worldRotation;

			}
		}

		public Vector3 Right {
			get {
				return Matrix.RotationQuaternion(worldRotation) * Vector3.Right;
			}
		}
		public Vector3 Left {
			get {
				return Matrix.RotationQuaternion(worldRotation) * Vector3.Left;
			}
		}
		public Vector3 Up {
			get {
				return Matrix.RotationQuaternion(worldRotation) * Vector3.Up;
			}
		}
		public Vector3 Down {
			get {
				return Matrix.RotationQuaternion(worldRotation) * Vector3.Down;
			}
		}
		public Vector3 Forward {
			get {
				return Matrix.RotationQuaternion(worldRotation) * Vector3.Forward;
			}
		}
		public Vector3 Back {
			get {
				return Matrix.RotationQuaternion(worldRotation) * Vector3.Back;
			}
		}


		public Transform(Node containedNode) {

			localPosition = Vector3.Zero;
			localRotation = Quaternion.Identity;
			localScale = Vector3.One;

			this.containedNode = containedNode;

		}
		public void Reset() {
			localPosition = Vector3.Zero;
			localRotation = Quaternion.Identity;
			localScale = Vector3.One;
		}
		public bool IsIdentity() {

			if (localPosition.SqrLength() > 0.0001f) { return false; }
			if (localRotation.IsIdentity == false) { return false; }
			if (MathF.Abs(localScale.x - 1f) > 0.0001f) { return false; }
			if (MathF.Abs(localScale.y - 1f) > 0.0001f) { return false; }
			if (MathF.Abs(localScale.z - 1f) > 0.0001f) { return false; }

			return true;

		}

		static Texture transformIcon;
		
		internal static IntPtr transformIconPtr {
			get {
				if (transformIcon == null) {
					transformIcon = new Texture(".res/NodeIcons/Transform.png");
					transformIconPtrCache = RenderImGui.RegisterTexture(transformIcon);
				}
				return transformIconPtrCache;
			}
		}
		private static IntPtr transformIconPtrCache;

		private bool normaliseRotation = false;
		public void DrawTransformGui() {

			



			// Transform options
			Vector4 rotVec4 = new Vector4(localRotation.x, localRotation.y, localRotation.z, localRotation.w);

			ImGuiExtras.ItemWidthForText("Rotation"); ImGui.DragFloat3("Position", ref localPosition, 0.01f);
			ImGuiExtras.ItemWidthForText("Rotation"); ImGui.DragFloat4("Rotation", ref rotVec4, 0.01f);
			ImGuiExtras.ItemWidthForText("Rotation"); ImGui.DragFloat3("Scale", ref localScale, 0.01f);

			localRotation = new Quaternion(rotVec4.x, rotVec4.y, rotVec4.z, rotVec4.w);

			ImGui.Checkbox("Normalise rotation", ref normaliseRotation);
			if (normaliseRotation == true) {
				localRotation.Normalize();
			}

			if (ImGui.Button("Reset") == true) {
				Reset();
			}




		}

		




	}

}
