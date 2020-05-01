using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;

#if !XENKO
using Microsoft.Xna.Framework;
#else
using Xenko.Core.Mathematics;
#endif

namespace Myra.Graphics2D.UI
{
	public abstract class ProgressBar : Widget
	{
		private IBrush _filler;
		private float _value;

		[Browsable(false)]
		[XmlIgnore]
		public abstract Orientation Orientation
		{
			get;
		}

		[Category("Behavior")]
		[DefaultValue(0.0f)]
		public float Minimum
		{
			get; set;
		}

		[Category("Behavior")]
		[DefaultValue(100.0f)]
		public float Maximum
		{
			get; set;
		}

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

				ValueChanged.Invoke(this);
			}
		}

		[Category("Appearance")]
		public IBrush Filler
		{
			get
			{
				return _filler;
			}

			set
			{
				_filler = value;
			}
		}

		public event EventHandler ValueChanged;

		protected ProgressBar(string styleName)
		{
			Maximum = 100;
			SetStyle(styleName);
		}

		public void ApplyProgressBarStyle(ProgressBarStyle style)
		{
			ApplyWidgetStyle(style);

			if (style.Filler == null)
				return;

			_filler = style.Filler;
		}

		public override void InternalRender(RenderContext context)
		{
			base.InternalRender(context);

			if (_filler == null)
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
				_filler.Draw(context.Batch,
					new Rectangle(bounds.X, bounds.Y, (int)(filledPart * bounds.Width), bounds.Height),
					Color.White);
			}
			else
			{
				_filler.Draw(context.Batch,
					new Rectangle(bounds.X, bounds.Y, bounds.Width, (int)(filledPart * bounds.Height)),
					Color.White);
			}
		}
	}
}