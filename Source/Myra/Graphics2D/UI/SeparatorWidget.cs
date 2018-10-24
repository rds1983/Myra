using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class SeparatorWidget : Image
	{
		private readonly Orientation _orientation;

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

			Drawable = style.Image;

			if (_orientation == Orientation.Horizontal)
			{
				HeightHint = style.Thickness;
			}
			else
			{
				WidthHint = style.Thickness;
			}
		}

		public override void InternalRender(RenderContext context)
		{
			base.InternalRender(context);
		}
	}
}