using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using Myra;
using System.ComponentModel;

namespace Myra.Samples.AllWidgets
{
	public enum ArrowDirection
	{
		Left,
		Right
	}

	public class Arrow : Widget
	{
		private const int HeadSize = 16;

		private Vector2[] _points;
		private ArrowDirection _direction;
		private int? _lastHeight;

		[Category("Appearance")]
		public ArrowDirection Direction
		{
			get
			{
				return _direction;
			}

			set
			{
				if (value == _direction)
				{
					return;
				}

				_direction = value;
				_points = null;
			}
		}

		[Category("Appearance")]
		public Color Color
		{
			get; set;
		}

		public Arrow()
		{
			Color = Color.White;
			Direction = ArrowDirection.Right;
		}

		public override void InternalRender(RenderContext context)
		{
			base.InternalRender(context);

			var bounds = ActualBounds;

			var h = bounds.Height / 2;
			if (_direction == ArrowDirection.Left)
			{
				if (_points == null || _lastHeight != bounds.Height)
				{
					_points = new Vector2[3];

					_points[0] = new Vector2(0, h);
					_points[1] = new Vector2(HeadSize, 0);
					_points[2] = new Vector2(HeadSize, bounds.Height);
				}

				context.Batch.DrawLine(new Vector2(bounds.Right, bounds.Top + h), new Vector2(bounds.Left + HeadSize, bounds.Top + h), Color);
				context.Batch.DrawPolygon(new Vector2(bounds.Left, bounds.Top), _points, Color);
			}
			else
			{
				if (_points == null || _lastHeight != bounds.Height)
				{
					_points = new Vector2[3];

					_points[0] = new Vector2(0, h);
					_points[1] = new Vector2(-HeadSize, 0);
					_points[2] = new Vector2(-HeadSize, bounds.Height);
				}

				context.Batch.DrawLine(new Vector2(bounds.Left, bounds.Top + h), new Vector2(bounds.Right - HeadSize, bounds.Top + h), Color);
				context.Batch.DrawPolygon(new Vector2(bounds.Right, bounds.Top), _points, Color);
			}

			_lastHeight = bounds.Height;
		}
	}
}
