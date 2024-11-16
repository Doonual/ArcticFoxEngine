Graphics contains all the logic for showing a texture to the screen
It contains
- Render form
- Swapchain
- Render Target
- Depth stencil
- [[Command Queue]]

It does not handle any rendering tasks, instead rendering tasks should write to the render target view

## Setup
Graphics is setup by running Graphics.Init and passing in a render form
```csharp
Graphics.Init(form);
```

This will do the following
- Attach the graphics device
- Setup the command allocator and command queue
- Setup the swap chain including the render target view (RTV) and depth stencil view (DSV)

## Usage
### Showing the render target to the screen
After writing data to the render target view (RTV), run Graphics.Buffer
```csharp
Graphics.Buffer();
```
This will swap the shown texture in the swap chain and present the RTV to the screen

### Compiling shaders
Shaders are compiled with Graphics.CompileShader
```csharp
ShaderBytecode pixelShader = Graphics.CompileShader(".res/Shaders/Lit/LitPixelShader.hlsl", Graphics.ShaderType.Pixel);
```