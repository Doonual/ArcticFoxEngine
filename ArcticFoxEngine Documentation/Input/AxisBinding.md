Represents any kind of input that returns a float value

## Getting Input
You can use any of the 2 methods to get the axis input

|Method|Result|
| --- | --- |
|GetValue|Returns the current value of the given axis|
|GetDelta|Returns the difference in value between the current frame, and the previous frame|

```csharp
float axisValue = axisBinding.GetValue();
float axisValueDelta = axisBinding.GetDelta();
```