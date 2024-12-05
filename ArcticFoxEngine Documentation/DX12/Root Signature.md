From: https://learn.microsoft.com/en-us/windows/win32/direct3d12/root-signatures-overview
A Root Signature describes the layout of data to be bound to the shader

To use a root signature:
1. Bind it to the CommandList
```csharp
GraphicsCommandList cmdList;
RootSignature rootSignature;

cmdList.SetGraphicsRootSignature(rootSignature);
```
2. Tell the GPU where to find each RootParameter
```csharp
DataSlot dummyDataSlot;
SetGraphicsRootDescriptorTable(dataSlot.rootParameterIndex, dataSlot.currentDescriptorLocation);
```