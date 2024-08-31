using CoolClassLibrary;


using SharpDX.DXGI;


namespace ArcticFoxEngine.Backend {


	using SharpDX.Direct3D12;
	using SixLabors.ImageSharp;
	using SixLabors.ImageSharp.PixelFormats;

	internal class Texture {

		Resource texture;
		int width;
		int height;

		internal Texture(int width, int height) {

			this.width = width;
			this.height = height;
			ResourceDescription textureDesc = ResourceDescription.Texture2D(Format.R8G8B8A8_UNorm, width, height);
			texture = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, textureDesc, ResourceStates.CopyDestination);

		}

		internal Texture(string path) {


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

		}

		internal void AddToDescriptorHeap(DescriptorHeap destDescriptorHeap, int offset) {

			ShaderResourceViewDescription srvDesc = new ShaderResourceViewDescription() {
				Shader4ComponentMapping = ComponentMapping(0, 1, 2, 3),
				Format = Format.R8G8B8A8_UNorm,
				Dimension = ShaderResourceViewDimension.Texture2D,
				Texture2D = { MipLevels = 1 },
			};
			Graphics.device.CreateShaderResourceView(texture, srvDesc, destDescriptorHeap.CPUDescriptorHandleForHeapStart + Backend.Render.GPU_Render.descHeapIncrement * offset);

		}
		
		internal void SetData(byte[] data) {
			GPU_Upload.Texture2DUpload(texture, width, height, Format.R8G8B8A8_UNorm, data);
		}

		internal IntPtr GetNativePointer() {
			return texture.NativePointer;
		}

		internal void Dispose() {
			texture.Dispose();
		}

		private static int ComponentMapping(int src0, int src1, int src2, int src3) {

			int componentMappingMask = 0x7;
			int componentMappingShift = 3;
			int componentMappingAlwaysSetBitAvoidingZeromemMistakes = (1 << (componentMappingShift * 4));

			return	((((src0) & componentMappingMask) |
					(((src1) & componentMappingMask) << componentMappingShift) |
					(((src2) & componentMappingMask) << (componentMappingShift * 2)) |
					(((src3) & componentMappingMask) << (componentMappingShift * 3)) |
					componentMappingAlwaysSetBitAvoidingZeromemMistakes));

		}


	}
}
