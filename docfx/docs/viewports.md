### Overview

Although Myra doesn't provide an explicit API to change the viewport, it can be customized through [Desktop](https://rds1983.github.io/Myra/api/Myra.Graphics2D.UI.Desktop.html) properties: BoundsFetcher, Scale, and TransformOrigin.

Desktop.BoundsFetcher is a function that returns the rectangle in which the Desktop will render. By default, it returns the entire viewport.

This document provides a few examples of changing the viewport through these properties.

### Fixed Size Viewport

Let's say we want Myra to be rendered in a fixed rectangle that is centered on the screen.

This can be achieved using the following code:
```c#
_desktop.BoundsFetcher = () =>
{
	var viewport = GraphicsDevice.Viewport;
	var x = (viewport.Width - FixedWidth) / 2;
	var y = (viewport.Height - FixedHeight) / 2;
	return new Rectangle(x, y, FixedWidth, FixedHeight);
};
```

It would render the following:
![alt text](~/images/viewports-1.png)

### Cinema Viewport

Now we want Myra to occupy the entire screen except for space at the top and bottom (cinema style):
```c#
_desktop.BoundsFetcher = () =>
{
	var viewport = GraphicsDevice.Viewport;
	return new Rectangle(0, CinemaBorder, viewport.Width, viewport.Height - CinemaBorder * 2);
};
```

It would render the following:
![alt text](~/images/viewports-2.png)

### Fixed Size/Scaled

Finally, we want Myra to be rendered in a fixed rectangle and then scaled over the entire screen.

Code for setting the BoundsFetcher and TransformOrigin:
```c#
_desktop.BoundsFetcher = () => new Rectangle(0, 0, FixedWidth, FixedHeight);

// Set Desktop.TransformOrigin to zero (by default it is set to [0.5, 0.5])
_desktop.TransformOrigin = Vector2.Zero;
```

We also need to set Desktop.Scale on every Draw call:
```c#
var viewport = GraphicsDevice.Viewport;
_desktop.Scale = new Vector2((float)viewport.Width / FixedWidth, (float)viewport.Height / FixedHeight);
```

It would render the following:
![alt text](~/images/viewports-3.png)

### Sample

Full sample is available here: https://github.com/rds1983/Myra/tree/master/samples/Myra.Samples.Viewports