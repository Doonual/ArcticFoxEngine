using ArcticFoxEngine.Input.Devices;
using SharpDX.DirectInput;

namespace ArcticFoxEngine.Input {
	public static class InputManager {

		static List<InputBinding> inputs;
		static List<Action> inputDeviceUpdateActions;
		internal static DirectInput directInput;

		internal static void Init() {
			inputs = new List<InputBinding>();
			inputDeviceUpdateActions = new List<Action>();
			directInput = new DirectInput();

			MouseInputDevice.Init();

		}

		internal static void AddBinding(InputBinding binding) {
			inputs.Add(binding);
		}
		internal static void RemoveBinding(InputBinding binding) {
			inputs.Remove(binding);
		}
		internal static void AddInputDevice(Action updateAction) {
			inputDeviceUpdateActions.Add(updateAction);
		}

		internal static void GetInputDeviceUpdates() {
			for (int i = 0; i < inputDeviceUpdateActions.Count; i ++) {
				inputDeviceUpdateActions[i]();
			}
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
