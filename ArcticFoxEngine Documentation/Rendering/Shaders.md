SHADERS SHOULD NOT NEED TO BE STORED ANYWHERE TO BE USED.
ALL THAT SHOULD BE NEEDED IS THE SHADER OBJECT ITSELF.

SHADERS ARE STORED IN RENDERING ONLY SO THEY CAN BE RETRIEVED WHEN NEEDED

Shaders serve as the bridge between the CPU and the GPU for rendering meshes. They contain all the information necessary for drawing an object and for binding data used for drawing

Shader is an abstract class that facilitates creating and using shaders for rendering, as well as binding data to the shader
Each shader represents one way to render an object

## Setup
To create a Shader, they require the following
- Name
- [[Graphics Pipeline State]]
- [[Root Signature]]
- [[#DataSlot]] (optional)
- [[#BufferSlot]] (optional)
- [[#TextureSlot]] (optional)
- An associated material


Before exiting the constructor, the shader must assign the "pipelineState" variable to a GraphicsPipelineState Object as well as the "rootSignature" variable to a RootSignature


## Resource Binding
One of the things the Shader class helps with the most is binding data to the GPU. It abstracts creating the [[Root Signature]] into 3 functions. 
- CreateDataSlot
- CreateBufferSlot
- CreateTextureSlot


### Shader Registers
Each DataSlot, BufferSlot and TextureSlot takes up a shader register. This is how you specify which variables are connected to what slots in HLSL.
For example
```c
Texture2D mainTex : register(t0);
Texture2D normalTex : register(t1);

cbuffer LightInfo : register(b0) {
	float3 position;
	float3 colour;
}

sampler defaultSampler : register(s0);
```

There are 4 kinds of shader registers

|Register Type|Description|
|---|---|
|t|Shader Resource Views (SRV)|
|u|Unordered Access Views (UAV)|
|b|Constant Buffer Views (CBV)|
|s|Samplers|

Each DataSlot takes up one kind of shader register

|Slot Type|Register|
|---|:---:|
|DataSlot|b|
|BufferSlot|t|
|TexutreSlot|t|


### DataSlot
A DataSlot is used to bind one group of data to a shader. You would use a DataSlot to get access to 1 struct / primitive in the shader. It can be rebound between drawing objects to get different data per object.

For shader global data, this is how the DataSlot should be used
```csharp title:"Global data using a DataSlot - C# Side"
public class MyShader : Shader {

	// Structure of the data to be set to the DataSlot
	struct RenderInfo {
		public Matrix projectionMatrix; // 64 bytes
		public int screenWidth;         // 4 bytes
		public int screenHeight;        // 4 bytes
		public float aspectRatio;      // 4 bytes
	}
	
	// Data to be set into a DataSlot must first be stored in a ConstBuffer
	ConstBuffer<RenderInfo> renderInfoBuffer;


	RenderInfo renderInfo;
	
	public MyShader() {
		CreateDataSlot("Camera info", ShaderVisibility.All); // Create the DataSlot
		renderInfo = new RenderInfo();
		
		renderInfoBuffer = new ConstBuffer<RenderInfo>(1); // Create the buffer that will have its 1st element set to the DataSlot
		renderInfoBuffer.Write(new RenderInfo[] {renderInfo}, 0); // Does not matter when the buffer is written to
		
		Finalise();
	}
	
	protected override void SetGlobalData() {
		renderInfoBuffer.Write(new RenderInfo[] {renderInfo}, 0); // Does not matter when the buffer is written to
		SetDataSlot("Camera info",renderInfoBuffer, 0); // Set the 1st element of the buffer into the DataSlot
	}
	
}
```

For object specific data, this is how the DataSlot should be used. The DataSlot should be created in the shader first!
```csharp title="Object data using a DataSlot - C# Side"
public class MyMaterial : Material {
	
	struct MaterialProperties {
		
		public float normalStrength;
		public float textureScale;
		
	}
	
	ConstBuffer<MaterialProperties> materialPropertiesBuffer;
	MaterialProperties materialProperties;
	
	public MyMaterial() {
		materialProperties = new MaterialProperties();
		
		materialPropertiesBuffer = new ConstBuffer<MaterialProperties>(1);
		materialPropertiesBuffer.Write(new MaterialProperties[] {materialProperties}, 0); // Does not matter when the buffer is written to
	}
	
	public override void BindResources(Shader shader) {
		materialPropertiesBuffer.Write(new MaterialProperties[] { materialProperties }, 0); // Does not matter when the buffer is written to
		shader.SetDataSlot("Material info", materialPropertiesBuffer, 0);
	}
	
}
```

Then to access these in the shader

```c title="Retreiving data from a Dataslot - HLSL Side"
cbuffer MaterialProperties : register(b0) {
	float normalStrength;
	float textureScale;
}

float4 Pixel_Main(Vertex input) : SV_Target {
	float retrievedNormalStrength = normalStrength;
	float retrievedTextureScale = textureScale;
	
	return flaot4(0.0, 0.0, 0.0, 0.0);
}
```

### BufferSlot
A BufferSlot works simmilarly to a DataSlot but can store any number of elements. You would use a BufferSlot to get access to an array of structs / primitives.
This is how a BufferSlot should be used
```csharp title="Using a BufferSlot - C# Side"
public class MyShader : Shader {
	StructuredBuffer<Vector3> lightPosBuffer;
	
	public MyShader() {
		CreateBufferSlot("Light pos", lightPosBuffer.numElements, ShaderVisibility.Pixel);
		
		lightPosBuffer = new StructuredBuffer<Vector3>(16);
		Vector3[] lightPositions = new Vector3[16];
		lightPosBuffer.Write(lightPositions, lightPositions.Length);	
	}
	
	protected override void SetGlobalData() {
		SetBufferSlot("Light pos", lightPosBuffer, 0);
	}

}
```

To access a BufferSlot in the shader
```c title="Using a BufferSlot - HLSL Side"
StructuredBuffer<float3> lightPositions : register(t0);

float4 Pixel_Main(Vertex input) : SV_Target {
	float3 firstLightPos = lightPositions[0];
	float3 seccondLightPos = lightPositions[1];
	// ect...
	
	return float4(0.0, 0.0, 0.0, 0.0);
}
```

### TextureSlot
Like the name suggests, TextureSlots are used to bind textures to shaders. You would use a TextureSlot to get access to a Texture in the shader
This is how you would use a TextureSlot
```csharp title="Using a TextureSlot - C# Side"
public class MyShader : Shader {
	
	Texture normalTexture;
	
	public MyShader() {
		CreateTextureSlot("Normal texture", lightPosBuffer.numElements, ShaderVisibility.Pixel);
		
		normalTexture = new Texture(".res/Textures/BrickNormal.png");
	}
	
	protected override void SetGlobalData() {
		SetTextureSlot("Normal texture", normalTexture);
		
	}
	
}
```

