using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Myra.Edit;
using Myra.Editor.Utils;
using Myra.Graphics2D;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;

namespace Myra.Editor.UI
{
	public class PropertyGrid : ScrollPane<Grid>
	{
		private abstract class Record
		{
			public bool HasSetter { get; set; }
			public IItemsProvider ItemsProvider { get; set; }

			public abstract string Name { get; }
			public abstract Type Type { get; }

			public abstract object GetValue(object obj);
			public abstract void SetValue(object obj, object value);
		}

		private class PropertyRecord : Record
		{
			private readonly PropertyInfo _propertyInfo;

			public override string Name
			{
				get { return _propertyInfo.Name; }
			}

			public override Type Type
			{
				get { return _propertyInfo.PropertyType; }
			}

			public PropertyRecord(PropertyInfo propertyInfo)
			{
				_propertyInfo = propertyInfo;
			}

			public override object GetValue(object obj)
			{
				return _propertyInfo.GetValue(obj, new object[0]);
			}

			public override void SetValue(object obj, object value)
			{
				_propertyInfo.SetValue(obj, value);
			}
		}

		private class FieldRecord : Record
		{
			private readonly FieldInfo _fieldInfo;

			public override string Name
			{
				get { return _fieldInfo.Name; }
			}

			public override Type Type
			{
				get { return _fieldInfo.FieldType; }
			}

			public FieldRecord(FieldInfo fieldInfo)
			{
				_fieldInfo = fieldInfo;
			}

			public override object GetValue(object obj)
			{
				return _fieldInfo.GetValue(obj);
			}

			public override void SetValue(object obj, object value)
			{
				_fieldInfo.SetValue(obj, value);
			}
		}

		private PropertyGrid _parentGrid;
		private Record _parentProperty;
		private readonly List<Record> _records = new List<Record>();

		public TreeStyle PropertyGridStyle { get; private set; }

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

		public event EventHandler<GenericEventArgs<string>>  PropertyChanged;

		public PropertyGrid(TreeStyle style)
		{
			Widget = new Grid
			{
				ColumnSpacing = 4,
				RowSpacing = 4,
			};

			Widget.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
			Widget.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
			Widget.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Fill));

			if (style != null)
			{
				ApplyPropertyGridStyle(style);
			}
		}

		public PropertyGrid() : this(DefaultAssets.UIStylesheet.TreeStyle)
		{
		}

		private void FireChanged(string name)
		{
			var ev = PropertyChanged;
			if (ev != null)
			{
				ev(this, new GenericEventArgs<string>(name));
			}
		}

		private static void UpdateLabelCount(TextBlock textBlock, int count)
		{
			textBlock.Text = string.Format("{0} Items", count);
		}

		private void Rebuild()
		{
			Widget.RowsProportions.Clear();
			Widget.Widgets.Clear();
			_records.Clear();

			if (_object == null)
			{
				return;
			}

			var properties = from p in _object.GetType().GetProperties() select p;
			foreach (var property in properties)
			{
				if (property.GetGetMethod() == null ||
				    !property.GetGetMethod().IsPublic ||
				    property.GetGetMethod().IsStatic)
				{
					continue;
				}

				var hasSetter = property.GetSetMethod() != null &&
				                property.GetSetMethod().IsPublic;

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

				var record = new PropertyRecord(property)
				{
					HasSetter = hasSetter
				};

				var selectionAttr = property.FindAttribute<SelectionAttribute>();
				if (selectionAttr != null)
				{
					record.ItemsProvider = (IItemsProvider) Activator.CreateInstance(selectionAttr.ItemsProviderType);
				}

				_records.Add(record);
			}

			var fields = from f in _object.GetType().GetFields() select f;
			foreach (var field in fields)
			{
				if (!field.IsPublic || field.IsStatic)
				{
					continue;
				}

				_records.Add(new FieldRecord(field)
				{
					HasSetter = true
				});
			}

			_records.Sort((a, b) => Comparer<string>.Default.Compare(a.Name, b.Name));

			var y = 0;
			for (var i = 0; i < _records.Count; ++i)
			{
				var property = _records[i];

				var value = property.GetValue(_object);
				Widget valueWidget = null;

				var oldY = y;

				var propertyType = property.Type;

				if (property.ItemsProvider != null)
				{
					var values = property.ItemsProvider.Items;

					var cb = new ComboBox();
					foreach (var v in values)
					{
						cb.Items.Add(new ListItem(v.ToString(), null, v));
					}

					cb.SelectedIndex = Array.IndexOf(values, value);
					if (property.HasSetter)
					{
						cb.SelectedIndexChanged += (sender, args) =>
						{
							var item = cb.SelectedIndex >= 0 ? values[cb.SelectedIndex] : null;
							property.SetValue(_object, item);
							FireChanged(propertyType.Name);
						};
					}
					else
					{
						cb.Enabled = false;
					}

					valueWidget = cb;
				}
				else if (propertyType == typeof (bool))
				{
					var isChecked = (bool) value;
					var cb = new CheckBox
					{
						IsPressed = isChecked
					};

					if (property.HasSetter)
					{

						cb.Down += (sender, args) =>
						{
							property.SetValue(_object, true);
							FireChanged(propertyType.Name);
						};

						cb.Up += (sender, args) =>
						{
							property.SetValue(_object, false);
							FireChanged(propertyType.Name);
						};
					}
					else
					{
						cb.Enabled = false;
					}

					valueWidget = cb;

				}
				else if (propertyType == typeof (Color) || propertyType == typeof(Color?))
				{
					var grid = new Grid
					{
						ColumnSpacing = 8,
						HorizontalAlignment = HorizontalAlignment.Stretch
					};

					var isColor = propertyType == typeof (Color);

					grid.ColumnsProportions.Add(new Grid.Proportion());
					grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Fill));

					var color = Color.Transparent;
					if (isColor)
					{
						color = (Color) value;
					}
					else if (value != null)
					{
						color = ((Color?) value).Value;
					}

					var sprite = Sprite.CreateSolidColorRect(color);

					var image = new Image
					{
						Drawable = sprite,
						VerticalAlignment = VerticalAlignment.Center,
						WidthHint = 32,
						HeightHint = 16
					};

					grid.Widgets.Add(image);

					var button = new Button
					{
						Text = "Change...",
						ContentHorizontalAlignment = HorizontalAlignment.Center,
						Tag = value,
						HorizontalAlignment = HorizontalAlignment.Stretch,
						GridPositionX = 1
					};

					grid.Widgets.Add(button);

					if (property.HasSetter)
					{
						button.Up += (sender, args) =>
						{
							var h = ColorChangeHandler;
							if (h != null)
							{
								var newColor = h(sprite.Color);
								if (!newColor.HasValue) return;

								sprite.Color = newColor.Value;

								if (isColor)
								{
									property.SetValue(_object, newColor.Value);
								}
								else
								{
									property.SetValue(_object, newColor);
								}

								FireChanged(propertyType.Name);
							}
						};
					}
					else
					{
						button.Enabled = false;
					}

					valueWidget = grid;
				}
				else if (propertyType.IsAssignableFrom(typeof (Drawable)))
				{
				}
				else if (propertyType.IsEnum)
				{
					var values = Enum.GetValues(propertyType);

					var cb = new ComboBox();
					foreach (var v in values)
					{
						cb.Items.Add(new ListItem(v.ToString(), null, v));
					}

					cb.SelectedIndex = Array.IndexOf(values, value);

					if (property.HasSetter)
					{
						cb.SelectedIndexChanged += (sender, args) =>
						{
							if (cb.SelectedIndex != -1)
							{
								property.SetValue(_object, cb.SelectedIndex);
								FireChanged(propertyType.Name);
							}
						};
					}
					else
					{
						cb.Enabled = false;
					}

					valueWidget = cb;
				}
				else if (propertyType == typeof (string) || propertyType.IsPrimitive || propertyType.IsNullablePrimitive())
				{

					var tf = new TextField
					{
						Text = value != null ? value.ToString() : string.Empty
					};

					if (property.HasSetter)
					{
						tf.TextChanged += (sender, args) =>
						{
							try
							{
								object result;

								if (propertyType.IsNullablePrimitive())
								{
									if (string.IsNullOrEmpty(tf.Text))
									{
										result = null;
									}
									else
									{
										result = Convert.ChangeType(tf.Text, property.Type.GetNullableType());
									}
								}
								else
								{
									result = Convert.ChangeType(tf.Text, property.Type);
								}

								property.SetValue(_object, result);

								if (property.Type.IsValueType)
								{
									var tg = this;
									var pg = tg._parentGrid;
									while (pg != null)
									{
										tg._parentProperty.SetValue(pg._object, tg._object);

										if (!tg._parentProperty.Type.IsValueType)
										{
											break;
										}

										tg = pg;
										pg = tg._parentGrid;
									}
								}

								FireChanged(property.Name);
							}
							catch (Exception)
							{
								// TODO: Rework this ugly type conversion solution
							}
						};
					}
					else
					{
						tf.Enabled = false;
					}

					valueWidget = tf;
				}
				else if (typeof (IList).IsAssignableFrom(propertyType))
				{
					var it = propertyType.FindGenericType(typeof (ICollection<>));
					if (it != null)
					{
						var itemType = it.GenericTypeArguments[0];
						if (value != null)
						{
							var items = (IList) value;

							var grid = new Grid
							{
								ColumnSpacing = 8,
								HorizontalAlignment = HorizontalAlignment.Stretch
							};

							grid.ColumnsProportions.Add(new Grid.Proportion());
							grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Fill));

							var label = new TextBlock
							{
								VerticalAlignment = VerticalAlignment.Center,
							};
							UpdateLabelCount(label, items.Count);

							grid.Widgets.Add(label);

							var button = new Button
							{
								Text = "Change...",
								ContentHorizontalAlignment = HorizontalAlignment.Center,
								Tag = value,
								HorizontalAlignment = HorizontalAlignment.Stretch,
								GridPositionX = 1
							};

							button.Up += (sender, args) =>
							{
								var collectionEditor = new CollectionEditor(items, itemType)
								{
									ColorChangeHandler = ColorChangeHandler
								};

								var dialog = Dialog.CreateMessageBox("Edit", collectionEditor);

								dialog.ButtonOk.Up += (o, eventArgs) =>
								{
									collectionEditor.SaveChanges();
									UpdateLabelCount(label, items.Count);
								};

								dialog.ShowModal(Desktop);
							};

							grid.Widgets.Add(button);
							valueWidget = grid;
						}
					}
				}
				else if (!(value is BitmapFont) && !(value is Drawable))
				{
					// Subgrid
					if (value != null)
					{
						var subGrid = new PropertyGrid(PropertyGridStyle)
						{
							Object = value,
							Visible = false,
							HorizontalAlignment = HorizontalAlignment.Stretch,
							GridPositionX = 1,
							GridPositionY =  y + 1,
							GridSpanX = 2,
							ColorChangeHandler = ColorChangeHandler,
							_parentGrid = this,
							_parentProperty = property
						};

						Widget.Widgets.Add(subGrid);

						// Mark
						var mark = new Button(null)
						{
							Toggleable = true,
							HorizontalAlignment = HorizontalAlignment.Center,
							VerticalAlignment = VerticalAlignment.Center,
							GridPositionY = y
						};

						mark.ApplyButtonStyle(PropertyGridStyle.MarkStyle);
						Widget.Widgets.Add(mark);

						mark.Down += (sender, args) =>
						{
							subGrid.Visible = true;
						};

						mark.Up += (sender, args) =>
						{
							subGrid.Visible = false;
						};

						var rp = new Grid.Proportion(Grid.ProportionType.Auto);
						Widget.RowsProportions.Add(rp);

						++y;
					}

					var tb = new TextBlock();
					tb.ApplyTextBlockStyle(PropertyGridStyle.LabelStyle);
					tb.Text = value != null ? value.ToString() : "null";

					valueWidget = tb;
				}

				if (valueWidget == null)
				{
					continue;
				}

				var nameLabel = new TextBlock
				{
					Text = property.Name,
					VerticalAlignment = VerticalAlignment.Center,
					GridPositionX = 1,
					GridPositionY =  oldY
				};

				Widget.Widgets.Add(nameLabel);

				valueWidget.GridPositionX = 2;
				valueWidget.GridPositionY = oldY;
				valueWidget.HorizontalAlignment = HorizontalAlignment.Stretch;
				valueWidget.VerticalAlignment = VerticalAlignment.Center;

				Widget.Widgets.Add(valueWidget);

				var rowProportion = new Grid.Proportion(Grid.ProportionType.Auto);
				Widget.RowsProportions.Add(rowProportion);

				++y;
			}
		}

		public void ApplyPropertyGridStyle(TreeStyle style)
		{
			ApplyWidgetStyle(style);

			PropertyGridStyle = style;
		}
	}
}