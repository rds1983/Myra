using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Myra.Attributes;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public abstract class SingleItemContainer<T> : Container where T : Widget
	{
		private T _widget;

		[HiddenInEditor]
		[JsonIgnore]
		public T Widget
		{
			get { return _widget; }
			set
			{
				if (_widget != null)
				{
					_widget.Parent = null;
					_widget.Desktop = null;

					_widget.VisibleChanged -= ChildOnVisibleChanged;
					_widget.MeasureChanged -= ChildOnMeasureChanged;

					_widget = null;
				}

				_widget = value;
				
				if (_widget != null)
				{
					_widget.Parent = this;
					_widget.Desktop = Desktop;

					_widget.VisibleChanged += ChildOnVisibleChanged;
					_widget.MeasureChanged += ChildOnMeasureChanged;
				}
			}
		}

		public override int ChildCount
		{
			get { return 1; }
		}

		private void ChildOnVisibleChanged(object sender, EventArgs eventArgs)
		{
			InvalidateMeasure();
		}

		private void ChildOnMeasureChanged(object sender, EventArgs eventArgs)
		{
			InvalidateMeasure();
		}

		public override IEnumerable<Widget> Children
		{
			get
			{
				if (Widget == null)
				{
					yield break;
				}

				yield return Widget;
			}
		}

		public override void Arrange()
		{
			base.Arrange();

			if (Widget == null)
			{
				return;
			}

			Widget.Layout(LayoutBounds);
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			var result = Point.Zero;
			;
			if (Widget != null)
			{
				result = Widget.Measure(availableSize);
			}

			return result;
		}
	}
}