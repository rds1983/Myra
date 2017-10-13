using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Myra.Attributes;
using Myra.Utility;

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