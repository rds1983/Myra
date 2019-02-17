using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using static Myra.Graphics2D.UI.Grid;

namespace Myra.Graphics2D.UI
{
	public abstract class ProgressBar : SingleItemContainer<Grid>
	{
		private readonly Image _filledImage;
		private float _value;

		[HiddenInEditor]
		[XmlIgnore]
		public abstract Orientation Orientation { get; }

		[EditCategory("Behavior")]
		[DefaultValue(0.0f)]
		public float Minimum { get; set; }

		[EditCategory("Behavior")]
		[DefaultValue(100.0f)]
		public float Maximum { get; set; }

		[EditCategory("Behavior")]
		[DefaultValue(0.0f)]
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
			get { return Orientation == Orientation.Horizontal ? InternalChild.GetColumnProportion(0).Value : InternalChild.GetRowProportion(1).Value; }

			set
			{
				if (Hint.EpsilonEquals(value))
				{
					return;
				}

				if (Orientation == Orientation.Horizontal)
				{
					InternalChild.ColumnsProportions[0].Value = value;
				}
				else
				{
					InternalChild.RowsProportions[1].Value = value;
				}

				var ev = ValueChanged;
				if (ev != null)
				{
					ev(this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler ValueChanged;

		protected ProgressBar(ProgressBarStyle style)
		{
			InternalChild = new Grid();
			_filledImage = new Image
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch
			};
			if (Orientation == Orientation.Horizontal)
			{
				InternalChild.ColumnsProportions.Add(new Proportion(ProportionType.Part, 0));
				InternalChild.ColumnsProportions.Add(new Proportion(ProportionType.Fill));
				InternalChild.TotalColumnsPart = 1.0f;
			}
			else
			{
				InternalChild.RowsProportions.Add(new Proportion(ProportionType.Fill));
				InternalChild.RowsProportions.Add(new Proportion(ProportionType.Part, 0));
				InternalChild.TotalRowsPart = 1.0f;

				_filledImage.GridRow = 1;
			}

			InternalChild.Widgets.Add(_filledImage);

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

			_filledImage.Renderable = style.Filled;
		}
	}
}