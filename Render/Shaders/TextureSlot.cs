using SharpDX.Direct3D12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine.Render {

	public class TextureSlot {

		public ShaderVisibility shaderVisibility;
		public int rootParameterIndex;

		public GpuDescriptorHandle currentDescriptorLocation;

		public TextureSlot(ShaderVisibility shaderVisibility) {

			this.shaderVisibility = shaderVisibility;
			rootParameterIndex = -1;
			currentDescriptorLocation = new GpuDescriptorHandle();

		}

		public void SetTexture(Texture texture) {

			if (texture == null) {
				SetTexture(RenderEngine.missingTexture);
				return;
			}

			GpuDescriptorHandle destDescriptor = RenderEngine.CopyDescriptorsIn(texture.descriptorHeap.CPUDescriptorHandleForHeapStart, 1);
			RenderEngine.cmdList.SetGraphicsRootDescriptorTable(rootParameterIndex, destDescriptor);

		}

	}

}
