
namespace ArcticFoxEngine.Debug {

	internal abstract class DebugWindow {

		internal abstract string name { get; }
		internal bool open = false;
		internal abstract void Render();

	}
}
