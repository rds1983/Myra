using Microsoft.Xna.Framework;

namespace Myra.Graphics2D.UI
{
	public class Panel : MultipleItemsContainer
	{
		public override void Arrange()
		{
			foreach (var control in _widgets)
			{
				if (!control.Visible)
				{
					continue;
				}

				LayoutControl(control);
			}
		}

		private void LayoutControl(Widget control)
		{
			control.Layout(ActualBounds);
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			Point result = Point.Zero;
			foreach (var control in _widgets)
			{
				if (!control.Visible)
				{
					continue;
				}

				Point measure = control.Measure(availableSize);

				if (measure.X > result.X)
				{
					result.X = measure.X;
				}

				if (measure.Y > result.Y)
				{
					result.Y = measure.Y;
				}
			}

			return result;
		}
	}
}