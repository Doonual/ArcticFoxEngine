using CoolClassLibrary;
using SharpDX.RawInput;

namespace ArcticFoxEngine.Input {
	public static class InputManager {

		static List<InputBinding> inputs;

		internal static void InitInput() {
			inputs = new List<InputBinding>();
		}

		internal static void AddBinding(InputBinding binding) {
			inputs.Add(binding);
		}
		internal static void RemoveBinding(InputBinding binding) {
			inputs.Remove(binding);
		}

		internal static void NextFrame() {
			for (int i = 0; i < inputs.Count; i ++) {
				inputs[i].BufferValues();
			}
			for (int i = 0; i < inputs.Count; i++) {
				inputs[i].NextFrame_();
			}

		}

	}
}
