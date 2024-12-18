using ArcticFoxEngine.Nodes;
using CoolClassLibrary;
using SharpDX;
using SharpDX.Direct3D12;
using SharpDX.DXGI;
using System;
using System.IO;
using static ArcticFoxEngine.Graphics;
using static ArcticFoxEngine.Rendering.Shader;
using Resource = SharpDX.Direct3D12.Resource;

namespace ArcticFoxEngine.Rendering {

	public abstract class Shader : IDisposable {

		public static class Cache {

			// The key of the dictionary is the type of the shader
			// The value of the dictionary is a pair of a Shader and an int
			// The shader is the instance of the shader, and the int is the number of refrences 
			private static Dictionary<Type, (Shader, int)> shaderCache;
			
			static Cache() {
				shaderCache = new Dictionary<Type, (Shader, int)>();
			}

			public static Shader FindOrLoad(Type type) {

				if (type.BaseType != typeof(Shader)) {
					Log.Warn("Cannot load shader of type " + type.Name + ", not a shader");
					return null;
				}

				if (shaderCache.ContainsKey(type) == true) {

					(Shader cachedShader, int numRefs) = shaderCache[type];
					shaderCache[type] = (cachedShader, numRefs + 1);

					return cachedShader;
				}


				Shader newShader = (Shader)Activator.CreateInstance(type);
				shaderCache.Add(type, (newShader, 1));

				return newShader;

			}

			public static void Release(Type type) {

				if (shaderCache.ContainsKey(type) == true) {

					// Retrieve the cached texture
					(Shader cachedShader, int numRefs) = shaderCache[type];

					// update the number of refrences to this texture
					shaderCache[type] = (cachedShader, numRefs - 1);

					// If there are no more refrences to this texture, dispose it and remove the dictionary entry
					if (numRefs == 0) {
						cachedShader.Dispose();
						shaderCache.Remove(type);
					}

					return;

				}

				Log.Warn("Cannot release shader from cache, not added to cache");

			}

			public static void Release(Shader shader) {

				for (int i = 0; i < shaderCache.Count; i ++) {
					
					if (shaderCache.ElementAt(i).Value.Item1 == shader) {
						Release(shaderCache.ElementAt(i).Key);
						return;
					}

				}

			}

		}

		
		
		public class TextureSampler {

			public TextureSampler(ShaderVisibility shaderVisibility) {

				StaticSamplerDescription defaultOptions = new StaticSamplerDescription();

				this.shaderVisibility = shaderVisibility;

				addressU = defaultOptions.AddressU;
				addressV = defaultOptions.AddressV;
				addressW = defaultOptions.AddressW;

				borderCol = defaultOptions.BorderColor;
				comparisonFunc = defaultOptions.ComparisonFunc;
				filter = defaultOptions.Filter;
				maxAnisotropy = defaultOptions.MaxAnisotropy;

				maxLOD = defaultOptions.MaxLOD;
				minLOD = defaultOptions.MinLOD;
				mipLODBias = defaultOptions.MipLODBias;

			}

			public ShaderVisibility shaderVisibility;

			public TextureAddressMode addressU;
			public TextureAddressMode addressV;
			public TextureAddressMode addressW;
			public TextureAddressMode addressUVW {
				set {
					addressU = value;
					addressV = value;
					addressW = value;
				}
			}

			public StaticBorderColor borderCol;
			public Comparison comparisonFunc;
			public Filter filter;
			public int maxAnisotropy;

			public float maxLOD;
			public float minLOD;
			public float mipLODBias;

		}

		public abstract string name { get; }

		public PipelineState pipelineState;
		public RootSignature rootSignature;

		public GeometryInfo geometryResources;

		public Shader() {

			geometryResources = new GeometryInfo();

		}
		public abstract Material GetDefaultMaterial();


		public enum ShaderType {
			Vertex,
			Geometry,
			Pixel,
		}
		
		/// <summary>
		/// Compiles the shader specified by the path
		/// </summary>
		/// <param name="path">Path to the shader code</param>
		/// <param name="type">The type of shader being compiled</param>
		/// <returns>The bytecode for that shader</returns>
		public static ShaderBytecode CompileShader(string path, ShaderType type) {

			#region Changing root folder of #includes

			string rootPath = "";
			for (int i = path.Length - 1; i >= 0; i--) {
				if (path[i] == '/') {
					rootPath = new string(path.Take(i + 1).ToArray());
					break;
				}
			}

			string shaderCode = File.ReadAllText(path);
			string includeDirective = "#include \"";

			string includeEditedShaderCode = "";
			for (int i = 0; i < shaderCode.Length; i++) {


				if (includeDirective.Length == 0) {
					includeEditedShaderCode += rootPath;
					includeDirective = "#include \"";
				}
				else {
					if (shaderCode[i] == includeDirective[0]) {
						includeDirective = new string(includeDirective.Skip(1).ToArray());
					}
					else {
						includeDirective = "#include \"";
					}
				}

				includeEditedShaderCode += shaderCode[i];
			}



			#endregion


			SharpDX.D3DCompiler.ShaderFlags flags = isDebug ? SharpDX.D3DCompiler.ShaderFlags.None : SharpDX.D3DCompiler.ShaderFlags.Debug;
			SharpDX.D3DCompiler.Include include = new StandardIncludeHandler();

			string entrypoint = "";
			string profile = "";

			switch (type) {

				case ShaderType.Vertex:
				entrypoint = "Vertex_Main";
				profile = "vs_5_0";
				break;

				case ShaderType.Geometry:
				entrypoint = "Geometry_Main";
				profile = "gs_5_0";
				break;

				case ShaderType.Pixel:
				entrypoint = "Pixel_Main";
				profile = "ps_5_0";
				break;

			}
			ShaderBytecode compiledShader = null;
			try {
				compiledShader = new ShaderBytecode(SharpDX.D3DCompiler.ShaderBytecode.Compile(includeEditedShaderCode, entrypoint, profile, flags, SharpDX.D3DCompiler.EffectFlags.None, new SharpDX.Direct3D.ShaderMacro[0], include));
				return compiledShader;
			}
			catch (Exception e) {
				switch (type) {
					case ShaderType.Vertex:
					Log.Error("Failed to compile vertex shader");
					break;

					case ShaderType.Geometry:
					Log.Error("Failed to compile geometry shader");
					break;

					case ShaderType.Pixel:
					Log.Error("Failed to compile pixel shader");
					break;
				}
				Log.Raw(e);
			}

			return null;



		}


		protected RootSignature CreateRootSignature(DataSlot[] dataSlots, BufferSlot[] bufferSlots, TextureSlot[] textureSlots, TextureSampler[] textureSamplers) {


			List<RootParameter> rootParameters = new List<RootParameter>();
			List<StaticSamplerDescription> samplerDescriptions = new List<StaticSamplerDescription>();

			for (int i = 0; i < dataSlots.Length; i ++) {

				DataSlot currentDataSlot = dataSlots[i];
				currentDataSlot.rootParameterIndex = i; // What root parameter does this correspond to
				dataSlots[i] = currentDataSlot;

				// Create a new Root parameter for the dataSlot
				RootParameter newRootParam = new RootParameter(currentDataSlot.shaderVisibility, new DescriptorRange() {
					RangeType = DescriptorRangeType.ConstantBufferView,
					BaseShaderRegister = i, // What index is this buffer out of all the buffers
					OffsetInDescriptorsFromTableStart = int.MinValue,
					DescriptorCount = 1,
				});
				rootParameters.Add(newRootParam);

			}
			
			for (int i = 0; i < bufferSlots.Length; i++) {

				BufferSlot currentBufferSlot = bufferSlots[i];
				currentBufferSlot.rootParameterIndex = dataSlots.Length + i; // What root parameter does this correspond to
				bufferSlots[i] = currentBufferSlot;


				// Create a new Root parameter for the buffer
				RootParameter newRootParam = new RootParameter(bufferSlots[i].shaderVisibility, new DescriptorRange() {
					RangeType = DescriptorRangeType.ShaderResourceView,
					BaseShaderRegister = i, // What index is this buffer out of all the buffers and textures
					OffsetInDescriptorsFromTableStart = int.MinValue,
					DescriptorCount = 1,
				});
				rootParameters.Add(newRootParam);


			}

			for (int i = 0; i < textureSlots.Length; i++) {

				TextureSlot currentTextureSlot = textureSlots[i];
				currentTextureSlot.rootParameterIndex = dataSlots.Length + bufferSlots.Length + i; // What root parameter does this correspond to
				textureSlots[i] = currentTextureSlot;

				RootParameter newRootParam = new RootParameter(textureSlots[i].shaderVisibility, new DescriptorRange() {
					RangeType = DescriptorRangeType.ShaderResourceView,
					BaseShaderRegister = bufferSlots.Length + i,
					OffsetInDescriptorsFromTableStart = int.MinValue,
					DescriptorCount = 1,
				});
				rootParameters.Add(newRootParam);

			}

			for (int i = 0; i < textureSamplers.Length; i ++) {

				StaticSamplerDescription desc = new StaticSamplerDescription(textureSamplers[i].shaderVisibility, i, 0);
				desc.AddressU = textureSamplers[i].addressU;
				desc.AddressV = textureSamplers[i].addressV;
				desc.AddressW = textureSamplers[i].addressW;

				desc.BorderColor = textureSamplers[i].borderCol;
				desc.ComparisonFunc = textureSamplers[i].comparisonFunc;
				desc.Filter = textureSamplers[i].filter;
				desc.MaxAnisotropy = textureSamplers[i].maxAnisotropy;

				desc.MaxLOD = textureSamplers[i].maxLOD;
				desc.MinLOD = textureSamplers[i].minLOD;
				desc.MipLODBias = textureSamplers[i].mipLODBias;

				samplerDescriptions.Add(desc);


			}

			RootSignatureDescription rootSignatureDesc = new RootSignatureDescription(RootSignatureFlags.AllowInputAssemblerInputLayout, rootParameters.ToArray(), samplerDescriptions.ToArray());
			return Graphics.device.CreateRootSignature(rootSignatureDesc.Serialize());

			

		}

		public abstract void Render(Camera camera, Resource renderTarget, DescriptorHeap rtvDescHeap, DescriptorHeap dsvDescHeap);

		public void DefaultRender(Camera camera, Resource renderTarget, DescriptorHeap rtvDescHeap, DescriptorHeap dsvDescHeap, DataSlot projectionInfoDataSlot, DataSlot transformInfoDataSlot) {

			geometryResources.UpdateObjectInfoBuffer();

			


			// Bind the projection data
			projectionInfoDataSlot.SetData(Rendering.projectionInfo, 0);

			// Bind the shader global data


			// Set geometry
			Rendering.cmdList.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
			Rendering.cmdList.SetVertexBuffer(0, geometryResources.vertexBufferView);
			Rendering.cmdList.SetIndexBuffer(geometryResources.indexBufferView);


			// Render each mesh
			for (int i = 0; i < geometryResources.meshRenderers.Count; i++) {

				int currentMeshIndexCount = geometryResources.meshRenderers[i].mesh.indices.Length;
				int vertexBufferStartIndex = geometryResources.GetMeshPosInVertexBuffer(i);
				int indexBufferStartIndex = geometryResources.GetMeshPosInIndexBuffer(i);
				int objectBufferStartIndex = geometryResources.GetMeshPosInObjectBuffer(i);

				// Bind the transform data
				transformInfoDataSlot.SetData(geometryResources.transformBuffer, objectBufferStartIndex);

				// Bind the data from the material
				geometryResources.meshRenderers[i].material.BindResources(this);

				// Draw the mesh
				Rendering.cmdList.DrawIndexedInstanced(currentMeshIndexCount, 1, indexBufferStartIndex, vertexBufferStartIndex, vertexBufferStartIndex);

			}

		}



		// Cleanup
		bool disposed = true;
		~Shader() {
			Dispose();
		}
		public void Dispose() {
			if (disposed == true) { return; }
			disposed = true;

			pipelineState.Dispose();
			rootSignature.Dispose();

			geometryResources.Dispose();

		}

	}

}
