using CoolClassLibrary;
using SharpDX.Windows;
using System.Windows.Forms;

namespace ArcticFoxEngine {
	public static class MainWindow {


		internal static RenderForm form;
		public static int width;
		public static int height;

		public static void CreateWindow(int width, int height, string title = "Arctic Fox Engine", string iconPath = ".res/icon.ico") {
			MainWindow.width = width;
			MainWindow.height = height;

			form = new RenderForm(title) {
				Width = width,
				Height = height,
				Icon = new Icon(iconPath),
				FormBorderStyle = FormBorderStyle.None,
			};
			form.BackColor = new Color(0, 0, 0);

			form.Width = width;
			form.Height = height;
			form.Location = new Point(0, 0);
			Log.Success("Created window");

		} 


		public static float aspectRatio {
			get {
				return (float)width / height;
			}
		}

	}
}
