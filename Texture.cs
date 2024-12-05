using SharpDX.DXGI;


namespace ArcticFoxEngine {
	using CoolClassLibrary;
	using SharpDX.Direct3D12;
	using SixLabors.ImageSharp;
	using SixLabors.ImageSharp.PixelFormats;

	public class Texture : IDisposable {

		public static class Cache {

			// The key is the path of the image file
			// The value is a Texture / int pair, where the Texture is the cached texture
			// and the int is the number of refrences to the texture that exists
			private static Dictionary<string, (Texture, int)> textureCache;

			static Cache() {
				textureCache = new Dictionary<string, (Texture, int)>();
			}


			public static Texture FindOrLoad(string path) {

				if (textureCache.ContainsKey(path) == true) {

					// Retrieve the cached texture
					(Texture cachedTexture, int numRefs) = textureCache[path];
					textureCache[path] = (cachedTexture, numRefs + 1); // update the number of refrences to this texture

					return cachedTexture;
				}

				Texture loadedTexture = new Texture(path);
				textureCache.Add(path, (loadedTexture, 1));
				return loadedTexture;

			}

			public static void Release(string path) {

				if (textureCache.ContainsKey(path) == true) {

					// Retrieve the cached texture
					(Texture cachedTexture, int numRefs) = textureCache[path];

					// update the number of refrences to this texture
					textureCache[path] = (cachedTexture, numRefs - 1); 

					// If there are no more refrences to this texture, dispose it and remove the dictionary entry
					if (numRefs == 0) {
						cachedTexture.Dispose();
						textureCache.Remove(path);
					}

					return;

				}

				Log.Warn("Cannot release texture from cache, not added to cache");

			}

			public static void Release(Texture texture) {

				for (int i = 0; i < textureCache.Count; i ++) {
					if (textureCache.ElementAt(i).Value.Item1 == texture) {
						Release(textureCache.ElementAt(i).Key);
						return;
					}
				}

				Log.Warn("Cannot release texture from cache, not added to cache");

			}


		}

		private bool disposed = true;

		internal DescriptorHeap descriptorHeap;
		Resource texture;

		public Format format { get; private set; }
		public int width;
		public int height;
		byte[] textureDataClone;

		/// <summary>
		/// Creates an empty texture
		/// </summary>
		/// <param name="width">Width of the texture</param>
		/// <param name="height">Height of the texture</param>
		public Texture(int width, int height, bool allowUnorderedAccess = false, Format format = Format.R8G8B8A8_UNorm) {
			disposed = false;

			this.width = width;
			this.height = height;
			this.format = format;
			ResourceDescription textureDesc = ResourceDescription.Texture2D(format, width, height);

			textureDesc.Flags |= allowUnorderedAccess ? ResourceFlags.AllowUnorderedAccess : ResourceFlags.None;

			texture = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, textureDesc, ResourceStates.CopyDestination);
			textureDataClone = new byte[width * height * format.SizeOfInBytes()];

			DescriptorHeapDescription dhd = new DescriptorHeapDescription() {
				DescriptorCount = 1,
				Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView,
			};
			descriptorHeap = Graphics.device.CreateDescriptorHeap(dhd);
			AddToDescriptorHeap(descriptorHeap, 0);

		}

		/// <summary>
		/// Creates a texture and uploads the contents of the specified image to it
		/// </summary>
		/// <param name="path">The path to the image containing the data to be uploaded</param>
		public Texture(string path) {


			Image<Rgba32> image = Image.Load<Rgba32>(path);

			width = image.Width;
			height = image.Height;
			format = Format.R8G8B8A8_UNorm;
			ResourceDescription textureDesc = ResourceDescription.Texture2D(Format.R8G8B8A8_UNorm, width, height);
			texture = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, textureDesc, ResourceStates.CopyDestination);
			textureDataClone = new byte[width * height * format.SizeOfInBytes()];

			byte[] imageData = new byte[image.Width * image.Height * 4];
			for (int i = 0; i < image.Width; i++) {
				for (int n = 0; n < image.Height; n++) {
					imageData[(i + n * image.Width) * 4 + 0] = image[i, n].R;
					imageData[(i + n * image.Width) * 4 + 1] = image[i, n].G;
					imageData[(i + n * image.Width) * 4 + 2] = image[i, n].B;
					imageData[(i + n * image.Width) * 4 + 3] = image[i, n].A;
				}
			}
			image.Dispose();
			SetData(imageData);

			DescriptorHeapDescription dhd = new DescriptorHeapDescription() {
				DescriptorCount = 1,
				Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView,
			};
			descriptorHeap = Graphics.device.CreateDescriptorHeap(dhd);
			AddToDescriptorHeap(descriptorHeap, 0);

		}

		/// <summary>
		/// Adds the texture to a descriptor heap
		/// </summary>
		/// <param name="destDescriptorHeap">The descriptor heap to add the texture to</param>
		/// <param name="offset">The offset into the descriptor heap the texture should be added to</param>
		internal void AddToDescriptorHeap(DescriptorHeap destDescriptorHeap, int offset) {

			ShaderResourceViewDescription srvDesc = new ShaderResourceViewDescription() {
				Shader4ComponentMapping = ComponentMapping(0, 1, 2, 3),
				Format = Format.R8G8B8A8_UNorm,
				Dimension = ShaderResourceViewDimension.Texture2D,
				Texture2D = { MipLevels = 1 },
			};

			Graphics.device.CreateShaderResourceView(texture, srvDesc, destDescriptorHeap.CPUDescriptorHandleForHeapStart + Graphics.device.GetDescriptorHandleIncrementSize(DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView) * offset);


		}

		/// <summary>
		/// Uploads data to the textue
		/// </summary>
		/// <param name="data">The data to be uploaded</param>
		public void SetData(byte[] data) {
			Upload.Texture2DUpload(texture, width, height, format, data);
			textureDataClone = data;
		}

		public void SetPixel(byte[] data, int x, int y) {
			if (x < 0 || y < 0 || x >= width || y >= height) {
				//Log.Warn("Trying to set pixel outside of the texture, ignoring");
				return;
			}
			Upload.Texture2DPixelUpload(texture, x, y, format, data);
		}
		public void SetAllPixels(byte[] data) {
			byte[] allData = new byte[format.SizeOfInBytes() * width * height];
			for (int i = 0; i < width * height; i ++) {

				for (int f = 0; f < format.SizeOfInBytes(); f ++) {
					allData[i * format.SizeOfInBytes() + f] = data[f];
				}

			}
			SetData(allData);
		}

		public byte[] GetPixel(int x, int y) {

			byte[] pixelData = new byte[format.SizeOfInBytes()];

			if (x < 0 || y < 0 || x >= width || y >= height) {
				//Log.Warn("Trying to get pixel outside of the texture, returning 0s");
				for (int i = 0; i < pixelData.Length; i ++) {
					pixelData[i] = 0x00;
				}
				return pixelData;
			}
			for (int i = 0; i < pixelData.Length; i++) {
				pixelData[i] = textureDataClone[x * format.SizeOfInBytes() + y * width * format.SizeOfInBytes() + i];
			}
			return pixelData;
		}

		public void SetPixelBatch(byte[] data, int x, int y) {
			if (x < 0 || y < 0 || x >= width || y >= height) {
				//Log.Warn("Trying to set pixel outside of the texture, ignoring");
				return;
			}
			for (int i = 0; i < data.Length; i ++) {
				textureDataClone[x * format.SizeOfInBytes() + y * width * format.SizeOfInBytes() + i] = data[i];
			}
			
		}
		public void BatchSync() {
			Upload.Texture2DUpload(texture, width, height, format, textureDataClone);
		}


		/// <summary>
		/// Gets the native pointer of the texture
		/// </summary>
		/// <returns>The native pointer of the texture</returns>
		internal IntPtr GetNativePointer() {
			return texture.NativePointer;
		}

		internal static int ComponentMapping(int src0, int src1, int src2, int src3) {

			int componentMappingMask = 0x7;
			int componentMappingShift = 3;
			int componentMappingAlwaysSetBitAvoidingZeromemMistakes = (1 << (componentMappingShift * 4));

			return ((((src0) & componentMappingMask) |
					(((src1) & componentMappingMask) << componentMappingShift) |
					(((src2) & componentMappingMask) << (componentMappingShift * 2)) |
					(((src3) & componentMappingMask) << (componentMappingShift * 3)) |
					componentMappingAlwaysSetBitAvoidingZeromemMistakes));

		}

		/// <summary>
		/// Disposes the resources held by Texture
		/// </summary>
		public void Dispose() {
			if (disposed == true) { return; }
			disposed = true;
			texture.Dispose();
			descriptorHeap.Dispose();
		}
		~Texture() {
			Dispose();
		}




	}
}
