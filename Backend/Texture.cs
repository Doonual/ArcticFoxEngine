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

		internal Texture(int width, int height) {

			ResourceDescription textureDesc = ResourceDescription.Texture2D(Format.R8G8B8A8_UNorm, width, height);
			texture = Graphics.device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, textureDesc, ResourceStates.CopyDestination);

			ShaderResourceViewDescription srvDesc = new ShaderResourceViewDescription() {
				Shader4ComponentMapping = ComponentMapping(0, 1, 2, 3),
				Format = Format.R8G8B8A8_UNorm,
				Dimension = ShaderResourceViewDimension.Texture2D,
				Texture2D = { MipLevels = 1 },
			};
			Graphics.device.CreateShaderResourceView(texture, srvDesc, RenderResources.combinedDescriptorHeap.CPUDescriptorHandleForHeapStart + RenderResources.combinedDescriptorHeapIncrement);

			Log.Info("Created texture");

			
			byte[] textureData = new byte[4 * width * height];
			for (int x = 0; x < width; x ++) {
				for (int y = 0; y < height; y ++) {

					bool checker = ((x % 2) ^ (y % 2)) == 1;

					int writePos = (x + y * width) * 4;
					textureData[writePos + 0] = (byte)(checker ? 0xff : 0x00);
					textureData[writePos + 1] = (byte)(checker ? 0x00 : 0xff);
					textureData[writePos + 2] = 0x00;
					textureData[writePos + 3] = 0xff;

				}
			}

			GPU_Upload.Texture2DUpload(texture, width, height, Format.R8G8B8A8_UNorm, textureData);

			
			

		}

		public static int ComponentMapping(int src0, int src1, int src2, int src3) {

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
