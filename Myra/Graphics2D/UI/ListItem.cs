using System;
using Microsoft.Xna.Framework;
using Myra.Edit;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class ListItem
	{
		private string _text;
		private Color? _color;

		public string Text
		{
			get { return _text; }

			set
			{
				if (value == _text)
				{
					return;
				}

				_text = value;
				FireChanged();
			}
		}

		public Color? Color
		{
			get { return _color; }

			set
			{
				if (value == _color)
				{
					return;
				}

				_color = value;
				FireChanged();
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
		public object Tag { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		public Widget Widget { get; set; }

		public event EventHandler Changed;
		public event EventHandler Selected;

		public ListItem()
		{
		}

		public ListItem(string text, Color? color, object tag)
		{
			Text = text;
			Color = color;
			Tag = tag;
		}

		public ListItem(string text, Color? color) : this(text, color, null)
		{
		}

		public ListItem(string text) : this(text, null)
		{
		}

		public override string ToString()
		{
			return Text;
		}

		protected void FireChanged()
		{
			var ev = Changed;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}

		public void FireSelected()
		{
			var ev = Selected;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}
	}
}
