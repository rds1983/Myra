using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Channels;
using Microsoft.Xna.Framework;
using Myra.Edit;
using Myra.Editor.Utils;
using Myra.Graphics2D;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.UI;

namespace Myra.Editor.UI
{
	public class PropertyGrid : ScrollPane<Grid>
	{
		private object _object;

		public object Object
		{
			get { return _object; }

			set
			{
				if (value == _object)
				{
					return;
				}

				_object = value;
				Rebuild();
			}
		}

		public Func<Color?, Color?> ColorChangeHandler { get; set; }

		public event EventHandler PropertyChanged;

		public PropertyGrid()
		{
			Widget = new Grid
			{
				ColumnSpacing = 5,
				RowSpacing = 5,
			};

			Widget.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
			Widget.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Fill));
		}

		private void FireChanged()
		{
			var ev = PropertyChanged;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}

		private void Rebuild()
		{
			Widget.RowsProportions.Clear();
			Widget.Children.Clear();

			if (_object == null)
			{
				return;
			}

			var properties = (from p in _object.GetType().GetProperties() orderby p.Name select p).ToArray();

			var y = 0;
			for (var i = 0; i < properties.Length; ++i)
			{
				var property = properties[i];

				if (property.GetGetMethod() == null ||
				    !property.GetGetMethod().IsPublic ||
				    property.GetSetMethod() == null ||
				    !property.GetSetMethod().IsPublic)
				{
					continue;
				}

				var browsableAttr = property.FindAttribute<BrowsableAttribute>();
				if (browsableAttr != null && !browsableAttr.Browsable)
				{
					continue;
				}

				var hiddenAttr = property.FindAttribute<HiddenInEditorAttribute>();
				if (hiddenAttr != null)
				{
					continue;
				}

				var value = property.GetValue(_object, new object[0]);
				Widget valueWidget = null;

				var propertyType = property.PropertyType;
				if (propertyType == typeof (bool))
				{
					var isChecked = (bool) value;
					var cb = new CheckBox
					{
						IsPressed = isChecked
					};

					cb.Down += (sender, args) =>
					{
						property.SetValue(_object, true);
						FireChanged();
					};

					cb.Up += (sender, args) =>
					{
						property.SetValue(_object, false);
						FireChanged();
					};

					valueWidget = cb;

				}
				else if (propertyType == typeof (Color))
				{
					var grid = new Grid
					{
						ColumnSpacing = 8,
						HorizontalAlignment = HorizontalAlignment.Stretch
					};

					grid.ColumnsProportions.Add(new Grid.Proportion());
					grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Fill));

					var sprite = Sprite.CreateSolidColorRect((Color) value);

					var image = new Image
					{
						Drawable = sprite,
						WidthHint = 32,
						HeightHint = 16,
						VerticalAlignment = VerticalAlignment.Center
					};

					grid.Children.Add(image);

					var button = new Button
					{
						Text = "Change...",
						ContentHorizontalAlignment = HorizontalAlignment.Center,
						GridPosition = {X = 1},
						HorizontalAlignment = HorizontalAlignment.Stretch,
						Tag = value
					};

					button.Down += (sender, args) =>
					{
						var h = ColorChangeHandler;
						if (h != null)
						{
							var newColor = h(sprite.Color);

							if (newColor.HasValue)
							{
								sprite.Color = newColor.Value;
								property.SetValue(_object, newColor.Value);
								FireChanged();
							}
						}
					};

					grid.Children.Add(button);

					valueWidget = grid;
				}
				else if (propertyType.IsAssignableFrom(typeof (Drawable)))
				{
				}
				else if (propertyType == typeof (Rectangle))
				{
					// Rectangle
				}
				else if (propertyType == typeof (Point))
				{
					// Point
				}
				else if (propertyType == typeof (FrameInfo))
				{
					// Frame Info
				}
				else if (propertyType.IsEnum)
				{
					var values = Enum.GetValues(propertyType);

					var cb = new ComboBox();
					foreach (var v in values)
					{
						var item = cb.AddItem(v.ToString());
						item.Tag = v;
					}

					cb.SelectedIndex = Array.IndexOf(values, value);
					cb.SelectedIndexChanged += (sender, args) =>
					{
						if (cb.SelectedIndex != -1)
						{
							property.SetValue(_object, cb.SelectedIndex);
							FireChanged();
						}
					};

					valueWidget = cb;
				}
				else
				{
					var tf = new TextField
					{
						Text = value != null ? value.ToString() : string.Empty
					};

					tf.TextChanged += (sender, args) =>
					{
						property.SetValue(_object, tf.Text);
						FireChanged();
					};

					valueWidget = tf;
				}

				if (valueWidget == null)
				{
					continue;
				}

				var nameLabel = new TextBlock
				{
					Text = property.Name,
					GridPosition =
					{
						Y = y
					}
				};
				Widget.Children.Add(nameLabel);

				valueWidget.GridPosition = new Point(1, y);
				valueWidget.HorizontalAlignment = HorizontalAlignment.Stretch;
				valueWidget.VerticalAlignment = VerticalAlignment.Center;

				Widget.Children.Add(valueWidget);

				var rowProportion = new Grid.Proportion(Grid.ProportionType.Auto);
				Widget.RowsProportions.Add(rowProportion);

				++y;
			}
		}
	}
}