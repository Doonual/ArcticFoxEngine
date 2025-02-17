using ArcticFoxEngine.Nodes;
using SharpDX.Direct3D12;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static ArcticFoxEngine.Rendering.LitShader;

namespace ArcticFoxEngine.Rendering {


	internal class SkyboxShader : Shader {

		public override string name => "Skybox";

		public DataSlot projMatrix = new DataSlot(ShaderVisibility.Pixel);
		public DataSlot camTransformMatrix = new DataSlot(ShaderVisibility.Pixel);
		public DataSlot lightingWorld = new DataSlot(ShaderVisibility.Pixel);
		public DataSlot skyBoxInfo = new DataSlot(ShaderVisibility.Pixel);

		private ConstBuffer<Matrix> projMatrixBuffer;
		private ConstBuffer<Matrix> camTransformMatrixBuffer;
		

		public struct SkyboxInfo {

			public Vector3 skyBottomCol;
			public float sunStrength;

			public Vector3 skyTopCol;
			public float horizonSharpness;

			public Vector3 groundTopCol;
			public float dummy;

			public Vector3 groundBottomCol;


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

			geometryResources.UpdateObjectInfoBuffer();

            // Update the pipeline state and set this shaders root signature
            Rendering.Render.cmdList.PipelineState = pipelineState;
            Rendering.Render.cmdList.SetGraphicsRootSignature(rootSignature);

			// Bind the projection data
			projMatrixBuffer.Write(camera.projectionMatrix.Invert(), 0);
			projMatrix.SetData(projMatrixBuffer, 0);

			camTransformMatrixBuffer.Write(camera.transform.worldMatrix, 0);
			camTransformMatrix.SetData(camTransformMatrixBuffer, 0);

			lightingWorld.SetData(LitShader.lightingInfoBuffer, 0);

            // Set geometry
            Rendering.Render.cmdList.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            Rendering.Render.cmdList.SetVertexBuffer(0, geometryResources.vertexBufferView);
            Rendering.Render.cmdList.SetIndexBuffer(geometryResources.indexBufferView);


			// Render each mesh
			for (int i = 0; i < geometryResources.meshRenderers.Count; i++) {

				int currentMeshIndexCount = geometryResources.meshRenderers[i].mesh.indices.Length;
				int vertexBufferStartIndex = geometryResources.GetMeshPosInVertexBuffer(i);
				int indexBufferStartIndex = geometryResources.GetMeshPosInIndexBuffer(i);
				int objectBufferStartIndex = geometryResources.GetMeshPosInObjectBuffer(i);

				// Bind the data from the material
				geometryResources.meshRenderers[i].material.BindResources(this);

                // Draw the mesh
                Rendering.Render.cmdList.DrawIndexedInstanced(currentMeshIndexCount, 1, indexBufferStartIndex, vertexBufferStartIndex, vertexBufferStartIndex);

			}

		}

		public override Material GetDefaultMaterial() {
			return new SkyboxMaterial();
		}

		
	}

	
}
