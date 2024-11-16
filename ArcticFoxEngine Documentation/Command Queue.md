Command Queues are use to queue work for the GPU to complete


## Setup
The command queue objects are first setup during Graphics.Init
It creates
- Command Allocator
- Command Queue

## Usage
Command lists are to be used by the engine systems to complete their work. They are used by
- Rendering
- Upload
To get a command list use either
```csharp
Graphis.CreateDirectCommandList();
// or
Graphics.CreateCopyCommandList();
```
This will return a GraphicsCommandList that is not currently recording

Before work is submitted to the command list, it must be reset
```csharp
Graphics.ResetDirectCommandList(commandList);
// or
Graphics.ResetCopyCommandList(commandList);
```

Work can then be added to the command list. Once adding work has been finished, the command list can be closed and executed
```csharp
commandList.Close();

Graphics.ExecuteDirectCommandList(commandList);
// or
Graphics.ExecuteCopyCommandList(commandList);
```

This will automatically increment the fence value for the command queue. If you need to wait for the command queue to finish executing all the command lists
```csharp
Graphics.WaitForDirectCommandQueue();
// or
Graphics.WaitForCopyCommandQueue();
```