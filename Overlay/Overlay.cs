#pragma warning disable CS8618

namespace ClickableTransparentOverlay {
	using ArcticFoxEngine;
	using ArcticFoxEngine.Debug;
	using ClickableTransparentOverlay.Win32;
	using SharpDX.Direct3D12;
	using SixLabors.ImageSharp;
	using SixLabors.ImageSharp.PixelFormats;
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Runtime.CompilerServices;
	using System.Threading;
	using System.Threading.Tasks;
	using Vortice.Direct3D;
	using Vortice.Direct3D11;
	using Vortice.DXGI;
	using Vortice.Mathematics;
	using Point = System.Drawing.Point;
	using Size = System.Drawing.Size;

	/// <summary>
	/// A class to create clickable transparent overlay on windows machine.
	/// </summary>
	public static class Overlay {


		public static bool render;




		internal static ImGuiRenderer renderer;
		private static ImGuiInputHandler inputhandler;

		
		private static bool replaceFont = false;
		private static ushort[]? fontCustomGlyphRange;
		private static string fontPathName;
		private static float fontSize;
		private static FontGlyphRangeType fontLanguage;

		private static Dictionary<string, (IntPtr Handle, uint Width, uint Height)> loadedTexturesPtrs;


		#region Constructors


		#endregion

		#region PublicAPI

		/// <summary>
		/// Starts the overlay
		/// </summary>
		/// <returns>A Task that finishes once the overlay window is ready</returns>
		public static void Start() {

			Console.WriteLine("Starting overlay");
			InitializeResources();
			Console.WriteLine("init overlay");
			renderer.Start();
			Console.WriteLine("start overlay");

			render = true;
			
		}


		/// <summary>
		/// Safely Closes the Overlay.
		/// </summary>
		public static void Close() {
			render = false;
		}

		/// <summary>
		/// Replaces the ImGui font with another one.
		/// </summary>
		/// <param name="pathName">pathname to the TTF font file.</param>
		/// <param name="size">font size to load.</param>
		/// <param name="language">supported language by the font.</param>
		/// <returns>true if the font replacement is valid otherwise false.</returns>
		public static bool ReplaceFont(string pathName, int size, FontGlyphRangeType language) {
			if (!File.Exists(pathName)) {
				return false;
			}

			fontPathName = pathName;
			fontSize = size;
			fontLanguage = language;
			replaceFont = true;
			fontCustomGlyphRange = null;
			return true;
		}

		/// <summary>
		/// Replaces the ImGui font with another one.
		/// </summary>
		/// <param name="pathName">pathname to the TTF font file.</param>
		/// <param name="size">font size to load.</param>
		/// <param name="glyphRange">custom glyph range of the font to load. Read <see cref="FontGlyphRangeType"/> for more detail.</param>
		/// <returns>>true if the font replacement is valid otherwise false.</returns>
		public static bool ReplaceFont(string pathName, int size, ushort[] glyphRange) {
			if (!File.Exists(pathName)) {
				return false;
			}

			fontPathName = pathName;
			fontSize = size;
			fontCustomGlyphRange = glyphRange;
			replaceFont = true;
			return true;
		}



		/// <summary>
		/// Adds the image to the Graphic Device as a texture.
		/// Then returns the pointer of the added texture. It also
		/// cache the image internally rather than creating a new texture on every call,
		/// so this function can be called multiple times per frame.
		/// </summary>
		/// <param name="filePath">Path to the image on disk.</param>
		/// <param name="srgb"> a value indicating whether pixel format is srgb or not.</param>
		/// <param name="handle">output pointer to the image in the graphic device.</param>
		/// <param name="width">width of the loaded texture.</param>
		/// <param name="height">height of the loaded texture.</param>
		public static void AddOrGetImagePointer(string filePath, bool srgb, out IntPtr handle, out uint width, out uint height) {
			if (loadedTexturesPtrs.TryGetValue(filePath, out var data)) {
				handle = data.Handle;
				width = data.Width;
				height = data.Height;
			}
			else {
				var configuration = Configuration.Default.Clone();
				configuration.PreferContiguousImageBuffers = true;
				using var image = Image.Load<Rgba32>(configuration, filePath);
				handle = renderer.CreateImageTexture(image, srgb ? SharpDX.DXGI.Format.R8G8B8A8_UNorm_SRgb : SharpDX.DXGI.Format.R8G8B8A8_UNorm);
				width = (uint)image.Width;
				height = (uint)image.Height;
				loadedTexturesPtrs.Add(filePath, new(handle, width, height));
			}
		}

		/// <summary>
		/// Adds the image to the Graphic Device as a texture.
		/// Then returns the pointer of the added texture. It also
		/// cache the image internally rather than creating a new texture on every call,
		/// so this function can be called multiple times per frame.
		/// </summary>
		/// <param name="name">user friendly name given to the image.</param>
		/// <param name="image">Image data in <see cref="Image"> format.</param>
		/// <param name="srgb"> a value indicating whether pixel format is srgb or not.</param>
		/// <param name="handle">output pointer to the image in the graphic device.</param>
		public static void AddOrGetImagePointer(string name, Image<Rgba32> image, bool srgb, out IntPtr handle) {
			if (loadedTexturesPtrs.TryGetValue(name, out var data)) {
				handle = data.Handle;
			}
			else {
				handle = renderer.CreateImageTexture(image, srgb ? SharpDX.DXGI.Format.R8G8B8A8_UNorm_SRgb : SharpDX.DXGI.Format.R8G8B8A8_UNorm);
				loadedTexturesPtrs.Add(name, new(handle, (uint)image.Width, (uint)image.Height));
			}
		}

		/// <summary>
		/// Removes the image from the Overlay.
		/// </summary>
		/// <param name="key">name or pathname which was used to add the image in the first place.</param>
		/// <returns> true if the image is removed otherwise false.</returns>
		public static bool RemoveImage(string key) {
			if (loadedTexturesPtrs.Remove(key, out var data)) {
				return renderer.RemoveImageTexture(data.Handle);
			}

			return false;
		}

		#endregion

		public static void Dispose() {

			if (loadedTexturesPtrs != null) {
				foreach (var key in loadedTexturesPtrs.Keys.ToArray()) {
					RemoveImage(key);
				}
			}
			

			renderer?.Dispose();

		}


		public static void OneLoop(float deltaTime, GraphicsCommandList gCmdList) {

			inputhandler.Update();
			renderer.Update(deltaTime, DebugManager.Render);
			renderer.Render(gCmdList);
			ReplaceFontIfRequired();
		}

		private static void ReplaceFontIfRequired() {
			if (replaceFont && renderer != null) {
				renderer.UpdateFontTexture(fontPathName, fontSize, fontCustomGlyphRange, fontLanguage);
				replaceFont = false;
			}
		}

		private static void OnResize() {
			renderer.Resize(Engine.form.Width, Engine.form.Height);
		}

		private static void InitializeResources() {

			renderer = new ImGuiRenderer(1920, 1080);
			inputhandler = new ImGuiInputHandler(Engine.form.Handle);

		}

		
	}
}

#pragma warning restore CS8618