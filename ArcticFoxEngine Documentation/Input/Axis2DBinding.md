Represents a combination of 2 AxisBindings

## Getting Input
You can use any of the 2 methods to get the axis 2D input

|Method|Result|
| --- | --- |
|GetValue|Returns the current value of the given axis|
|GetDelta|Returns the difference in value between the current frame, and the previous frame|

```csharp
Vector2 axisValue = axisBinding.GetValue();
Vector2 axisValueDelta = axisBinding.GetDelta();
```