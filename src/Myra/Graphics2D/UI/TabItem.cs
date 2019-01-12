using Myra.Attributes;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Text;

#if !XENKO
using Microsoft.Xna.Framework;
#else
using Xenko.Core.Mathematics;
#endif

namespace Myra.Graphics2D.UI
{
	public class TabItem : IItemWithId, IContent
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
		public Widget Content
		{
			get; set;
		}

		[HiddenInEditor]
		[JsonIgnore]
		public object Tag
		{
			get; set;
		}

		[HiddenInEditor]
		[JsonIgnore]
		public IRenderable Image
		{
			get; set;
		}

		[HiddenInEditor]
		[JsonIgnore]
		public int ImageTextSpacing
		{
			get; set;
		}

		[HiddenInEditor]
		[JsonIgnore]
		internal Button Button
		{
			get; set;
		}

		public event EventHandler Changed;
		public event EventHandler Selected;

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
			var ev = Selected;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}
	}
}
