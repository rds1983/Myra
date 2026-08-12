using System.Collections.Generic;

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
	/// Defines the interface for layout systems that measure and arrange widgets.
	/// </summary>
	public interface ILayout
	{
		/// <summary>
		/// Measures the desired size of widgets within the given available size.
		/// </summary>
		/// <param name="widgets">The widgets to measure.</param>
		/// <param name="availableSize">The available size for layout.</param>
		/// <returns>The desired size needed to display all widgets.</returns>
		Point Measure(IEnumerable<Widget> widgets, Point availableSize);

		/// <summary>
		/// Arranges the widgets within the specified bounds.
		/// </summary>
		/// <param name="widgets">The widgets to arrange.</param>
		/// <param name="bounds">The bounds in which to arrange the widgets.</param>
		void Arrange(IEnumerable<Widget> widgets, Rectangle bounds);
	}
}
