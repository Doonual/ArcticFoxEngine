using ArcticFoxEngine.Debug;
using ArcticFoxEngine.ImGuiIntegration;
using CoolClassLibrary;
using ImGuiNET;
using SixLabors.ImageSharp;

namespace ArcticFoxEngine.Nodes {
	public abstract class Node {

		public static Node rootNode;
		internal static Node nextParentNode = null;

		private bool disposed = true;

		public string name = "";
		internal virtual string description => "";
		internal virtual string nodeIconPath => ".res/NodeIcons/EmptyNode.png";
		internal virtual string nodeIconPath32 => "";
		internal IntPtr nodeIconId;
		internal IntPtr nodeIconId32;


		protected bool globalEnabled;
		public bool enabled { get; private set; }

		// Object Hierarchy
		public Node parentNode { get; private set; } // When null, indicates it is in the scene root
		private List<Node> childNodes;

		public Transform transform;

		protected Node() {

			disposed = false;
			transform = new Transform(this);

			childNodes = new List<Node>();
			SetParent(nextParentNode);

			nodeIconId = NodeIconBank.LoadIcon(nodeIconPath);
			if (nodeIconPath32 == "") {
				nodeIconId32 = NodeIconBank.LoadIcon(nodeIconPath);
			}
			else {
				nodeIconId32 = NodeIconBank.LoadIcon(nodeIconPath32);
			}
			name = GetType().Name;

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

				Node currentNode = parentNode;
				while (currentNode != null) {
					currentNode.OnTreeChanged();
					currentNode = currentNode.parentNode;
				}

			}

		}

		public T CreateChild<T>() where T : Node, new() {

			nextParentNode = this;
			T newChild = new T();
			return newChild;

		}
		public T CreateChild<T>(string name) where T : Node, new() {

			nextParentNode = this;
			T newChild = new T();
			newChild.name = name;
			return newChild;

		}
		public int GetChildCount() {
			return childNodes.Count;
		}
		public Node GetChild(int index) {
			return childNodes[index];
		}
		public T GetChild<T>() where T : Node {
			for (int i = 0; i < childNodes.Count; i++) {
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

		/// <summary>
		/// Searches the Node tree upwards for the 1st instance of this node. Search starts on this node, then to this node's parent, then to this node's grandparent
		/// </summary>
		/// <typeparam name="T">The type of node to search for</typeparam>
		/// <param name="includeSiblings">Whether to include the siblings of each node in the search</param>
		/// <returns>The first instance of that node. null if it was not found</returns>
		public T SearchNodeTreeUp<T>(bool includeSiblings) where T : Node {

			if (GetType() == typeof(T)) { return (T)this; }
			if (parentNode != null) {
				if (includeSiblings == true) {
					for (int i = 0; i < parentNode.childNodes.Count; i ++) {
						if (parentNode.childNodes[i].GetType() == typeof(T)) {
							return (T)parentNode.childNodes[i];
						}
					}
				}
				return parentNode.SearchNodeTreeUp<T>(includeSiblings);
			}
			return null;

		}

		/// <summary>
		/// Searches the Node tree upwards for all the instances of this node. Search starts on this node, then to this node's parent, then to this node's grandparent
		/// </summary>
		/// <typeparam name="T">The type of node to search for</typeparam>
		/// <param name="includeSiblings">Wheather to include the siblings of each node in the search</param>
		/// <returns>A list with all the instances of that node</returns>
		public List<T> SearchNodeTreeUpAll<T>(bool includeSiblings) where T : Node {

			List<T> nodeList = new List<T>();

			if (GetType() == typeof(T)) {
				nodeList.Add((T)this);
			}
			if (parentNode != null) {
				if (includeSiblings == true) {
					for (int i = 0; i < parentNode.childNodes.Count; i++) {
						if (parentNode.childNodes[i].GetType() == typeof(T) && parentNode.childNodes[i] != this) {
							nodeList.Add((T)parentNode.childNodes[i]);
						}
					}
				}
				nodeList.AddRange(parentNode.SearchNodeTreeUpAll<T>(includeSiblings));
			}
			return nodeList;

		}

		/// <summary>
		/// Searches the Node tree for the 1st instance of this node. Search results will include this node.
		/// </summary>
		/// <typeparam name="T">The type of node to search for</typeparam>
		/// <returns>The first instance of that node. null if the node wasn't found</returns>
		public T SearchNodeTreeDown<T>() where T : Node {

			if (typeof(T) == GetType()) {
				return (T)this;
			}
			for (int i = 0; i < childNodes.Count; i++) {
				T childNodeSearch = childNodes[i].SearchNodeTreeDown<T>();
				if (childNodeSearch != null) {
					return childNodeSearch;
				}
			}
			return null;

		}
		
		/// <summary>
		/// Searches the Node tree for the 1st instance of this node up to the maximum depth. Search results will include this node.
		/// </summary>
		/// <typeparam name="T">The type of node to search for</typeparam>
		/// <param name="maxDepth">The maximum depth to search. 0 - this node only. 1 - this node's children. 2 - this node's grandchildren</param>
		/// <returns>The first instance of that node. null if the node wasn't found</returns>
		public T SearchNodeTreeDown<T>(int maxDepth) where T : Node {

			if (typeof(T) == GetType()) {
				return (T)this;
			}
			if (maxDepth <= 0) { return null; }
			for (int i = 0; i < childNodes.Count; i++) {
				T childNodeSearch = childNodes[i].SearchNodeTreeDown<T>(maxDepth - 1);
				if (childNodeSearch != null) {
					return childNodeSearch;
				}
			}
			return null;

		}
		
		/// <summary>
		/// Searches the Node tree for all the instances of this node. Search results will include this node.
		/// </summary>
		/// <typeparam name="T">The type of node to search for</typeparam>
		/// <returns>A list containing all the found nodes</returns>
		public List<T> SearchNodeTreeDownAll<T>() where T : Node {

			List<T> foundNodes = new List<T>();
			if (typeof(T) == GetType()) {
				foundNodes.Add((T)this);
			}
			for (int i = 0; i < childNodes.Count; i++) {
				foundNodes.AddRange(childNodes[i].SearchNodeTreeDownAll<T>());
			}
			return foundNodes;

		}

		/// <summary>
		/// Searches the Node tree for all the instances of this node up to the maximum depth. Search results will include this node.
		/// </summary>
		/// <typeparam name="T">The type of node to search for</typeparam>
		/// <param name="maxDepth">The maximum depth to search. 0 - this node only. 1 - this node's children. 2 - this node's grandchildren</param>
		/// <returns>A list containing all the found nodes</returns>
		public List<T> SearchNodeTreeDownAll<T>(int maxDepth) where T : Node {

			List<T> foundNodes = new List<T>();
			if (typeof(T) == GetType()) {
				foundNodes.Add((T)this);
			}
			if (maxDepth <= 0) { return foundNodes; }
			for (int i = 0; i < childNodes.Count; i++) {
				foundNodes.AddRange(childNodes[i].SearchNodeTreeDownAll<T>(maxDepth - 1));
			}
			return foundNodes;

		}

		/// <summary>
		/// Searches the Node tree for the 1st instance of this node. Search results will include this node.
		/// </summary>
		/// <param name="name">The name of the node to search for</param>
		/// <returns>The first instance of that node.  null if the node wasn't found</returns>
		public Node SearchNodeTreeDown(string name) {

			if (this.name == name) {
				return this;
			}
			for (int i = 0; i < childNodes.Count; i++) {
				Node childNodeSearch = childNodes[i].SearchNodeTreeDown(name);
				if (childNodeSearch != null) {
					return childNodeSearch;
				}
			}
			return null;

		}

		/// <summary>
		/// Searches the Node tree for the 1st instance of this node up to the maximum depth. Search results will include this node.
		/// </summary>
		/// <param name="name">The name of the node to search for</param>
		/// <param name="maxDepth">The maximum depth to search. 0 - this node only. 1 - this node's children. 2 - this node's grandchildren</param>
		/// <returns>The first instance of that node. null if the node wasn't found</returns>
		public Node SearchNodeTreeDown(string name, int maxDepth) {

			if (this.name == name) {
				return this;
			}
			if (maxDepth <= 0) { return null; }
			for (int i = 0; i < childNodes.Count; i++) {
				Node childNodeSearch = childNodes[i].SearchNodeTreeDown(name, maxDepth - 1);
				if (childNodeSearch != null) {
					return childNodeSearch;
				}
			}
			return null;

		}
		
		/// <summary>
		/// Searches the Node tree for all the instances of this node. Search results will include this node.
		/// </summary>
		/// <param name="name">The name of the node to search for</param>
		/// <returns>A list containing all the found nodes</returns>
		public List<Node> SearchNodeTreeDownAll(string name) {

			List<Node> foundNodes = new List<Node>();
			if (this.name == name) {
				foundNodes.Add(this);
			}
			for (int i = 0; i < childNodes.Count; i++) {
				foundNodes.AddRange(childNodes[i].SearchNodeTreeDownAll(name));
			}
			return foundNodes;

		}
		
		/// <summary>
		/// Searches the Node tree for all the instances of this node up to the maximum depth. Search results will include this node.
		/// </summary>
		/// <param name="name">The name of the node to search for</param>
		/// <param name="maxDepth">The maximum depth to search. 0 - this node only. 1 - this node's children. 2 - this node's grandchildren</param>
		/// <returns>A list containing all the found nodes</returns>
		public List<Node> SearchNodeTreeDownAll(string name, int maxDepth) {

			List<Node> foundNodes = new List<Node>();
			if (this.name == name) {
				foundNodes.Add(this);
			}
			if (maxDepth <= 0) { return foundNodes; }
			for (int i = 0; i < childNodes.Count; i++) {
				foundNodes.AddRange(childNodes[i].SearchNodeTreeDownAll(name, maxDepth - 1));
			}
			return foundNodes;

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

			for (int i = 0; i < childNodes.Count; i++) {
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
		
		
		~Node() {
			DisposeEvent();
		}
		internal void DisposeEvent() {
			if (disposed == false) {
				for (int i = 0; i < childNodes.Count(); i++) {
					childNodes[i].DisposeEvent();
				}
				Disable();
				Dispose();
				disposed = true;
			}
		}

		public virtual void OnEnable() { }
		public virtual void OnDisable() { }
		public virtual void OnTreeChanged() { }
		public virtual void Update() { }
		public virtual void Render() { }
		public virtual void Debug() { }
		
		
		public virtual void Dispose() { }

		internal void DebugEvent(bool callingFromRoot) {

			ImGuiWindowFlags flags = ImGuiWindowFlags.MenuBar;
			ImGui.BeginChild((uint)(GetHashCode() + " parameters").GetHashCode(), new Vector2(0f, 300f), true, flags);


			// Default node open states
			if (callingFromRoot == true && prevCallingFromRoot == false) { inspectorNodeOpen = true; }
			if (callingFromRoot == false && prevCallingFromRoot == true) { inspectorNodeOpen = false; }
			prevCallingFromRoot = callingFromRoot;


			ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, -1f));

			ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.3f, 0.3f, 0.3f, 0.0f));
			ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.3f, 0.3f, 0.3f, 0.3f));
			ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.3f, 0.3f, 0.3f, 0.5f));
			float cursorPosY = ImGui.GetCursorPosY();

			ImGui.BeginMenuBar();

			// Node icon
			ImGui.ImageButton(name + "image button", nodeIconId, new Vector2(16f, 16f));

			ImGui.SameLine();

			// Name
			ImGui.PushStyleVar(ImGuiStyleVar.ButtonTextAlign, new Vector2(0f, 0.5f));
			ImGui.Button(name, new Vector2(-1f, 16f + 6f));

			ImGui.EndMenuBar();

			ImGui.PopStyleVar(1);
			ImGui.PopStyleVar(1);
			ImGui.PopStyleColor(3);

			// Description
			if (description != "") {
				ImGui.TextWrapped(description);
			}



			// Transform and Debug
			//ImGui.SeparatorText("Transform");
			if (ImGui.TreeNode("Transform") == true) {
				transform.Debug();
				ImGui.TreePop();
			}
			ImGui.SeparatorText("Node Options");
			Debug();


			ImGui.EndChild();

			if (callingFromRoot == true) {
				for (int i = 0; i < childNodes.Count; i++) {
					ImGui.PushID(childNodes[i].GetHashCode() + " debug event");
					ImGui.NewLine();
					childNodes[i].DebugEvent(false);
					ImGui.PopID();
				}
			}

			

			ImGui.NewLine();


		}
		internal Node DebugNodeTree(bool rootNode) {

			nodeOpen |= rootNode;

			ImGui.TableNextRow();
			ImGui.TableNextColumn();

			float childLinesX = ImGui.GetCursorScreenPos().X + 9f;

			bool prevGlobalEnabled = globalEnabled;

			ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, -1f));

			ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.3f, 0.3f, 0.3f, 0.0f));
			ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.3f, 0.3f, 0.3f, 0.3f));
			ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.3f, 0.3f, 0.3f, 0.5f));



			float cursorPosY = ImGui.GetCursorPosY();

			// Dropdown / Bullet
			if (childNodes.Count() > 0) {
				// Dropdown
				ImGui.SetCursorPosY(cursorPosY + 1f);
				nodeOpen ^= ImGui.ArrowButton(name + "drop down", nodeOpen ? ImGuiDir.Down : ImGuiDir.Right);
				ImGui.SameLine();
			}
			else {
				// Bullet
				ImGui.SetCursorPosY(cursorPosY + 4f);
				ImGui.Bullet();
			}


			ImGui.SetCursorPosY(cursorPosY);
			Vector2 buttonStartPos = ImGui.GetCursorPos();

			Node selectedNode = null;

			// Cover button
			bool nodeSelectedColourChange = false;
			if (DebugScene.selectedNode == this) {
				nodeSelectedColourChange = true;
				ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(66f / 255f, 150f / 255f, 250f / 255, 102f / 255f));
				ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(66f / 255f, 150f / 255f, 250f / 255, 102f / 255f));
				ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(66f / 255f, 150f / 255f, 250f / 255, 102f / 255f));
			}


			if (ImGui.Button("", new Vector2(-1f, 16f + 6f)) == true) {
				selectedNode = this;
			}
			if (nodeSelectedColourChange == true) {
				ImGui.PopStyleColor(3);
			}

			// Icon button
			ImGui.SetCursorPos(buttonStartPos);
			ImGui.ImageButton(name + "image button", nodeIconId, new Vector2(16f, 16f));

			ImGui.SameLine();

			// Name button
			ImGui.SetCursorPosY(cursorPosY);
			ImGui.PushStyleVar(ImGuiStyleVar.ButtonTextAlign, new Vector2(0f, 0.5f));

			string actualTypeString = "";
			if (name != GetType().Name) {
				actualTypeString = " (" + GetType().Name + ")";
			}
			ImGui.Button(name + actualTypeString, new Vector2(-1f, 16f + 6f));

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

				for (int i = 0; i < childNodes.Count; i++) {

					float childLinesCurrentY = ImGui.GetCursorScreenPos().Y + 11f + 26f;
					ImGui.GetWindowDrawList().AddLine(new Vector2(childLinesX, childLinesCurrentY), new Vector2(childLinesX + indentSize / 2f, childLinesCurrentY), ImGui.ColorConvertFloat4ToU32(new Vector4(0.5f, 0.5f, 0.5f, 1f)));

					ImGui.PushID(childNodes[i].GetHashCode() + " debug node tree");
					Node childSelectedNode = childNodes[i].DebugNodeTree(false);
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


		// Debug Gizmos
		public virtual void DebugContextMenu() {
;
			if (ImGui.MenuItem("Edit") == true) {
				DebugNodeGizmos.OpenFloatingNodeEdit(this);
			}
			ImGui.MenuItem("Reveal in node tree");

		}

		bool nodeOpen = false;
		
	}
}
