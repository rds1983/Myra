using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Myra.Graphics2D.UI.ColorPicker;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using System.Xml.Serialization;
using Myra.MML;
using Myra.Graphics2D.UI.File;
using System.IO;
using Myra.Attributes;
using FontStashSharp;
using FontStashSharp.RichText;
using Myra.Graphics2D.Brushes;
using AssetManagementBase;
using Myra.Events;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#elif STRIDE
using Stride.Core.Mathematics;
using Texture2D = Stride.Graphics.Texture;
#else
using System.Drawing;
using SolidBrush = Myra.Graphics2D.Brushes.SolidBrush;
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI.Properties
{
	public class PropertyGrid : Widget
	{
		private const string DefaultCategoryName = "Miscellaneous";

		private class SubGrid : Widget
		{
			private readonly GridLayout _layout = new GridLayout();

			private readonly ToggleButton _mark;
			private readonly PropertyGrid _propertyGrid;

			public ToggleButton Mark
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
					var headerBounds = new Rectangle(0, 0, ActualBounds.Width, _layout.GetRowHeight(0));

					return headerBounds;
				}
			}

			[Browsable(false)]
			[XmlIgnore]
			public bool IsEmpty
			{
				get
				{
					return _propertyGrid.IsEmpty;
				}
			}

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

		private readonly GridLayout _layout = new GridLayout();
		private readonly PropertyGrid _parentGrid;
		private Record _parentProperty;
		private readonly Dictionary<string, List<Record>> _records = new Dictionary<string, List<Record>>();
		private readonly HashSet<string> _expandedCategories = new HashSet<string>();
		private object _object;
		private bool _ignoreCollections;
		private readonly PropertyGridSettings _settings = new PropertyGridSettings();
		private string _filter;
		private Type _parentType;

		[Browsable(false)]
		[XmlIgnore]
		public TreeStyle PropertyGridStyle { get; private set; }

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

		/// <summary>
		/// Used to determine the attached properties
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public Type ParentType
		{
			get
			{
				if (_parentGrid != null)
				{
					return _parentGrid.ParentType;
				}

				return _parentType;
			}

			set
			{
				_parentType = value;
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public string Category { get; private set; }

		[Category("Behavior")]
		[DefaultValue(false)]
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

		[Browsable(false)]
		[XmlIgnore]
		public bool IsEmpty
		{
			get
			{
				return Children.Count == 0;
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public PropertyGridSettings Settings
		{
			get
			{
				if (_parentGrid != null)
				{
					return _parentGrid.Settings;
				}

				return _settings;
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public int FirstColumnWidth
		{
			get
			{
				return (int)_layout.ColumnsProportions[0].Value;
			}

			set
			{
				_layout.ColumnsProportions[0].Value = value;
			}
		}

		[DefaultValue(HorizontalAlignment.Stretch)]
		public override HorizontalAlignment HorizontalAlignment
		{
			get { return base.HorizontalAlignment; }
			set { base.HorizontalAlignment = value; }
		}

		[DefaultValue(VerticalAlignment.Stretch)]
		public override VerticalAlignment VerticalAlignment
		{
			get { return base.VerticalAlignment; }
			set { base.VerticalAlignment = value; }
		}

		[XmlIgnore]
		[Browsable(false)]
		public string Filter
		{
			get => _filter;
			set
			{
				if (_filter == value)
				{
					return;
				}

				_filter = value;
				Rebuild();
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public Func<Record, object[]> CustomValuesProvider;

		[Browsable(false)]
		[XmlIgnore]
		public Func<Record, object, object, bool> CustomSetter;

		[Browsable(false)]
		[XmlIgnore]
		public Func<Record, object, Widget> CustomWidgetProvider;

		public event EventHandler<GenericEventArgs<string>> PropertyChanged;

		private PropertyGrid(TreeStyle style, string category, Record parentProperty, PropertyGrid parentGrid = null)
		{
			ChildrenLayout = _layout;

			_parentGrid = parentGrid;

			_parentProperty = parentProperty;
			_layout.ColumnSpacing = 8;
			_layout.RowSpacing = 8;
			_layout.ColumnsProportions.Add(new Proportion(ProportionType.Part, 1));
			_layout.ColumnsProportions.Add(new Proportion(ProportionType.Part, 1));

			Category = category;

			if (style != null)
			{
				ApplyPropertyGridStyle(style);
			}

			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;
			Filter = string.Empty;

			this.CustomWidgetProvider = parentGrid?.CustomWidgetProvider;
			this.CustomSetter = parentGrid?.CustomSetter;
			this.CustomValuesProvider = parentGrid?.CustomValuesProvider;
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

		private static void UpdateLabelCount(Label textBlock, int count)
		{
			textBlock.Text = string.Format("{0} Items", count);
		}

		private void SetValue(Record record, object obj, object value)
		{
			if (CustomSetter != null && CustomSetter(record, obj, value))
			{
				return;
			}

			record.SetValue(obj, value);
		}

		private ComboView CreateCustomValuesEditor(Record record, object[] customValues, bool hasSetter)
		{
			var propertyType = record.Type;
			var value = record.GetValue(_object);

			var cv = new ComboView();
			foreach (var v in customValues)
			{
				var label = new Label
				{
					Text = v.ToString(),
					Tag = v
				};

				cv.Widgets.Add(label);
			}

			cv.SelectedIndex = Array.IndexOf(customValues, value);
			if (hasSetter)
			{
				cv.SelectedIndexChanged += (sender, args) =>
				{
					var item = cv.SelectedIndex != null ? customValues[cv.SelectedIndex.Value] : null;
					SetValue(record, _object, item);
					FireChanged(propertyType.Name);
				};
			}
			else
			{
				cv.Enabled = false;
			}

			return cv;
		}

		private CheckButton CreateBooleanEditor(Record record, bool hasSetter)
		{
			var propertyType = record.Type;
			var value = record.GetValue(_object);

			var isChecked = (bool)value;
			var cb = new CheckButton
			{
				IsChecked = isChecked
			};

			if (hasSetter)
			{
				cb.Click += (sender, args) =>
				{
					SetValue(record, _object, cb.IsChecked);
					FireChanged(propertyType.Name);
				};
			}
			else
			{
				cb.Enabled = false;
			}

			return cb;
		}

		private Grid CreateColorEditor(Record record, bool hasSetter)
		{
			var propertyType = record.Type;
			var value = record.GetValue(_object);

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
				Renderable = Stylesheet.Current.WhiteRegion,
				VerticalAlignment = VerticalAlignment.Center,
				Width = 32,
				Height = 16,
				Color = color
			};

			subGrid.Widgets.Add(image);

			var button = new Button
			{
				Tag = value,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				Content = new Label
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					Text = "Change..."
				}
			};
			Grid.SetColumn(button, 1);

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
						SetValue(record, _object, dlg.Color);

						FireChanged(propertyType.Name);
					};

					dlg.ShowModal(Desktop);
				};
			}
			else
			{
				button.Enabled = false;
			}

			return subGrid;
		}

		private Grid CreateBrushEditor(Record record, bool hasSetter)
		{
			var propertyType = record.Type;

			var value = record.GetValue(_object) as SolidBrush;

			var subGrid = new Grid
			{
				ColumnSpacing = 8,
				HorizontalAlignment = HorizontalAlignment.Stretch
			};

			subGrid.ColumnsProportions.Add(new Proportion());
			subGrid.ColumnsProportions.Add(new Proportion(ProportionType.Fill));

			var color = Color.Transparent;
			if (value != null)
			{
				color = value.Color;
			}

			var image = new Image
			{
				Renderable = Stylesheet.Current.WhiteRegion,
				VerticalAlignment = VerticalAlignment.Center,
				Width = 32,
				Height = 16,
				Color = color
			};

			subGrid.Widgets.Add(image);

			var button = new Button
			{
				Tag = value,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				Content = new Label
				{
					Text = "Change...",
					HorizontalAlignment = HorizontalAlignment.Center,
				}
			};
			Grid.SetColumn(button, 1);

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
						SetValue(record, _object, new SolidBrush(dlg.Color));
						var baseObject = _object as BaseObject;
						if (baseObject != null)
						{
							baseObject.Resources[record.Name] = dlg.Color.ToHexString();
						}
						FireChanged(propertyType.Name);
					};

					dlg.ShowModal(Desktop);
				};
			}
			else
			{
				button.Enabled = false;
			}

			return subGrid;
		}

		private ComboView CreateEnumEditor(Record record, bool hasSetter)
		{
			var propertyType = record.Type;
			var value = record.GetValue(_object);

			var isNullable = propertyType.IsNullableEnum();
			var enumType = isNullable ? propertyType.GetNullableType() : propertyType;
			var values = Enum.GetValues(enumType);

			var cv = new ComboView();

			if (isNullable)
			{
				cv.Widgets.Add(new Label
				{
					Text = string.Empty
				});
			}

			foreach (var v in values)
			{
				cv.Widgets.Add(new Label
				{
					Text = v.ToString(),
					Tag = v
				});
			}

			var selectedIndex = Array.IndexOf(values, value);
			if (isNullable)
			{
				++selectedIndex;
			}
			cv.SelectedIndex = selectedIndex;

			if (hasSetter)
			{
				cv.SelectedIndexChanged += (sender, args) =>
				{
					if (cv.SelectedIndex != -1)
					{
						SetValue(record, _object, cv.SelectedItem.Tag);
						FireChanged(enumType.Name);
					}
				};
			}
			else
			{
				cv.Enabled = false;
			}

			return cv;
		}

		private SpinButton CreateNumericEditor(Record record, bool hasSetter)
		{
			var propertyType = record.Type;
			var value = record.GetValue(_object);

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

						SetValue(record, _object, result);

						if (record.Type.IsValueType)
						{
							// Handle structs
							var tg = this;
							var pg = tg._parentGrid;
							while (pg != null && tg._parentProperty != null && tg._parentProperty.Type.IsValueType)
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

			return spinButton;
		}

		private TextBox CreateStringEditor(Record record, bool hasSetter)
		{
			var propertyType = record.Type;
			var value = record.GetValue(_object);

			var tf = new TextBox
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

						SetValue(record, _object, result);

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

			return tf;
		}

		private Grid CreateCollectionEditor(Record record, Type itemType)
		{
			var value = record.GetValue(_object);

			var items = (IList)value;

			var subGrid = new Grid
			{
				ColumnSpacing = 8,
				HorizontalAlignment = HorizontalAlignment.Stretch
			};

			subGrid.ColumnsProportions.Add(new Proportion());
			subGrid.ColumnsProportions.Add(new Proportion(ProportionType.Fill));

			var label = new Label
			{
				VerticalAlignment = VerticalAlignment.Center,
			};
			UpdateLabelCount(label, items.Count);

			subGrid.Widgets.Add(label);

			var button = new Button
			{
				Tag = value,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				Content = new Label
				{
					Text = "Change...",
					HorizontalAlignment = HorizontalAlignment.Center,
				}
			};
			Grid.SetColumn(button, 1);

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

			return subGrid;
		}

		private Grid CreateFileEditor<T>(Record record, bool hasSetter, string filter, Func<string, T> loader)
		{
			if (Settings.AssetManager == null)
			{
				return null;
			}

			var propertyType = record.Type;
			var value = record.GetValue(_object);

			var subGrid = new Grid
			{
				ColumnSpacing = 8,
				HorizontalAlignment = HorizontalAlignment.Stretch
			};

			subGrid.ColumnsProportions.Add(new Proportion(ProportionType.Fill));
			subGrid.ColumnsProportions.Add(new Proportion());

			var baseObject = _object as BaseObject;
			var path = string.Empty;
			if (baseObject != null)
			{
				baseObject.Resources.TryGetValue(record.Name, out path);
			}
			else if (Settings.ImagePropertyValueGetter != null)
			{
				path = Settings.ImagePropertyValueGetter(record.Name);
			}

			var textBox = new TextBox
			{
				Text = path
			};

			subGrid.Widgets.Add(textBox);

			var button = new Button
			{
				Tag = value,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				Content = new Label
				{
					Text = "Change...",
					HorizontalAlignment = HorizontalAlignment.Center,
				}
			};
			Grid.SetColumn(button, 1);

			subGrid.Widgets.Add(button);

			if (hasSetter)
			{
				button.Click += (sender, args) =>
				{
					var dlg = new FileDialog(FileDialogMode.OpenFile)
					{
						Filter = filter
					};

					if (!string.IsNullOrEmpty(textBox.Text))
					{
						var filePath = textBox.Text;
						if (!Path.IsPathRooted(filePath) && !string.IsNullOrEmpty(Settings.BasePath))
						{
							filePath = Path.Combine(Settings.BasePath, filePath);
						}
						dlg.FilePath = filePath;
					}
					else if (!string.IsNullOrEmpty(Settings.BasePath))
					{
						dlg.Folder = Settings.BasePath;
					}

					dlg.Closed += (s, a) =>
					{
						if (!dlg.Result)
						{
							return;
						}

						try
						{
							var filePath = dlg.FilePath;
							if (!string.IsNullOrEmpty(Settings.BasePath))
							{
								filePath = PathUtils.TryToMakePathRelativeTo(filePath, Settings.BasePath);
							}

							var newValue = loader(filePath);
							textBox.Text = filePath;
							SetValue(record, _object, newValue);
							if (baseObject != null)
							{
								baseObject.Resources[record.Name] = filePath;
							}
							else if (Settings.ImagePropertyValueSetter != null)
							{
								Settings.ImagePropertyValueSetter(record.Name, filePath);
							}

							FireChanged(propertyType.Name);
						}
						catch (Exception)
						{

						}
					};

					dlg.ShowModal(Desktop);
				};
			}
			else
			{
				button.Enabled = false;
			}

			return subGrid;
		}

		private Widget CreateAttributeFileEditor(Record record, bool hasSetter, FilePathAttribute attribute)
		{
			var propertyType = record.Type;
			var value = record.GetValue(_object);

			var result = new HorizontalStackPanel
			{
				Spacing = 8
			};

			TextBox path = null;
			if (attribute.ShowPath)
			{
				path = new TextBox
				{
					Readonly = true,
					HorizontalAlignment = HorizontalAlignment.Stretch
				};

				if (value != null)
				{
					path.Text = value.ToString();
				}

				StackPanel.SetProportionType(path, ProportionType.Fill);
				result.Widgets.Add(path);
			}

			var button = new Button
			{
				Tag = value,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				Content = new Label
				{
					Text = "Change...",
					HorizontalAlignment = HorizontalAlignment.Center,
				}
			};
			Grid.SetColumn(button, 1);

			if (hasSetter)
			{
				button.Click += (sender, args) =>
				{
					var dlg = new FileDialog(attribute.DialogMode)
					{
						Filter = attribute.Filter
					};

					if (value != null)
					{
						var filePath = value.ToString();
						if (!Path.IsPathRooted(filePath) && !string.IsNullOrEmpty(Settings.BasePath))
						{
							filePath = Path.Combine(Settings.BasePath, filePath);
						}
						dlg.FilePath = filePath;
					}
					else if (!string.IsNullOrEmpty(Settings.BasePath))
					{
						dlg.Folder = Settings.BasePath;
					}

					dlg.Closed += (s, a) =>
					{
						if (!dlg.Result)
						{
							return;
						}

						try
						{
							var filePath = dlg.FilePath;
							if (!string.IsNullOrEmpty(Settings.BasePath))
							{
								filePath = PathUtils.TryToMakePathRelativeTo(filePath, Settings.BasePath);
							}

							if (path != null)
							{
								path.Text = filePath;
							}

							SetValue(record, _object, filePath);

							FireChanged(propertyType.Name);
						}
						catch (Exception)
						{
						}
					};

					dlg.ShowModal(Desktop);
				};
			}
			else
			{
				button.Enabled = false;
			}

			result.Widgets.Add(button);

			return result;
		}

		private void FillSubGrid(ref int y, IReadOnlyList<Record> records)
		{
			for (var i = 0; i < records.Count; ++i)
			{
				var record = records[i];

				var hasSetter = record.HasSetter;
				if (_parentProperty != null && _parentProperty.Type.IsValueType && !_parentProperty.HasSetter)
				{
					hasSetter = false;
				}

				var value = record.GetValue(_object);
				Widget valueWidget = null;

				var oldY = y;

				var propertyType = record.Type;

				Proportion rowProportion;
				object[] customValues = null;
				if ((valueWidget = CustomWidgetProvider?.Invoke(record, _object)) != null)
				{

				}
				else if (CustomValuesProvider != null && (customValues = CustomValuesProvider(record)) != null)
				{
					if (customValues.Length == 0)
					{
						continue;
					}

					valueWidget = CreateCustomValuesEditor(record, customValues, hasSetter);
				}
				else if (propertyType == typeof(bool))
				{
					valueWidget = CreateBooleanEditor(record, hasSetter);
				}
				else if (propertyType == typeof(Color) || propertyType == typeof(Color?))
				{
					valueWidget = CreateColorEditor(record, hasSetter);
				}
				else if (propertyType.IsEnum || propertyType.IsNullableEnum())
				{
					valueWidget = CreateEnumEditor(record, hasSetter);
				}
				else if (propertyType.IsNumericType() ||
						 (propertyType.IsNullablePrimitive() && propertyType.GetNullableType().IsNumericType()))
				{

					valueWidget = CreateNumericEditor(record, hasSetter);
				}
				else if (propertyType == typeof(string) && record.FindAttribute<FilePathAttribute>() != null)
				{
					var filePathAttr = record.FindAttribute<FilePathAttribute>();
					valueWidget = CreateAttributeFileEditor(record, hasSetter, filePathAttr);
				}
				else if (propertyType == typeof(string) || propertyType.IsPrimitive || propertyType.IsNullablePrimitive())
				{
					valueWidget = CreateStringEditor(record, hasSetter);
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
								valueWidget = CreateCollectionEditor(record, itemType);
							}
						}
					}
				}
				else if (propertyType == typeof(SpriteFontBase))
				{
					valueWidget = CreateFileEditor(record, hasSetter, "*.fnt", name => Settings.AssetManager.LoadFont(name));
				}
				else if (propertyType == typeof(IBrush))
				{
					valueWidget = CreateBrushEditor(record, hasSetter);
				}
				else if (propertyType == typeof(IImage))
				{
					valueWidget = CreateFileEditor(record, hasSetter, "*.png|*.jpg|*.bmp|*.gif", name => Settings.AssetManager.LoadTextureRegion(name));
				}
#if !PLATFORM_AGNOSTIC
				else if (propertyType == typeof(Texture2D))
				{
					valueWidget = CreateFileEditor(record, hasSetter, "*.png|*.jpg|*.bmp|*.gif", name => Settings.AssetManager.LoadTexture2D(MyraEnvironment.GraphicsDevice, name));
				}
#endif
				else
				{
					// Subgrid
					if (value != null)
					{
						if (PassesFilter(record.Name))
						{
							var subGrid = new SubGrid(this, value, record.Name, DefaultCategoryName, string.Empty, record);
							Grid.SetColumnSpan(subGrid, 2);
							Grid.SetRow(subGrid, y);

							Children.Add(subGrid);

							rowProportion = new Proportion(ProportionType.Auto);
							_layout.RowsProportions.Add(rowProportion);
							++y;
						}

						continue;
					}

					var tb = new Label();
					tb.ApplyLabelStyle(PropertyGridStyle.LabelStyle);
					tb.Text = "null";

					valueWidget = tb;
				}

				if (valueWidget == null)
				{
					continue;
				}

				var name = record.Name;
				var dn = record.FindAttribute<DisplayNameAttribute>();

				if (dn != null)
				{
					name = dn.DisplayName;
				}

				if (!PassesFilter(name))
				{
					continue;
				}

				var nameLabel = new Label
				{
					Text = name,
					VerticalAlignment = VerticalAlignment.Center,
				};
				Grid.SetColumn(nameLabel, 0);
				Grid.SetRow(nameLabel, oldY);

				Children.Add(nameLabel);

				Grid.SetColumn(valueWidget, 1);
				Grid.SetRow(valueWidget, oldY);
				valueWidget.HorizontalAlignment = HorizontalAlignment.Stretch;
				valueWidget.VerticalAlignment = VerticalAlignment.Top;

				Children.Add(valueWidget);

				rowProportion = new Proportion(ProportionType.Auto);
				_layout.RowsProportions.Add(rowProportion);
				++y;
			}
		}

		public bool PassesFilter(string name)
		{
			if (string.IsNullOrEmpty(Filter) || string.IsNullOrEmpty(name))
			{
				return true;
			}

			return name.ToLower().Contains(_filter.ToLower());
		}

		public void Rebuild()
		{
			_layout.RowsProportions.Clear();
			Children.Clear();
			_records.Clear();
			_expandedCategories.Clear();

			if (_object == null)
			{
				return;
			}

			// Properties
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

				var categoryAttr = property.FindAttribute<CategoryAttribute>();
				record.Category = categoryAttr != null ? categoryAttr.Category : DefaultCategoryName;

				records.Add(record);
			}

			// Fields
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

			// Attached properties
			var asWidget = _object as Widget;
			if (asWidget != null && ParentType != null)
			{
				var attachedProperties = AttachedPropertiesRegistry.GetPropertiesOfType(ParentType);
				foreach (var attachedProperty in attachedProperties)
				{
					var record = new AttachedPropertyRecord(attachedProperty)
					{
						Category = attachedProperty.OwnerType.Name
					};

					records.Add(record);
				}
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

				var subGrid = new SubGrid(this, Object, category.Key, category.Key, Filter, null);
				Grid.SetColumnSpan(subGrid, 2);
				Grid.SetRow(subGrid, y); ;


				if (subGrid.IsEmpty)
				{
					continue;
				}

				Children.Add(subGrid);

				if (_expandedCategories.Contains(category.Key))
				{
					subGrid.Mark.IsPressed = true;
				}

				var rp = new Proportion(ProportionType.Auto);
				_layout.RowsProportions.Add(rp);

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