using System;
using System.ComponentModel;
using System.Linq;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using System.Xml.Serialization;
using Myra.Attributes;

#if !XENKO
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Xenko.Core.Mathematics;
using Xenko.Graphics;
using Xenko.Input;
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
		private Point? _startPos;
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

		public override bool IsPlaced
		{
			get
			{
				return base.IsPlaced;
			}

			internal set
			{
				if (IsPlaced)
				{
                    if (Parent != null) {
                        Parent.TouchMoved -= DesktopOnTouchMoved;
                        Parent.TouchUp -= DesktopTouchUp;
                    }
                    else
                    {
                        Desktop.TouchMoved -= DesktopOnTouchMoved;
					    Desktop.TouchUp -= DesktopTouchUp;
                    }
				}

				base.IsPlaced = value;

				if (IsPlaced)
				{
                    if (Parent != null)
                    {
                        Parent.TouchMoved += DesktopOnTouchMoved;
                        Parent.TouchUp += DesktopTouchUp;
                    }
                    else
                    {
                        Desktop.TouchMoved += DesktopOnTouchMoved;
                        Desktop.TouchUp += DesktopTouchUp;
                    }
                }
			}
		}

		public event EventHandler<WindowClosingEventArgs> Closing;
		public event EventHandler Closed;

		public Window(string styleName = Stylesheet.DefaultStyleName)
		{
			IsModal = true;

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

		private void DesktopOnTouchMoved(object sender, EventArgs args)
		{
			if (_startPos == null)
			{
				return;
			}

			var position = new Point(Desktop.TouchPosition.X - _startPos.Value.X,
				Desktop.TouchPosition.Y - _startPos.Value.Y);

			if (position.X < 0)
			{
				position.X = 0;
			}

			if (Parent != null)
			{
				if (position.X + Bounds.Width > Parent.Bounds.Right)
				{
					position.X = Parent.Bounds.Right - Bounds.Width;
				}
			}
			else
			{
				if (position.X + Bounds.Width > Desktop.InternalBounds.Right)
				{
					position.X = Desktop.InternalBounds.Right - Bounds.Width;
				}
			}

			if (position.Y < 0)
			{
				position.Y = 0;
			}

			if (Parent != null)
			{
				if (position.Y + Bounds.Height > Parent.Bounds.Bottom)
				{
					position.Y = Parent.Bounds.Bottom - Bounds.Height;
				}
			}
			else
			{
				if (position.Y + Bounds.Height > Desktop.InternalBounds.Bottom)
				{
					position.Y = Desktop.InternalBounds.Bottom - Bounds.Height;
				}
			}

			Left = position.X;
			Top = position.Y;
		}

		private void DesktopTouchUp(object sender, EventArgs args)
		{
			_startPos = null;
		}

		public override void OnTouchUp()
		{
			base.OnTouchUp();

			_startPos = null;
		}

		public override void OnTouchDown()
		{
			base.OnTouchDown();

			var x = Bounds.X;
			var y = Bounds.Y;
			var bounds = new Rectangle(x, y,
				TitleGrid.Bounds.Right - x,
				TitleGrid.Bounds.Bottom - y);
			var touchPos = Desktop.TouchPosition;

			if (CloseButton.Visible && CloseButton.Bounds.Contains(touchPos))
			{
				return;
			}

			if (bounds.Contains(touchPos))
			{
				_startPos = new Point(touchPos.X - ActualBounds.Location.X,
					touchPos.Y - ActualBounds.Location.Y);
			}
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