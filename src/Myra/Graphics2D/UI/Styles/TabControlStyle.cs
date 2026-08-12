using System.ComponentModel;

namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class that defines the visual appearance of tab control widgets.
	/// </summary>
	public class TabControlStyle : WidgetStyle
	{
		/// <summary>
		/// Gets or sets the style applied to individual tab header buttons.
		/// </summary>
		[Browsable(false)]
		public ImageTextButtonStyle TabItemStyle { get; set; }

		/// <summary>
		/// Gets or sets the style applied to the tab content area.
		/// </summary>
		[Browsable(false)]
		public WidgetStyle ContentStyle { get; set; }

		/// <summary>
		/// Gets or sets the spacing in pixels between tab buttons in the header.
		/// </summary>
		public int ButtonSpacing { get; set; }

		/// <summary>
		/// Gets or sets the spacing in pixels between the tab header and the content area.
		/// </summary>
		public int HeaderSpacing { get; set; }

		/// <summary>
		/// Gets or sets the position of the tab selector (header) relative to the content.
		/// </summary>
		public TabSelectorPosition TabSelectorPosition { get; set; }

		/// <summary>
		/// Gets or sets the style applied to the close button on tab headers, if present.
		/// </summary>
		[Browsable(false)]
		public ImageButtonStyle CloseButtonStyle { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="TabControlStyle"/> class.
		/// </summary>
		public TabControlStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TabControlStyle"/> class by copying properties from another style.
		/// </summary>
		/// <param name="style">The source tab control style to copy from.</param>
		public TabControlStyle(TabControlStyle style) : base(style)
		{
			TabItemStyle = style.TabItemStyle != null ? new ImageTextButtonStyle(style.TabItemStyle) : null;
			ContentStyle = style.ContentStyle != null ? new WidgetStyle(style.ContentStyle) : null;

			ButtonSpacing = style.ButtonSpacing;
			HeaderSpacing = style.HeaderSpacing;

			TabSelectorPosition = style.TabSelectorPosition;
		}

		/// <summary>
		/// Creates a deep copy of this tab control style.
		/// </summary>
		/// <returns>A new TabControlStyle instance with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new TabControlStyle(this);
		}
	}
}
