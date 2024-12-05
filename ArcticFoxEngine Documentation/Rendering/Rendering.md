The Rendering subsystem encompasses everything needed to render meshs
[[How it should work.canvas|How it should work]]

The bulk of it is in [[Shaders]]


## Descriptor Heap Usage
As setting [[Descriptor Heap|descriptor heaps]] is a costly operation we only want to do this once per frame if possible. 
For this we only use 1 [[Descriptor Heap|descriptor heap]] for all of the objects and shaders.

The descriptor heap is created with a capacity of 100,000 descriptors



The shader's materials will set 

The final step is to call ```GraphicsCommandList.SetGraphicsRootDescriptorTable``` which tells the GPU where to find the descriptor of an item in the root signature