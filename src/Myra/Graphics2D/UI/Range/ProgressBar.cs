using System.ComponentModel;
using System.Xml.Serialization;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using Myra.Events;


#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// An abstract base class for progress bar widgets that display a filled portion based on a value.
	/// </summary>
	public abstract class ProgressBar : Widget
	{
		private float _value;

		/// <summary>
		/// Gets the orientation of the progress bar (horizontal or vertical).
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public abstract Orientation Orientation { get; }

		/// <summary>
		/// Gets or sets the minimum value of the progress bar. Default is 0.0.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(0.0f)]
		public float Minimum { get; set; }

		/// <summary>
		/// Gets or sets the maximum value of the progress bar. Default is 100.0.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(100.0f)]
		public float Maximum { get; set; }

		/// <summary>
		/// Gets or sets the current value of the progress bar between Minimum and Maximum. Default is 0.0.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(0.0f)]
		public float Value
		{
			get
			{
				return _value;
			}

			set
			{
				if (_value.EpsilonEquals(value))
				{
					return;
				}

				_value = value;

				ValueChanged.Invoke(this, InputEventType.ValueChanged);
			}
		}

		/// <summary>
		/// Gets or sets the brush used to fill the progress bar.
		/// </summary>
		[Category("Appearance")]
		public IBrush Filler { get; set; }

		/// <summary>
		/// Occurs when the value of the progress bar changes.
		/// </summary>
		public event MyraEventHandler ValueChanged;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProgressBar"/> class with the specified stylesheet and style.
		/// </summary>
		/// <param name="stylesheet">The stylesheet to use for applying the style.</param>
		/// <param name="styleName">The name of the style to apply.</param>
		protected ProgressBar(Stylesheet stylesheet, string styleName)
		{
			Maximum = 100;
			SetStyle(stylesheet, styleName);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProgressBar"/> class with the specified style.
		/// </summary>
		/// <param name="styleName">The name of the style to apply.</param>
		protected ProgressBar(string styleName) : this(Stylesheet.Current, styleName)
		{
		}

		/// <summary>
		/// Applies the specified widget style to this progress bar.
		/// </summary>
		/// <param name="style">The widget style to apply.</param>
		protected override void ApplyStyle(WidgetStyle style)
		{
			base.ApplyStyle(style);

			var progressBarStyle = (ProgressBarStyle)style;
			if (progressBarStyle.Filler == null)
				return;

			Filler = progressBarStyle.Filler;
		}

		/// <summary>
		/// Renders the progress bar with a filled portion based on the current value.
		/// </summary>
		/// <param name="context">The render context used for drawing.</param>
		public override void InternalRender(RenderContext context)
		{
			base.InternalRender(context);

			if (Filler == null)
			{
				return;
			}

			var v = _value;
			if (v < Minimum)
			{
				v = Minimum;
			}

			if (v > Maximum)
			{
				v = Maximum;
			}

			var delta = Maximum - Minimum;
			if (delta.IsZero())
			{
				return;
			}

			var filledPart = (v - Minimum) / delta;
			if (filledPart.EpsilonEquals(0.0f))
			{
				return;
			}

			var bounds = ActualBounds;
			if (Orientation == Orientation.Horizontal)
			{
				Filler.Draw(context,
					new Rectangle(bounds.X, bounds.Y, (int)(filledPart * bounds.Width), bounds.Height),
					Color.White);
			}
			else
			{
				Filler.Draw(context,
					new Rectangle(bounds.X, bounds.Y, bounds.Width, (int)(filledPart * bounds.Height)),
					Color.White);
			}
		}

		/// <summary>
		/// Copies the properties from another progress bar widget.
		/// </summary>
		/// <param name="w">The source progress bar widget to copy from.</param>
		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var progressBar = (ProgressBar)w;

			Minimum = progressBar.Minimum;
			Maximum = progressBar.Maximum;
			Value = progressBar.Value;
			Filler = progressBar.Filler;
		}
	}
}