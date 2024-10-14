namespace ArcticFoxEngine.Debug {
	internal abstract class DebugOverlay {

		internal abstract string name { get; }
		internal bool open = false;
		internal abstract void Render();

	}
}
