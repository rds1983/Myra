# Event System

Myra provides a comprehensive event system for handling user interactions and widget state changes.

### Event Arguments

All event arguments inherit from the base `MyraEventArgs` class, which provides:

```csharp
public class MyraEventArgs
{
    /// <summary>
    /// Gets the type of the input event.
    /// </summary>
    public InputEventType EventType { get; }

    /// <summary>
    /// Stops the propagation of the current event, preventing it from reaching other widgets in the propagation chain.
    /// </summary>
    public void StopPropagation();
}
```

## Subscribing to Events

### Basic Event Subscription

Subscribing to events in Myra uses standard C# event patterns:

```csharp
var button = new Button();
button.MouseEntered += OnButtonMouseEntered;
button.MouseLeft += OnButtonMouseLeft;

private void OnButtonMouseEntered(object sender, MyraEventArgs e)
{
    Console.WriteLine("Mouse entered button");
}

private void OnButtonMouseLeft(object sender, MyraEventArgs e)
{
    Console.WriteLine("Mouse left button");
}
```

### Using Lambda Expressions

For simple event handlers, lambda expressions are convenient:

```csharp
var button = new Button();
button.Click += (sender, e) => 
{
    Console.WriteLine("Button was clicked!");
};
```

## Event Propagation

Myra supports two event propagation strategies defined by the `EventHandlingStrategy` enumeration:

### Event Capturing (Default)

In **event capturing**, events are captured at the top level and propagate down to child widgets:

```
Desktop (receives event first)
  ↓
Parent Container
  ↓
Button (receives event last)
```

This allows parent containers to intercept events before they reach children.

### Event Bubbling

In **event bubbling**, events start at the widget level and propagate up to parent widgets:

```
Button (receives event first)
  ↓
Parent Container
  ↓
Desktop (receives event last)
```

This allows parent containers to respond to child widget events.

### Configuring Event Handling Strategy

By default, Myra uses **event capturing** strategy. You can change the event handling strategy globally by setting the `MyraEnvironment.EventHandlingModel` property:

```csharp
using Myra;
using Myra.Events;

// Change to event bubbling (events bubble up from child to parent)
MyraEnvironment.EventHandlingModel = EventHandlingStrategy.EventBubbling;

// Or explicitly set to event capturing (events captured from top down)
MyraEnvironment.EventHandlingModel = EventHandlingStrategy.EventCapturing;
```

## Stopping Event Propagation

You can stop an event from propagating through the widget hierarchy by calling `StopPropagation()`. This prevents the event from being delivered to other widgets in the propagation chain:

```csharp
var button = new Button();
button.MouseEntered += (sender, e) => 
{
    Console.WriteLine("Button received mouse entered event");
    e.StopPropagation();  // Other widgets in the propagation chain won't receive this event
};

var container = new Panel();
container.MouseEntered += (sender, e) => 
{
    // May or may not be called depending on the event handling strategy and propagation order
    Console.WriteLine("Container received mouse entered event");
};
container.Widgets.Add(button);
```

## Common Event Patterns

### Cancellable Events

Some events can be cancelled to prevent their default behavior. Check if an event is cancellable by looking at its argument type:

```csharp
var window = new Window();
window.Closing += (sender, e) => 
{
    // Check some condition before allowing the window to close
    if (!IsDataSaved())
    {
        e.Cancel = true;  // Prevent the window from closing
        Console.WriteLine("Close cancelled - unsaved changes");
    }
};

private bool IsDataSaved()
{
    // Your logic here
    return true;
}
```

### Text Input Handling

Text widgets provide multiple text-related events:

```csharp
var textBox = new TextBox();

// Fired when text changes (programmatically or by user)
textBox.TextChanged += (sender, e) => 
{
    Console.WriteLine($"Text changed from '{e.OldValue}' to '{e.NewValue}'");
};

// Fired when text is changed by user input
textBox.TextChangedByUser += (sender, e) => 
{
    Console.WriteLine($"User typed: {textBox.Text}");
};

// Fired after text is deleted
textBox.TextDeleted += (sender, e) => 
{
    Console.WriteLine($"Text '{e.Value}' was deleted at position {e.StartPosition}");
};

// Fired when user types a character
textBox.CharInput += (sender, e) => 
{
    Console.WriteLine($"Character input: {e.Data}");
};
```

### Selection Changes

Collection-based widgets fire selection change events:

```csharp
var listView = new ListView();

listView.SelectedIndexChanged += (sender, e) => 
{
    var selectedIndex = listView.SelectedIndex;
    if (selectedIndex.HasValue)
    {
        Console.WriteLine($"Selected index changed to: {selectedIndex.Value}");
    }
};
```

### Focus Management

Desktop and widgets provide focus-related events:

```csharp
var desktop = new Desktop();

desktop.WidgetLosingKeyboardFocus += (sender, e) => 
{
    Console.WriteLine($"Widget {e.Data} is losing focus");
};

desktop.WidgetGotKeyboardFocus += (sender, e) => 
{
    Console.WriteLine($"Widget {e.Data} got focus");
};
```

### Dialog Closing

Windows and dialogs provide closing events:

```csharp
var dialog = new Window();

dialog.Closing += (sender, e) => 
{
    // Prevent closing if validation fails
    if (!ValidateDialogContent())
    {
        e.Cancel = true;
        Console.WriteLine("Cannot close - validation failed");
    }
};

dialog.Closed += (sender, e) => 
{
    Console.WriteLine("Dialog closed successfully");
};

private bool ValidateDialogContent()
{
    // Your validation logic here
    return true;
}
```

## Value Change Events

Value change events allow you to respond to changes in widget values:

```csharp
var slider = new HorizontalSlider();

// Fired after the slider value has changed
slider.ValueChanged += (sender, e) => 
{
    Console.WriteLine($"Slider value changed from {e.OldValue} to {e.NewValue}");
};

// Fired after the slider value has changed by user interaction
slider.ValueChangedByUser += (sender, e) => 
{
    Console.WriteLine($"User changed slider to: {e.NewValue}");
};
```

For widgets like TextBox that support cancellable value changes, you can validate and modify values before they are applied:

```csharp
var textBox = new TextBox();

// Fired before the text value is changed - can be cancelled or modified
textBox.ValueChanging += (sender, e) => 
{
    // Limit text to maximum length
    if (e.NewValue?.Length > 100)
    {
        e.NewValue = e.NewValue.Substring(0, 100);
    }
};

textBox.ValueChanged += (sender, e) => 
{
    // Value change is committed
    Console.WriteLine($"Text changed to: {e.NewValue}");
};
```