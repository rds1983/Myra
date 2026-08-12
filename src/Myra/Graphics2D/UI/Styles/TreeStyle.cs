using System.ComponentModel;

namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class that defines the visual appearance of tree view widgets.
	/// </summary>
	public class TreeStyle : WidgetStyle
	{
		/// <summary>
		/// Gets or sets the style applied to the tree node expand/collapse toggle mark button.
		/// </summary>
		[Browsable(false)]
		public ImageButtonStyle MarkStyle { get; set; }

		/// <summary>
		/// Gets or sets the style applied to the tree node text label.
		/// </summary>
		[Browsable(false)]
		public LabelStyle LabelStyle { get; set; }

		/// <summary>
		/// Gets or sets the brush used for the background of selected tree nodes.
		/// </summary>
		public IBrush SelectionBackground
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the brush used for the background of hovered tree nodes.
		/// </summary>
		public IBrush SelectionHoverBackground
		{
			get; set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TreeStyle"/> class.
		/// </summary>
		public TreeStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TreeStyle"/> class by copying properties from another style.
		/// </summary>
		/// <param name="style">The source tree style to copy from.</param>
		public TreeStyle(TreeStyle style) : base(style)
		{
			MarkStyle = style.MarkStyle != null ? new ImageButtonStyle(style.MarkStyle) : null;
			LabelStyle = style.LabelStyle != null ? new LabelStyle(style.LabelStyle) : null;
			SelectionBackground = style.SelectionBackground;
			SelectionHoverBackground = style.SelectionHoverBackground;
		}

		/// <summary>
		/// Creates a deep copy of this tree style.
		/// </summary>
		/// <returns>A new TreeStyle instance with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new TreeStyle(this);
		}
	}
}