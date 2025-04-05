using ArcticFoxEngine.Render;
using SharpDX.Direct3D12;
using static ArcticFoxEngine.Render.Shader;

namespace ArcticFoxEngine.Compute {
	public class ComputeShader {

		public PipelineState computePipeline;
		public RootSignature rootSignature;

		private GraphicsCommandList computeCmdList;
		private DescriptorHeap descriptorHeap;

		Texture boundTexture;

		public ComputeShader(string path) {
			computeCmdList = Graphics.CreateComputeCommandList();

			ShaderBytecode shaderBytecode = CompileShader(path, "main");

			RootSignatureDescription rootSignatureDesc = new RootSignatureDescription() {
				Parameters = new RootParameter[] {
					new RootParameter(ShaderVisibility.All, new DescriptorRange() {
						BaseShaderRegister = 0,
						DescriptorCount = 1,
						OffsetInDescriptorsFromTableStart = int.MinValue,
						RangeType = DescriptorRangeType.UnorderedAccessView,
					}),
				},
			};
			rootSignature = Graphics.device.CreateRootSignature(rootSignatureDesc.Serialize());
			computePipeline = CreateComputePipeline(rootSignature, shaderBytecode);


			DescriptorHeapDescription descriptorHeapDesc = new DescriptorHeapDescription() {
				DescriptorCount = 1,
				Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView,
				Flags = DescriptorHeapFlags.ShaderVisible,
			};
			descriptorHeap = Graphics.device.CreateDescriptorHeap(descriptorHeapDesc);



		}

		private ShaderBytecode CompileShader(string path, string entrypoint) {

			string shaderCode = File.ReadAllText(path);
			string profile = "cs_5_0";
			SharpDX.D3DCompiler.ShaderFlags flags = SharpDX.D3DCompiler.ShaderFlags.None;
			SharpDX.D3DCompiler.Include include = new StandardIncludeHandler();

			ShaderBytecode compiledShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.Compile(shaderCode, entrypoint, profile, flags, SharpDX.D3DCompiler.EffectFlags.None, new SharpDX.Direct3D.ShaderMacro[0], include));
			return compiledShader;

		}

		private PipelineState CreateComputePipeline(RootSignature rootSignature, ShaderBytecode shaderBytecode) {

			ComputePipelineStateDescription computePipelineDesc = new ComputePipelineStateDescription() {
				RootSignaturePointer = rootSignature,
				ComputeShader = shaderBytecode,
			};
			PipelineState state = Graphics.device.CreateComputePipelineState(computePipelineDesc);

			return state;

		}

		public void SetTexture(Texture texture) {

			CpuDescriptorHandle srcDescriptor = texture.descriptorHeap.CPUDescriptorHandleForHeapStart + Graphics.descriptorHeapIncrement;
			CpuDescriptorHandle dstDescriptor = descriptorHeap.CPUDescriptorHandleForHeapStart;

			Graphics.device.CopyDescriptorsSimple(1, dstDescriptor, srcDescriptor, DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);

			boundTexture = texture;

		}

		public void Dispatch() {

			Graphics.WaitForComputeCommandQueue();
			Graphics.ResetComputeCommandList(computeCmdList);

			// TODO: Combine this descriptor heap into the one used for rendering. For now, im just trying to get it to work.
			computeCmdList.SetDescriptorHeaps(descriptorHeap);
			computeCmdList.PipelineState = computePipeline;
			computeCmdList.SetComputeRootSignature(rootSignature);

			computeCmdList.ResourceBarrierTransition(boundTexture.resource, Texture.defaultState, ResourceStates.UnorderedAccess);
			
			computeCmdList.SetComputeRootDescriptorTable(0, descriptorHeap.GPUDescriptorHandleForHeapStart);

			computeCmdList.Dispatch(MainWindow.width / 8, MainWindow.height / 8, 1);

			computeCmdList.ResourceBarrierTransition(boundTexture.resource, ResourceStates.UnorderedAccess, Texture.defaultState);
			computeCmdList.Close();

			Graphics.ExecuteComputeCommandList(computeCmdList);

		}


	}
}
