using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Myra.Graphics2D.UI.ColorPicker;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using static Myra.Graphics2D.UI.Grid;
using System.Xml.Serialization;
using Myra.Attributes;

#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Xenko.Core.Mathematics;
using Xenko.Graphics;
#endif

namespace Myra.Graphics2D.UI.Properties
{
	public class PropertyGrid : SingleItemContainer<Grid>
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

		private class SubGrid : SingleItemContainer<Grid>
		{
			private readonly ImageButton _mark;
			private readonly PropertyGrid _propertyGrid;

			public ImageButton Mark
			{
				get { return _mark; }
			}

			public PropertyGrid PropertyGrid
			{
				get { return _propertyGrid; }
			}

			public Rectangle HeaderBounds
			{
				get
				{
					var headerBounds = new Rectangle(ActualBounds.X, ActualBounds.Y, ActualBounds.Width, InternalChild.GetRowHeight(0));

					return headerBounds;
				}
			}

			public bool IsEmpty
			{
				get
				{
					return _propertyGrid.IsEmpty;
				}
			}

			public SubGrid(PropertyGrid parent, object value, string header, string category, Record parentProperty)
			{
				InternalChild = new Grid();

				InternalChild.ColumnSpacing = 4;
				InternalChild.RowSpacing = 4;

				InternalChild.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
				InternalChild.ColumnsProportions.Add(new Proportion(ProportionType.Fill));
				InternalChild.RowsProportions.Add(new Proportion(ProportionType.Auto));
				InternalChild.RowsProportions.Add(new Proportion(ProportionType.Auto));

				_propertyGrid = new PropertyGrid(parent.PropertyGridStyle, category, parentProperty, parent)
				{
					Object = value,
					Visible = false,
					HorizontalAlignment = HorizontalAlignment.Stretch,
					GridColumn = 1,
					GridRow = 1
				};

				// Mark
				_mark = new ImageButton(parent.PropertyGridStyle.MarkStyle)
				{
					Toggleable = true,
					HorizontalAlignment = HorizontalAlignment.Left,
					VerticalAlignment = VerticalAlignment.Center
				};

				InternalChild.Widgets.Add(_mark);

				_mark.PressedChanged += (sender, args) =>
				{
					if (_mark.IsPressed)
					{
						_propertyGrid.Visible = true;
						parent._expandedCategories.Add(category);
					}
					else
					{
						_propertyGrid.Visible = false;
						parent._expandedCategories.Remove(category);
					}
				};

				_mark.IsPressed = true;

				var label = new TextBlock(parent.PropertyGridStyle.LabelStyle)
				{
					Text = header,
					GridColumn = 1
				};

				InternalChild.Widgets.Add(label);
				InternalChild.Widgets.Add(_propertyGrid);

				HorizontalAlignment = HorizontalAlignment.Stretch;
				VerticalAlignment = VerticalAlignment.Stretch;
			}

			public override void OnTouchDoubleClick()
			{
				base.OnTouchDoubleClick();

				var mousePosition = Desktop.MousePosition;
				if (!HeaderBounds.Contains(mousePosition) || _mark.Bounds.Contains(mousePosition))
				{
					return;
				}

				_mark.IsPressed = !_mark.IsPressed;
			}

			public override void InternalRender(RenderContext context)
			{
				if (_propertyGrid.PropertyGridStyle.SelectionHoverBackground != null && UseHoverRenderable)
				{
					var headerBounds = HeaderBounds;
					if (headerBounds.Contains(Desktop.MousePosition))
					{
						context.Draw(_propertyGrid.PropertyGridStyle.SelectionHoverBackground, headerBounds);
					}
				}

				base.InternalRender(context);
			}
		}

		private readonly PropertyGrid _parentGrid;
		private Record _parentProperty;
		private readonly Dictionary<string, List<Record>> _records = new Dictionary<string, List<Record>>();
		private readonly HashSet<string> _expandedCategories = new HashSet<string>();
		private object _object;
		private bool _ignoreCollections;

		[Browsable(false)]
		[XmlIgnore]
		public TreeStyle PropertyGridStyle
		{
			get; private set;
		}

		[Browsable(false)]
		[XmlIgnore]
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

		[Browsable(false)]
		[XmlIgnore]
		public string Category { get; private set; }
		
		public bool IgnoreCollections
		{
			get
			{
				if (_parentGrid != null)
				{
					return _parentGrid.IgnoreCollections;
				}

				return _ignoreCollections;
			}

			set
			{
				_ignoreCollections = value;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return InternalChild.Widgets.Count == 0;
			}
		}

		public event EventHandler<GenericEventArgs<string>> PropertyChanged;

		private PropertyGrid(TreeStyle style, string category, Record parentProperty, PropertyGrid parentGrid = null)
		{
			_parentGrid = parentGrid;
			InternalChild = new Grid();

			_parentProperty = parentProperty;
			InternalChild.ColumnSpacing = 8;
			InternalChild.RowSpacing = 8;
			InternalChild.ColumnsProportions.Add(new Proportion(ProportionType.Pixels, 150));
			InternalChild.ColumnsProportions.Add(new Proportion(ProportionType.Fill));

			Category = category;

			if (style != null)
			{
				ApplyPropertyGridStyle(style);
			}

			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;
		}

		public PropertyGrid(TreeStyle style, string category) : this(style, category, null)
		{
		}

		public PropertyGrid(string category) : this(Stylesheet.Current.TreeStyle, category)
		{
		}

		public PropertyGrid() : this(DefaultCategoryName)
		{
		}

		private void FireChanged(string name)
		{
			var ev = PropertyChanged;

			var p = _parentGrid;
			while (p != null)
			{
				ev = p.PropertyChanged;
				p = p._parentGrid;
			}

			if (ev != null)
			{
				ev(this, new GenericEventArgs<string>(name));
			}
		}

		private static void UpdateLabelCount(TextBlock textBlock, int count)
		{
			textBlock.Text = string.Format("{0} Items", count);
		}

		private void FillSubGrid(ref int y, IReadOnlyList<Record> records)
		{
			for (var i = 0; i < records.Count; ++i)
			{
				var record = records[i];

				var hasSetter = record.HasSetter;
				if (_parentProperty != null && !_parentProperty.HasSetter)
				{
					hasSetter = false;
				}

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
					if (hasSetter)
					{
						cb.SelectedIndexChanged += (sender, args) =>
						{
							var item = cb.SelectedIndex != null ? values[cb.SelectedIndex.Value] : null;
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

					if (hasSetter)
					{
						cb.Click += (sender, args) =>
						{
							record.SetValue(_object, cb.IsPressed);
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

					var image = new Image
					{
						Renderable = DefaultAssets.WhiteRegion,
						VerticalAlignment = VerticalAlignment.Center,
						Width = 32,
						Height = 16,
						Color = color
					};

					subGrid.Widgets.Add(image);

					var button = new ImageTextButton
					{
						Text = "Change...",
						ContentHorizontalAlignment = HorizontalAlignment.Center,
						Tag = value,
						HorizontalAlignment = HorizontalAlignment.Stretch,
						GridColumn = 1
					};

					subGrid.Widgets.Add(button);

					if (hasSetter)
					{
						button.Click += (sender, args) =>
						{
							var dlg = new ColorPickerDialog()
							{
								Color = image.Color
							};

							dlg.Closed += (s, a) =>
							{
								if (!dlg.Result)
								{
									return;
								}

								image.Color = dlg.Color;
								record.SetValue(_object, dlg.Color);

								FireChanged(propertyType.Name);
							};

							dlg.ShowModal(Desktop);
						};
					}
					else
					{
						button.Enabled = false;
					}

					valueWidget = subGrid;
				}
				else if (propertyType.IsAssignableFrom(typeof(IRenderable)))
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

					if (hasSetter)
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
						Value = value != null ? (float)Convert.ChangeType(value, typeof(float)) : default(float?)
					};

					if (hasSetter)
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
							catch (InvalidCastException)
							{
								// TODO: Rework this ugly type conversion solution
							}
							catch (Exception ex)
							{
								spinButton.Value = args.OldValue;
								var dialog = Dialog.CreateMessageBox("Error", ex.ToString());
								dialog.ShowModal(Desktop);
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

					if (hasSetter)
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
					if (!IgnoreCollections)
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

								var button = new ImageTextButton
								{
									Text = "Change...",
									ContentHorizontalAlignment = HorizontalAlignment.Center,
									Tag = value,
									HorizontalAlignment = HorizontalAlignment.Stretch,
									GridColumn = 1
								};

								button.Click += (sender, args) =>
								{
									var collectionEditor = new CollectionEditor(items, itemType);

									var dialog = Dialog.CreateMessageBox("Edit", collectionEditor);

									dialog.ButtonOk.Click += (o, eventArgs) =>
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
				}
				else if (!(value is SpriteFont) && !(value is IRenderable))
				{
					// Subgrid
					if (value != null)
					{
						var subGrid = new SubGrid(this, value, record.Name, DefaultCategoryName, record)
						{
							GridColumnSpan = 2,
							GridRow = y
						};

						InternalChild.Widgets.Add(subGrid);

						rowProportion = new Proportion(ProportionType.Auto);
						InternalChild.RowsProportions.Add(rowProportion);
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
					GridColumn = 0,
					GridRow = oldY
				};

				InternalChild.Widgets.Add(nameLabel);

				valueWidget.GridColumn = 1;
				valueWidget.GridRow = oldY;
				valueWidget.HorizontalAlignment = HorizontalAlignment.Stretch;
				valueWidget.VerticalAlignment = VerticalAlignment.Top;

				InternalChild.Widgets.Add(valueWidget);

				rowProportion = new Proportion(ProportionType.Auto);
				InternalChild.RowsProportions.Add(rowProportion);
				++y;
			}
		}

		private void Rebuild()
		{
			InternalChild.RowsProportions.Clear();
			InternalChild.Widgets.Clear();
			_records.Clear();
			_expandedCategories.Clear();

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

				var readOnlyAttr = property.FindAttribute<ReadOnlyAttribute>();
				if (readOnlyAttr != null && readOnlyAttr.IsReadOnly)
				{
					hasSetter = false;
				}

				var record = new PropertyRecord(property)
				{
					HasSetter = hasSetter
				};

				var selectionAttr = property.FindAttribute<SelectionAttribute>();
				if (selectionAttr != null)
				{
					record.ItemsProvider = (IItemsProvider)Activator.CreateInstance(selectionAttr.ItemsProviderType, _object);
				}

				var categoryAttr = property.FindAttribute<CategoryAttribute>();
				record.Category = categoryAttr != null ? categoryAttr.Category : DefaultCategoryName;

				records.Add(record);
			}

			var fields = from f in _object.GetType().GetFields() select f;
			foreach (var field in fields)
			{
				if (!field.IsPublic || field.IsStatic)
				{
					continue;
				}

				var browsableAttr = field.FindAttribute<BrowsableAttribute>();
				if (browsableAttr != null && !browsableAttr.Browsable)
				{
					continue;
				}

				var categoryAttr = field.FindAttribute<CategoryAttribute>();

				var hasSetter = true;
				var readOnlyAttr = field.FindAttribute<ReadOnlyAttribute>();
				if (readOnlyAttr != null && readOnlyAttr.IsReadOnly)
				{
					hasSetter = false;
				}


				var record = new FieldRecord(field)
				{
					HasSetter = hasSetter,
					Category = categoryAttr != null ? categoryAttr.Category : DefaultCategoryName
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

				var subGrid = new SubGrid(this, Object, category.Key, category.Key, null)
				{
					GridColumnSpan = 2,
					GridRow = y
				};

				if (subGrid.IsEmpty)
				{
					continue;
				}

				InternalChild.Widgets.Add(subGrid);

				if (_expandedCategories.Contains(category.Key))
				{
					subGrid.Mark.IsPressed = true;
				}

				var rp = new Proportion(ProportionType.Auto);
				InternalChild.RowsProportions.Add(rp);

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