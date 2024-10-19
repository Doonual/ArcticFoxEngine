
namespace ArcticFoxEngine.Debug {

	public abstract class GuiWindow {

		public abstract string name { get; }
		public bool open = false;
		public abstract void Render();

	}
}
