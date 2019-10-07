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
	public class Window : SingleItemContainer<VerticalStackPanel>, IContent
	{
		private Point? _startPos;
		private readonly Grid _titleGrid;
		private readonly Label _titleLabel;
		private readonly ImageButton _closeButton;
		private Widget _content;
		private Widget _previousKeyboardFocus;
		private Widget _previousMouseWheelFocus;

		[Browsable(false)]
		[XmlIgnore]
		public bool IsModal
		{
			get; set;
		}

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

		[Browsable(false)]
		[XmlIgnore]
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
		public Grid TitleGrid
		{
			get
			{
				return _titleGrid;
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public ImageButton CloseButton
		{
			get
			{
				return _closeButton;
			}
		}

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

		public override Desktop Desktop
		{
			get
			{
				return base.Desktop;
			}
			set
			{
				if (Desktop != null)
				{
					Desktop.TouchMoved -= DesktopOnTouchMoved;
					Desktop.TouchUp -= DesktopTouchUp;
				}

				base.Desktop = value;

				if (Desktop != null)
				{
					Desktop.TouchMoved += DesktopOnTouchMoved;
					Desktop.TouchUp += DesktopTouchUp;
				}

				IsWindowPlaced = false;
			}
		}

		public event EventHandler Closed;

		public Window(string styleName = Stylesheet.DefaultStyleName)
		{
			InternalChild = new VerticalStackPanel();

			Result = false;
			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;

			InternalChild.Spacing = 8;

			InternalChild.Proportions.Add(new Proportion(ProportionType.Auto));
			InternalChild.Proportions.Add(new Proportion(ProportionType.Fill));

			_titleGrid = new Grid
			{
				ColumnSpacing = 8
			};

			_titleGrid.ColumnsProportions.Add(new Proportion(ProportionType.Fill));
			_titleGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));

			_titleLabel = new Label();
			_titleGrid.Widgets.Add(_titleLabel);

			_closeButton = new ImageButton
			{
				GridColumn = 1
			};

			_closeButton.Click += (sender, args) =>
			{
				Close();
			};

			_titleGrid.Widgets.Add(_closeButton);

			InternalChild.Widgets.Add(_titleGrid);

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
			if (Desktop == null)
			{
				return;
			}

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
			else if (Desktop != null)
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
			else if (Desktop != null)
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
				_titleGrid.Bounds.Right - x,
				_titleGrid.Bounds.Bottom - y);
			var touchPos = Desktop.TouchPosition;
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
					Close();
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
				_closeButton.ApplyImageButtonStyle(style.CloseButtonStyle);
			}
		}

		public void ShowModal(Desktop desktop)
		{
			desktop.Widgets.Add(this);
			_previousKeyboardFocus = desktop.FocusedKeyboardWidget;
			_previousMouseWheelFocus = desktop.FocusedMouseWheelWidget;
			desktop.FocusedKeyboardWidget = this;
			desktop.FocusedMouseWheelWidget = this;
			IsModal = true;
		}

		public virtual void Close()
		{
			if (Desktop != null && Desktop.Widgets.Contains(this))
			{
				if (Desktop.FocusedKeyboardWidget == this)
				{
					Desktop.FocusedKeyboardWidget = null;
				}

				var desktop = Desktop;
				Desktop.Widgets.Remove(this);
				desktop.FocusedKeyboardWidget = _previousKeyboardFocus;
				desktop.FocusedMouseWheelWidget = _previousMouseWheelFocus;

				var ev = Closed;
				if (ev != null)
				{
					ev(this, EventArgs.Empty);
				}

				IsModal = false;
			}
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyWindowStyle(stylesheet.WindowStyles[name]);
		}
	}
}