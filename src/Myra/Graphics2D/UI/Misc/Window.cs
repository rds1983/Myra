using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using System.Xml.Serialization;
using FontStashSharp;
using Myra.Events;
using System.Collections;
using Myra.Attributes;


#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#elif STRIDE
using Stride.Core.Mathematics;
using Stride.Input;
#else
using System.Drawing;
using Myra.Platform;
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A draggable window widget with a title bar and optional close button.
	/// </summary>
	public class Window : ContentControl
	{
		private readonly StackPanelLayout _layout = new StackPanelLayout(Orientation.Vertical);
		private readonly Label _titleLabel;
		private Widget _content;
		private Widget _previousKeyboardFocus;

		/// <summary>
		/// Gets or sets the title text displayed in the window's title bar.
		/// </summary>
		[Category("Appearance")]
		public string Title
		{
			get
			{
				return _titleLabel.Text;
			}

			set
			{
				_titleLabel.Text = value;
			}
		}

		/// <summary>
		/// Gets or sets the color of the title text.
		/// </summary>
		[Category("Appearance")]
		[StylePropertyPath("TitleStyle/TextColor")]
		public Color TitleTextColor
		{
			get
			{
				return _titleLabel.TextColor;
			}
			set
			{
				_titleLabel.TextColor = value;
			}
		}

		/// <summary>
		/// Gets or sets the font used to render the title text.
		/// </summary>
		[Category("Appearance")]
		[StylePropertyPath("TitleStyle/Font")]
		public SpriteFontBase TitleFont
		{
			get
			{
				return _titleLabel.Font;
			}
			set
			{
				_titleLabel.Font = value;
			}
		}

		/// <summary>
		/// Gets the panel containing the title and close button.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public HorizontalStackPanel TitlePanel { get; private set; }

		/// <summary>
		/// Gets the close button displayed in the window's title bar.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public Button CloseButton { get; private set; }

		/// <summary>
		/// Gets or sets the widget displayed as the window's content.
		/// </summary>
		[Browsable(false)]
		[Content]
		public override Widget Content
		{
			get
			{
				return _content;
			}

			set
			{
				if (value == Content)
				{
					return;
				}

				// Remove existing
				if (_content != null)
				{
					Children.Remove(_content);
				}

				if (value != null)
				{
					StackPanel.SetProportionType(value, ProportionType.Fill);
					Children.Insert(1, value);
				}

				_content = value;
			}
		}

		/// <summary>
		/// Gets or sets a boolean result value for modal dialog operations.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public bool Result { get; set; }

		/// <summary>
		/// Gets or sets the horizontal alignment of the window.
		/// </summary>
		[DefaultValue(HorizontalAlignment.Left)]
		public override HorizontalAlignment HorizontalAlignment
		{
			get
			{
				return base.HorizontalAlignment;
			}
			set
			{
				base.HorizontalAlignment = value;
			}
		}

		/// <summary>
		/// Gets or sets the vertical alignment of the window.
		/// </summary>
		[DefaultValue(VerticalAlignment.Top)]
		public override VerticalAlignment VerticalAlignment
		{
			get
			{
				return base.VerticalAlignment;
			}
			set
			{
				base.VerticalAlignment = value;
			}
		}

		/// <summary>
		/// Gets or sets the direction in which the window can be dragged.
		/// </summary>
		[DefaultValue(DragDirection.Both)]
		public override DragDirection DragDirection { get => base.DragDirection; set => base.DragDirection = value; }

		/// <summary>
		/// Gets or sets the key that closes the window, or null for no key-based close.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(Keys.Escape)]
		public Keys? CloseKey { get; set; }

		private bool IsWindowPlaced { get; set; }

		/// <summary>
		/// Occurs before the window is closed. Set Cancel to true to prevent closing.
		/// </summary>
		public event MyraEventHandler<CancellableEventArgs> Closing;

		/// <summary>
		/// Occurs after the window has been closed.
		/// </summary>
		public event MyraEventHandler Closed;

		/// <summary>
		/// Initializes a new instance of the <see cref="Window"/> class with the specified stylesheet and style.
		/// </summary>
		/// <param name="stylesheet">The stylesheet to use for applying the style.</param>
		/// <param name="styleName">The name of the style to apply. Defaults to the default stylesheet style.</param>
		public Window(Stylesheet stylesheet, string styleName = Stylesheet.DefaultStyleName)
		{
			_layout.Spacing = 8;
			ChildrenLayout = _layout;

			AcceptsKeyboardFocus = true;
			CloseKey = Keys.Escape;

			DragDirection = DragDirection.Both;

			Result = false;
			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;

			TitlePanel = new HorizontalStackPanel
			{
				Spacing = 8
			};
			DragHandle = TitlePanel;

			_titleLabel = new Label();
			StackPanel.SetProportionType(_titleLabel, ProportionType.Fill);
			TitlePanel.Widgets.Add(_titleLabel);

			CloseButton = new Button
			{
				Content = new Image()
			};

			CloseButton.Click += (sender, args) =>
			{
				Close();
			};

			TitlePanel.Widgets.Add(CloseButton);

			Children.Add(TitlePanel);

			SetStyle(stylesheet, styleName);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Window"/> class with the specified style.
		/// </summary>
		/// <param name="styleName">The name of the style to apply. Defaults to the default stylesheet style.</param>
		public Window(string styleName = Stylesheet.DefaultStyleName) : this(Stylesheet.Current, styleName)
		{
		}

		/// <summary>
		/// Arranges the window and centers it on the desktop if it hasn't been manually placed.
		/// </summary>
		protected override void InternalArrange()
		{
			base.InternalArrange();

			if (!IsWindowPlaced)
			{
				CenterOnDesktop();
				IsWindowPlaced = true;
			}
		}

		/// <summary>
		/// Centers the window on the desktop.
		/// </summary>
		public void CenterOnDesktop()
		{
			var size = Bounds.Size();
			Left = (ContainerBounds.Width - size.X) / 2;
			Top = (ContainerBounds.Height - size.Y) / 2;
		}

		/// <summary>
		/// Handles touch down events and brings the window to the front.
		/// </summary>
		public override void OnTouchDown()
		{
			BringToFront();
			base.OnTouchDown();
		}

		/// <summary>
		/// Handles keyboard input, closing the window if the CloseKey is pressed.
		/// </summary>
		/// <param name="k">The key being pressed.</param>
		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			if (k == CloseKey)
			{
				Close();
			}
		}

		internal override IDictionary GetStylesDictionary(Stylesheet stylesheet) => stylesheet.WindowStyles;

		/// <summary>
		/// Applies the specified widget style to this window.
		/// </summary>
		/// <param name="style">The widget style to apply.</param>
		protected override void ApplyStyle(WidgetStyle style)
		{
			base.ApplyStyle(style);

			var windowStyle = (WindowStyle)style;
			if (windowStyle.TitleStyle != null)
			{
				_titleLabel.ApplyLabelStyle(windowStyle.TitleStyle);
			}

			if (windowStyle.CloseButtonStyle != null)
			{
				CloseButton.ApplyButtonStyle(windowStyle.CloseButtonStyle);
				if (windowStyle.CloseButtonStyle.ImageStyle != null)
				{
					var image = (Image)CloseButton.Content;
					image.ApplyImageStyle(windowStyle.CloseButtonStyle.ImageStyle);
				}
			}
		}

		private void InternalShow(Desktop desktop, Point? position = null)
		{
			Visible = true;
			Desktop = desktop;
			Desktop.Widgets.Add(this);

			if (position != null)
			{
				Left = position.Value.X;
				Top = position.Value.Y;
				IsWindowPlaced = true;
			}
		}

		/// <summary>
		/// Shows the window on the specified desktop as a non-modal window.
		/// </summary>
		/// <param name="desktop">The desktop to display the window on.</param>
		/// <param name="position">Optional position for the window. If null, the window will be centered.</param>
		public void Show(Desktop desktop, Point? position = null)
		{
			IsModal = false;
			InternalShow(desktop, position);
		}

		/// <summary>
		/// Shows the window on the specified desktop as a modal dialog that blocks interaction with other widgets.
		/// </summary>
		/// <param name="desktop">The desktop to display the window on.</param>
		/// <param name="position">Optional position for the window. If null, the window will be centered.</param>
		public void ShowModal(Desktop desktop, Point? position = null)
		{
			IsModal = true;
			InternalShow(desktop, position);

			_previousKeyboardFocus = desktop.FocusedKeyboardWidget;

			// Force mouse wheel focused to be set to the first appropriate widget in the next Desktop.UpdateLayout
			if (AcceptsKeyboardFocus)
			{
				Desktop.FocusedKeyboardWidget = this;
			}
		}

		/// <summary>
		/// Closes the window and raises the Closing and Closed events.
		/// </summary>
		public virtual void Close()
		{
			if (Desktop == null)
			{
				// Is closed already
				return;
			}

			var ev = Closing;
			if (ev != null)
			{
				var args = new CancellableEventArgs(InputEventType.Closing);
				ev(this, args);
				if (args.Cancel)
				{
					return;
				}
			}

			if (IsModal)
			{
				Desktop.FocusedKeyboardWidget = _previousKeyboardFocus;
			}

			if (Desktop.Widgets.Contains(this))
			{
				RemoveFromDesktop();
			}
			else
			{
				//todo fix remove error. DONE
				RemoveFromParent();
			}

			Closed.Invoke(this, InputEventType.Closing);
		}

		/// <summary>
		/// Copies the properties from another window widget.
		/// </summary>
		/// <param name="w">The source window widget to copy from.</param>
		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var window = (Window)w;

			Title = window.Title;
			TitleTextColor = window.TitleTextColor;
			TitleFont = window.TitleFont;
			CloseKey = window.CloseKey;
		}
	}
}