using Myra.Utility;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

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
	/// A layout system for containers that hold a single child widget of a specific type.
	/// </summary>
	/// <typeparam name="T">The type of widget contained.</typeparam>
	public class SingleItemLayout<T> : ILayout where T : Widget
	{
		private readonly Widget _container;

		private ObservableCollection<Widget> Children => _container.Children;

		/// <summary>
		/// Gets or sets the child widget.
		/// </summary>
		public T Child
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

		/// <summary>
		/// Initializes a new instance of the <see cref="SingleItemLayout{T}"/> class with the specified container.
		/// </summary>
		/// <param name="container">The container widget.</param>
		/// <exception cref="ArgumentNullException"><paramref name="container"/> is null.</exception>
		public SingleItemLayout(Widget container)
		{
			_container = container ?? throw new ArgumentNullException(nameof(container));
		}

		/// <summary>
		/// Measures the desired size of the child widget within the given available size.
		/// </summary>
		/// <param name="widgets">The widgets to measure (should contain the single child).</param>
		/// <param name="availableSize">The available size for layout.</param>
		/// <returns>The desired size of the child widget, or zero if no child is present.</returns>
		public Point Measure(IEnumerable<Widget> widgets, Point availableSize)
		{
			var result = Mathematics.PointZero;

			if (Child != null)
			{
				result = Child.Measure(availableSize);
			}

			return result;
		}

		/// <summary>
		/// Arranges the child widget within the specified bounds.
		/// </summary>
		/// <param name="widgets">The widgets to arrange (should contain the single child).</param>
		/// <param name="bounds">The bounds in which to arrange the child widget.</param>
		public void Arrange(IEnumerable<Widget> widgets, Rectangle bounds)
		{
			if (Child != null && Child.Visible)
			{
				Child.Arrange(bounds);
			}
		}
	}
}
