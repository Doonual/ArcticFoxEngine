using SharpDX.DXGI;


namespace ArcticFoxEngine {
	using ArcticFoxEngine.Gui.Tools;
	using ArcticFoxEngine.ImGuiIntegration;
	using CoolClassLibrary;
	using SharpDX.Direct3D12;
	using SixLabors.ImageSharp;
	using SixLabors.ImageSharp.PixelFormats;

	public enum Format {
		//
		// Summary:
		//     A four-component, 128-bit typeless format that supports 32 bits per channel including
		//     alpha. ?
		R32G32B32A32_Typeless = 1,
		//
		// Summary:
		//     A four-component, 128-bit floating-point format that supports 32 bits per channel
		//     including alpha. 1,5,8
		R32G32B32A32_Float = 2,
		//
		// Summary:
		//     A four-component, 128-bit unsigned-integer format that supports 32 bits per channel
		//     including alpha. ?
		R32G32B32A32_UInt = 3,
		//
		// Summary:
		//     A four-component, 128-bit signed-integer format that supports 32 bits per channel
		//     including alpha. ?
		R32G32B32A32_SInt = 4,
		//
		// Summary:
		//     A three-component, 96-bit typeless format that supports 32 bits per color channel.
		R32G32B32_Typeless = 5,
		//
		// Summary:
		//     A three-component, 96-bit floating-point format that supports 32 bits per color
		//     channel.5,8
		R32G32B32_Float = 6,
		//
		// Summary:
		//     A three-component, 96-bit unsigned-integer format that supports 32 bits per color
		//     channel.
		R32G32B32_UInt = 7,
		//
		// Summary:
		//     A three-component, 96-bit signed-integer format that supports 32 bits per color
		//     channel.
		R32G32B32_SInt = 8,
		//
		// Summary:
		//     A four-component, 64-bit typeless format that supports 16 bits per channel including
		//     alpha.
		R16G16B16A16_Typeless = 9,
		//
		// Summary:
		//     A four-component, 64-bit floating-point format that supports 16 bits per channel
		//     including alpha.5,7
		R16G16B16A16_Float = 10,
		//
		// Summary:
		//     A four-component, 64-bit unsigned-normalized-integer format that supports 16
		//     bits per channel including alpha.
		R16G16B16A16_UNorm = 11,
		//
		// Summary:
		//     A four-component, 64-bit unsigned-integer format that supports 16 bits per channel
		//     including alpha.
		R16G16B16A16_UInt = 12,
		//
		// Summary:
		//     A four-component, 64-bit signed-normalized-integer format that supports 16 bits
		//     per channel including alpha.
		R16G16B16A16_SNorm = 13,
		//
		// Summary:
		//     A four-component, 64-bit signed-integer format that supports 16 bits per channel
		//     including alpha.
		R16G16B16A16_SInt = 14,
		//
		// Summary:
		//     A two-component, 64-bit typeless format that supports 32 bits for the red channel
		//     and 32 bits for the green channel.
		R32G32_Typeless = 15,
		//
		// Summary:
		//     A two-component, 64-bit floating-point format that supports 32 bits for the red
		//     channel and 32 bits for the green channel.5,8
		R32G32_Float = 16,
		//
		// Summary:
		//     A two-component, 64-bit unsigned-integer format that supports 32 bits for the
		//     red channel and 32 bits for the green channel.
		R32G32_UInt = 17,
		//
		// Summary:
		//     A two-component, 64-bit signed-integer format that supports 32 bits for the red
		//     channel and 32 bits for the green channel.
		R32G32_SInt = 18,
		//
		// Summary:
		//     A four-component, 32-bit typeless format that supports 10 bits for each color
		//     and 2 bits for alpha.
		R10G10B10A2_Typeless = 23,
		//
		// Summary:
		//     A four-component, 32-bit unsigned-normalized-integer format that supports 10
		//     bits for each color and 2 bits for alpha.
		R10G10B10A2_UNorm = 24,
		//
		// Summary:
		//     A four-component, 32-bit unsigned-integer format that supports 10 bits for each
		//     color and 2 bits for alpha.
		R10G10B10A2_UInt = 25,
		//
		// Summary:
		//     Three partial-precision floating-point numbers encoded into a single 32-bit value
		//     (a variant of s10e5, which is sign bit, 10-bit mantissa, and 5-bit biased (15)
		//     exponent). There are no sign bits, and there is a 5-bit biased (15) exponent
		//     for each channel, 6-bit mantissa for R and G, and a 5-bit mantissa for B, as
		//     shown in the following illustration.5,7
		R11G11B10_Float = 26,
		//
		// Summary:
		//     A four-component, 32-bit typeless format that supports 8 bits per channel including
		//     alpha.
		R8G8B8A8_Typeless = 27,
		//
		// Summary:
		//     A four-component, 32-bit unsigned-normalized-integer format that supports 8 bits
		//     per channel including alpha.
		R8G8B8A8_UNorm = 28,
		//
		// Summary:
		//     A four-component, 32-bit unsigned-normalized integer sRGB format that supports
		//     8 bits per channel including alpha.
		R8G8B8A8_UNorm_SRgb = 29,
		//
		// Summary:
		//     A four-component, 32-bit unsigned-integer format that supports 8 bits per channel
		//     including alpha.
		R8G8B8A8_UInt = 30,
		//
		// Summary:
		//     A four-component, 32-bit signed-normalized-integer format that supports 8 bits
		//     per channel including alpha.
		R8G8B8A8_SNorm = 31,
		//
		// Summary:
		//     A four-component, 32-bit signed-integer format that supports 8 bits per channel
		//     including alpha.
		R8G8B8A8_SInt = 32,
		//
		// Summary:
		//     A two-component, 32-bit typeless format that supports 16 bits for the red channel
		//     and 16 bits for the green channel.
		R16G16_Typeless = 33,
		//
		// Summary:
		//     A two-component, 32-bit floating-point format that supports 16 bits for the red
		//     channel and 16 bits for the green channel.5,7
		R16G16_Float = 34,
		//
		// Summary:
		//     A two-component, 32-bit unsigned-normalized-integer format that supports 16 bits
		//     each for the green and red channels.
		R16G16_UNorm = 35,
		//
		// Summary:
		//     A two-component, 32-bit unsigned-integer format that supports 16 bits for the
		//     red channel and 16 bits for the green channel.
		R16G16_UInt = 36,
		//
		// Summary:
		//     A two-component, 32-bit signed-normalized-integer format that supports 16 bits
		//     for the red channel and 16 bits for the green channel.
		R16G16_SNorm = 37,
		//
		// Summary:
		//     A two-component, 32-bit signed-integer format that supports 16 bits for the red
		//     channel and 16 bits for the green channel.
		R16G16_SInt = 38,
		//
		// Summary:
		//     A single-component, 32-bit typeless format that supports 32 bits for the red
		//     channel.
		R32_Typeless = 39,
		//
		// Summary:
		//     A single-component, 32-bit floating-point format that supports 32 bits for depth.5,8
		D32_Float = 40,
		//
		// Summary:
		//     A single-component, 32-bit floating-point format that supports 32 bits for the
		//     red channel.5,8
		R32_Float = 41,
		//
		// Summary:
		//     A single-component, 32-bit unsigned-integer format that supports 32 bits for
		//     the red channel.
		R32_UInt = 42,
		//
		// Summary:
		//     A single-component, 32-bit signed-integer format that supports 32 bits for the
		//     red channel.
		R32_SInt = 43,
		//
		// Summary:
		//     A two-component, 32-bit typeless format that supports 24 bits for the red channel
		//     and 8 bits for the green channel.
		R24G8_Typeless = 44,
		//
		// Summary:
		//     A 32-bit z-buffer format that supports 24 bits for depth and 8 bits for stencil.
		D24_UNorm_S8_UInt = 45,
		//
		// Summary:
		//     A two-component, 16-bit typeless format that supports 8 bits for the red channel
		//     and 8 bits for the green channel.
		R8G8_Typeless = 48,
		//
		// Summary:
		//     A two-component, 16-bit unsigned-normalized-integer format that supports 8 bits
		//     for the red channel and 8 bits for the green channel.
		R8G8_UNorm = 49,
		//
		// Summary:
		//     A two-component, 16-bit unsigned-integer format that supports 8 bits for the
		//     red channel and 8 bits for the green channel.
		R8G8_UInt = 50,
		//
		// Summary:
		//     A two-component, 16-bit signed-normalized-integer format that supports 8 bits
		//     for the red channel and 8 bits for the green channel.
		R8G8_SNorm = 51,
		//
		// Summary:
		//     A two-component, 16-bit signed-integer format that supports 8 bits for the red
		//     channel and 8 bits for the green channel.
		R8G8_SInt = 52,
		//
		// Summary:
		//     A single-component, 16-bit typeless format that supports 16 bits for the red
		//     channel.
		R16_Typeless = 53,
		//
		// Summary:
		//     A single-component, 16-bit floating-point format that supports 16 bits for the
		//     red channel.5,7
		R16_Float = 54,
		//
		// Summary:
		//     A single-component, 16-bit unsigned-normalized-integer format that supports 16
		//     bits for depth.
		D16_UNorm = 55,
		//
		// Summary:
		//     A single-component, 16-bit unsigned-normalized-integer format that supports 16
		//     bits for the red channel.
		R16_UNorm = 56,
		//
		// Summary:
		//     A single-component, 16-bit unsigned-integer format that supports 16 bits for
		//     the red channel.
		R16_UInt = 57,
		//
		// Summary:
		//     A single-component, 16-bit signed-normalized-integer format that supports 16
		//     bits for the red channel.
		R16_SNorm = 58,
		//
		// Summary:
		//     A single-component, 16-bit signed-integer format that supports 16 bits for the
		//     red channel.
		R16_SInt = 59,
		//
		// Summary:
		//     A single-component, 8-bit typeless format that supports 8 bits for the red channel.
		R8_Typeless = 60,
		//
		// Summary:
		//     A single-component, 8-bit unsigned-normalized-integer format that supports 8
		//     bits for the red channel.
		R8_UNorm = 61,
		//
		// Summary:
		//     A single-component, 8-bit unsigned-integer format that supports 8 bits for the
		//     red channel.
		R8_UInt = 62,
		//
		// Summary:
		//     A single-component, 8-bit signed-normalized-integer format that supports 8 bits
		//     for the red channel.
		R8_SNorm = 63,
		//
		// Summary:
		//     A single-component, 8-bit signed-integer format that supports 8 bits for the
		//     red channel.
		R8_SInt = 64,
		//
		// Summary:
		//     A single-component, 8-bit unsigned-normalized-integer format for alpha only.
		A8_UNorm = 65,
		//
		// Summary:
		//     A single-component, 1-bit unsigned-normalized integer format that supports 1
		//     bit for the red channel. ?.
		R1_UNorm = 66,
	}

	public class Texture : GraphicsResource {

		internal bool disposed = true;

		internal static ResourceStates defaultState = ResourceStates.CopyDestination;
		internal DescriptorHeap descriptorHeap;
		internal Resource resource;
		internal IntPtr imGuiID;

		public string name;
		public Format format { get; private set; }
		public int width;
		public int height;
		bool allowUnorderedAccess;

		/// <summary>
		/// Creates an empty texture
		/// </summary>
		/// <param name="width">Width of the texture</param>
		/// <param name="height">Height of the texture</param>
		public Texture(int width, int height, Format format = Format.R8G8B8A8_UNorm, ResourceFlags flags = ResourceFlags.None) {
			
			name = GetHashCode() + "";
			if ((flags & ResourceFlags.AllowUnorderedAccess) != 0) {
				allowUnorderedAccess = true;
			}
			else {
				allowUnorderedAccess = false;
			}

			TextureInspectorWindow.RegisterTexture(this);
			disposed = false;

			this.width = width;
			this.height = height;
			this.format = format;

			ResourceDescription textureDesc = ResourceDescription.Texture2D((SharpDX.DXGI.Format)format, width, height, flags: flags, mipLevels: 1);
			resource = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, textureDesc, defaultState);
			
			PrepareDescriptorHeaps();
			imGuiID = RenderImGui.RegisterTexture(this);

		}

		/// <summary>
		/// Creates a texture and uploads the contents of the specified image to it
		/// </summary>
		/// <param name="path">The path to the image containing the data to be uploaded</param>
		public Texture(string path) {
			name = path;
			TextureInspectorWindow.RegisterTexture(this);

			disposed = false;

			Image<Rgba32> image = Image.Load<Rgba32>(path);

			width = image.Width;
			height = image.Height;
			format = Format.R8G8B8A8_UNorm;
			ResourceDescription textureDesc = ResourceDescription.Texture2D((SharpDX.DXGI.Format)format, width, height);
			resource = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, textureDesc, ResourceStates.CopyDestination);

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

			PrepareDescriptorHeaps();

			imGuiID = RenderImGui.RegisterTexture(this);

		}

		/// <summary>
		/// Creates a descriptor heap for a shader resource view, and adds the texture to it
		/// </summary>
		internal void PrepareDescriptorHeaps() {

			// Create descriptor heap
			DescriptorHeapDescription dhd = new DescriptorHeapDescription() {
				DescriptorCount = 2,
				Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView,
			};
			descriptorHeap = Graphics.device.CreateDescriptorHeap(dhd);


			int componentMapping = format.ComponentMapping();
			ShaderResourceViewDescription srvDesc = new ShaderResourceViewDescription() {
				Shader4ComponentMapping = componentMapping,
				Format = (SharpDX.DXGI.Format)format.SRVFormat(),
				Dimension = ShaderResourceViewDimension.Texture2D,
				Texture2D = { MipLevels = 1 },
			};
			Graphics.device.CreateShaderResourceView(resource, srvDesc, descriptorHeap.CPUDescriptorHandleForHeapStart);

			if (allowUnorderedAccess == true) {
				UnorderedAccessViewDescription uavDesc = new UnorderedAccessViewDescription() {
					Texture2D = new UnorderedAccessViewDescription.Texture2DResource() { },
					Dimension = UnorderedAccessViewDimension.Texture2D,
					Format = (SharpDX.DXGI.Format)format.SRVFormat(),
				};
				Graphics.device.CreateUnorderedAccessView(resource, null, uavDesc, descriptorHeap.CPUDescriptorHandleForHeapStart + Graphics.descriptorHeapIncrement);
			}


		}

		internal override Resource GetResource() {
			return resource;
		}
		internal override int[] GetLength() {
			return new int[] { width, height };
		}
		internal override CpuDescriptorHandle GetSRVDescriptorLocation() {
			return descriptorHeap.CPUDescriptorHandleForHeapStart;
		}
		internal override CpuDescriptorHandle GetUAVDescriptorLocation() {
			return descriptorHeap.CPUDescriptorHandleForHeapStart + Graphics.descriptorHeapIncrement;
		}

		/// <summary>
		/// Adds the texture to a shader resource view descriptor heap.
		/// </summary>
		/// <param name="destDescriptorHeap">The descriptor heap to add the texture to</param>
		internal void CreateExternalShaderResourceView(CpuDescriptorHandle destDescriptor) {

			int componentMappingR = 0;
			int componentMappingG = 1;
			int componentMappingB = 2;
			int componentMappingA = 3;

			if (format == Format.D32_Float) {
				componentMappingR = 0;
				componentMappingG = 0;
				componentMappingB = 0;
				componentMappingA = 0;
			}

			if (format == Format.D32_Float) {
				format = Format.R32_Float;
			}

			int componentMapping = ComponentMapping(componentMappingR, componentMappingG, componentMappingB, componentMappingA);
			ShaderResourceViewDescription srvDesc = new ShaderResourceViewDescription() {
				Shader4ComponentMapping = componentMapping,
				Format = (SharpDX.DXGI.Format)format.SRVFormat(),
				Dimension = ShaderResourceViewDimension.Texture2D,
				Texture2D = { MipLevels = 1 },
			};
			Graphics.device.CreateShaderResourceView(resource, srvDesc, destDescriptor);

		}

		

		/// <summary>
		/// Uploads data to the textue
		/// </summary>
		/// <param name="data">The data to be uploaded</param>
		public void SetData(byte[] data) {
			Upload.Texture2DUpload(resource, width, height, format, data);
		}

		public void SetPixel(byte[] data, int x, int y) {
			if (x < 0 || y < 0 || x >= width || y >= height) {
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

		byte[] batchUploadData;
		public void SetPixelBatch(byte[] data, int x, int y) {
			if (x < 0 || y < 0 || x >= width || y >= height) {
				return;
			}
			if (batchUploadData == null) { batchUploadData = new byte[width * height * format.SizeOfInBytes()]; }

			for (int i = 0; i < data.Length; i ++) {
				batchUploadData[x * format.SizeOfInBytes() + y * width * format.SizeOfInBytes() + i] = data[i];
			}
			
		}
		public void BatchSync() {
			Upload.Texture2DUpload(resource, width, height, format, batchUploadData);
			batchUploadData = null;
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
			TextureInspectorWindow.DeRegisterTexture(this);
			RenderImGui.DeRegisterTexture(imGuiID);
		}
		~Texture() {
			Dispose();
		}




	}
	
	public static class TextureFormat {
		public static int SizeOfInBits(this Format format) {

			switch (format) {

				case Format.R32G32B32A32_Typeless: return 128;
				case Format.R32G32B32A32_Float: return 128;
				case Format.R32G32B32A32_UInt: return 128;
				case Format.R32G32B32A32_SInt: return 128;
				case Format.R32G32B32_Typeless: return 96;
				case Format.R32G32B32_Float: return 96;
				case Format.R32G32B32_UInt: return 96;
				case Format.R32G32B32_SInt: return 96;
				case Format.R16G16B16A16_Typeless: return 64;
				case Format.R16G16B16A16_Float: return 64;
				case Format.R16G16B16A16_UNorm: return 64;
				case Format.R16G16B16A16_UInt: return 64;
				case Format.R16G16B16A16_SNorm: return 64;
				case Format.R16G16B16A16_SInt: return 64;
				case Format.R32G32_Typeless: return 64;
				case Format.R32G32_Float: return 64;
				case Format.R32G32_UInt: return 64;
				case Format.R32G32_SInt: return 64;
				case Format.R10G10B10A2_Typeless: return 32;
				case Format.R10G10B10A2_UNorm: return 32;
				case Format.R10G10B10A2_UInt: return 32;
				case Format.R11G11B10_Float: return 32;
				case Format.R8G8B8A8_Typeless: return 32;
				case Format.R8G8B8A8_UNorm: return 32;
				case Format.R8G8B8A8_UNorm_SRgb: return 32;
				case Format.R8G8B8A8_UInt: return 32;
				case Format.R8G8B8A8_SNorm: return 32;
				case Format.R8G8B8A8_SInt: return 32;
				case Format.R16G16_Typeless: return 32;
				case Format.R16G16_Float: return 32;
				case Format.R16G16_UNorm: return 32;
				case Format.R16G16_UInt: return 32;
				case Format.R16G16_SNorm: return 32;
				case Format.R16G16_SInt: return 32;
				case Format.R32_Typeless: return 32;
				case Format.D32_Float: return 32;
				case Format.R32_Float: return 32;
				case Format.R32_UInt: return 32;
				case Format.R32_SInt: return 32;
				case Format.R24G8_Typeless: return 32;
				case Format.D24_UNorm_S8_UInt: return 32;
				case Format.R8G8_Typeless: return 16;
				case Format.R8G8_UNorm: return 16;
				case Format.R8G8_UInt: return 16;
				case Format.R8G8_SNorm: return 16;
				case Format.R8G8_SInt: return 16;
				case Format.R16_Typeless: return 16;
				case Format.R16_Float: return 16;
				case Format.D16_UNorm: return 16;
				case Format.R16_UNorm: return 16;
				case Format.R16_UInt: return 16;
				case Format.R16_SNorm: return 16;
				case Format.R16_SInt: return 16;
				case Format.R8_Typeless: return 8;
				case Format.R8_UNorm: return 8;
				case Format.R8_UInt: return 8;
				case Format.R8_SNorm: return 8;
				case Format.R8_SInt: return 8;
				case Format.A8_UNorm: return 8;
				case Format.R1_UNorm: return 1;

			}
			return 0;

		}
		public static int SizeOfInBytes(this Format format) {
			return format.SizeOfInBits() >> 3;
		}
		public static int ComponentMapping(this Format format) {

			switch (format) {

				case Format.R32G32B32A32_Typeless: return Texture.ComponentMapping(0, 1, 2, 3);
				case Format.R32G32B32A32_Float: return Texture.ComponentMapping(0, 1, 2, 3);
				case Format.R32G32B32A32_UInt: return Texture.ComponentMapping(0, 1, 2, 3);
				case Format.R32G32B32A32_SInt: return Texture.ComponentMapping(0, 1, 2, 3);
				case Format.R32G32B32_Typeless: return Texture.ComponentMapping(0, 1, 2, 4);
				case Format.R32G32B32_Float: return Texture.ComponentMapping(0, 1, 2, 4);
				case Format.R32G32B32_UInt: return Texture.ComponentMapping(0, 1, 2, 4);
				case Format.R32G32B32_SInt: return Texture.ComponentMapping(0, 1, 2, 4);
				case Format.R16G16B16A16_Typeless: return Texture.ComponentMapping(0, 1, 2, 3);
				case Format.R16G16B16A16_Float: return Texture.ComponentMapping(0, 1, 2, 3);
				case Format.R16G16B16A16_UNorm: return Texture.ComponentMapping(0, 1, 2, 3);
				case Format.R16G16B16A16_UInt: return Texture.ComponentMapping(0, 1, 2, 3);
				case Format.R16G16B16A16_SNorm: return Texture.ComponentMapping(0, 1, 2, 3);
				case Format.R16G16B16A16_SInt: return Texture.ComponentMapping(0, 1, 2, 3);
				case Format.R32G32_Typeless: return Texture.ComponentMapping(0, 1, 4, 4);
				case Format.R32G32_Float: return Texture.ComponentMapping(0, 1, 4, 4);
				case Format.R32G32_UInt: return Texture.ComponentMapping(0, 1, 4, 4);
				case Format.R32G32_SInt: return Texture.ComponentMapping(0, 1, 4, 4);
				case Format.R10G10B10A2_Typeless: return Texture.ComponentMapping(0, 1, 2, 3);
				case Format.R10G10B10A2_UNorm: return Texture.ComponentMapping(0, 1, 2, 3);
				case Format.R10G10B10A2_UInt: return Texture.ComponentMapping(0, 1, 2, 3);
				case Format.R11G11B10_Float: return Texture.ComponentMapping(0, 1, 2, 4);
				case Format.R8G8B8A8_Typeless: return Texture.ComponentMapping(0, 1, 2, 3);
				case Format.R8G8B8A8_UNorm: return Texture.ComponentMapping(0, 1, 2, 3);
				case Format.R8G8B8A8_UNorm_SRgb: return Texture.ComponentMapping(0, 1, 2, 3);
				case Format.R8G8B8A8_UInt: return Texture.ComponentMapping(0, 1, 2, 3);
				case Format.R8G8B8A8_SNorm: return Texture.ComponentMapping(0, 1, 2, 3);
				case Format.R8G8B8A8_SInt: return Texture.ComponentMapping(0, 1, 2, 3);
				case Format.R16G16_Typeless: return Texture.ComponentMapping(0, 1, 4, 4);
				case Format.R16G16_Float: return Texture.ComponentMapping(0, 1, 4, 4);
				case Format.R16G16_UNorm: return Texture.ComponentMapping(0, 1, 4, 4);
				case Format.R16G16_UInt: return Texture.ComponentMapping(0, 1, 4, 4);
				case Format.R16G16_SNorm: return Texture.ComponentMapping(0, 1, 4, 4);
				case Format.R16G16_SInt: return Texture.ComponentMapping(0, 1, 4, 4);
				case Format.R32_Typeless: return Texture.ComponentMapping(0, 4, 4, 4);
				case Format.D32_Float: return Texture.ComponentMapping(0, 4, 4, 4);
				case Format.R32_Float: return Texture.ComponentMapping(0, 4, 4, 4);
				case Format.R32_UInt: return Texture.ComponentMapping(0, 4, 4, 4);
				case Format.R32_SInt: return Texture.ComponentMapping(0, 4, 4, 4);
				case Format.R24G8_Typeless: return Texture.ComponentMapping(0, 1, 4, 4);
				case Format.D24_UNorm_S8_UInt: return Texture.ComponentMapping(0, 1, 4, 4);
				case Format.R8G8_Typeless: return Texture.ComponentMapping(0, 1, 4, 4);
				case Format.R8G8_UNorm: return Texture.ComponentMapping(0, 1, 4, 4);
				case Format.R8G8_UInt: return Texture.ComponentMapping(0, 1, 4, 4);
				case Format.R8G8_SNorm: return Texture.ComponentMapping(0, 1, 4, 4);
				case Format.R8G8_SInt: return Texture.ComponentMapping(0, 1, 4, 4);
				case Format.R16_Typeless: return Texture.ComponentMapping(0, 4, 4, 4);
				case Format.R16_Float: return Texture.ComponentMapping(0, 4, 4, 4);
				case Format.D16_UNorm: return Texture.ComponentMapping(0, 4, 4, 4);
				case Format.R16_UNorm: return Texture.ComponentMapping(0, 4, 4, 4);
				case Format.R16_UInt: return Texture.ComponentMapping(0, 4, 4, 4);
				case Format.R16_SNorm: return Texture.ComponentMapping(0, 4, 4, 4);
				case Format.R16_SInt: return Texture.ComponentMapping(0, 4, 4, 4);
				case Format.R8_Typeless: return Texture.ComponentMapping(0, 4, 4, 4);
				case Format.R8_UNorm: return Texture.ComponentMapping(0, 4, 4, 4);
				case Format.R8_UInt: return Texture.ComponentMapping(0, 4, 4, 4);
				case Format.R8_SNorm: return Texture.ComponentMapping(0, 4, 4, 4);
				case Format.R8_SInt: return Texture.ComponentMapping(0, 4, 4, 4);
				case Format.A8_UNorm: return Texture.ComponentMapping(4, 4, 4, 0);
				case Format.R1_UNorm: return Texture.ComponentMapping(0, 4, 4, 4);

			}
			return 0;

		}
		public static Format SRVFormat(this Format format) {
			switch (format) {
				case Format.D32_Float: return Format.R32_Float;
				case Format.D24_UNorm_S8_UInt: return Format.R24G8_Typeless;
			}
			return format;
		}
	}
	

}
