
namespace ArcticFoxEngine.Gui {

	public abstract class GuiWindow {

		public string[] menuGroups;

		public abstract string name { get; }
		public bool open = false;

		// When these two are not negative, the window's position is updated in GuiManager
		internal Vector2 setWindowPos;
		internal Vector2 setWindowSize;

		

		public GuiWindow(params string[] menuGroups) {
			this.menuGroups = menuGroups;
			setWindowPos = new Vector2(-1f, -1f);
			setWindowSize = new Vector2(-1f, -1f);
		}
		public void SetPosition(Vector2 windowPosition) {
			setWindowPos = windowPosition;
		}
		public void SetSize(Vector2 windowSize) {
			setWindowSize = windowSize;
		}

		public abstract void Render();

	}
}
