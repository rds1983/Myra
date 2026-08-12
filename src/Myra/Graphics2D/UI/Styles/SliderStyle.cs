using System.ComponentModel;

namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class that defines the visual appearance of slider widgets.
	/// </summary>
	public class SliderStyle : WidgetStyle
	{
		/// <summary>
		/// Gets or sets the style applied to the slider's draggable knob button.
		/// </summary>
		[Browsable(false)]
		public ImageButtonStyle KnobStyle { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SliderStyle"/> class.
		/// </summary>
		public SliderStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SliderStyle"/> class by copying properties from another style.
		/// </summary>
		/// <param name="style">The source slider style to copy from.</param>
		public SliderStyle(SliderStyle style) : base(style)
		{
			KnobStyle = style.KnobStyle != null ? new ImageButtonStyle(style.KnobStyle) : null;
		}

		/// <summary>
		/// Creates a deep copy of this slider style.
		/// </summary>
		/// <returns>A new SliderStyle instance with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new SliderStyle(this);
		}
	}
}
