using Myra.Graphics2D.UI.Styles;

#if !XENKO
using Microsoft.Xna.Framework;
#else
using Xenko.Core.Mathematics;
#endif

namespace Myra.Graphics2D.UI
{
	public class SeparatorWidget : Image
	{
		private readonly Orientation _orientation;

		public int Thickness
		{
			get; set;
		}

		internal SeparatorWidget(Orientation orientation, SeparatorStyle style)
		{
			_orientation = orientation;

			if (style != null)
			{
				ApplyMenuSeparatorStyle(style);
			}
		}

		public void ApplyMenuSeparatorStyle(SeparatorStyle style)
		{
			ApplyWidgetStyle(style);

			Renderable = style.Image;
			Thickness = style.Thickness;
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			var result = Point.Zero;

			if (_orientation == Orientation.Horizontal)
			{
				result.Y = Thickness;
			}
			else
			{
				result.X = Thickness;
			}

			return result;
		}

		public override void InternalRender(RenderContext context)
		{
			base.InternalRender(context);
		}
	}
}