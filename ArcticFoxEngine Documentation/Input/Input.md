The input system in ArcticFoxEngine was designed to make rebinding controls very easy

## Structure
**InputManager**
At the highest level, there is InputManager
InputManager stores a list of InputBindings
InputManager is in charge of keeping track of and updating all of the bindings


**InputBinding**
InputBinding represents one button / axis / scroll / whatever on one device. InputBinding cannot be used by itself, it is an abstract class which provides the structure to the actual bindings

These classes are what you are going to actually use for input binding
- [[ButtonBinding]]
- [[AxisBinding]]
- [[Axis2DBinding]]

Each one is an implementation of InputBinding and thus is kept track of and updated by InputManager.
Each one represents one type of input you can get from any device.

ButtonBinding may be any key on your keyboard or button on your mouse or anywhere else. It represents buttons only, that give only boolean output
AxisBinding may be any input that gives a range of values. This may include a single axis on a joystick, a volume slider, a single mouse axis. It will give a float output
Axis2DBinding may be any input that give 2 ranges of values. This includes joysticks, and mouse axes. It will give a Vector2 output

**Devices**
The device classes connect the physical devices to the InputBinding. They are told to update by the InputManager, where they will record device updates and pass them on to the InputBindings

**Bindings**
Bindings are implementations of the InputBinding abstract classes. They are specific to one input on one device and will take the data from the Devices and pass it to the InputBindings.
When creating the InputBindings you instantiate one of the following
- KeyboardButtonInput
	This is an implementation of ButtonBinding. It can be used to check for keyboard buttons
- MouseButtonInput
	This is an implementation of ButtonBinding. It can be used to check for mouse buttons
- MouseAxisInput
	This is an implementation of AxisBinding. It can be used to check for 1 mouse axis
- GenericAxis2DInput
	This is an implementation of Axis2DBinding. It is used to combine 2 AxisBindings together to make a Axis2DBinding


## Examples
```csharp title="Binding a Jump Button"

ButtonBinding jumpButton;

public Player() {

	jumpButton = new KeyboardButtonInput(KeyboardButtonInput.KeyboardButton.Space); 

}

public override void Update() {
	
	if (jumpButton.GetButton() == true) {
		// Do jump
	}
	
}

```

^4e3ac5

```csharp title="Binding a Mouse Look"

Axis2DBinding lookBinding;

public CameraController() {
	AxisBinding mouseX = new MouseAxisInput(MouseAxisInput.MouseAxis.x);
	AxisBinding mouseY = new MouseAxisInput(MouseAxisInput.MouseAxis.y);
	lookBinding = new GenericAxis2DInput(mouseX, mouseY);
}


public override void Update() {
	Vector2 lookVector = lookBinding.GetValue();
	Quaternion lookQuaternion = Quaternion.RotationYawPitchRoll(lookVector.x * 0.002f, lookVector.y * 0.002f, 0f);
	transform.localRotation *= lookQuaternion;
}

```

^84c492

