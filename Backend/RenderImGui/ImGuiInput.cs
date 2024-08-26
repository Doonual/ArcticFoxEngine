namespace ArcticFoxEngine {
	
	using ArcticFoxEngine.Input;
	using ArcticFoxEngine.Input.Bindings;
	using CoolClassLibrary;
	using ImGuiNET;
	using Microsoft.VisualBasic.Devices;
	using SharpDX.DirectInput;
	using SixLabors.ImageSharp.ColorSpaces.Conversion;
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Drawing;
	using System.Security.Policy;
	using System.Threading;
	using System.Windows.Forms;
	using static System.Formats.Asn1.AsnWriter;
	using static System.Net.Mime.MediaTypeNames;
	using static System.Windows.Forms.VisualStyles.VisualStyleElement;

	internal static class ImGuiInput {

		static IntPtr hwnd;
		static ImGuiMouseCursor lastCursor;

		static ButtonBinding shiftKey;
		static float scrollDelta;

		public static void Init(IntPtr hwnd) {
			ImGuiInput.hwnd = hwnd;
			shiftKey = new KeyboardButtonInput(KeyboardButtonInput.KeyboardButton.LeftShift);
			scrollDelta = 0f;
		}


		public static bool Update() {
			var io = ImGui.GetIO();
			UpdateMousePosition(io, hwnd);
			io.AddMouseWheelEvent(0f, scrollDelta);
			scrollDelta = 0f;


			var mouseCursor = io.MouseDrawCursor ? ImGuiMouseCursor.None : ImGui.GetMouseCursor();
			if (mouseCursor != lastCursor) {
				lastCursor = mouseCursor;

				// only required if mouse icon changes
				// while mouse isn't moved otherwise redundent.
				// so practically it's redundent.
				UpdateMouseCursor(io, mouseCursor);
			}

			if (!io.WantCaptureMouse && ImGui.IsAnyMouseDown()) {
				// workaround: where overlay gets stuck in a non-clickable mode forever.
				for (var i = 0; i < 5; i++) {
					io.AddMouseButtonEvent(i, false);
				}
			}

			return io.WantCaptureMouse;
		}


		private static void UpdateMousePosition(ImGuiIOPtr io, IntPtr handleWindow) {
			if (User32.GetCursorPos(out POINT pos) && User32.ScreenToClient(handleWindow, ref pos)) {
				io.AddMousePosEvent(pos.X, pos.Y);
			}
		}


		private static bool UpdateMouseCursor(ImGuiIOPtr io, ImGuiMouseCursor requestedcursor) {
			if ((io.ConfigFlags & ImGuiConfigFlags.NoMouseCursorChange) != 0)
				return false;

			if (requestedcursor == ImGuiMouseCursor.None) {
				User32.SetCursor(IntPtr.Zero);
			}
			else {
				var cursor = SystemCursor.IDC_ARROW;
				switch (requestedcursor) {
					case ImGuiMouseCursor.Arrow: cursor = SystemCursor.IDC_ARROW; break;
					case ImGuiMouseCursor.TextInput: cursor = SystemCursor.IDC_IBEAM; break;
					case ImGuiMouseCursor.ResizeAll: cursor = SystemCursor.IDC_SIZEALL; break;
					case ImGuiMouseCursor.ResizeEW: cursor = SystemCursor.IDC_SIZEWE; break;
					case ImGuiMouseCursor.ResizeNS: cursor = SystemCursor.IDC_SIZENS; break;
					case ImGuiMouseCursor.ResizeNESW: cursor = SystemCursor.IDC_SIZENESW; break;
					case ImGuiMouseCursor.ResizeNWSE: cursor = SystemCursor.IDC_SIZENWSE; break;
					case ImGuiMouseCursor.Hand: cursor = SystemCursor.IDC_HAND; break;
					case ImGuiMouseCursor.NotAllowed: cursor = SystemCursor.IDC_NO; break;
				}

				User32.SetCursor(User32.LoadCursor(IntPtr.Zero, cursor));
			}

			return true;
		}

		internal static void UpdateKeyboard(KeyboardUpdate keyboardUpdate) {

			ImGuiIOPtr io = ImGui.GetIO();
			io.AddKeyEvent(MapDirectInputKey(keyboardUpdate.Key), keyboardUpdate.IsPressed);
			ushort? keyChar = (ushort?)GetKeyChar(keyboardUpdate.Key, shiftKey.GetButton());
			if (keyChar != null && keyboardUpdate.IsPressed == true) {
				io.AddInputCharacterUTF16((ushort)keyChar);
			}

		}
		private static ImGuiKey MapDirectInputKey(Key key) {

			switch (key) {

				case Key.Escape:			return ImGuiKey.Escape;
				case Key.D1:				return ImGuiKey._1;
				case Key.D2:				return ImGuiKey._2;
				case Key.D3:				return ImGuiKey._3;
				case Key.D4:				return ImGuiKey._4;
				case Key.D5:				return ImGuiKey._5;
				case Key.D6:				return ImGuiKey._6;
				case Key.D7:				return ImGuiKey._7;
				case Key.D8:				return ImGuiKey._8;
				case Key.D9:				return ImGuiKey._9;
				case Key.D0:				return ImGuiKey._0;

				case Key.Minus:				return ImGuiKey.Minus;
				case Key.Equals:			return ImGuiKey.Equal;
				case Key.Back:				return ImGuiKey.Backspace;
				case Key.Tab:				return ImGuiKey.Tab;
				case Key.Q:					return ImGuiKey.Q;
				case Key.W:					return ImGuiKey.W;
				case Key.E:					return ImGuiKey.E;
				case Key.R:					return ImGuiKey.R;
				case Key.T:					return ImGuiKey.T;
				case Key.Y:					return ImGuiKey.Y;
				case Key.U:					return ImGuiKey.U;
				case Key.I:					return ImGuiKey.I;
				case Key.O:					return ImGuiKey.O;
				case Key.P:					return ImGuiKey.P;
				case Key.LeftBracket:		return ImGuiKey.LeftBracket;
				case Key.RightBracket:		return ImGuiKey.RightBracket;
				case Key.Return:			return ImGuiKey.Enter;
				case Key.LeftControl:		return ImGuiKey.LeftCtrl;
				case Key.A:					return ImGuiKey.A;
				case Key.S:					return ImGuiKey.S;
				case Key.D:					return ImGuiKey.D;
				case Key.F:					return ImGuiKey.F;
				case Key.G:					return ImGuiKey.G;
				case Key.H:					return ImGuiKey.H;
				case Key.J:					return ImGuiKey.J;
				case Key.K:					return ImGuiKey.K;
				case Key.L:					return ImGuiKey.L;
				case Key.Semicolon:			return ImGuiKey.Semicolon;
				case Key.Apostrophe:		return ImGuiKey.Apostrophe;
				case Key.Grave:				return ImGuiKey.GraveAccent;
				case Key.LeftShift:			return ImGuiKey.LeftShift;
				case Key.Backslash:			return ImGuiKey.Backslash;
				case Key.Z:					return ImGuiKey.Z;
				case Key.X:					return ImGuiKey.X;
				case Key.C:					return ImGuiKey.C;
				case Key.V:					return ImGuiKey.V;
				case Key.B:					return ImGuiKey.B;
				case Key.N:					return ImGuiKey.N;
				case Key.M:					return ImGuiKey.M;
				case Key.Comma:				return ImGuiKey.Comma;
				case Key.Period:			return ImGuiKey.Period;
				case Key.Slash:				return ImGuiKey.Slash;
				case Key.RightShift:		return ImGuiKey.RightShift;
				case Key.Multiply:			return ImGuiKey.KeypadMultiply;
				case Key.LeftAlt:			return ImGuiKey.LeftAlt;
				case Key.Space:				return ImGuiKey.Space;
				case Key.Capital:			return ImGuiKey.CapsLock;
				case Key.F1:				return ImGuiKey.F1;
				case Key.F2:				return ImGuiKey.F2;
				case Key.F3:				return ImGuiKey.F3;
				case Key.F4:				return ImGuiKey.F4;
				case Key.F5:				return ImGuiKey.F5;
				case Key.F6:				return ImGuiKey.F6;
				case Key.F7:				return ImGuiKey.F7;
				case Key.F8:				return ImGuiKey.F8;
				case Key.F9:				return ImGuiKey.F9;
				case Key.F10:				return ImGuiKey.F10;
				case Key.F11:				return ImGuiKey.F11;
				case Key.F12:				return ImGuiKey.F12;

				case Key.NumberLock:		return ImGuiKey.NumLock;
				case Key.ScrollLock:		return ImGuiKey.ScrollLock;
				case Key.NumberPad7:		return ImGuiKey.Keypad7;
				case Key.NumberPad8:		return ImGuiKey.Keypad8;
				case Key.NumberPad9:		return ImGuiKey.Keypad9;
				case Key.Subtract:			return ImGuiKey.KeypadSubtract;
				case Key.NumberPad4:		return ImGuiKey.Keypad4;
				case Key.NumberPad5:		return ImGuiKey.Keypad5;
				case Key.NumberPad6:		return ImGuiKey.Keypad6;
				case Key.Add:				return ImGuiKey.KeypadAdd;
				case Key.NumberPad1:		return ImGuiKey.Keypad1;
				case Key.NumberPad2:		return ImGuiKey.Keypad2;
				case Key.NumberPad3:		return ImGuiKey.Keypad3;
				case Key.NumberPad0:		return ImGuiKey.Keypad0;
				case Key.Decimal:			return ImGuiKey.KeypadDecimal;
				case Key.NumberPadEquals:	return ImGuiKey.KeypadEqual;
				case Key.NumberPadEnter:	return ImGuiKey.KeypadEnter;
				case Key.RightControl:		return ImGuiKey.RightCtrl;
				case Key.Divide:			return ImGuiKey.KeypadDivide;
				case Key.PrintScreen:		return ImGuiKey.PrintScreen;
				case Key.RightAlt:			return ImGuiKey.RightAlt;
				case Key.Pause:				return ImGuiKey.Pause;
				case Key.Home:				return ImGuiKey.Home;
				case Key.Up:				return ImGuiKey.UpArrow;
				case Key.PageUp:			return ImGuiKey.PageUp;
				case Key.Left:				return ImGuiKey.LeftArrow;
				case Key.Right:				return ImGuiKey.RightArrow;
				case Key.End:				return ImGuiKey.End;
				case Key.Down:				return ImGuiKey.DownArrow;
				case Key.PageDown:			return ImGuiKey.PageDown;
				case Key.Insert:			return ImGuiKey.Insert;
				case Key.Delete:			return ImGuiKey.Delete;
				case Key.LeftWindowsKey:	return ImGuiKey.LeftSuper;
				case Key.RightWindowsKey:	return ImGuiKey.RightSuper;

			}

			return ImGuiKey.None;

		}
		private static int? GetKeyChar(Key key, bool capital) {

			switch (key) {

				case Key.D1:				return capital ? '!' : '1';
				case Key.D2:				return capital ? '@' : '2';
				case Key.D3:				return capital ? '#' : '3';
				case Key.D4:				return capital ? '$' : '4';
				case Key.D5:				return capital ? '%' : '5';
				case Key.D6:				return capital ? '^' : '6';
				case Key.D7:				return capital ? '&' : '7';
				case Key.D8:				return capital ? '*' : '8';
				case Key.D9:				return capital ? '(' : '9';
				case Key.D0:				return capital ? ')' : '0';

				case Key.Minus:				return capital ? '-' : '_';
				case Key.Equals:			return capital ? '=' : '+';
				case Key.Back:				return 8;
				case Key.Tab:				return 9;
				case Key.Q:					return capital ? 'Q' : 'q';
				case Key.W:					return capital ? 'W' : 'w';
				case Key.E:					return capital ? 'E' : 'e';
				case Key.R:					return capital ? 'R' : 'r';
				case Key.T:					return capital ? 'T' : 't';
				case Key.Y:					return capital ? 'Y' : 'y';
				case Key.U:					return capital ? 'U' : 'u';
				case Key.I:					return capital ? 'I' : 'i';
				case Key.O:					return capital ? 'O' : 'o';
				case Key.P:					return capital ? 'P' : 'p';
				case Key.LeftBracket:		return capital ? '[' : '{';
				case Key.RightBracket:		return capital ? ']' : '}';

				case Key.A:					return capital ? 'A' : 'a';
				case Key.S:					return capital ? 'S' : 's';
				case Key.D:					return capital ? 'D' : 'd';
				case Key.F:					return capital ? 'F' : 'f';
				case Key.G:					return capital ? 'G' : 'g';
				case Key.H:					return capital ? 'H' : 'h';
				case Key.J:					return capital ? 'J' : 'j';
				case Key.K:					return capital ? 'K' : 'k';
				case Key.L:					return capital ? 'K' : 'l';

				case Key.Semicolon:			return capital ? ':' : ';';
				case Key.Apostrophe:		return capital ? '\"' : '\'';
				case Key.Grave:				return capital ? '~' : '`';
				case Key.Backslash:			return capital ? '|' : '\\';
				case Key.Z:					return capital ? 'Z' : 'z';
				case Key.X:					return capital ? 'X' : 'x';
				case Key.C:					return capital ? 'C' : 'c';
				case Key.V:					return capital ? 'V' : 'v';
				case Key.B:					return capital ? 'B' : 'b';
				case Key.N:					return capital ? 'N' : 'n';
				case Key.M:					return capital ? 'M' : 'm';
				case Key.Comma:				return capital ? ';' : ':';
				case Key.Period:			return capital ? ';' : ':';
				case Key.Slash:				return capital ? '?' : '/';
				case Key.Multiply:			return capital ? '*' : '*';

				case Key.Space:				return capital ? ' ' : ' ';

				case Key.NumberPad7:		return capital ? null : '7';
				case Key.NumberPad8:		return capital ? null : '8';
				case Key.NumberPad9:		return capital ? null : '9';
				case Key.Subtract:			return capital ? '-' : '-';
				case Key.NumberPad4:		return capital ? null : '4';
				case Key.NumberPad5:		return capital ? null : '5';
				case Key.NumberPad6:		return capital ? null : '6';
				case Key.Add:				return capital ? '+' : '+';
				case Key.NumberPad1:		return capital ? null : '1';
				case Key.NumberPad2:		return capital ? null : '2';
				case Key.NumberPad3:		return capital ? null : '3';
				case Key.NumberPad0:		return capital ? null : '0';
				case Key.Decimal:			return capital ? null : '.';
				case Key.NumberPadEquals:	return capital ? '=' : '=';
				case Key.Divide:			return '/';

			}
			
			return null;

		}

		internal static void UpdateMouse(MouseUpdate mouseUpdate) {

			ImGuiIOPtr io = ImGui.GetIO();
			switch (mouseUpdate.RawOffset) {

				case 12: io.AddMouseButtonEvent(0, mouseUpdate.Value == 128); break;
				case 13: io.AddMouseButtonEvent(1, mouseUpdate.Value == 128); break;
				case 14: io.AddMouseButtonEvent(2, mouseUpdate.Value == 128); break;
				case 8: scrollDelta += Math.Sign(mouseUpdate.Value); break;
				
			}

		}


	}
}
