using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;
using Myra.Attributes;
using Myra.MML;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
#endif

namespace Myra.Graphics2D.UI
{
	public abstract class StackPanel : MultipleItemsContainerBase
	{
		public static readonly AttachedPropertyInfo<ProportionType> ProportionTypeProperty =
			AttachedPropertiesRegistry.Create(typeof(StackPanel), "ProportionType", ProportionType.Auto, AttachedPropertyOption.AffectsMeasure);
		public static readonly AttachedPropertyInfo<float> ProportionValueProperty =
			AttachedPropertiesRegistry.Create(typeof(StackPanel), "ProportionValue", 1.0f, AttachedPropertyOption.AffectsMeasure);


		private readonly ObservableCollection<Proportion> _proportions = new ObservableCollection<Proportion>();
		private readonly GridLayout _layout = new GridLayout();
		private bool _childrenDirty = true;
		private int _spacing;

		[Browsable(false)]
		[XmlIgnore]
		public abstract Orientation Orientation { get; }

		[Category("Debug")]
		[DefaultValue(false)]
		public bool ShowGridLines { get; set; }

		[Category("Debug")]
		[DefaultValue("White")]
		public Color GridLinesColor { get; set; }

		[Category("Layout")]
		[DefaultValue(0)]
		public int Spacing
		{
			get => _spacing;
			set
			{
				_spacing = value;
				if (Orientation == Orientation.Horizontal)
				{
					_layout.ColumnSpacing = value;
				}
				else
				{
					_layout.RowSpacing = value;
				}
			}
		}

		[Browsable(false)]
		public Proportion DefaultProportion
		{
			get => Orientation == Orientation.Horizontal ? _layout.DefaultColumnProportion : _layout.DefaultRowProportion;
			set
			{
				if (Orientation == Orientation.Horizontal)
				{
					_layout.DefaultColumnProportion = value;
				}
				else
				{
					_layout.DefaultRowProportion = value;
				}
			}
		}

		private ObservableCollection<Proportion> InternalProportions
		{
			get => Orientation == Orientation.Horizontal ? _layout.ColumnsProportions : _layout.RowsProportions;
		}


		[Browsable(false)]
		[Obsolete("Use StackPanel.GetProportion/StackPanel.SetProportion")]
		[SkipSave]
		public ObservableCollection<Proportion> Proportions => _proportions;

		protected StackPanel()
		{
			GridLinesColor = Color.White;
			DefaultProportion = Proportion.StackPanelDefault;

			_proportions.CollectionChanged += (s, e) => InvalidateChildren();
		}

		private void InvalidateChildren()
		{
			_childrenDirty = true;
		}

		protected void UpdateChildren()
		{
			if (!_childrenDirty)
			{
				return;
			}

			InternalProportions.Clear();

			var index = 0;
			foreach (var widget in Widgets)
			{
				if (Orientation == Orientation.Horizontal)
				{
					Grid.SetColumn(widget, index);
				}
				else
				{
					Grid.SetRow(widget, index);
				}

				if (index < _proportions.Count)
				{
					var prop = _proportions[index];
					SetProportionType(widget, prop.Type);
					SetProportionValue(widget, prop.Value);
				}

				InternalProportions.Add(new Proportion(GetProportionType(widget), GetProportionValue(widget)));

				++index;
			}

			_childrenDirty = false;
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			UpdateChildren();

			return _layout.Measure(Widgets, availableSize);
		}

		protected override void InternalArrange()
		{
			UpdateChildren();

			_layout.Arrange(Widgets, ActualBounds);
		}

		public override void OnAttachedPropertyChanged(BaseAttachedPropertyInfo propertyInfo)
		{
			base.OnAttachedPropertyChanged(propertyInfo);

			if (propertyInfo.Id == ProportionTypeProperty.Id ||
				propertyInfo.Id == ProportionValueProperty.Id)
			{
				InvalidateChildren();
			}
		}

		public static ProportionType GetProportionType(Widget widget) => ProportionTypeProperty.GetValue(widget);
		public static void SetProportionType(Widget widget, ProportionType value) => ProportionTypeProperty.SetValue(widget, value);
		public static float GetProportionValue(Widget widget) => ProportionValueProperty.GetValue(widget);
		public static void SetProportionValue(Widget widget, float value) => ProportionValueProperty.SetValue(widget, value);
	}
}
