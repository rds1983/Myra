#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using Color = FontStashSharp.FSColor;
#endif

using System.ComponentModel;

namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class that defines the visual appearance of menu widgets.
	/// </summary>
	public class MenuStyle : WidgetStyle
	{
		/// <summary>
		/// Gets or sets the style applied to menu item icons.
		/// </summary>
		[Browsable(false)]
		public ImageStyle ImageStyle { get; set; }

		/// <summary>
		/// Gets or sets the style applied to menu item text labels.
		/// </summary>
		[Browsable(false)]
		public LabelStyle LabelStyle { get; set; }

		/// <summary>
		/// Gets or sets the style applied to menu item shortcut key text.
		/// </summary>
		[Browsable(false)]
		public LabelStyle ShortcutStyle { get; set; }

		/// <summary>
		/// Gets or sets the style applied to menu separator lines.
		/// </summary>
		[Browsable(false)]
		public SeparatorStyle SeparatorStyle { get; set; }

		/// <summary>
		/// Gets or sets the brush used for the background of hovered menu items.
		/// </summary>
		public IBrush SelectionHoverBackground { get; set; }

		/// <summary>
		/// Gets or sets the brush used for the background of selected menu items.
		/// </summary>
		public IBrush SelectionBackground { get; set; }

		/// <summary>
		/// Gets or sets the color of the special character indicator (underscore) in menu item text.
		/// </summary>
		public Color? SpecialCharColor { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="MenuStyle"/> class.
		/// </summary>
		public MenuStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MenuStyle"/> class by copying properties from another style.
		/// </summary>
		/// <param name="style">The source menu style to copy from.</param>
		public MenuStyle(MenuStyle style) : base(style)
		{
			ImageStyle = style.ImageStyle != null ? new ImageStyle(style.ImageStyle) : null;
			LabelStyle = style.LabelStyle != null ? new LabelStyle(style.LabelStyle) : null;
			ShortcutStyle = style.ShortcutStyle != null ? new LabelStyle(style.ShortcutStyle) : null;
			SeparatorStyle = style.SeparatorStyle != null ? new SeparatorStyle(style.SeparatorStyle) : null;
			SpecialCharColor = style.SpecialCharColor;
		}

		/// <summary>
		/// Creates a deep copy of this menu style.
		/// </summary>
		/// <returns>A new MenuStyle instance with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new MenuStyle(this);
		}
	}
}
