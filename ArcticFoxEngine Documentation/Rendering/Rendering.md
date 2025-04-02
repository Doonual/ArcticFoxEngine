---
tags:
  - "#rendering"
---
## Overview
The rendering system encompasses the work required to render the scene to a texture. 

The rendering system stores the main descriptor heap for the entire program. Because switching descriptor heaps is very expensive we only want to do this once per frame if possible. For this reason the rendering system manages 1 descriptor heap that is bound only once. Just before each object is to be rendered, the object's descriptors are copied into the main descriptor heap.

## Rendering a scene
To render the scene, you can use ```RenderScene(Camera camera)```. This will:
1. Bind the main descriptor heap
2. Loop over all the loaded [[Shader|shaders]] and call ```Shader.Render(Camera camera, Resource renderTarget, DescriptorHeap rtvDescHeap, DescriptorHeap dsvDescHeap)```

![[Rendering Pipeline.canvas|Rendering Pipeline]]

## Descriptor Heap Usage
As setting [[Descriptor Heap|descriptor heaps]] is a costly operation we only want to do this once per frame if possible. For this we only use 1 descriptor heap for all of the objects and shaders. Rendering manages the main descriptor heap with a capacity of 100,000 descriptors. Copying the descriptors into the main one is handled by the individual [[Shader|shader]].


