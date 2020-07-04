using System.ComponentModel;
using System;
using System.Text;
using System.Xml.Serialization;
using Myra.Attributes;
using Myra.Utility;
using Myra.MML;

#if !STRIDE
using Microsoft.Xna.Framework;
#else
using Stride.Core.Mathematics;
#endif

namespace Myra.Graphics2D.UI
{
	public class TabItem : BaseObject, ISelectorItem, IContent
	{
		private string _text;
		private Color? _color;

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

		[Browsable(false)]
		[Content]
		public Widget Content
		{
			get; set;
		}

		[Browsable(false)]
		[XmlIgnore]
		public object Tag
		{
			get; set;
		}

		[Browsable(false)]
		[XmlIgnore]
		public IImage Image
		{
			get; set;
		}

		[Browsable(false)]
		[XmlIgnore]
		public int ImageTextSpacing
		{
			get; set;
		}

		[Browsable(false)]
		[XmlIgnore]
		internal ImageTextButton Button
		{
			get; set;
		}

		[Browsable(false)]
		[XmlIgnore]
		public bool IsSelected
		{
			get
			{
				return Button.IsToggled;
			}

			set
			{
				if (value == IsSelected)
				{
					return;
				}

				Button.IsToggled = value;
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

		protected internal override void OnIdChanged()
		{
			base.OnIdChanged();

			FireChanged();
		}

		protected void FireChanged()
		{
			Changed.Invoke(this);
		}

		public void FireSelected()
		{
			SelectedChanged.Invoke(this);
		}
	}
}
