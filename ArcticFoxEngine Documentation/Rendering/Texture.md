---
tags:
  - rendering
---

A texture object is a 2D image.
Textures can be used for many things, including:
- Creating and storing images
- Binding to shaders for use in rendering
- Being rendered to


## Setup
To create a texture, instantiate a new texture object.
```csharp title:"Creating a texture"
Texture texture = new Texture(int width, int height, Format format, ResourceFlags flags, ResourceStates initialState);
```

|Type|Parameter|Description|
|---|---|---|
|int|width|The width of the texture in pixels|
|int|height|The height of the texture in pixels|
|Format|format|Optional. Specifies how the texture data is laid out. Encodes how many bits for each channel as well as how many channels|
|ResourceFlags|flags|Optional. DirectX12 Resource Flags to add to the texture|


## Descriptors
A Texture is a Shader Resource and as such may be used within Shaders.
When a texture is created, it also creates a small descriptor heap with a capacity of 1.
The texture then fills this descriptor heap with a shader resource view pointing to the texture.

When a texture is to be used in a shader. The shader resource view is copied into the main rendering descriptor heap