using Microsoft.Xna.Framework;
using Myra.Attributes;
using Myra.Graphics2D;
using System.Collections.Generic;
using System.ComponentModel;

namespace Myra.Samples.ObjectEditor
{
	public enum State
	{
		Sleeping,
		Moving,
		Attacking
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
		[RenderAsSlider(1, 200)]
		public int Attack
		{
			get; private set;
		}

		[Category("Data")]
		[DisplayName("Defense (ReadOnly)")]
		[RenderAsSlider(1, 200)]
		public int Defense
		{
			get; private set;
		}

		[Category("Data")]
		[DesignerFolded]
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
}
