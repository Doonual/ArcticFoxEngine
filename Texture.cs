using SharpDX.DXGI;


namespace ArcticFoxEngine {
	using CoolClassLibrary;
	using SharpDX.Direct3D12;
	using SixLabors.ImageSharp;
	using SixLabors.ImageSharp.PixelFormats;

	public class Texture : IDisposable {

		private bool disposed = true;

		internal DescriptorHeap descriptorHeap;
		internal Resource resource;

		public Format format { get; private set; }
		public int width;
		public int height;
		byte[] textureDataClone;

		/// <summary>
		/// Creates an empty texture
		/// </summary>
		/// <param name="width">Width of the texture</param>
		/// <param name="height">Height of the texture</param>
		public Texture(int width, int height, Format format = Format.R8G8B8A8_UNorm, ResourceFlags flags = ResourceFlags.None, ResourceStates initialState = ResourceStates.CopyDestination) {
			disposed = false;

			this.width = width;
			this.height = height;
			this.format = format;
			textureDataClone = new byte[width * height * format.SizeOfInBytes()];

			ResourceDescription textureDesc = ResourceDescription.Texture2D(format, width, height, flags: flags);
			resource = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, textureDesc, initialState);

			if (format == Format.D32_Float) {
				PrepareAsShaderResource(0, 0, 0, 0);
			}
			else {
				PrepareAsShaderResource(0, 1, 2, 3);
			}
			

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
			resource = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, textureDesc, ResourceStates.CopyDestination);
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

			PrepareAsShaderResource(0, 1, 2, 3);

		}

		/// <summary>
		/// Adds the texture to a descriptor heap
		/// </summary>
		/// <param name="destDescriptorHeap">The descriptor heap to add the texture to</param>
		/// <param name="offset">The offset into the descriptor heap the texture should be added to</param>
		internal void PrepareAsShaderResource(int componentMappingR, int componentMappingG, int componentMappingB, int componentMappingA) {

			// Create descriptor heap
			DescriptorHeapDescription dhd = new DescriptorHeapDescription() {
				DescriptorCount = 1,
				Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView,
			};
			descriptorHeap = Graphics.device.CreateDescriptorHeap(dhd);

			Format testFormat = format;
			if (format == Format.D32_Float) {
				format = Format.R32_Float;
			}

			int componentMapping = ComponentMapping(componentMappingR, componentMappingG, componentMappingB, componentMappingA);
			ShaderResourceViewDescription srvDesc = new ShaderResourceViewDescription() {
				Shader4ComponentMapping = componentMapping,
				Format = format,
				Dimension = ShaderResourceViewDimension.Texture2D,
				Texture2D = { MipLevels = 1 },
			};
			Graphics.device.CreateShaderResourceView(resource, srvDesc, descriptorHeap.CPUDescriptorHandleForHeapStart);

		}

		/// <summary>
		/// Uploads data to the textue
		/// </summary>
		/// <param name="data">The data to be uploaded</param>
		public void SetData(byte[] data) {
			Upload.Texture2DUpload(resource, width, height, format, data);
			textureDataClone = data;
		}

		public void SetPixel(byte[] data, int x, int y) {
			if (x < 0 || y < 0 || x >= width || y >= height) {
				//Log.Warn("Trying to set pixel outside of the texture, ignoring");
				return;
			}
			Upload.Texture2DPixelUpload(resource, x, y, format, data);
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
			Upload.Texture2DUpload(resource, width, height, format, textureDataClone);
		}


		/// <summary>
		/// Gets the native pointer of the texture
		/// </summary>
		/// <returns>The native pointer of the texture</returns>
		internal IntPtr GetNativePointer() {
			return resource.NativePointer;
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
			resource.Dispose();
			descriptorHeap.Dispose();
		}
		~Texture() {
			Dispose();
		}




	}
}
