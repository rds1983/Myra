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
		public virtual T Widget
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

		private void ChildOnVisibleChanged(object sender, EventArgs eventArgs)
		{
			InvalidateMeasure();
		}

		private void ChildOnMeasureChanged(object sender, EventArgs eventArgs)
		{
			InvalidateMeasure();
		}

		protected override IEnumerable<Widget> Children
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

			Widget.Layout(ActualBounds);
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			var result = Point.Zero;

			if (Widget != null)
			{
				result = Widget.Measure(availableSize);
			}

			return result;
		}

		public override void RemoveChild(Widget widget)
		{
			if (widget != Widget)
			{
				return;
			}

			Widget = null;
		}
	}
}