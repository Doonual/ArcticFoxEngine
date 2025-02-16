From: https://learn.microsoft.com/en-us/windows/win32/direct3d12/resource-binding-flow-of-control

Shader resources encompasses resources a shader might want to use. This includes...
- Textures
- Images
- Buffers
- Constant tables

Shader resources are not bound directly to the shader, instead they are referenced through a descriptor.

## Creation
When a shader resource is created, an accompanying descriptor heap and relevant descriptor is created.
The descriptor heap that is created is created that is just big enough to store all the descriptors.

For example, for a [[Constant Buffer|constant buffer]] that has 16 elements, the [[Descriptors|descriptor]] heap will need to be 16 descriptors big
```csharp title:"Creating descriptor heap and descriptors for a Constant Buffer"

// How many elements is the constant buffer going to store
int numElements = 16;

// Create the descriptor heap
DescriptorHeapDescription descriptorHeapDescription = new DescriptorHeapDescription() {
	DescriptorCount = numElements,
	Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView,
};
descriptorHeap = Graphics.device.CreateDescriptorHeap(descriptorHeapDescription);


// Create descriptors for the descriptor heap
int descHeapIncrement = Graphics.device.GetDescriptorHandleIncrementSize(DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);
for (int i = 0; i < numElements; i++) {
	ConstantBufferViewDescription cbvDescription = new ConstantBufferViewDescription() {
		BufferLocation = constantBuffer.GPUVirtualAddress + stride * i,
		SizeInBytes = stride,
	};
	Graphics.device.CreateConstantBufferView(cbvDescription, destDescriptorHeap.CPUDescriptorHandleForHeapStart + (offset + i) * descHeapIncrement);
}

```

For another example, this is how this would work for a Texture.
```csharp title:"Creating descriptor heap and descriptor for a Texture"

// Create descriptor heap
DescriptorHeapDescription descriptorHeapDescription = new DescriptorHeapDescription() {
	DescriptorCount = 1,
	Type = DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView,
};
descriptorHeap = Graphics.device.CreateDescriptorHeap(descriptorHeapDescription);

// Add descriptor to the descriptorHeap
int descHeapIncrement = Graphics.device.GetDescriptorHandleIncrementSize(DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);
ShaderResourceViewDescription srvDescription = new ShaderResourceViewDescription() {
	Shader4ComponentMapping = ComponentMapping(0, 1, 2, 3),
	Format = Format.R8G8B8A8_UNorm,
	Dimension = ShaderResourceViewDimension.Texture2D,
	Texture2D = { MipLevels = 1 },
};
Graphics.device.CreateShaderResourceView(texture, srvDescription, destDescriptorHeap.CPUDescriptorHandleForHeapStart + descHeapIncrement * offset);


```

## Usage
When a Shader Resource is to be used, the relevant descriptor must first be coppied into the main [[Descriptor Heap|descriptor heap]]. This is done with Rendering.CopyDescriptorsIn(). This will...
- Find the 1st empty space in the main [[Descriptor Heap|descriptor heap]]
- Copy all the required descriptors into the main [[Descriptor Heap|descriptor heap]] starting at the 1st empty space.
- Return the start of where the copied descriptors ended up
```csharp title:"Copying descriptors into the main descriptor heap"

internal static GpuDescriptorHandle CopyDescriptorsIn(CpuDescriptorHandle srcDescriptorHandle, int numDescriptors) {

	int destinationDescriptorIndex = descriptorCopyPos;
	descriptorCopyPos += numDescriptors;

	// Copy the descriptors
	CpuDescriptorHandle destDescriptor = gpuDescriptorHeap.CPUDescriptorHandleForHeapStart + destinationDescriptorIndex * descriptorHeapIncrement;
	Graphics.device.CopyDescriptorsSimple(numDescriptors, destDescriptor, srcDescriptorHandle, DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);

	// Tell the dataslot where to find the descriptors
	return gpuDescriptorHeap.GPUDescriptorHandleForHeapStart + destinationDescriptorIndex * descriptorHeapIncrement;

}

```

Then the newly copied descriptors are assigned to the relevant root parameter within the currently bound [[Root Signature|root signature]]