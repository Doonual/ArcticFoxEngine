using SharpDX.DXGI;


namespace ArcticFoxEngine {


	using SharpDX.Direct3D12;
	using SixLabors.ImageSharp;
	using SixLabors.ImageSharp.PixelFormats;

	public class Texture {

		private bool disposed = true;

		internal DescriptorHeap descriptorHeap;
		Resource texture;
		int width;
		int height;


		/// <summary>
		/// Creates an empty texture
		/// </summary>
		/// <param name="width">Width of the texture</param>
		/// <param name="height">Height of the texture</param>
		public Texture(int width, int height, bool allowUnorderedAccess = false) {
			disposed = false;

			this.width = width;
			this.height = height;
			ResourceDescription textureDesc = ResourceDescription.Texture2D(Format.R8G8B8A8_UNorm, width, height);

			textureDesc.Flags |= allowUnorderedAccess ? ResourceFlags.AllowUnorderedAccess : ResourceFlags.None;

			texture = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, textureDesc, ResourceStates.CopyDestination);


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
			ResourceDescription textureDesc = ResourceDescription.Texture2D(Format.R8G8B8A8_UNorm, width, height);
			texture = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, textureDesc, ResourceStates.CopyDestination);


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
			Upload.Texture2DUpload(texture, width, height, Format.R8G8B8A8_UNorm, data);
		}

		/// <summary>
		/// Gets the native pointer of the texture
		/// </summary>
		/// <returns>The native pointer of the texture</returns>
		internal IntPtr GetNativePointer() {
			return texture.NativePointer;
		}


		private static int ComponentMapping(int src0, int src1, int src2, int src3) {

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
