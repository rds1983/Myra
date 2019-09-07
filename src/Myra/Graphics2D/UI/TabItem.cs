using Myra.Attributes;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

#if !XENKO
using Microsoft.Xna.Framework;
#else
using Xenko.Core.Mathematics;
#endif

namespace Myra.Graphics2D.UI
{
	public class TabItem : ISelectorItem, IContent
	{
		private string _id;
		private string _text;
		private Color? _color;

		public string Id
		{
			get
			{
				return _id;
			}

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
			get
			{
				return _text;
			}

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
			get
			{
				return _color;
			}

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
		[Content]
		public Widget Content
		{
			get; set;
		}

		[HiddenInEditor]
		[XmlIgnore]
		public object Tag
		{
			get; set;
		}

		[HiddenInEditor]
		[XmlIgnore]
		public IRenderable Image
		{
			get; set;
		}

		[HiddenInEditor]
		[XmlIgnore]
		public int ImageTextSpacing
		{
			get; set;
		}

		[HiddenInEditor]
		[XmlIgnore]
		internal ImageTextButton Button
		{
			get; set;
		}

		[HiddenInEditor]
		[XmlIgnore]
		public bool IsSelected
		{
			get
			{
				return Button.IsPressed;
			}

			set
			{
				if (value == IsSelected)
				{
					return;
				}

				Button.IsPressed = value;
				FireSelected();
			}
		}

		public event EventHandler Changed;
		public event EventHandler SelectedChanged;

		public TabItem()
		{
		}

		public TabItem(string text, Color? color = null, object tag = null, Widget content = null)
		{
			Text = text;
			Color = color;
			Tag = tag;
			Content = content;
		}

		public TabItem(string text, Widget content): this(text, null, null, content)
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
			var ev = SelectedChanged;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}
	}
}
