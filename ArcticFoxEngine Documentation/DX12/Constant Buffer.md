A constant buffer is a contiguous block of memory on the GPU, simmilar to an array. They hold an array of structs where each struct is 256 byte aligned.
A constant buffer is used when you only need to bind one struct of data to a shader at once, but you need to change which struct you are binding many times.
Constant buffers must be 256 byte aligned

Constant buffers are bound to the shader by using DataSlot.SetData