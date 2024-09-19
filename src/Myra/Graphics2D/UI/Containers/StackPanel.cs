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
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI
{
	public abstract class StackPanel : Container
	{
		public static readonly AttachedPropertyInfo<ProportionType> ProportionTypeProperty =
			AttachedPropertiesRegistry.Create(typeof(StackPanel), "ProportionType", ProportionType.Auto, AttachedPropertyOption.AffectsMeasure);
		public static readonly AttachedPropertyInfo<float> ProportionValueProperty =
			AttachedPropertiesRegistry.Create(typeof(StackPanel), "ProportionValue", 1.0f, AttachedPropertyOption.AffectsMeasure);

		private readonly StackPanelLayout _layout;
		private readonly ObservableCollection<Proportion> _proportions = new ObservableCollection<Proportion>();
		private bool _childrenDirty = true;

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
			get => _layout.Spacing;
			set => _layout.Spacing = value;
		}

		[Browsable(false)]
		public Proportion DefaultProportion
		{
			get => _layout.DefaultProportion;
			set => _layout.DefaultProportion = value;
		}


		[Browsable(false)]
		[Obsolete("Use StackPanel.GetProportion/StackPanel.SetProportion")]
		[SkipSave]
		public ObservableCollection<Proportion> Proportions => _proportions;

		protected StackPanel()
		{
			_layout = new StackPanelLayout(Orientation);
			ChildrenLayout = _layout;
			GridLinesColor = Color.White;

			_proportions.CollectionChanged += (s, e) => InvalidateChildren();
		}

		public int GetCellSize(int index) => _layout.GetCellSize(index);

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

			var index = 0;
			foreach (var widget in ChildrenCopy)
			{
				if (index < _proportions.Count)
				{
					var prop = _proportions[index];
					SetProportionType(widget, prop.Type);
					SetProportionValue(widget, prop.Value);
				}

				++index;
			}

			_childrenDirty = false;
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			UpdateChildren();

			return base.InternalMeasure(availableSize);
		}

		protected override void InternalArrange()
		{
			UpdateChildren();

			base.InternalArrange();
		}

		public override void InternalRender(RenderContext context)
		{

			base.InternalRender(context);

			if (!ShowGridLines)
			{
				return;
			}

			var bounds = ActualBounds;

			int i;
			for (i = 0; i < _layout.GridLinesX.Count; ++i)
			{
				var x = _layout.GridLinesX[i] + bounds.Left;
				context.FillRectangle(new Rectangle(x, bounds.Top, 1, bounds.Height), GridLinesColor);
			}

			for (i = 0; i < _layout.GridLinesY.Count; ++i)
			{
				var y = _layout.GridLinesY[i] + bounds.Top;
				context.FillRectangle(new Rectangle(bounds.Left, y, bounds.Width, 1), GridLinesColor);
			}
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

		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var stackPanel = (StackPanel)w;

			ShowGridLines = stackPanel.ShowGridLines;
			GridLinesColor = stackPanel.GridLinesColor;
			Spacing = stackPanel.Spacing;
			DefaultProportion = stackPanel.DefaultProportion;
		}

		public static ProportionType GetProportionType(Widget widget) => ProportionTypeProperty.GetValue(widget);
		public static void SetProportionType(Widget widget, ProportionType value) => ProportionTypeProperty.SetValue(widget, value);
		public static float GetProportionValue(Widget widget) => ProportionValueProperty.GetValue(widget);
		public static void SetProportionValue(Widget widget, float value) => ProportionValueProperty.SetValue(widget, value);
	}
}
