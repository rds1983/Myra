using System.ComponentModel;
using System.Xml.Serialization;
using Myra.Attributes;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#endif

namespace Myra.Graphics2D.UI.Properties
{
    public partial class PropertyGrid
    {
        private class SubGrid : Widget
		{
			private readonly GridLayout _layout = new GridLayout();

			private readonly ToggleButton _mark;
			private readonly PropertyGrid _propertyGrid;

			public ToggleButton Mark => _mark;
			public PropertyGrid PropertyGrid => _propertyGrid;
			
			public Rectangle HeaderBounds => new Rectangle(0, 0, ActualBounds.Width, _layout.GetRowHeight(0));
			
			[Browsable(false)]
			[XmlIgnore]
			public bool IsEmpty => _propertyGrid.IsEmpty;

			public SubGrid(PropertyGrid parent, object value, string header, string category, string filter, Record parentProperty)
			{
				ChildrenLayout = _layout;

				_layout.ColumnSpacing = 4;
				_layout.RowSpacing = 4;

				_layout.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
				_layout.ColumnsProportions.Add(new Proportion(ProportionType.Fill));
				_layout.RowsProportions.Add(new Proportion(ProportionType.Auto));
				_layout.RowsProportions.Add(new Proportion(ProportionType.Auto));

				_propertyGrid = new PropertyGrid(parent.PropertyGridStyle, category, parentProperty, parent)
				{
					Object = value,
					Filter = filter,
					HorizontalAlignment = HorizontalAlignment.Stretch,
				};
				Grid.SetColumn(_propertyGrid, 1);
				Grid.SetRow(_propertyGrid, 1);

				// Mark
				var markImage = new Image();
				var imageStyle = parent.PropertyGridStyle.MarkStyle.ImageStyle;
				if (imageStyle != null)
				{
					markImage.ApplyPressableImageStyle(imageStyle);
				}

				_mark = new ToggleButton(null)
				{
					HorizontalAlignment = HorizontalAlignment.Left,
					VerticalAlignment = VerticalAlignment.Center,
					Content = markImage
				};

				Children.Add(_mark);

				_mark.PressedChanged += (sender, args) =>
				{
					if (_mark.IsPressed)
					{
						Children.Add(_propertyGrid);
						parent._expandedCategories.Add(category);
					}
					else
					{
						Children.Remove(_propertyGrid);
						parent._expandedCategories.Remove(category);
					}
				};

				var expanded = true;
				if (parentProperty != null && parentProperty.FindAttribute<DesignerFoldedAttribute>() != null)
				{
					expanded = false;
				}

				if (expanded)
				{
					_mark.IsPressed = true;
				}

				var label = new Label(null)
				{
					Text = header,
				};
				Grid.SetColumn(label, 1);
				label.ApplyLabelStyle(parent.PropertyGridStyle.LabelStyle);

				Children.Add(label);

				HorizontalAlignment = HorizontalAlignment.Stretch;
				VerticalAlignment = VerticalAlignment.Stretch;
			}

			public override void OnTouchDoubleClick()
			{
				base.OnTouchDoubleClick();

				var mousePosition = ToLocal(Desktop.MousePosition);
				if (!HeaderBounds.Contains(mousePosition) || _mark.Bounds.Contains(mousePosition))
				{
					return;
				}

				_mark.IsPressed = !_mark.IsPressed;
			}

			public override void InternalRender(RenderContext context)
			{
				if (_propertyGrid.PropertyGridStyle.SelectionHoverBackground != null && IsMouseInside)
				{
					var headerBounds = HeaderBounds;
					if (headerBounds.Contains(ToLocal(Desktop.MousePosition)))
					{
						_propertyGrid.PropertyGridStyle.SelectionHoverBackground.Draw(context, headerBounds);
					}
				}

				base.InternalRender(context);
			}
		}
    }
}