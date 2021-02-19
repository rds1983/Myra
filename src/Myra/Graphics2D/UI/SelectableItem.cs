using System.ComponentModel;
using System;
using System.Text;
using System.Xml.Serialization;
using Myra.MML;
using Myra.Utility;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
#endif

namespace Myra.Graphics2D.UI
{
	public class SelectableItem: BaseObject
	{
		private string _text;
		private Color? _color;

		[DefaultValue(null)]
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

		private string _l18nText;
		public string L18nTextKey 
		{
			get 
			{
				return _l18nText;
			}
			set 
			{
				_l18nText = value;
				Text = Project.Localize.Invoke(value);
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
		[XmlIgnore]
		public object Tag
		{
			get; set;
		}

		public event EventHandler Changed;

		public SelectableItem()
		{
		}

		public SelectableItem(string text, Color? color, object tag)
		{
			Text = text;
			Color = color;
			Tag = tag;
		}

		public SelectableItem(string text, Color? color) : this(text, color, null)
		{
		}

		public SelectableItem(string text) : this(text, null)
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
	}
}
