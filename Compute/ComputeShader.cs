using ArcticFoxEngine.Demos.ComputeTest;
using ArcticFoxEngine.Render;
using CoolClassLibrary;
using ImGuiNET;
using SharpDX.Direct3D12;
using SharpDX.D3DCompiler;
using ShaderBytecode = SharpDX.Direct3D12.ShaderBytecode;
using Swan;

namespace ArcticFoxEngine.Compute {
	public class ComputeShader {

		public PipelineState computePipeline;
		public RootSignature rootSignature;

		private GraphicsCommandList computeCmdList;

		//DescriptorHeap descriptorHeap;
		private Dictionary<string, UnorderedAccessBinding> unorderedAccessBindings;
		private Dictionary<string, ConstantBufferBinding> constantBufferBindings;
		private Dictionary<string, ShaderResourceBinding> shaderResourceBindings;

		private Dictionary<string, PipelineState> shaderEntrypoints;

		public ComputeShader(string path, params string[] entrypoints) {

			computeCmdList = Graphics.CreateComputeCommandList();

			unorderedAccessBindings = new Dictionary<string, UnorderedAccessBinding>();
			constantBufferBindings = new Dictionary<string, ConstantBufferBinding>();
			shaderResourceBindings = new Dictionary<string, ShaderResourceBinding>();
			shaderEntrypoints = new Dictionary<string, PipelineState>();

			UpdateShaderResources(path);
			rootSignature = CreateRootSignature();

			for (int i = 0; i < entrypoints.Length; i ++) {
				ShaderBytecode shaderBytecode = CompileShader(path, entrypoints[i]);
				PipelineState computePipeline = CreateComputePipeline(rootSignature, shaderBytecode);
				shaderEntrypoints.Add(entrypoints[i], computePipeline);
			}


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

			string[] shaderBlocks = shaderCode.Split(';', ' ');
			for (int i = 0; i < shaderBlocks.Length; i++) {
				if (shaderBlocks[i].Contains("register") == true) {

					int openBracketIndex = shaderBlocks[i].IndexOf('(');
					int closeBracketIndex = shaderBlocks[i].IndexOf(')');

					string variableType = shaderBlocks[i - 3];
					string variableName = shaderBlocks[i - 2];
					string register = new string(shaderBlocks[i].Skip(openBracketIndex + 1).Take(closeBracketIndex - openBracketIndex - 1).ToArray());
					int registerIndex = int.Parse(new string(register.Skip(1).ToArray()));

					if (register.Contains("u") == true) {
						// Unordered access view
						if (unorderedAccessBindings.ContainsKey(variableName) == true) { continue; }
						UnorderedAccessBinding unorderedAccessBinding = new UnorderedAccessBinding(registerIndex);
						unorderedAccessBindings.Add(variableName, unorderedAccessBinding);
					}
					if (register.Contains("b") == true) {
						// Constant buffer view
						if (constantBufferBindings.ContainsKey(variableName) == true) { continue; }
						ConstantBufferBinding constantBufferBinding = new ConstantBufferBinding(registerIndex);
						constantBufferBindings.Add(variableName, constantBufferBinding);
					}
					if (register.Contains("t") == true) {
						// Shader resource view
						if (shaderResourceBindings.ContainsKey(variableName) == true) { continue; }
						ShaderResourceBinding shaderResourceBinding = new ShaderResourceBinding(registerIndex);
						shaderResourceBindings.Add(variableName, shaderResourceBinding);
					}

					//Log.Info("(Type: " + variableType + ", Name: " + variableName + ", Register: " + register + ")");

				}
			}


		}
		private RootSignature CreateRootSignature() {

			List<RootParameter> rootParameters = new List<RootParameter>();

			for (int i = 0; i < unorderedAccessBindings.Values.Count(); i++) {
				UnorderedAccessBinding uavBinding = unorderedAccessBindings.Values.ElementAt(i);
				uavBinding.AssignRootParameterIndex(rootParameters.Count());
				rootParameters.Add(new RootParameter(ShaderVisibility.All, new DescriptorRange() {
					BaseShaderRegister = uavBinding.registerIndex,
					DescriptorCount = 1,
					OffsetInDescriptorsFromTableStart = int.MinValue,
					RangeType = DescriptorRangeType.UnorderedAccessView,
				}));
			}
			for (int i = 0; i < constantBufferBindings.Values.Count(); i++) {
				ConstantBufferBinding cbvBinding = constantBufferBindings.Values.ElementAt(i);
				cbvBinding.AssignRootParameterIndex(rootParameters.Count());
				rootParameters.Add(new RootParameter(ShaderVisibility.All, new DescriptorRange() {
					BaseShaderRegister = cbvBinding.registerIndex,
					DescriptorCount = 1,
					OffsetInDescriptorsFromTableStart = int.MinValue,
					RangeType = DescriptorRangeType.ConstantBufferView,
				}));
			}
			for (int i = 0; i < shaderResourceBindings.Values.Count(); i++) {
				ShaderResourceBinding srvBinding = shaderResourceBindings.Values.ElementAt(i);
				srvBinding.AssignRootParameterIndex(rootParameters.Count());
				rootParameters.Add(new RootParameter(ShaderVisibility.All, new DescriptorRange() {
					BaseShaderRegister = srvBinding.registerIndex,
					DescriptorCount = 1,
					OffsetInDescriptorsFromTableStart = int.MinValue,
					RangeType = DescriptorRangeType.ShaderResourceView,
				}));
			}

			RootSignatureDescription rootSignatureDesc = new RootSignatureDescription() {
				Parameters = rootParameters.ToArray(),
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

		public void SetTexture(string name, Texture texture) {
			if (unorderedAccessBindings.ContainsKey(name) == true) {
				unorderedAccessBindings[name].AssignResource(texture);
				return;
			}
			if (shaderResourceBindings.ContainsKey(name) == true) {
				shaderResourceBindings[name].AssignResource(texture);
				return;
			}
		}
		public void SetBuffer<T>(string name, StructuredBuffer<T> buffer) where T : struct {
			unorderedAccessBindings[name].AssignResource(buffer);
		}
		public void SetBuffer<T>(string name, ConstantBuffer<T> buffer) where T : struct {
			constantBufferBindings[name].AssignResource(buffer);
		}

		public void Dispatch(string entrypoint, int threadGroupsX, int threadGroupsY, int threadGroupsZ) {

			// Potential optimisation!
			// You dont have to copy descriptors always!
			// If the descriptor wasnt overwritten, it will retain your descriptor
			// Maybe if a descriptor was overwritten, notify whatever resource it belonged to
			// and let it know it needs to be copied again

			Graphics.WaitForComputeCommandQueue();
			Graphics.ResetComputeCommandList(computeCmdList);

			computeCmdList.SetDescriptorHeaps(RenderEngine.gpuDescriptorHeap);
			computeCmdList.PipelineState = shaderEntrypoints[entrypoint];
			computeCmdList.SetComputeRootSignature(rootSignature);

			for (int i = 0; i < unorderedAccessBindings.Values.Count(); i++) {
				UnorderedAccessBinding currentUAV = unorderedAccessBindings.Values.ElementAt(i);
				currentUAV.ResourceTransitionToUA(computeCmdList);
				currentUAV.BindResource(computeCmdList);
			}
			for (int i = 0; i < constantBufferBindings.Values.Count(); i++) {
				ConstantBufferBinding currentCBV = constantBufferBindings.Values.ElementAt(i);
				currentCBV.ResourceTransitionToGenericRead(computeCmdList);
				currentCBV.BindResource(computeCmdList);
			}
			for (int i = 0; i < shaderResourceBindings.Values.Count(); i++) {
				ShaderResourceBinding currentSRV = shaderResourceBindings.Values.ElementAt(i);
				currentSRV.ResourceTransitionToGenericRead(computeCmdList);
				currentSRV.BindResource(computeCmdList);
			}

			computeCmdList.Dispatch(threadGroupsX, threadGroupsY, threadGroupsZ);

			for (int i = 0; i < unorderedAccessBindings.Values.Count(); i++) {
				UnorderedAccessBinding currentUAV = unorderedAccessBindings.Values.ElementAt(i);
				currentUAV.ResourceTransitionFromUA(computeCmdList);
			}
			for (int i = 0; i < constantBufferBindings.Values.Count(); i++) {
				ConstantBufferBinding currentCBV = constantBufferBindings.Values.ElementAt(i);
				currentCBV.ResourceTransitionFromGenericRead(computeCmdList);
			}
			for (int i = 0; i < shaderResourceBindings.Values.Count(); i++) {
				ShaderResourceBinding currentSRV = shaderResourceBindings.Values.ElementAt(i);
				currentSRV.ResourceTransitionFromGenericRead(computeCmdList);
			}

			computeCmdList.Close();

			Graphics.ExecuteComputeCommandList(computeCmdList);


		}


	}
}
