using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class SeparatorWidget : GridBased
	{
		private readonly Orientation _orientation;
		private readonly Widget _image;

		internal SeparatorWidget(Orientation orientation, SeparatorStyle style)
		{
			_orientation = orientation;
			_image = new Widget();

			if (orientation == Orientation.Horizontal)
			{
				_image.HorizontalAlignment = HorizontalAlignment.Center;
				_image.VerticalAlignment = VerticalAlignment.Stretch;
			}
			else
			{
				_image.HorizontalAlignment = HorizontalAlignment.Stretch;
				_image.VerticalAlignment = VerticalAlignment.Center;
			}

			Widgets.Add(_image);

			if (style != null)
			{
				ApplyMenuSeparatorStyle(style);
			}
		}

		public void ApplyMenuSeparatorStyle(SeparatorStyle style)
		{
			ApplyWidgetStyle(style);

			_image.Background = style.Image;

			if (_orientation == Orientation.Horizontal)
			{
				_image.WidthHint = style.Thickness;
			}
			else
			{
				_image.HeightHint = style.Thickness;
			}
		}
	}
}