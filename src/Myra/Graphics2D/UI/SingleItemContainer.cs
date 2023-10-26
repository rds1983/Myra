using System.Xml.Serialization;
using Myra.Utility;
using System.ComponentModel;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
#endif

namespace Myra.Graphics2D.UI
{
	public class SingleItemContainer<T> : ContentControl where T : Widget
	{
		[Browsable(false)]
		[XmlIgnore]
		protected internal virtual T InternalChild
		{
			get { return Children.Count > 0 ? (T)Children[0] : null; }
			set
			{
				Children.Clear();

				if (value != null)
				{
					Children.Add(value);
				}
			}
		}

		public override Widget Content
		{
			get => InternalChild;
			set => InternalChild = (T)value;
		}

		protected override void InternalArrange()
		{
			base.InternalArrange();

			if (InternalChild != null && InternalChild.Visible)
			{
				InternalChild.Arrange(ActualBounds);
			}
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			var result = Mathematics.PointZero;

			if (InternalChild != null)
			{
				result = InternalChild.Measure(availableSize);
			}

			return result;
		}
	}
}