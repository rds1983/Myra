using System;
using System.ComponentModel;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public abstract class ProgressBar : GridBased
	{
		private readonly Image _filledImage;
		private float _value;

		[HiddenInEditor]
		[JsonIgnore]
		public abstract Orientation Orientation { get; }

		[EditCategory("Behavior")]
		public float Minimum { get; set; }

		[EditCategory("Behavior")]
		[DefaultValue(100)]
		public float Maximum { get; set; }

		[EditCategory("Behavior")]
		public float Value
		{
			get { return _value; }

			set
			{
				if (_value.EpsilonEquals(value))
				{
					return;
				}

				_value = value;
				var v = value;
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

				Hint = (v - Minimum)/delta;
			}
		}

		private float Hint
		{
			get { return Orientation == Orientation.Horizontal ? GetColumnProportion(0).Value : GetRowProportion(1).Value; }

			set
			{
				if (Hint.EpsilonEquals(value))
				{
					return;
				}

				if (Orientation == Orientation.Horizontal)
				{
					ColumnsProportions[0].Value = value;
				}
				else
				{
					RowsProportions[1].Value = value;
				}

				var ev = ValueChanged;
				if (ev != null)
				{
					ev(this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler ValueChanged;

		protected ProgressBar(ProgressBarStyle style): base(style)
		{
			_filledImage = new Image
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch
			};
			if (Orientation == Orientation.Horizontal)
			{
				ColumnsProportions.Add(new Proportion(ProportionType.Part, 0));
				ColumnsProportions.Add(new Proportion(ProportionType.Fill));
				TotalColumnsPart = 1.0f;
			}
			else
			{
				RowsProportions.Add(new Proportion(ProportionType.Fill));
				RowsProportions.Add(new Proportion(ProportionType.Part, 0));
				TotalRowsPart = 1.0f;

				_filledImage.GridPositionY = 1;
			}

			Widgets.Add(_filledImage);

			if (style != null)
			{
				ApplyProgressBarStyle(style);
			}

			Maximum = 100;
		}

		public void ApplyProgressBarStyle(ProgressBarStyle style)
		{
			ApplyWidgetStyle(style);

			if (style.Filled == null) return;

			_filledImage.Drawable = style.Filled;
			_filledImage.UpdateImageSize(style.Filled);
		}
	}
}