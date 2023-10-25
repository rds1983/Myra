using System;
using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using System.Xml.Serialization;
using Myra.Attributes;
using FontStashSharp;

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
	public class Window : SingleItemContainer<VerticalStackPanel>, IContent
	{
		private readonly Label _titleLabel;
		private Widget _content;
		private Widget _previousKeyboardFocus;

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

		[Browsable(false)]
		[XmlIgnore]
		public HorizontalStackPanel TitlePanel { get; private set; }

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
					StackPanel.SetProportionType(value, ProportionType.Fill);
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

		[DefaultValue(DragDirection.Both)]
		public override DragDirection DragDirection { get => base.DragDirection; set => base.DragDirection = value; }

		[Category("Behavior")]
		[DefaultValue(Keys.Escape)]
		public Keys CloseKey { get; set; }

		private bool IsWindowPlaced
		{
			get; set;
		}

		public event EventHandler<CancellableEventArgs> Closing;
		public event EventHandler Closed;

		public Window(string styleName = Stylesheet.DefaultStyleName)
		{
			AcceptsKeyboardFocus = true;
			CloseKey = Keys.Escape;

			IsModal = true;
			DragDirection = DragDirection.Both;

			InternalChild = new VerticalStackPanel();

			Result = false;
			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;

			InternalChild.Spacing = 8;

			TitlePanel = new HorizontalStackPanel
			{
				Spacing = 8
			};
			DragHandle = TitlePanel;

			_titleLabel = new Label();
			StackPanel.SetProportionType(_titleLabel, ProportionType.Fill);
			TitlePanel.Widgets.Add(_titleLabel);

			CloseButton = new ImageButton();

			CloseButton.Click += (sender, args) =>
			{
				Close();
			};

			TitlePanel.Widgets.Add(CloseButton);

			InternalChild.Widgets.Add(TitlePanel);

			SetStyle(styleName);
		}

		protected override void InternalArrange()
		{
			base.InternalArrange();

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

			if (k == CloseKey)
			{
				Close();
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

		public void Show(Desktop desktop, Point? position = null)
		{
			IsModal = false;
			InternalShow(desktop, position);
		}

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
				var args = new CancellableEventArgs();
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

			Closed.Invoke(this);
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyWindowStyle(stylesheet.WindowStyles.SafelyGetStyle(name));
		}
	}
}