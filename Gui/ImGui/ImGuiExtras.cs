using ImGuiNET;

namespace ArcticFoxEngine {
	public static class ImGuiExtras {

		static int currentIndex = 0;

		public static bool ComboEnum<T>(ref T enumVar) where T : struct, Enum {

			bool changed = false;

			string[] enumNames = Enum.GetNames(enumVar.GetType());
			string name = Enum.GetName(enumVar);

			for (int i = 0; i < enumNames.Length; i++) {
				if (name == enumNames[i]) {
					currentIndex = i;
					break;
				}
			}

			changed |= ImGui.Combo(enumVar.GetType().Name, ref currentIndex, enumNames, enumNames.Length);
			enumVar = Enum.GetValues<T>()[currentIndex];

			return changed;

		}
		public static void ItemWidthForText(string text) {
			ImGui.SetNextItemWidth(-(ImGui.CalcTextSize(text).x + ImGui.GetStyle().ItemInnerSpacing.x));
		}

	}
}
