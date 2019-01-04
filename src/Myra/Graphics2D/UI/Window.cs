using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using Newtonsoft.Json;
using static Myra.Graphics2D.UI.Grid;

namespace Myra.Graphics2D.UI
{
	public class Window : SingleItemContainer<Grid>
	{
		[Obsolete("This enum is obsolete and will be removed in the future versions")]
		public enum DefaultModalResult
		{
			Ok,
			Cancel
		}

		private Point? _startPos;
		private readonly Grid _titleGrid;
		private readonly TextBlock _titleLabel;
		private readonly ImageButton _closeButton;
		private Widget _content;

		[EditCategory("Appearance")]
		public string Title
		{
			get { return _titleLabel.Text; }

			set { _titleLabel.Text = value; }
		}

		[EditCategory("Appearance")]
		[StylePropertyPath("TitleStyle.TextColor")]
		public Color TitleTextColor
		{
			get { return _titleLabel.TextColor; }
			set { _titleLabel.TextColor = value; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		public SpriteFont TitleFont
		{
			get { return _titleLabel.Font; }
			set { _titleLabel.Font = value; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		public Grid TitleGrid
		{
			get { return _titleGrid; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		public ImageButton CloseButton
		{
			get { return _closeButton; }
		}

		[HiddenInEditor]
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
					value.GridPositionY = 1;
					InternalChild.Widgets.Add(value);
				}

				_content = value;
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
		[Obsolete("Property is obsolete and will be removed in the future version. Use Result.")]
		public int ModalResult { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		public bool Result { get; set; }

		[DefaultValue(HorizontalAlignment.Left)]
		public override HorizontalAlignment HorizontalAlignment
		{
			get { return base.HorizontalAlignment; }
			set { base.HorizontalAlignment = value; }
		}

		[DefaultValue(VerticalAlignment.Top)]
		public override VerticalAlignment VerticalAlignment
		{
			get { return base.VerticalAlignment; }
			set { base.VerticalAlignment = value; }
		}

		[DefaultValue(true)]
		public override bool CanFocus
		{
			get { return base.CanFocus; }
			set { base.CanFocus = value; }
		}

		private bool IsWindowPlaced
		{
			get;set;
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
					Desktop.MouseMoved -= DesktopOnMouseMoved;
					Desktop.TouchUp -= DesktopTouchUp;
				}

				base.Desktop = value;

				if (Desktop != null)
				{
					Desktop.MouseMoved += DesktopOnMouseMoved;
					Desktop.TouchUp += DesktopTouchUp;
				}

				IsWindowPlaced = false;
			}
		}

		public event EventHandler Closed;

		public Window(WindowStyle style)
		{
			InternalChild = new Grid();
			ModalResult = (int)DefaultModalResult.Cancel;
			Result = false;
			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;
			CanFocus = true;

			InternalChild.RowSpacing = 8;

			InternalChild.RowsProportions.Add(new Proportion(ProportionType.Auto));
			InternalChild.RowsProportions.Add(new Proportion(ProportionType.Fill));

			_titleGrid = new Grid
			{
				ColumnSpacing = 8
			};

			_titleGrid.ColumnsProportions.Add(new Proportion(ProportionType.Fill));
			_titleGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));

			_titleLabel = new TextBlock();
			_titleGrid.Widgets.Add(_titleLabel);

			_closeButton = new ImageButton
			{
				GridPositionX = 1
			};

			_closeButton.Click += (sender, args) =>
			{
				Close();
			};

			_titleGrid.Widgets.Add(_closeButton);

			InternalChild.Widgets.Add(_titleGrid);

			if (style != null)
			{
				ApplyWindowStyle(style);
			}
		}

		public Window(string style)
			: this(Stylesheet.Current.WindowStyles[style])
		{
		}

		public Window() : this(Stylesheet.Current.WindowStyle)
		{
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

		private void DesktopOnMouseMoved(object sender, EventArgs args)
		{
			if (_startPos == null)
			{
				return;
			}

			var position = Desktop.MousePosition;

			var delta = new Point(position.X - _startPos.Value.X,
				position.Y - _startPos.Value.Y);

			var right = delta.X + Bounds.Width;
			if (Parent != null)
			{
				if (right > Parent.Bounds.Right)
				{
					delta.X = Parent.Bounds.Right - Bounds.Width;
				}
			}
			else if (Desktop != null)
			{
				if (right > Desktop.Bounds.Right)
				{
					delta.X = Desktop.Bounds.Right - Bounds.Width;
				}
			}

			var bottom = delta.Y + Bounds.Height;
			if (Parent != null)
			{
				if (bottom > Parent.Bounds.Bottom)
				{
					delta.Y = Parent.Bounds.Bottom - Bounds.Height;
				}
			}
			else if (Desktop != null)
			{
				if (bottom > Desktop.Bounds.Bottom)
				{
					delta.Y = Desktop.Bounds.Bottom - Bounds.Height;
				}
			}

			Left += delta.X;

			if (Left < 0)
			{
				Left = 0;
			}

			if (Desktop != null)
			{
				if (Left + Bounds.Width > Desktop.Bounds.Right)
				{
					Left = Desktop.Bounds.Right - Bounds.Width;
				}
			}

			Top += delta.Y;

			if (Top < 0)
			{
				Top = 0;
			}

			if (Desktop != null)
			{
				if (Top + Bounds.Height > Desktop.Bounds.Bottom)
				{
					Top = Desktop.Bounds.Bottom - Bounds.Height;
				}
			}

			_startPos = position;
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
			var mousePos = Desktop.MousePosition;
			if (bounds.Contains(mousePos))
			{
				_startPos = mousePos;
			}
		}

		public void ApplyWindowStyle(WindowStyle style)
		{
			ApplyWidgetStyle(style);

			if (style.TitleStyle != null)
			{
				_titleLabel.ApplyTextBlockStyle(style.TitleStyle);
			}

			if (style.CloseButtonStyle != null)
			{
				_closeButton.ApplyImageButtonStyle(style.CloseButtonStyle);
			}
		}

		public void ShowModal(Desktop desktop)
		{
			desktop.Widgets.Add(this);
			desktop.FocusedWidget = this;
		}

		public virtual void Close()
		{
			if (Desktop != null && Desktop.Widgets.Contains(this))
			{
				if (Desktop.FocusedWidget == this)
				{
					Desktop.FocusedWidget = null;
				}

				Desktop.Widgets.Remove(this);

				var ev = Closed;
				if (ev != null)
				{
					ev(this, EventArgs.Empty);
				}
			}
		}

		protected override void SetStyleByName(Stylesheet stylesheet, string name)
		{
			ApplyWindowStyle(stylesheet.WindowStyles[name]);
		}

		internal override string[] GetStyleNames(Stylesheet stylesheet)
		{
			return stylesheet.WindowStyles.Keys.ToArray();
		}
	}
}