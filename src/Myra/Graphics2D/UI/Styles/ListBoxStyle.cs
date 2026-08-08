using System.ComponentModel;

namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class that defines the visual appearance of list box widgets.
	/// </summary>
	public class ListBoxStyle : WidgetStyle
	{
		/// <summary>
		/// Gets or sets the style applied to list box items.
		/// </summary>
		[Browsable(false)]
		public ButtonStyle ListItemStyle { get; set; }

		/// <summary>
		/// Gets or sets the style applied to separator lines between list items.
		/// </summary>
		[Browsable(false)]
		public SeparatorStyle SeparatorStyle { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ListBoxStyle"/> class.
		/// </summary>
		public ListBoxStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ListBoxStyle"/> class by copying properties from another style.
		/// </summary>
		/// <param name="style">The source list box style to copy from.</param>
		public ListBoxStyle(ListBoxStyle style) : base(style)
		{
			ListItemStyle = style.ListItemStyle != null ? new ButtonStyle(style.ListItemStyle) : null;
			SeparatorStyle = style.SeparatorStyle != null ? new SeparatorStyle(style.SeparatorStyle) : null;
		}

		/// <summary>
		/// Creates a deep copy of this list box style.
		/// </summary>
		/// <returns>A new ListBoxStyle instance with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new ListBoxStyle(this);
		}
	}
}