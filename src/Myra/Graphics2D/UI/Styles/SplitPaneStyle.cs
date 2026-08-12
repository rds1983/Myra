using System.ComponentModel;

namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class for split pane splitter handle buttons.
	/// </summary>
	public class SplitPanelButtonStyle : ButtonStyle
	{
		/// <summary>
		/// Gets or sets the width or height of the splitter handle in pixels, or null for default sizing.
		/// </summary>
		public int? HandleSize { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SplitPanelButtonStyle"/> class.
		/// </summary>
		public SplitPanelButtonStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SplitPanelButtonStyle"/> class by copying properties from another style.
		/// </summary>
		/// <param name="style">The source split panel button style to copy from.</param>
		public SplitPanelButtonStyle(SplitPanelButtonStyle style) : base(style)
		{
			HandleSize = style.HandleSize;
		}

		/// <summary>
		/// Creates a deep copy of this split panel button style.
		/// </summary>
		/// <returns>A new SplitPanelButtonStyle instance with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new SplitPanelButtonStyle(this);
		}
	}

	/// <summary>
	/// Style class that defines the visual appearance of split pane widgets.
	/// </summary>
	public class SplitPaneStyle : WidgetStyle
	{
		/// <summary>
		/// Gets or sets the style applied to the split pane's splitter handle.
		/// </summary>
		[Browsable(false)]
		public SplitPanelButtonStyle HandleStyle { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SplitPaneStyle"/> class.
		/// </summary>
		public SplitPaneStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SplitPaneStyle"/> class by copying properties from another style.
		/// </summary>
		/// <param name="style">The source split pane style to copy from.</param>
		public SplitPaneStyle(SplitPaneStyle style) : base(style)
		{
			HandleStyle = style.HandleStyle != null ? new SplitPanelButtonStyle(style.HandleStyle) : null;
		}

		/// <summary>
		/// Creates a deep copy of this split pane style.
		/// </summary>
		/// <returns>A new SplitPaneStyle instance with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new SplitPaneStyle(this);
		}
	}
}
