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

		#region Directions

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


		#endregion


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

		static Texture transformIcon;
		
		internal static IntPtr transformIconPtr {
			get {
				if (transformIcon == null) {
					transformIcon = Texture.Cache.FindOrLoad(".res/NodeIcons/Transform.png");
					transformIconPtrCache = RenderImGui.RegisterTexture(transformIcon);
				}
				return transformIconPtrCache;
			}
		}
		private static IntPtr transformIconPtrCache;

		private bool normaliseRotation = false;
		public void DrawTransformGui() {

			



			// Transform options
			System.Numerics.Vector3 systemPos = (System.Numerics.Vector3)localPosition;
			System.Numerics.Vector4 systemRot = new System.Numerics.Vector4(localRotation.x, localRotation.y, localRotation.z, localRotation.w);
			System.Numerics.Vector3 systemScale = (System.Numerics.Vector3)localScale;

			ImGuiExtras.ItemWidthForText("Rotation"); ImGui.DragFloat3("Position", ref systemPos, 0.01f);
			ImGuiExtras.ItemWidthForText("Rotation"); ImGui.DragFloat4("Rotation", ref systemRot, 0.01f);
			ImGuiExtras.ItemWidthForText("Rotation"); ImGui.DragFloat3("Scale", ref systemScale, 0.01f);

			localPosition = new Vector3(systemPos);
			localRotation = new Quaternion(systemRot.X, systemRot.Y, systemRot.Z, systemRot.W);

			ImGui.Checkbox("Normalise rotation", ref normaliseRotation);
			if (normaliseRotation == true) {
				localRotation.Normalize();
			}

			localScale = new Vector3(systemScale);

			if (ImGui.Button("Reset") == true) {
				Reset();
			}




		}

		




	}

}
