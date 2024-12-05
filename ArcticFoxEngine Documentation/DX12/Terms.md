A Guide to Direct3D12 terms
DirectX is hard...

Descriptor - https://learn.microsoft.com/en-us/windows/win32/direct3d12/descriptors-overview
Descriptors encode a refrence to the D3D resource, not store them themselves
A descriptor is data that describes an object to the GPU
A descriptor can be any of the following types:
	Render Target Views (RTV)
	Depth Stencil Views (DSV)
	Shader Resource Views (SRV)
	Unordered Access Views (UAV)
	Constant Buffer Views (CBV)
	Samplers

Descriptor Heaps - https://learn.microsoft.com/en-us/windows/win32/direct3d12/descriptor-heaps-overview
A descriptor heap is a block of memory on the GPU which contains descriptors
A descriptor heap can contain either a combination of SRVs UAVs and CBVs or a combination of Samplers
Only one conbined SRV, UAV and CBV descriptor heap and one Sampler descriptor heap can be bound at once

Descriptor Tables - https://learn.microsoft.com/en-us/windows/win32/direct3d12/descriptor-tables-overview
Descriptor tables do not allocate any memore but are instead an offset and length in a descriptor heap
Descriptor tables work with the root signature to access a specific descriptor in a descriptor heap
Just a subrange of a descriptor heap


Resources - https://learn.microsoft.com/en-us/windows/win32/direct3d12/resource-binding-flow-of-control
Resources are the actual D3D resources on the GPU.
Resources are of the following types:
	- Constant Buffer View (CBV)
	- Unordered access view (UAV)
	- Shader resource view (SRV)
	- Samplers
	- Render Target View (RTV)
	- Depth Stencil View (DSV)
	- Index Buffer View (IBV)
	- Vertex Buffer View (VBV)
	- Stream Output View (SOV)
	- Stream Output View (SOV)

Descriptors are another way of saying views