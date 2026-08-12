using System.Collections;
using Myra.Utility;
using Myra.Graphics2D.UI.Styles;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
#endif

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A container that arranges all of its children at the same location, allowing them to overlap.
	/// </summary>
	public class Panel : Container
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Panel"/> class with the specified stylesheet and style name.
		/// </summary>
		/// <param name="stylesheet">The stylesheet to use for styling this panel.</param>
		/// <param name="styleName">The name of the style to apply to this panel.</param>
		public Panel(Stylesheet stylesheet, string styleName = Stylesheet.DefaultStyleName)
		{
			SetStyle(stylesheet, styleName);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Panel"/> class with the specified style name.
		/// </summary>
		/// <param name="styleName">The name of the style to apply to this panel.</param>
		public Panel(string styleName = Stylesheet.DefaultStyleName) : this(Stylesheet.Current, styleName)
		{
		}
		/// <summary>
		/// Arranges child controls at the same location in the panel bounds.
		/// </summary>
		protected override void InternalArrange()
		{
			foreach (var control in ChildrenCopy)
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
			control.Arrange(ActualBounds);
		}

		/// <summary>
		/// Measures the size required for all child controls in the panel.
		/// </summary>
		/// <param name="availableSize">The available size for measurement.</param>
		/// <returns>The largest size required by any child control.</returns>
		protected override Point InternalMeasure(Point availableSize)
		{
			Point result = Mathematics.PointZero;

			foreach (var control in ChildrenCopy)
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

		/// <summary>
		/// Gets the dictionary of styles for this panel from the stylesheet.
		/// </summary>
		/// <param name="stylesheet">The stylesheet to retrieve styles from.</param>
		/// <returns>The dictionary of panel styles.</returns>
		internal override IDictionary GetStylesDictionary(Stylesheet stylesheet) => stylesheet.PanelStyles;

		/// <summary>
		/// Gets a value indicating whether the style for this panel can be null.
		/// </summary>
		internal override bool CanStyleBeNull => true;

		/// <summary>
		/// Applies the specified widget style to this panel.
		/// </summary>
		/// <param name="style">The style to apply.</param>
		protected override void ApplyStyle(WidgetStyle style)
		{
			base.ApplyStyle(style);
		}
	}
}