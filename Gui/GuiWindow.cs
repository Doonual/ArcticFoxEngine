
namespace ArcticFoxEngine.Debug {

	public abstract class GuiWindow {

		public string[] menuGroups;

		public GuiWindow(params string[] menuGroups) {
			this.menuGroups = menuGroups;
		}
		public abstract string name { get; }
		public bool open = false;
		public abstract void Render();

	}
}
