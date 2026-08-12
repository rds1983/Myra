using Myra.Attributes;
using System;
using System.Collections.Generic;
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
	/// <summary>
	/// An interface for widgets that contain a collection of child widgets.
	/// </summary>
	public interface IContainer
	{
		/// <summary>
		/// Gets the collection of child widgets in the container.
		/// </summary>
		IList<Widget> Widgets { get; }
	}

	/// <summary>
	/// An abstract base class for container widgets that hold multiple child widgets.
	/// </summary>
	public abstract class Container : Widget, IContainer
	{
		/// <summary>
		/// Gets the collection of child widgets in the container.
		/// </summary>
		[Content]
		[Browsable(false)]
		public virtual IList<Widget> Widgets => Children;

		/// <summary>
		/// Gets or sets the horizontal alignment of the container. Default is Stretch.
		/// </summary>
		[DefaultValue(HorizontalAlignment.Stretch)]
		public override HorizontalAlignment HorizontalAlignment
		{
			get { return base.HorizontalAlignment; }
			set { base.HorizontalAlignment = value; }
		}

		/// <summary>
		/// Gets or sets the vertical alignment of the container. Default is Stretch.
		/// </summary>
		[DefaultValue(VerticalAlignment.Stretch)]
		public override VerticalAlignment VerticalAlignment
		{
			get { return base.VerticalAlignment; }
			set { base.VerticalAlignment = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Container"/> class with stretch alignment.
		/// </summary>
		public Container()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;
		}

		/// <summary>
		/// Determines whether input at the specified local position falls through the container.
		/// </summary>
		/// <param name="localPos">The local position to check.</param>
		/// <returns>True if input falls through (when background is null); otherwise, false.</returns>
		public override bool InputFallsThrough(Point localPos) => Background == null;

		/// <summary>
		/// Copies properties from another widget to this container, including all child widgets.
		/// </summary>
		/// <param name="w">The source widget to copy from.</param>
		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var container = (Container)w;
			foreach(var child in container.Widgets)
			{
				Widgets.Add(child.Clone());
			}
		}
	}
}