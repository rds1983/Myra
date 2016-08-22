using Myra.Graphics2D.Text;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class MenuSeparator: Grid
	{
		private readonly Orientation _orientation;
		private readonly Widget _image;

		public MenuSeparator(Orientation orientation, MenuSeparatorStyle style)
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

			Children.Add(_image);

			if (style != null)
			{
				ApplyMenuSeparatorStyle(style);
			}
		}

		public void ApplyMenuSeparatorStyle(MenuSeparatorStyle style)
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
