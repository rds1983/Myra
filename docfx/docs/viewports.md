### Overview

While Myra doesn't provide explicit API to change the viewport. Still it could be customized through [Desktop](https://rds1983.github.io/Myra/api/Myra.Graphics2D.UI.Desktop.html) properties: BoundsFetcher, Scale and TransformOrigin.

Desktop.BoundsFetcher is function returning rectangle the Desktop will render into. By default it returns the whole viewport.

See [Widget Transformations](widget-transformations.md) for details about Scale and TransformOrigin.

This document provides a few examples of changing the viewport through these properties.

### Fixed Size Viewport

Let's say we want Myra to be rendered in the fixed rectangle, that is centered on the screen.

It could be archieved using following code:
```c#
_desktop.BoundsFetcher = () =>
{
	var viewport = GraphicsDevice.Viewport;
	var x = (viewport.Width - FixedWidth) / 2;
	var y = (viewport.Height - FixedHeight) / 2;
	return new Rectangle(x, y, FixedWidth, FixedHeight);
};
```

It would render following:
![alt text](~/images/viewports-1.png)

### Cinema Viewport

Now we want Myra to occupy whole screen except space at the top and at the bottom(cinema style):
```c#
_desktop.BoundsFetcher = () =>
{
	var viewport = GraphicsDevice.Viewport;
	return new Rectangle(0, CinemaBorder, viewport.Width, viewport.Height - CinemaBorder * 2);
};
```

It would render following:
![alt text](~/images/viewports-2.png)

### Fixed Size/Scaled

Finally we want Myra to be rendered in fixed rectangle and then scaled over the whole screen.

Code for setting the BoundsFetcher/TransformOrigin:
```c#
_desktop.BoundsFetcher = () => new Rectangle(0, 0, FixedWidth, FixedHeight);

// Zero Desktop.TransformOrigin(by default it is set to [0.5, 0.5])
_desktop.TransformOrigin = Vector2.Zero;
```

Also we need to set Desktop.Scale every Draw call:
```c#
var viewport = GraphicsDevice.Viewport;
_desktop.Scale = new Vector2((float)viewport.Width / FixedWidth, (float)viewport.Height / FixedHeight);
```

It would render following:
![alt text](~/images/viewports-3.png)

### Sample

Full sample is available here: https://github.com/rds1983/Myra/tree/master/samples/Myra.Samples.Viewports