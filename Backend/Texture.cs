using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoolClassLibrary;

using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;

namespace ArcticFoxEngine.Backend {

	using SharpDX;
	using SharpDX.Direct3D12;
	using System.Runtime.InteropServices;

	internal class Texture {

		Resource texture;
		int width;
		int height;

		internal Texture(int width, int height, DescriptorHeap destDescriptorHeap, int descriptorHeapIndex) {

			Log.Info("Creating texture");
			this.width = width;
			this.height = height;
			ResourceDescription textureDesc = ResourceDescription.Texture2D(Format.R8G8B8A8_UNorm, width, height);
			texture = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, textureDesc, ResourceStates.CopyDestination);

			ShaderResourceViewDescription srvDesc = new ShaderResourceViewDescription() {
				Shader4ComponentMapping = ComponentMapping(0, 1, 2, 3),
				Format = Format.R8G8B8A8_UNorm,
				Dimension = ShaderResourceViewDimension.Texture2D,
				Texture2D = { MipLevels = 1 },
			};
			Graphics.device.CreateShaderResourceView(texture, srvDesc, destDescriptorHeap.CPUDescriptorHandleForHeapStart + Backend.Render.GPU_Render.descHeapIncrement * descriptorHeapIndex);

		}
		
		public void SetData(byte[] data) {
			GPU_Upload.Texture2DUpload(texture, width, height, Format.R8G8B8A8_UNorm, data);
		}

		public IntPtr GetNativePointer() {
			return texture.NativePointer;
		}

		public void Dispose() {
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
