using System.ComponentModel;
using System;
using System.Text;
using System.Xml.Serialization;
using Myra.Attributes;
using Myra.Utility;
using Myra.MML;
using Myra.Events;


#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// Represents a single tab item in a tab control, containing text, an optional image, and associated content.
	/// </summary>
	public class TabItem : BaseObject, ISelectorItem, IContent
	{
		private Widget _content;
		private string _text;
		private Color? _color;
		private ListViewButton _button;

		/// <summary>
		/// Gets or sets the text displayed on the tab.
		/// </summary>
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

		/// <summary>
		/// Gets or sets the color of the tab text, or null to use the style default.
		/// </summary>
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

		/// <summary>
		/// Gets or sets the widget to display as the tab's content when selected.
		/// </summary>
		[Browsable(false)]
		[Content]
		public Widget Content
		{
			get => _content;
			set
			{
				if (_content == value)
				{
					return;
				}

				_content = value;
				FireChanged();
			}
		}

		/// <summary>
		/// Gets or sets an optional tag object associated with this tab item.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public object Tag { get; set; }

		/// <summary>
		/// Gets or sets an optional image displayed alongside the tab text.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public IImage Image { get; set; }

		/// <summary>
		/// Gets or sets the spacing in pixels between the image and text in the tab.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public int ImageTextSpacing { get; set; }

		/// <summary>
		/// Gets or sets the height of the tab in pixels, or null to use the style default.
		/// </summary>
		[DefaultValue(null)]
		public int? Height { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		internal ListViewButton Button
		{
			get => _button;
			set
			{
				if (value == _button)
				{
					return;
				}

				if (_button != null)
				{
					_button.PressedChanged -= OnPressedChanged;
				}

				_button = value;

				if (_button != null)
				{
					_button.PressedChanged += OnPressedChanged;
				}
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		private HorizontalStackPanel Panel => (HorizontalStackPanel)Button.Content;

		[Browsable(false)]
		[XmlIgnore]
		internal Image ImageWidget => (Image)Panel.Widgets[0];

		[Browsable(false)]
		[XmlIgnore]
		internal Label LabelWidget => (Label)Panel.Widgets[1];

		/// <summary>
		/// Gets or sets a value indicating whether this tab item is currently selected.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public bool IsSelected
		{
			get => _button.IsPressed;
			set => _button.IsPressed = value;
		}

		/// <summary>
		/// Occurs when any property of the tab item changes.
		/// </summary>
		public event MyraEventHandler Changed;

		/// <summary>
		/// Occurs when the tab's selected state changes.
		/// </summary>
		public event MyraEventHandler SelectedChanged;

		/// <summary>
		/// Initializes a new instance of the <see cref="TabItem"/> class with default values.
		/// </summary>
		public TabItem()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TabItem"/> class with specified properties.
		/// </summary>
		/// <param name="text">The text to display on the tab.</param>
		/// <param name="color">The color of the tab text, or null to use the style default.</param>
		/// <param name="tag">An optional object to associate with the tab item.</param>
		/// <param name="content">The widget to display as the tab's content when selected.</param>
		public TabItem(string text, Color? color = null, object tag = null, Widget content = null)
		{
			Text = text;
			Color = color;
			Tag = tag;
			Content = content;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TabItem"/> class with text and content.
		/// </summary>
		/// <param name="text">The text to display on the tab.</param>
		/// <param name="content">The widget to display as the tab's content when selected.</param>
		public TabItem(string text, Widget content) : this(text, null, null, content)
		{
		}

		/// <summary>
		/// Returns a string representation of the tab item, including its text and id if available.
		/// </summary>
		/// <returns>A string containing the tab's text and id in the format "Text (#id)".</returns>
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

		/// <summary>
		/// Called when the Id property of this tab item changes. Raises the Changed event.
		/// </summary>
		protected internal override void OnIdChanged()
		{
			base.OnIdChanged();

			FireChanged();
		}

		/// <summary>
		/// Raises the Changed event to notify listeners of property changes.
		/// </summary>
		protected void FireChanged()
		{
			Changed.Invoke(this, InputEventType.ValueChanged);
		}

		private void OnPressedChanged(object sender, MyraEventArgs args)
		{
			if (_button.IsPressed)
			{
				SelectedChanged.Invoke(this, InputEventType.SelectionChanged);
			}
		}

		/// <summary>
		/// Creates a deep copy of this tab item, including a clone of its content widget if present.
		/// </summary>
		/// <returns>A new <see cref="TabItem"/> instance with copies of all properties and content.</returns>
		public TabItem Clone()
		{
			var result = new TabItem
			{
				Text = Text,
				Color = Color,
				Tag = Tag,
				Image = Image,
				ImageTextSpacing = ImageTextSpacing,
				Height = Height
			};

			if (Content != null)
			{
				result.Content = Content.Clone();
			}

			return result;
		}
	}
}
