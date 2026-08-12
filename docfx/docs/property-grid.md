## Overview
PropertyGrid uses reflection to browse through the provided object's public fields and properties and places them on the editing grid.

This process could be controlled by assigning the fields and properties following attributes:

Attribute|Description
---------|-----------
[Browsable(false)]|Ignores a field/property
[Category(_categoryName_)]|Puts a field/property under the specified category
[DisplayName(_displayName_)]|Uses provided _displayName_ as name

## Example
Consider following definition:
```c#
	public enum State
	{
		Sleeping,
		Moving,
		Attacking
	}

	public class HitPoints
	{
		public int Current;
		public int Maximum
		{
			get; set;
		}
	}

	public class CollectionItem
	{
		public string Text;

		[Category("Layout")]
		public int X;

		[Category("Layout")]
		public int Y;

		public override string ToString()
		{
			return string.Format("Text={0}, X={1}, Y={2}", Text, X, Y);
		}
	}

	public class Player
	{
		public string Name;

		public bool Visible
		{
			get; set;
		}

		public State State;

		[Category("Appearance")]
		public Color Color;

		[Category("Appearance")]
		public IBrush Background;

		[Category("Layout")]
		public int X
		{
			get; set;
		}

		[Category("Layout")]
		public int Y
		{
			get; set;
		}

		[Category("Layout")]
		public int Width;

		[Category("Layout")]
		public int Height;

		[Category("Data")]
		[DisplayName("Attack (ReadOnly)")]
		public int Attack
		{
			get; private set;
		}

		[Category("Data")]
		[DisplayName("Defense (ReadOnly)")]
		public int Defense
		{
			get; private set;
		}

		[Category("Data")]
		public HitPoints HitPoints
		{
			get;
		} = new HitPoints();

		[Browsable(false)]
		public int Ignored;

		[Category("Data")]
		public List<CollectionItem> Collection { get; } = new List<CollectionItem>();

		public Player()
		{
			Name = "Player";

			Color = Color.White;

			X = Y = 300;
			Width = 100;
			Height = 100;

			Visible = true;

			Attack = 100;
			Defense = 200;

			HitPoints.Current = 100;
			HitPoints.Maximum = 150;

			Collection.Add(new CollectionItem
			{
				Text = "Item 1",
				X = 10,
				Y = 20
			});

			Collection.Add(new CollectionItem
			{
				Text = "Item 2",
				X = 30,
				Y = 40
			});
		}
	}
```

Now if we provide Player object to the PropertyGrid:
```c#
	Player player = new Player();
	PropertyGrid propertyGrid = new PropertyGrid
	{
		Object = player,
		Width = 350
	};

	Window window = new Window
	{
		Title = "Object Editor",
		Content = propertyGrid
	};

	window.ShowModal();
```

It would result in the following:

![alt text](~/images/property-grid1.png)

## CollectionEditor
As you may have noticed, Player.Collection is also present in the PropertyGrid. Now, if you click on "Change...", it would show the following:

![alt text](~/images/property-grid2.png)

It's important to note that for the CollectionEditor to work, the property must implement the System.Collections.Generic.ICollection<T> interface.

Full sample is available here: https://github.com/rds1983/Myra/tree/master/samples/Myra.Samples.ObjectEditor