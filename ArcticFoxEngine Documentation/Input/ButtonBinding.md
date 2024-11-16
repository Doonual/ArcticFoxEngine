Represents any kind of input that return a bool value.

## Getting Input
There are 3 different ways to get input from a ButtonBinding

|Method|Result|
| --- | --- |
|GetButtonDown|Returns true for the 1st frame that a button is held down, but false after that|
|GetButton|Returns true for the entire time that a button is held down|
|GetButtonUp|Returns true for the 1st frame that a button is released|

```csharp
if (buttonBinding.GetButtonDown() == true) {
	// True for the 1st frame the button is held down
}
if (buttonBinding.GetButton() == true) {
	// True when the button is held down
}
if (buttonBinding.GetButtonUp() == true) {
	// True for the 1st frame the button is released
}
```