using Myra.Graphics2D.UI.Styles;
using System.ComponentModel;
using System.Xml.Serialization;
using Myra.Utility;

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
	/// An abstract base class for separator widgets that display a horizontal or vertical line.
	/// </summary>
	public abstract class SeparatorWidget : Image
	{
		/// <summary>
		/// Gets or sets the thickness of the separator line in pixels.
		/// </summary>
		public int Thickness { get; set; }

		/// <summary>
		/// Gets the orientation (horizontal or vertical) of the separator.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public abstract Orientation Orientation { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SeparatorWidget"/> class with the specified stylesheet and style.
		/// </summary>
		/// <param name="stylesheet">The stylesheet to use for applying the style.</param>
		/// <param name="styleName">The name of the style to apply to the separator.</param>
		protected SeparatorWidget(Stylesheet stylesheet, string styleName)
		{
			SetStyle(stylesheet, styleName);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SeparatorWidget"/> class.
		/// </summary>
		/// <param name="styleName">The name of the style to apply to the separator.</param>
		protected SeparatorWidget(string styleName) : this(Stylesheet.Current, styleName)
		{
		}

		/// <summary>
		/// Applies the specified widget style to this separator.
		/// </summary>
		/// <param name="style">The widget style to apply.</param>
		protected override void ApplyStyle(WidgetStyle style)
		{
			base.ApplyStyle(style);

			var separatorStyle = (SeparatorStyle)style;
			Renderable = separatorStyle.Image;
			Thickness = separatorStyle.Thickness;
		}

		/// <summary>
		/// Applies the specified separator style to this separator.
		/// </summary>
		/// <param name="style">The separator style to apply.</param>
		public void ApplySeparatorStyle(SeparatorStyle style) => ApplyStyle(style);

		/// <summary>
		/// Measures the size required for the separator widget based on its orientation.
		/// </summary>
		/// <param name="availableSize">The available size for the separator.</param>
		/// <returns>The measured size needed for the separator.</returns>
		protected override Point InternalMeasure(Point availableSize)
		{
			var result = Mathematics.PointZero;

			if (Orientation == Orientation.Horizontal)
			{
				result.Y = Thickness;
			}
			else
			{
				result.X = Thickness;
			}

			return result;
		}

		/// <summary>
		/// Renders the separator widget.
		/// </summary>
		/// <param name="context">The render context to draw with.</param>
		public override void InternalRender(RenderContext context)
		{
			base.InternalRender(context);
		}

		/// <summary>
		/// Copies the separator properties from another separator widget.
		/// </summary>
		/// <param name="w">The source separator to copy from.</param>
		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var separator = (SeparatorWidget)w;
			Thickness = separator.Thickness;
		}
	}
}