using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Myra.Attributes;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public abstract class SingleItemContainer<T> : Container where T : Widget
	{
		private T _internalChild;

		[HiddenInEditor]
		[JsonIgnore]
		protected internal virtual T InternalChild
		{
			get { return _internalChild; }
			set
			{
				if (_internalChild != null)
				{
					_internalChild.Parent = null;
					_internalChild.Desktop = null;

					_internalChild.VisibleChanged -= ChildOnVisibleChanged;
					_internalChild.MeasureChanged -= ChildOnMeasureChanged;

					_internalChild = null;
				}

				_internalChild = value;

				if (_internalChild != null)
				{
					_internalChild.Parent = this;
					_internalChild.Desktop = Desktop;

					_internalChild.VisibleChanged += ChildOnVisibleChanged;
					_internalChild.MeasureChanged += ChildOnMeasureChanged;
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

		public override IEnumerable<Widget> Children
		{
			get
			{
				if (InternalChild == null)
				{
					yield break;
				}

				yield return InternalChild;
			}
		}

		public override void Arrange()
		{
			base.Arrange();

			if (InternalChild == null)
			{
				return;
			}

			InternalChild.Layout(ActualBounds);
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			var result = Point.Zero;

			if (InternalChild != null)
			{
				result = InternalChild.Measure(availableSize);
			}

			return result;
		}

		public override void RemoveChild(Widget widget)
		{
			if (widget != InternalChild)
			{
				return;
			}

			InternalChild = null;
		}
	}
}