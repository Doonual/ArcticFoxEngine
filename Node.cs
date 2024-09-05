using ArcticFoxEngine.Backend.RenderImGui;
using CoolClassLibrary;
using ImGuiNET;
using SixLabors.ImageSharp.PixelFormats;

namespace ArcticFoxEngine {
	public abstract class Node {

		public static Node rootNode;

		private bool disposed = true;

		internal virtual string debugName => GetType().Name;
		internal virtual string debugDescription => "";
		internal virtual string nodeIconPath => ".res/NodeIcons/EmptyNode.png";
		internal IntPtr nodeIconId;

		// Object properties
		public string name;

		protected bool globalEnabled;
		public bool enabled { get; private set; }

		// Object Hierarchy
		public Node parentNode { get; private set; } // When null, indicates it is in the scene root
		private List<Node> childNodes;

		// Shortcuts
		public Transform transformChild { get { return GetChild<Transform>(); } }
		public Transform transform { get { return GetSibling<Transform>(); } }

		protected Node() {
			disposed = false;
			childNodes = new List<Node>();
			nodeIconId = NodeIconBank.LoadIcon(nodeIconPath);
		}

		public void SetName(string name) {
			this.name = name;
		}

		public static void SetRootNode(Node rootNode) {
			Node.rootNode = rootNode;
			rootNode.globalEnabled = true;
		}
		public void SetParent(Node parentNode) {
			if (this.parentNode != null) {
				parentNode.childNodes.Remove(this);
			}
			
			this.parentNode = parentNode;
			globalEnabled |= enabled;

			if (parentNode != null) {
				parentNode.childNodes.Add(this);
				globalEnabled &= parentNode.globalEnabled;
			}

		}

		public T CreateChild<T>() where T : Node, new() {

			T newChild = new T();
			newChild.SetParent(this);
			return newChild;

		}
		public T CreateChild<T>(string name) where T : Node, new() {

			T newChild = new T();
			newChild.SetParent(this);
			newChild.SetName(name);
			return newChild;

		}
		public int GetChildCount() {
			return childNodes.Count;
		}
		public Node GetChild(int index) {
			return childNodes[index];
		}
		public T GetChild<T>() where T : Node {
			for (int i = 0; i < childNodes.Count; i ++) {
				if (childNodes[i].GetType() == typeof(T)) {
					return (T)childNodes[i];
				}
			}
			return null;
		}
		

		public int GetSiblingCount() {
			if (parentNode == null) { Log.Warn("GetSiblingCount failed, this is the root node"); return 0; }
			return parentNode.GetChildCount();
		}
		public Node GetSibling(int index) {
			if (parentNode == null) { Log.Warn("GetSibling failed, this is the root node"); return null; }
			return parentNode.GetChild(index);
		}
		public T GetSibling<T>() where T : Node {
			Type thisType = GetType();
			Type tType = typeof(T);
			if (typeof(T).GetType() == thisType) {
				return (T)this;
			}
			if (parentNode == null) {
				return null;
			}
			return parentNode.GetChild<T>();
		}


		// Events
		public void Disable() {
			enabled = false;
			CheckEnableEvents();
		}
		public void Enable() {
			enabled = true;
			CheckEnableEvents();
		}
		protected void CheckEnableEvents() {

			bool prevGlobalEnabled = globalEnabled;

			if (enabled == true) {
				if (parentNode != null) {
					globalEnabled = parentNode.globalEnabled;
				}
				else {
					globalEnabled = true;
				}
			}
			if (enabled == false) {
				globalEnabled = false;
			}

			if (globalEnabled != prevGlobalEnabled) {
				if (globalEnabled == true) {
					OnEnable();
				}
				else {
					OnDisable();
				}
			}

			if (globalEnabled != prevGlobalEnabled) {
				for (int i = 0; i < childNodes.Count; i++) {
					childNodes[i].CheckEnableEvents();
				}
			}

		}
		
		internal void UpdateEvent() {
			if (enabled == false) { return; }

			for (int i = 0; i < childNodes.Count; i ++) {
				childNodes[i].UpdateEvent();
			}

			Update();

		}
		internal void RenderEvent() {
			if (enabled == false) { return; }

			for (int i = 0; i < childNodes.Count; i++) {
				childNodes[i].RenderEvent();
			}

			Render();

		}

		bool inspectorNodeOpen = false;
		bool prevCallingFromRoot = false;
		internal void DebugEvent(bool callingFromRoot) {

			if (callingFromRoot == true && prevCallingFromRoot == false) { inspectorNodeOpen = true; }
			if (callingFromRoot == false && prevCallingFromRoot == true) { inspectorNodeOpen = false; }
			prevCallingFromRoot = callingFromRoot;

			ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, -1f));

			ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.3f, 0.3f, 0.3f, 0.0f));
			ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.3f, 0.3f, 0.3f, 0.3f));
			ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.3f, 0.3f, 0.3f, 0.5f));



			float cursorPosY = ImGui.GetCursorPosY();
			ImGui.SetCursorPosY(cursorPosY + 1f);
			inspectorNodeOpen ^= ImGui.ArrowButton(name + "drop down", inspectorNodeOpen ? ImGuiDir.Down : ImGuiDir.Right);
			ImGui.SameLine();


			ImGui.SetCursorPosY(cursorPosY);
			ImGui.ImageButton(name + "image button", nodeIconId, new Vector2(16f, 16f));

			ImGui.SameLine();


			ImGui.SetCursorPosY(cursorPosY);
			ImGui.PushStyleVar(ImGuiStyleVar.ButtonTextAlign, new Vector2(0f, 0.5f));
			Node selectedNode = null;
			if (ImGui.Button(name, new Vector2(-1f, 16f + 6f)) == true) {
				selectedNode = this;
			}
			ImGui.PopStyleVar(2);
			ImGui.PopStyleColor(3);

			if (inspectorNodeOpen == true) {
				ImGui.TreePush(GetHashCode() + " parameters");
				if (debugDescription != "") {
					ImGui.TextWrapped(debugDescription);
				}
				Debug();


				for (int i = 0; i < childNodes.Count; i ++) {
					childNodes[i].DebugEvent(false);
				}

				ImGui.TreePop();
			}



		}

		~Node() {
			DisposeEvent();
		}
		internal void DisposeEvent() {
			if (disposed == false) {
				for (int i = 0; i < childNodes.Count(); i ++) {
					childNodes[i].DisposeEvent();
				}
				Disable();
				Dispose();
				disposed = true;
			}
		}

		public virtual void OnEnable() { }
		public virtual void OnDisable() { }
		public virtual void Update() { }
		public virtual void Render() { }
		public virtual void Debug() { }
		public virtual void Dispose() { }

		bool nodeOpen = false;
		internal Node DebugNodeTree() {

			ImGui.TableNextRow();
			ImGui.TableNextColumn();

			float childLinesX = ImGui.GetCursorScreenPos().X + 9f;

			bool prevGlobalEnabled = globalEnabled;

			ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, -1f));

			ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.3f, 0.3f, 0.3f, 0.0f));
			ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.3f, 0.3f, 0.3f, 0.3f));
			ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.3f, 0.3f, 0.3f, 0.5f));

			

			float cursorPosY = ImGui.GetCursorPosY();
			if (childNodes.Count() > 0) {
				ImGui.SetCursorPosY(cursorPosY + 1f);
				nodeOpen ^= ImGui.ArrowButton(name + "drop down", nodeOpen ? ImGuiDir.Down : ImGuiDir.Right);
				ImGui.SameLine();
			}
			else {
				ImGui.SetCursorPosY(cursorPosY + 4f);
				ImGui.Bullet();
			}


			ImGui.SetCursorPosY(cursorPosY);
			ImGui.ImageButton(name + "image button", nodeIconId, new Vector2(16f, 16f));

			ImGui.SameLine();

			
			ImGui.SetCursorPosY(cursorPosY);
			ImGui.PushStyleVar(ImGuiStyleVar.ButtonTextAlign, new Vector2(0f, 0.5f));
			Node selectedNode = null;
			if (ImGui.Button(name, new Vector2(-1f, 16f + 6f)) == true) {
				selectedNode = this;
			}
			ImGui.PopStyleVar();

			ImGui.TableNextColumn();

			bool enabledCheck = enabled;
			ImGui.PushID(GetHashCode() + " enabled check");
			ImGui.SetCursorPosY(cursorPosY + 1f);
			if (ImGui.Checkbox("", ref enabledCheck) == true) {
				if (enabledCheck == false) {
					Disable();
				}
				else {
					Enable();
				}
			}
			ImGui.SetCursorPosY(cursorPosY);
			ImGui.PopID();




			ImGui.PopStyleColor(3);
			ImGui.PopStyleVar();


			if (nodeOpen == true) {
				ImGui.TreePush(name + " tree push");

				float childLinesStartY = ImGui.GetCursorScreenPos().Y + 21f;
				float indentSize = ImGui.GetStyle().IndentSpacing;

				for (int i = 0; i < childNodes.Count; i ++) {

					float childLinesCurrentY = ImGui.GetCursorScreenPos().Y + 11f + 26f;
					ImGui.GetWindowDrawList().AddLine(new Vector2(childLinesX, childLinesCurrentY), new Vector2(childLinesX + indentSize / 2f, childLinesCurrentY), ImGui.ColorConvertFloat4ToU32(new Vector4(0.5f, 0.5f, 0.5f, 1f)));


					Node childSelectedNode = childNodes[i].DebugNodeTree();
					if (childSelectedNode != null) {
						selectedNode = childSelectedNode;
					}

					
				}

				float childLinesEndY = ImGui.GetCursorScreenPos().Y + 11f;

				ImGui.GetWindowDrawList().AddLine(new Vector2(childLinesX, childLinesStartY), new Vector2(childLinesX, childLinesEndY), ImGui.ColorConvertFloat4ToU32(new Vector4(0.5f, 0.5f, 0.5f, 1f)));

				ImGui.TreePop();
			}

			return selectedNode;

		}

	}
}
