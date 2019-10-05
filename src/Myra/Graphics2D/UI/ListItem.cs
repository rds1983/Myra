using System;
using System.ComponentModel;
using System.Xml.Serialization;

#if !XENKO
using Microsoft.Xna.Framework;
#else
using Xenko.Core.Mathematics;
#endif

namespace Myra.Graphics2D.UI
{
	public class ListItem : SelectableItem, ISelectorItem
	{
		private Widget _widget;

		[Browsable(false)]
		[XmlIgnore]
		public bool IsSeparator { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public IRenderable Image { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public int ImageTextSpacing { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public Widget Widget
		{
			get
			{
				return _widget;
			}

			set
			{
				_widget = value;

				var asButton = _widget as ImageTextButton;
				if (asButton != null)
				{
					asButton.PressedChanged += (s, a) => FireSelectedChanged();
				}
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public bool IsSelected
		{
			get
			{
				var asButton = _widget as ImageTextButton;
				if (asButton == null)
				{
					return false;
				}

				return asButton.IsPressed;
			}

			set
			{
				var asButton = _widget as ImageTextButton;
				if (asButton == null)
				{
					return;
				}

				asButton.IsPressed = value;
			}
		}


		public event EventHandler SelectedChanged;

		public ListItem()
		{
		}

		public ListItem(string text, Color? color, object tag): base(text, color, tag)
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

		public void FireSelectedChanged()
		{
			var ev = SelectedChanged;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}
	}
}
