#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#elif STRIDE
using Stride.Core.Mathematics;
using Texture2D = Stride.Graphics.Texture;
#else
using System.Drawing;
using Texture2D = System.Object;
#endif

namespace Myra.Graphics2D.TextureAtlases
{
	public class NinePatchRegion : TextureRegion
	{
		private readonly Thickness _info;

		private readonly TextureRegion _topLeft,
			_topCenter,
			_topRight,
			_centerLeft,
			_center,
			_centerRight,
			_bottomLeft,
			_bottomCenter,
			_bottomRight;

		public Thickness Info
		{
			get { return _info; }
		}

		public NinePatchRegion(Texture2D texture, Rectangle bounds, Thickness info) : base(texture, bounds)
		{
			_info = info;

			var centerWidth = bounds.Width - info.Left - info.Right;
			var centerHeight = bounds.Height - info.Top - info.Bottom;

			var y = bounds.Y;
			if (info.Top > 0)
			{
				if (info.Left > 0)
				{
					_topLeft = new TextureRegion(texture,
						new Rectangle(bounds.X,
							y,
							info.Left,
							info.Top));
				}

				if (centerWidth > 0)
				{
					_topCenter = new TextureRegion(texture,
						new Rectangle(bounds.X + info.Left,
							y,
							centerWidth,
							info.Top));
				}

				if (info.Right > 0)
				{
					_topRight = new TextureRegion(texture,
						new Rectangle(bounds.X + info.Left + centerWidth,
							y,
							info.Right,
							info.Top));
				}
			}

			y += info.Top;
			if (centerHeight > 0)
			{
				if (info.Left > 0)
				{
					_centerLeft = new TextureRegion(texture,
						new Rectangle(bounds.X,
							y,
							info.Left,
							centerHeight));
				}

				if (centerWidth > 0)
				{
					_center = new TextureRegion(texture,
						new Rectangle(bounds.X + info.Left,
							y,
							centerWidth,
							centerHeight));
				}

				if (info.Right > 0)
				{
					_centerRight = new TextureRegion(texture,
						new Rectangle(bounds.X + info.Left + centerWidth,
							y,
							info.Right,
							centerHeight));
				}
			}

			y += centerHeight;
			if (info.Bottom > 0)
			{
				if (info.Left > 0)
				{
					_bottomLeft = new TextureRegion(texture,
						new Rectangle(bounds.X,
							y,
							info.Left,
							info.Bottom));
				}

				if (centerWidth > 0)
				{
					_bottomCenter = new TextureRegion(texture,
						new Rectangle(bounds.X + info.Left,
							y,
							centerWidth,
							info.Bottom));
				}

				if (info.Right > 0)
				{
					_bottomRight = new TextureRegion(texture,
						new Rectangle(bounds.X + info.Left + centerWidth,
							y,
							info.Right,
							info.Bottom));
				}
			}
		}

		public override void Draw(RenderContext context, Rectangle dest, Color color)
		{
			var y = dest.Y;

			var centerWidth = dest.Width - _info.Left - _info.Right;
			if (centerWidth < 0)
			{
				centerWidth = 0;
			}

			var centerHeight = dest.Height - _info.Top - _info.Bottom;
			if (centerHeight < 0)
			{
				centerHeight = 0;
			}

			if (_topLeft != null)
			{
				_topLeft.Draw(context,
					new Rectangle(dest.X,
						y,
						_info.Left,
						_info.Top),
					color);
			}

			if (_topCenter != null && centerWidth > 0)
			{
				_topCenter.Draw(context,
					new Rectangle(dest.X + _info.Left,
						y,
						centerWidth,
						_info.Top),
					color);
			}

			if (_topRight != null)
			{
				_topRight.Draw(context,
					new Rectangle(dest.X + Info.Left + centerWidth,
						y,
						_info.Right,
						_info.Top),
					color);
			}

			y += _info.Top;
			if (_centerLeft != null && centerHeight > 0)
			{
				_centerLeft.Draw(context,
					new Rectangle(dest.X,
						y,
						_info.Left,
						centerHeight),
					color);
			}

			if (_center != null && centerWidth > 0 && centerHeight > 0)
			{
				_center.Draw(context,
					new Rectangle(dest.X + _info.Left,
						y,
						centerWidth,
						centerHeight),
					color);
			}

			if (_centerRight != null && centerHeight > 0)
			{
				_centerRight.Draw(context,
					new Rectangle(dest.X + Info.Left + centerWidth,
						y,
						_info.Right,
						centerHeight),
					color);
			}

			y += centerHeight;
			if (_bottomLeft != null)
			{
				_bottomLeft.Draw(context,
					new Rectangle(dest.X,
						y,
						_info.Left,
						_info.Bottom),
					color);
			}

			if (_bottomCenter != null && centerWidth > 0)
			{
				_bottomCenter.Draw(context,
					new Rectangle(dest.X + _info.Left,
						y,
						centerWidth,
						_info.Bottom),
					color);
			}

			if (_bottomRight != null)
			{
				_bottomRight.Draw(context,
					new Rectangle(dest.X + Info.Left + centerWidth,
						y,
						_info.Right,
						_info.Bottom),
					color);
			}
		}
	}
}