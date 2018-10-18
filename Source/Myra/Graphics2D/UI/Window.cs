using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class Window : GridBased
	{
		[Obsolete("Enum is obsolete and will be removed in the future version")]
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
					Widgets.Remove(_content);
				}

				if (value != null)
				{
					value.GridPositionY = 1;
					Widgets.Add(value);
				}

				_content = value;
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
		[Obsolete("Property is obsolete and will be removed in the future version. Use Result.")]
		public int ModalResult { get; set; }

		public bool Result { get; set; }

		public event EventHandler Closed;

		public Window(WindowStyle style)
		{
			ModalResult = (int)DefaultModalResult.Cancel;
			Result = false;
			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;
			CanFocus = true;

			RowSpacing = 8;

			RowsProportions.Add(new Proportion(ProportionType.Auto));
			RowsProportions.Add(new Proportion(ProportionType.Fill));

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

			_closeButton.Up += (sender, args) =>
			{
				Close();
			};

			_titleGrid.Widgets.Add(_closeButton);

			Widgets.Add(_titleGrid);

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

		public override void OnDesktopChanging()
		{
			base.OnDesktopChanging();

			if (Desktop != null)
			{
				Desktop.MouseMoved -= DesktopOnMouseMoved;
			}
		}

		public override void OnDesktopChanged()
		{
			base.OnDesktopChanged();

			if (Desktop != null)
			{
				Desktop.MouseMoved += DesktopOnMouseMoved;
			}

			CenterOnDesktop();
		}

		public void CenterInBounds(Rectangle bounds)
		{
			var size = Measure(bounds.Size());
			XHint = (bounds.Width - size.X) / 2;
			YHint = (bounds.Height - size.Y) / 2;
		}

		public void CenterOnDesktop()
		{
			if (Desktop == null)
			{
				return;
			}

			CenterInBounds(Desktop.Bounds);
		}

		private void DesktopOnMouseMoved(object sender, GenericEventArgs<Point> genericEventArgs)
		{
			if (_startPos == null)
			{
				return;
			}

			var position = genericEventArgs.Data;

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

			XHint += delta.X;

			if (XHint < 0)
			{
				XHint = 0;
			}

			if (Desktop != null)
			{
				if (XHint + Bounds.Width > Desktop.Bounds.Right)
				{
					XHint = Desktop.Bounds.Right - Bounds.Width;
				}
			}

			YHint += delta.Y;

			if (YHint < 0)
			{
				YHint = 0;
			}

			if (Desktop != null)
			{
				if (YHint + Bounds.Height > Desktop.Bounds.Bottom)
				{
					YHint = Desktop.Bounds.Bottom - Bounds.Height;
				}
			}

			_startPos = position;
		}

		public override void OnMouseUp(MouseButtons mb)
		{
			base.OnMouseUp(mb);

			_startPos = null;
		}

		public override void OnMouseDown(MouseButtons mb)
		{
			base.OnMouseDown(mb);

			if (Desktop != null)
			{
				var mousePos = Desktop.MousePosition;
				if (_titleGrid.Bounds.Contains(mousePos))
				{
					_startPos = mousePos;
				}
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