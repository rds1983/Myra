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
	public class PropertyGrid : GridBased
	{
		private const string DefaultCategoryName = "Miscellaneous";

		private abstract class Record
		{
			public bool HasSetter { get; set; }
			public IItemsProvider ItemsProvider { get; set; }

			public abstract string Name { get; }
			public abstract Type Type { get; }
			public string Category { get; set; }

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
		private readonly Dictionary<string, List<Record>> _records = new Dictionary<string, List<Record>>();
		private readonly HashSet<string> _expandedCategories = new HashSet<string>();

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

		public string Category { get; private set; }

		public Func<Color?, Color?> ColorChangeHandler { get; set; }

		public event EventHandler<GenericEventArgs<string>>  PropertyChanged;

		public PropertyGrid(TreeStyle style, string category)
		{
			ColumnSpacing = 8;
			RowSpacing = 8;
			ColumnsProportions.Add(new Proportion(ProportionType.Pixels, 100));
			ColumnsProportions.Add(new Proportion(ProportionType.Fill));

			Category = category;

			if (style != null)
			{
				ApplyPropertyGridStyle(style);
			}
		}

		public PropertyGrid(string category) : this(DefaultAssets.UIStylesheet.TreeStyle, category)
		{
		}

		public PropertyGrid() : this(DefaultCategoryName)
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

		private Grid CreateSubGrid(object value, string header, string category, Record parentProperty)
		{
			var subGrid = new Grid
			{
				ColumnSpacing = 2,
				RowSpacing = 2
			};

			subGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
			subGrid.ColumnsProportions.Add(new Proportion(ProportionType.Fill));
			subGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));
			subGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));

			var propertyGrid = new PropertyGrid(PropertyGridStyle, category)
			{
				Object = value,
				Visible = false,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				GridPositionX = 1,
				GridPositionY = 1,
				ColorChangeHandler = ColorChangeHandler,
				_parentGrid = this,
				_parentProperty = parentProperty
			};

			// Mark
			var mark = new ImageButton(PropertyGridStyle.MarkStyle)
			{
				Toggleable = true,
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Center
			};

			subGrid.Widgets.Add(mark);

			mark.Down += (sender, args) =>
			{
				propertyGrid.Visible = true;
			};

			mark.Up += (sender, args) =>
			{
				propertyGrid.Visible = false;
			};

			var label = new TextBlock(PropertyGridStyle.LabelStyle)
			{
				Text = header,
				GridPositionX = 1
			};

			label.DoubleClick += (sender, args) =>
			{
				mark.IsPressed = !mark.IsPressed;
			};

			subGrid.Widgets.Add(label);

			subGrid.Widgets.Add(propertyGrid);

			return subGrid;
		}

		private void FillSubGrid(ref int y, IReadOnlyList<Record> records)
		{
			for (var i = 0; i < records.Count; ++i)
			{
				var record = records[i];

				var value = record.GetValue(_object);
				Widget valueWidget = null;

				var oldY = y;

				var propertyType = record.Type;

				Proportion rowProportion;
				if (record.ItemsProvider != null)
				{
					var values = record.ItemsProvider.Items;

					var cb = new ComboBox();
					foreach (var v in values)
					{
						cb.Items.Add(new ListItem(v.ToString(), null, v));
					}


					cb.SelectedIndex = Array.IndexOf(values, value);
					if (record.HasSetter)
					{
						cb.SelectedIndexChanged += (sender, args) =>
						{
							var item = cb.SelectedIndex >= 0 ? values[cb.SelectedIndex] : null;
							record.SetValue(_object, item);
							FireChanged(propertyType.Name);
						};
					}
					else
					{
						cb.Enabled = false;
					}

					valueWidget = cb;
				}
				else if (propertyType == typeof(bool))
				{
					var isChecked = (bool)value;
					var cb = new CheckBox
					{
						IsPressed = isChecked
					};

					if (record.HasSetter)
					{

						cb.Down += (sender, args) =>
						{
							record.SetValue(_object, true);
							FireChanged(propertyType.Name);
						};

						cb.Up += (sender, args) =>
						{
							record.SetValue(_object, false);
							FireChanged(propertyType.Name);
						};
					}
					else
					{
						cb.Enabled = false;
					}

					valueWidget = cb;

				}
				else if (propertyType == typeof(Color) || propertyType == typeof(Color?))
				{
					var subGrid = new Grid
					{
						ColumnSpacing = 8,
						HorizontalAlignment = HorizontalAlignment.Stretch
					};

					var isColor = propertyType == typeof(Color);

					subGrid.ColumnsProportions.Add(new Proportion());
					subGrid.ColumnsProportions.Add(new Proportion(ProportionType.Fill));

					var color = Color.Transparent;
					if (isColor)
					{
						color = (Color)value;
					}
					else if (value != null)
					{
						color = ((Color?)value).Value;
					}

					var sprite = Sprite.CreateSolidColorRect(color);

					var image = new Image
					{
						Drawable = sprite,
						VerticalAlignment = VerticalAlignment.Center,
						WidthHint = 32,
						HeightHint = 16
					};

					subGrid.Widgets.Add(image);

					var button = new Button
					{
						Text = "Change...",
						ContentHorizontalAlignment = HorizontalAlignment.Center,
						Tag = value,
						HorizontalAlignment = HorizontalAlignment.Stretch,
						GridPositionX = 1
					};

					subGrid.Widgets.Add(button);

					if (record.HasSetter)
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
									record.SetValue(_object, newColor.Value);
								}
								else
								{
									record.SetValue(_object, newColor);
								}

								FireChanged(propertyType.Name);
							}
						};
					}
					else
					{
						button.Enabled = false;
					}

					valueWidget = subGrid;
				}
				else if (propertyType.IsAssignableFrom(typeof(Drawable)))
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

					if (record.HasSetter)
					{
						cb.SelectedIndexChanged += (sender, args) =>
						{
							if (cb.SelectedIndex != -1)
							{
								record.SetValue(_object, cb.SelectedIndex);
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
				else if (propertyType.IsNumericType() || 
					(propertyType.IsNullablePrimitive() && propertyType.GetNullableType().IsNumericType()))
				{
					var numericType = propertyType;
					if (propertyType.IsNullablePrimitive())
					{
						numericType = propertyType.GetNullableType();
					}

					var spinButton = new SpinButton
					{
						Integer = numericType.IsNumericInteger(),
						Nullable = propertyType.IsNullablePrimitive(),
						Value = value != null ? (float)Convert.ChangeType(value, typeof (float)) : default(float?)
					};

					if (record.HasSetter)
					{
						spinButton.ValueChanged += (sender, args) =>
						{
							try
							{
								object result;

								if (spinButton.Value != null)
								{
									result = Convert.ChangeType(spinButton.Value.Value, numericType);
								}
								else
								{
									result = null;
								}

								record.SetValue(_object, result);

								if (record.Type.IsValueType)
								{
									var tg = this;
									var pg = tg._parentGrid;
									while (pg != null && tg._parentProperty != null)
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

								FireChanged(record.Name);
							}
							catch (Exception)
							{
								// TODO: Rework this ugly type conversion solution
							}
						};
					}
					else
					{
						spinButton.Enabled = false;
					}

					valueWidget = spinButton;
				}
				else if (propertyType == typeof(string) || propertyType.IsPrimitive || propertyType.IsNullablePrimitive())
				{
					var tf = new TextField
					{
						Text = value != null ? value.ToString() : string.Empty
					};

					if (record.HasSetter)
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
										result = Convert.ChangeType(tf.Text, record.Type.GetNullableType());
									}
								}
								else
								{
									result = Convert.ChangeType(tf.Text, record.Type);
								}

								record.SetValue(_object, result);

								if (record.Type.IsValueType)
								{
									var tg = this;
									var pg = tg._parentGrid;
									while (pg != null && tg._parentProperty != null)
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

								FireChanged(record.Name);
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
				else if (typeof(IList).IsAssignableFrom(propertyType))
				{
					var it = propertyType.FindGenericType(typeof(ICollection<>));
					if (it != null)
					{
						var itemType = it.GenericTypeArguments[0];
						if (value != null)
						{
							var items = (IList)value;

							var subGrid = new Grid
							{
								ColumnSpacing = 8,
								HorizontalAlignment = HorizontalAlignment.Stretch
							};

							subGrid.ColumnsProportions.Add(new Proportion());
							subGrid.ColumnsProportions.Add(new Proportion(ProportionType.Fill));

							var label = new TextBlock
							{
								VerticalAlignment = VerticalAlignment.Center,
							};
							UpdateLabelCount(label, items.Count);

							subGrid.Widgets.Add(label);

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

							subGrid.Widgets.Add(button);
							valueWidget = subGrid;
						}
					}
				}
				else if (!(value is BitmapFont) && !(value is Drawable))
				{
					// Subgrid
					if (value != null)
					{
						var subGrid = CreateSubGrid(value, record.Name, DefaultCategoryName, record);

						subGrid.GridSpanX = 2;
						subGrid.GridPositionY = y;

						Widgets.Add(subGrid);

						rowProportion = new Proportion(ProportionType.Auto);
						RowsProportions.Add(rowProportion);
						++y;

						continue;
					}

					var tb = new TextBlock();
					tb.ApplyTextBlockStyle(PropertyGridStyle.LabelStyle);
					tb.Text = "null";

					valueWidget = tb;
				}

				if (valueWidget == null)
				{
					continue;
				}

				var nameLabel = new TextBlock
				{
					Text = record.Name,
					VerticalAlignment = VerticalAlignment.Center,
					GridPositionX = 0,
					GridPositionY = oldY
				};

				Widgets.Add(nameLabel);

				valueWidget.GridPositionX = 1;
				valueWidget.GridPositionY = oldY;
				valueWidget.HorizontalAlignment = HorizontalAlignment.Stretch;
				valueWidget.VerticalAlignment = VerticalAlignment.Top;

				Widgets.Add(valueWidget);

				rowProportion = new Proportion(ProportionType.Auto);
				RowsProportions.Add(rowProportion);
				++y;
			}
		}

		private void Rebuild()
		{
			RowsProportions.Clear();
			Widgets.Clear();
			_records.Clear();

			if (_object == null)
			{
				return;
			}

			var properties = from p in _object.GetType().GetProperties() select p;
			var records = new List<Record>();
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

				var categoryAttr = property.FindAttribute<EditCategoryAttribute>();
				record.Category = categoryAttr != null ? categoryAttr.Name : DefaultCategoryName;

				records.Add(record);
			}

			var fields = from f in _object.GetType().GetFields() select f;
			foreach (var field in fields)
			{
				if (!field.IsPublic || field.IsStatic)
				{
					continue;
				}

				var categoryAttr = field.FindAttribute<EditCategoryAttribute>();

				var record = new FieldRecord(field)
				{
					HasSetter = true,
					Category = categoryAttr != null ? categoryAttr.Name : DefaultCategoryName
				};

				records.Add(record);
			}

			// Sort by categories
			for (var i = 0; i < records.Count; ++i)
			{
				var record = records[i];

				List<Record> categoryRecords;
				if (!_records.TryGetValue(record.Category, out categoryRecords))
				{
					categoryRecords = new List<Record>();
					_records[record.Category] = categoryRecords;
				}

				categoryRecords.Add(record);
			}

			// Sort by names within categories
			foreach (var category in _records)
			{
				category.Value.Sort((a, b) => Comparer<string>.Default.Compare(a.Name, b.Name));
			}

			var ordered = _records.OrderBy(key => key.Key);

			var y = 0;
			List<Record> defaultCategoryRecords;


			if (_records.TryGetValue(Category, out defaultCategoryRecords))
			{
				FillSubGrid(ref y, defaultCategoryRecords);
			}

			if (Category != DefaultCategoryName)
			{
				return;
			}

			foreach (var category in ordered)
			{
				if (category.Key == DefaultCategoryName)
				{
					continue;
				}

				var subGrid = CreateSubGrid(Object, category.Key, category.Key, null);

				subGrid.GridSpanX = 2;
				subGrid.GridPositionY = y;

				Widgets.Add(subGrid);

				if (_expandedCategories.Contains(category.Key))
				{
//					mark.IsPressed = true;
				}

				var rp = new Proportion(ProportionType.Auto);
				RowsProportions.Add(rp);

				y++;
			}
	}

		public void ApplyPropertyGridStyle(TreeStyle style)
		{
			ApplyWidgetStyle(style);

			PropertyGridStyle = style;
		}
	}
}