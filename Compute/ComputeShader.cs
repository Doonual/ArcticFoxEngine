using SharpDX.Direct3D12;

namespace ArcticFoxEngine.Compute {
	public class ComputeShader {

		public PipelineState computePipeline;
		public RootSignature rootSignature;

		private GraphicsCommandList computeCmdList;

		DescriptorHeap descriptorHeap;
		private Dictionary<string, TextureBinding> textureBindings;

		public ComputeShader(string path) {

			computeCmdList = Graphics.CreateComputeCommandList();

			textureBindings = new Dictionary<string, TextureBinding>();

			ShaderBytecode shaderBytecode = CompileShader(path, "main");
			UpdateShaderResources(path);

			rootSignature = CreateRootSignature(textureBindings.Values.ToList());
			computePipeline = CreateComputePipeline(rootSignature, shaderBytecode);

			DescriptorHeapDescription descriptorHeapDesc = new DescriptorHeapDescription() {
				DescriptorCount = textureBindings.Count,
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

		private void UpdateShaderResources(string path) {

			string shaderCode = File.ReadAllText(path);

			textureBindings.Clear();

			string[] shaderBlocks = shaderCode.Split(';', ' ');
			for (int i = 0; i < shaderBlocks.Length; i++) {
				if (shaderBlocks[i].Contains("register") == true) {

					int openBracketIndex = shaderBlocks[i].IndexOf('(');
					int closeBracketIndex = shaderBlocks[i].IndexOf(')');

					string variableType = shaderBlocks[i - 3];
					string variableName = shaderBlocks[i - 2];
					string register = new string(shaderBlocks[i].Skip(openBracketIndex + 1).Take(closeBracketIndex - openBracketIndex - 1).ToArray());
					int registerIndex = int.Parse(new string(register.Skip(1).ToArray()));

					if (variableType.Contains("Texture") == true) {
						TextureBinding textureBinding = new TextureBinding(registerIndex);
						textureBindings.Add(variableName, textureBinding);
					}

					//Log.Info("(Type: " + variableType + ", Name: " + variableName + ", Register: " + register + ")");



				}
			}


		}
		private RootSignature CreateRootSignature(List<TextureBinding> textureBindings) {

			RootParameter[] rootParameters = new RootParameter[textureBindings.Count()];
			for (int i = 0; i < textureBindings.Count(); i++) {
				rootParameters[i] = new RootParameter(ShaderVisibility.All, new DescriptorRange() {
					BaseShaderRegister = i,
					DescriptorCount = 1,
					OffsetInDescriptorsFromTableStart = int.MinValue,
					RangeType = DescriptorRangeType.UnorderedAccessView,
				});
			}

			RootSignatureDescription rootSignatureDesc = new RootSignatureDescription() {
				Parameters = rootParameters,
			};
			RootSignature rootSignature = Graphics.device.CreateRootSignature(rootSignatureDesc.Serialize());
			return rootSignature;

		}

		private PipelineState CreateComputePipeline(RootSignature rootSignature, ShaderBytecode shaderBytecode) {

			ComputePipelineStateDescription computePipelineDesc = new ComputePipelineStateDescription() {
				RootSignaturePointer = rootSignature,
				ComputeShader = shaderBytecode,
			};
			PipelineState state = Graphics.device.CreateComputePipelineState(computePipelineDesc);

			return state;

		}

		public void SetTexture(Texture texture, string name) {
			textureBindings[name].AssignTexture(texture);
		}

		public void Dispatch() {

			Graphics.WaitForComputeCommandQueue();
			Graphics.ResetComputeCommandList(computeCmdList);

			// TODO: Combine this descriptor heap into the one used for rendering. For now, im just trying to get it to work.
			computeCmdList.SetDescriptorHeaps(descriptorHeap);
			computeCmdList.PipelineState = computePipeline;
			computeCmdList.SetComputeRootSignature(rootSignature);

			List<TextureBinding> textureBindingsToBind = textureBindings.Values.ToList();
			for (int i = 0; i < textureBindingsToBind.Count(); i++) {
				textureBindingsToBind[i].ResourceTransitionToUA(computeCmdList);
				textureBindingsToBind[i].BindTexture(computeCmdList, descriptorHeap);
			}

			computeCmdList.Dispatch(MainWindow.width / 8, MainWindow.height / 8, 1);

			for (int i = 0; i < textureBindingsToBind.Count(); i++) {
				textureBindingsToBind[i].ResourceTransitionFromUA(computeCmdList);
			}

			computeCmdList.Close();

			Graphics.ExecuteComputeCommandList(computeCmdList);


		}


	}
}
