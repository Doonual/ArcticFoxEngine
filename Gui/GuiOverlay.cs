namespace ArcticFoxEngine.Gui {
	internal abstract class GuiOverlay {

		internal abstract string name { get; }
		internal bool open = false;
		internal abstract void Render();

	}
}
