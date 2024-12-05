The Graphics Pipeline State is an object which describes to the GPU how to render meshes.
It is created by filling out a GraphicsPipelineStateDescription object, then using the device to create the Graphics Pipeline State from that

## Contents
The Graphics Pipeline State contains the following
- Input Layout
- Primitive Topology Type
- Root Signature
- Rasterizer State
- Depth Stencil State
- Blend State
- Vertex Shader
- Hull Shader
- Domain Shader
- Geometry Shader
- Pixel Shader
- Render Target Count
- Render Target Format
- Sample Description
- Stream Output
- Sample Mask
- Flags


This is not everything the Graphics Pipeline State includes but these are the main ones

### Input Layout
The Input Layout specifies the layout of the data stored in each vertex.
Here is an example of how you might organise vertex data

|Semantic name|Format|
|---|---|
|SV_Position|R32 G32 B32 Float|
|COLOR|R32 G32 B32 A32 Float|
|TEXCOORD|R32 G32 Float|
|Normal|R32 G32 B32 A32 Float|

```csharp title="Input Layout setup"
InputElement[] inputLayout = new InputElement[] {
	new InputElement("SV_Position", 0, Format.R32G32B32_Float, 0, 0), // 12 bytes
	new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 12, 0), // 16 bytes
	new InputElement("TEXCOORD", 0, Format.R32G32_Float, 28, 0), // 8 bytes
	new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 36, 0), // 16 bytes
}
```

### Primitive Topology Type
Primitive Topology Type specifies what kind of topology should be used for the primitives in rendering.
In most cases, just use
```csharp
PrimitiveTopologyType primitiveTopology = PrimitiveTopologyType.Triangle;
```

### Root Signature
![[Root Signature]]
If you are working in the Shader class, you can create a Root Signature automatically with the Create 

### Rasterizer State
The Rasterizer State describes how the screen space vertex positions should be turned into pixels.
If you are unsure about what to use here, go with.
```csharp title="Default Rasterizer State setup"
RasterizerStateDescription rasterState = RasterizerStateDescription.Default();
```

### Depth Stencil State
The Depth Stencil State describes how to decide how to combine renders of different depths.
A good default Depth Stencil State looks like this
```csharp title="Default Depth Stencil State setup"
DepthStencilOperationDescription stencilOperationDesc = new DepthStencilOperationDescription() {
	FailOperation = StencilOperation.Keep,
	DepthFailOperation = StencilOperation.Keep,
	PassOperation = StencilOperation.Keep,
	Comparison = Comparison.Always
};
DepthStencilStateDescription depthStencilDesc = new DepthStencilStateDescription() {
	IsDepthEnabled = true,
	DepthWriteMask = DepthWriteMask.All,
	DepthComparison = Comparison.Less,
	IsStencilEnabled = false,
	StencilReadMask = 0xff,
	StencilWriteMask = 0xff,
	FrontFace = stencilOperationDesc,
	BackFace = stencilOperationDesc,
};
```

### Blend State
Not really sure what this one is exactly doing.
If you are unsure about what to put here, do.
```csharp title="Default Blend State setup"
BlendStateDescription blendDesc = BlendStateDescription.Default();
```

### Vertex Shader
The Vertex Shader is a shader that works on the verticies of a mesh.
Shaders are compiled with ```Shader.CompileShader```

### Hull Shader
The Hull Shader is an optional shader. Leave this one blank.

### Domain Shader
The Domain Shader is an optional shader. Leave this one blank.

### Geometry Shader
The Geometry Shader is a shader stage that happens after the vertex shader.
In ArcticFoxEngine the Geometry Shader is used to
- Calculate normal vectors
- Calculate tangent vectors
- Projects geometry into world space
- Projects geometry into screen space
Shaders are compiled with ```Shader.CompileShader```

### Pixel Shader
The Pixel Shader works on every pixel about to be rendered to the screen and is used to calculate lighting effects, and other shadery goodness
Shaders are compiled with ```Shader.CompileShader```

### Render Target Count
Render Target Count specifies how many render targets are being rendered to.
If you are unsure about what to put here
```csharp
RenderTargetCount = 1;
```

### Render Target Format
The Render Target Format specifies what format the render target is. It is an array as there may be multiple render targets
If you are unsure about what to put here
```csharp
GraphicsPipelineStateDescription pipelineStateDescription = new GraphicsPipelineStateDescription();
pipelineStateDescription.RenderTargetFormats[0] = Format.R8G8B8A8_UNorm;
```

### Sample Description
If you are unsure about what to put here, put
```csharp
SampleDescription sampleDescription = new SampleDescription(1, 0);
```

### Stream Output
If you are unsure about what to put here, put
```csharp
StreamOutputDescription streamOutputDescription = new StreamOutputDescription();
```


### Sample Mask
If you are unsure about what to put here, put
```csharp
int sampleMask = int.MaxValue;
```

### Flags
If you are unsure about what to put here, put
```csharp
PipelineStateFlags flags = PipelineStateFlags.None;
```