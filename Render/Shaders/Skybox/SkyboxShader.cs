using ArcticFoxEngine.Nodes;
using SharpDX.Direct3D12;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static ArcticFoxEngine.Render.LitShader;

namespace ArcticFoxEngine.Render {


	internal class SkyboxShader : Shader {

		public override string name => "Skybox";

		public DataSlot projMatrix = new DataSlot(ShaderVisibility.Pixel);
		public DataSlot camTransformMatrix = new DataSlot(ShaderVisibility.Pixel);
		public DataSlot lightingWorld = new DataSlot(ShaderVisibility.Pixel);
		public DataSlot skyBoxInfo = new DataSlot(ShaderVisibility.Pixel);

		private ConstBuffer<Matrix> projMatrixBuffer;
		private ConstBuffer<Matrix> camTransformMatrixBuffer;

		[StructLayout(LayoutKind.Explicit)]
		public struct SkyboxInfo {

			[FieldOffset(0 * 4)] public Vector3 skyTopCol;
			[FieldOffset(4 * 4)] public Vector3 skyBottomCol;
			[FieldOffset(8 * 4)] public Vector3 groundTopCol;
			[FieldOffset(12 * 4)] public Vector3 groundBottomCol;

			[FieldOffset(15 * 4)] public float sunSharpness;
			[FieldOffset(16 * 4)] public float horizonSharpness;

		}

		public SkyboxShader() {

			

			rootSignature = CreateRootSignature(
				new DataSlot[] { projMatrix, camTransformMatrix, lightingWorld, skyBoxInfo },
				new BufferSlot[] { },
				new TextureSlot[] { },
				new TextureSampler[] { }
			);
			pipelineState = CreatePipelineObject();

			projMatrixBuffer = new ConstBuffer<Matrix>(1);
			camTransformMatrixBuffer = new ConstBuffer<Matrix>(1);
			
		}
		
		private PipelineState CreatePipelineObject() {

			InputElement[] inputLayout = new InputElement[] {
				new InputElement("SV_Position", 0, SharpDX.DXGI.Format.R32G32B32_Float, 0, 0), // 12 bytes
				new InputElement("COLOR", 0, SharpDX.DXGI.Format.R32G32B32A32_Float, 12, 0), // 16 bytes
				new InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, 28, 0), // 8 bytes
				new InputElement("NORMAL", 0, SharpDX.DXGI.Format.R32G32B32A32_Float, 36, 0), // 16 bytes
			};

			RasterizerStateDescription rasterStateDescription = RasterizerStateDescription.Default();
			rasterStateDescription.CullMode = CullMode.None;

			DepthStencilOperationDescription stencilOperationDesc = new DepthStencilOperationDescription() {
				FailOperation = StencilOperation.Keep,
				DepthFailOperation = StencilOperation.Keep,
				PassOperation = StencilOperation.Keep,
				Comparison = Comparison.Always
			};
			DepthStencilStateDescription depthStencilDesc = new DepthStencilStateDescription() {
				IsDepthEnabled = true,
				DepthWriteMask = DepthWriteMask.All,
				DepthComparison = Comparison.LessEqual,
				IsStencilEnabled = false,
				StencilReadMask = 0xff,
				StencilWriteMask = 0xff,
				FrontFace = stencilOperationDesc,
				BackFace = stencilOperationDesc,
			};

			ShaderBytecode vertexShader = CompileShader(".res/Shaders/Skybox/SkyboxVertexShader.hlsl", ShaderType.Vertex);
			ShaderBytecode pixelShader = CompileShader(".res/Shaders/Skybox/SkyboxPixelShader.hlsl", ShaderType.Pixel);



			GraphicsPipelineStateDescription pipelineStateDescription = new GraphicsPipelineStateDescription() {
				InputLayout = inputLayout,
				RootSignature = rootSignature,
				PrimitiveTopologyType = PrimitiveTopologyType.Triangle,
				RasterizerState = rasterStateDescription,
				DepthStencilState = depthStencilDesc,
				DepthStencilFormat = SharpDX.DXGI.Format.D32_Float,
				BlendState = BlendStateDescription.Default(),
				VertexShader = vertexShader,
				PixelShader = pixelShader,
				RenderTargetCount = 1,
				SampleDescription = new SampleDescription(1, 0),
				StreamOutput = new StreamOutputDescription(),
				SampleMask = int.MaxValue,
				Flags = PipelineStateFlags.None,
			};
			pipelineStateDescription.RenderTargetFormats[0] = SharpDX.DXGI.Format.R8G8B8A8_UNorm;

			return Graphics.device.CreateGraphicsPipelineState(pipelineStateDescription);

		}

		
		public override void Render(Camera camera, SharpDX.Direct3D12.Resource renderTarget, DescriptorHeap rtvDescHeap, DescriptorHeap dsvDescHeap) {

			geometryBank.UpdateObjectInfoBuffer();

            // Update the pipeline state and set this shaders root signature
            ArcticFoxEngine.Render.RenderEngine.cmdList.PipelineState = pipelineState;
            ArcticFoxEngine.Render.RenderEngine.cmdList.SetGraphicsRootSignature(rootSignature);

			// Bind the projection data
			projMatrixBuffer.Write(camera.projectionMatrix.Invert(), 0);
			projMatrix.SetData(projMatrixBuffer, 0);

			camTransformMatrixBuffer.Write(camera.transform.worldMatrix, 0);
			camTransformMatrix.SetData(camTransformMatrixBuffer, 0);

			lightingWorld.SetData(LitShader.lightingInfoBuffer, 0);

            // Set geometry
            ArcticFoxEngine.Render.RenderEngine.cmdList.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            ArcticFoxEngine.Render.RenderEngine.cmdList.SetVertexBuffer(0, geometryBank.vertexBufferView);
            ArcticFoxEngine.Render.RenderEngine.cmdList.SetIndexBuffer(geometryBank.indexBufferView);


			// Render each mesh
			for (int i = 0; i < geometryBank.meshRenderers.Count; i++) {

				int currentMeshIndexCount = geometryBank.meshRenderers[i].mesh.indices.Length;
				int vertexBufferStartIndex = geometryBank.GetMeshPosInVertexBuffer(i);
				int indexBufferStartIndex = geometryBank.GetMeshPosInIndexBuffer(i);
				int objectBufferStartIndex = geometryBank.GetMeshPosInObjectBuffer(i);

				// Bind the data from the material
				geometryBank.meshRenderers[i].material.BindResources(this);

                // Draw the mesh
                ArcticFoxEngine.Render.RenderEngine.cmdList.DrawIndexedInstanced(currentMeshIndexCount, 1, indexBufferStartIndex, vertexBufferStartIndex, vertexBufferStartIndex);

			}

		}

		public override Material GetDefaultMaterial() {
			return new SkyboxMaterial();
		}

		
	}

	
}
