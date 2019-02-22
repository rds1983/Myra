using System;
using System.ComponentModel;
using System.Text;
using Myra.Attributes;
using System.Xml.Serialization;

#if !XENKO
using Microsoft.Xna.Framework;
#else
using Xenko.Core.Mathematics;
#endif

namespace Myra.Graphics2D.UI
{
	public class ListItem : ISelectorItem
	{
		private string _id;
		private string _text;
		private Color? _color;

		public string Id
		{
			get { return _id; }

			set
			{
				if (value == _id)
				{
					return;
				}

				_id = value;
				FireChanged();
			}
		}

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

		[DefaultValue(null)]
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
		[XmlIgnore]
		public bool IsSeparator { get; set; }

		[HiddenInEditor]
		[XmlIgnore]
		public object Tag { get; set; }

		[HiddenInEditor]
		[XmlIgnore]
		public IRenderable Image { get; set; }

		[HiddenInEditor]
		[XmlIgnore]
		public int ImageTextSpacing { get; set; }

		[HiddenInEditor]
		[XmlIgnore]
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
			var sb = new StringBuilder();

			if (!string.IsNullOrEmpty(Text))
			{
				sb.Append(Text);
				sb.Append(" ");
			}

			if (!string.IsNullOrEmpty(Id))
			{
				sb.Append("(#");
				sb.Append(Id);
				sb.Append(")");
			}
			return sb.ToString();
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
