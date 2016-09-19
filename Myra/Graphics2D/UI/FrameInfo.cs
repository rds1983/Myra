using System;

namespace Myra.Graphics2D.UI
{
	public struct FrameInfo
	{
		private PaddingInfo _margin, _padding, _border;

		public PaddingInfo Margin
		{
			get { return _margin; }

			set
			{
				if (value == _margin)
				{
					return;
				}

				_margin = value;
				FireChanged();
			}
		}

		public PaddingInfo Padding
		{
			get { return _padding; }

			set
			{
				if (value == _padding)
				{
					return;
				}

				_padding = value;
				FireChanged();
			}
		}

		public PaddingInfo Border
		{
			get { return _border; }

			set
			{
				if (value == _border)
				{
					return;
				}

				_border = value;
				FireChanged();
			}
		}

		public Drawable BorderDrawable { get; set; }

		public int Left { get; private set; }
		public int Right { get; private set; }
		public int Top { get; private set; }
		public int Bottom { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }

		public event EventHandler SizeChanged;

		private void FireChanged()
		{
			Left = Margin.Left + Padding.Left + Border.Left;
			Right = Margin.Right + Padding.Right + Border.Right;
			Width = Margin.Width + Padding.Width + Border.Width;
			Top = Margin.Top + Padding.Top + Border.Top;
			Bottom = Margin.Bottom + Padding.Bottom + Border.Bottom;
			Height = Margin.Height + Padding.Height + Border.Height;

			var ev = SizeChanged;

			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}

		public static bool operator ==(FrameInfo a, FrameInfo b)
		{
			return a._margin == b._margin &&
			       a._border == b._border &&
			       a._padding == b._padding;
		}

		public static bool operator !=(FrameInfo a, FrameInfo b)
		{
			return !(a == b);
		}
	}
}