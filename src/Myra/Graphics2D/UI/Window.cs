using System;
using System.ComponentModel;
using System.Linq;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using System.Xml.Serialization;
using Myra.Attributes;

#if !STRIDE
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Stride.Core.Mathematics;
using Stride.Graphics;
using Stride.Input;
#endif

namespace Myra.Graphics2D.UI
{
	public class WindowClosingEventArgs : EventArgs
	{
		public bool Cancel { get; set; }

		public WindowClosingEventArgs()
		{
		}
	}

	public class Window : SingleItemContainer<VerticalStackPanel>, IContent
	{
		private readonly Label _titleLabel;
		private Widget _content;
		private Widget _previousMouseWheelFocus;

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

		[Category("Appearance")]
		public SpriteFont TitleFont
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

		[Browsable(false)]
		[XmlIgnore]
		public Grid TitleGrid { get; private set; }

		[Browsable(false)]
		[XmlIgnore]
		public ImageButton CloseButton { get; private set; }

		[Browsable(false)]
		[Content]
		public Widget Content
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
					InternalChild.Widgets.Remove(_content);
				}

				if (value != null)
				{
					InternalChild.Widgets.Insert(1, value);
				}

				_content = value;
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public bool Result
		{
			get; set;
		}

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

		private bool IsWindowPlaced
		{
			get; set;
		}

		public event EventHandler<WindowClosingEventArgs> Closing;
		public event EventHandler Closed;

		public Window(string styleName = Stylesheet.DefaultStyleName)
		{
			IsModal = true;
			IsDraggable = true;

			InternalChild = new VerticalStackPanel();

			Result = false;
			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;

			InternalChild.Spacing = 8;

			InternalChild.Proportions.Add(new Proportion(ProportionType.Auto));
			InternalChild.Proportions.Add(new Proportion(ProportionType.Fill));

			TitleGrid = new Grid
			{
				ColumnSpacing = 8
			};
			DragHandle = TitleGrid;

			TitleGrid.ColumnsProportions.Add(new Proportion(ProportionType.Fill));
			TitleGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));

			_titleLabel = new Label();
			TitleGrid.Widgets.Add(_titleLabel);

			CloseButton = new ImageButton
			{
				GridColumn = 1
			};

			CloseButton.Click += (sender, args) =>
			{
				Close();
			};

			TitleGrid.Widgets.Add(CloseButton);

			InternalChild.Widgets.Add(TitleGrid);

			SetStyle(styleName);
		}

		public override void UpdateLayout()
		{
			base.UpdateLayout();

			if (!IsWindowPlaced)
			{
				CenterOnDesktop();
				IsWindowPlaced = true;
			}
		}

		public void CenterOnDesktop()
		{
			var size = Bounds.Size();
			Left = (ContainerBounds.Width - size.X) / 2;
			Top = (ContainerBounds.Height - size.Y) / 2;
		}

		public override void OnTouchDown()
		{
			BringToFront();
			base.OnTouchDown();
		}

		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			switch (k)
			{
				case Keys.Escape:
					if (CloseButton.Visible)
					{
						Close();
					}
					break;
			}
		}

		public void ApplyWindowStyle(WindowStyle style)
		{
			ApplyWidgetStyle(style);

			if (style.TitleStyle != null)
			{
				_titleLabel.ApplyLabelStyle(style.TitleStyle);
			}

			if (style.CloseButtonStyle != null)
			{
				CloseButton.ApplyImageButtonStyle(style.CloseButtonStyle);
			}
		}

		private void InternalShow(Point? position = null)
		{
			Desktop.Widgets.Add(this);

			// Force mouse wheel focused to be set to the first appropriate widget in the next Desktop.UpdateLayout
			Desktop.FocusedMouseWheelWidget = null;

			if (position != null)
			{
				Left = position.Value.X;
				Top = position.Value.Y;
				IsWindowPlaced = true;
			}
		}

		public void Show(Point? position = null)
		{
			IsModal = false;
			InternalShow(position);
		}

		public void ShowModal(Point? position = null)
		{
			IsModal = true;
			_previousMouseWheelFocus = Desktop.FocusedMouseWheelWidget;

			InternalShow(position);
		}

		public virtual void Close()
		{
			var ev = Closing;
			if (ev != null)
			{
				var args = new WindowClosingEventArgs();
				ev(this, args);
				if (args.Cancel)
				{
					return;
				}
			}

			if (Desktop.FocusedKeyboardWidget == this)
			{
				Desktop.FocusedKeyboardWidget = null;
			}

			if (Desktop.Widgets.Contains(this))
			{
				Desktop.Widgets.Remove(this);
				Desktop.FocusedMouseWheelWidget = _previousMouseWheelFocus;
			}
			else
			{
				//todo fix remove error. DONE
				Parent.RemoveChild(this);
			}

			Closed.Invoke(this);
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyWindowStyle(stylesheet.WindowStyles[name]);
		}
	}
}