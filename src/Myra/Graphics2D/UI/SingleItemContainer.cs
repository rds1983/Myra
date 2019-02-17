using System;
using Myra.Attributes;
using System.Xml.Serialization;

#if !XENKO
using Microsoft.Xna.Framework;
#else
using Xenko.Core.Mathematics;
#endif

namespace Myra.Graphics2D.UI
{
	public class SingleItemContainer<T> : Container where T : Widget
	{
		private T _internalChild;

		[HiddenInEditor]
		[XmlIgnore]
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

				InvalidateChildren();
			}
		}

		public override int ChildrenCount
		{
			get
			{
				return InternalChild != null ? 1 : 0;
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

		public override Widget GetChild(int index)
		{
			if (index < 0 ||
				InternalChild == null ||
				index >= 1)
			{
				throw new ArgumentOutOfRangeException("index");
			}

			return InternalChild;
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