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
	public partial class PropertyGrid : Widget, IInspector
	{
		private const string DefaultCategoryName = "Miscellaneous";
		private const int ALLOC = 32;

		private readonly GridLayout _layout = new GridLayout();
		private readonly PropertyGrid _parentGrid;
		private readonly Record _parentProperty;
		private readonly Dictionary<string, List<Record>> _records = new Dictionary<string, List<Record>>();
		private readonly List<Record> _recMemory = new List<Record>(ALLOC);
		private readonly List<PropertyEditor> _editors = new List<PropertyEditor>(ALLOC);
		private readonly HashSet<string> _expandedCategories = new HashSet<string>();
		private readonly PropertyGridSettings _settings = new PropertyGridSettings();
		private object _object;
		private bool _ignoreCollections;
		private bool _doFancyLayout = true;
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
				// TODO send notification to active editors before 
				if (value == _object)
				{
					return;
				}

				_object = value;
				Rebuild();

				ObjectChanged?.Invoke(this, EventArgs.Empty);
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
					return _parentGrid.ParentType;
				return _parentType;
			}
			set => _parentType = value;
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
					return _parentGrid.IgnoreCollections;
				return _ignoreCollections;
			}
			set => _ignoreCollections = value;
		}

		[Category("Behavior")]
		[DefaultValue(true)]
		public bool DoFancyLayout
		{
			get
			{
				if (_parentGrid != null)
					return _parentGrid.DoFancyLayout;
				return _doFancyLayout;
			}
			set
			{
				if (_parentGrid != null)
					return;
				if (_doFancyLayout != value)
				{
					_doFancyLayout = value;
					Rebuild();
				}
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public bool IsEmpty => Children.Count == 0;

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
		public Func<object, Record, CustomValues> CustomValuesProvider;

		[Browsable(false)]
		[XmlIgnore]
		public Func<Record, object, object, bool> CustomSetter;

		[Browsable(false)]
		[XmlIgnore]
		public Func<Record, object, Widget> CustomWidgetProvider;
		
		public event EventHandler<GenericEventArgs<string>> PropertyChanged;
		public event EventHandler ObjectChanged;

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
		
		object IInspector.SelectedField => _object;
		/// <inheritdoc />
		string IInspector.BasePath => Settings.BasePath;
		
		AssetManager IInspector.AssetManager => Settings.AssetManager;

		void IInspector.FireChanged(string name)
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
		
		private void SetValue(Record record, object obj, object value)
		{
			if (CustomSetter != null && CustomSetter(record, obj, value)) //TODO reimplement custom setter
				return;

			record.SetValue(obj, value);
		}

		private ComboView CreateCustomValuesEditor(Record record, CustomValues customValues, bool hasSetter)
		{
			var propertyType = record.Type;
			var value = record.GetValue(_object);

			var cv = new ComboView();
			foreach (var v in customValues.Values)
			{
				var label = new Label
				{
					Text = v.Name,
					Tag = v.Value
				};

				cv.Widgets.Add(label);
			}

			cv.SelectedIndex = customValues.SelectedIndex;
			if (hasSetter)
			{
				cv.SelectedIndexChanged += (sender, args) =>
				{
					var item = cv.SelectedIndex != null ? customValues.Values[cv.SelectedIndex.Value].Value : null;
					SetValue(record, _object, item);
					(this as IInspector).FireChanged(record.Name);
				};
			}
			else
			{
				cv.Enabled = false;
			}

			return cv;
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

							(this as IInspector).FireChanged(propertyType.Name);
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
/*
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
		}*/

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
				CustomValues customValues = null;

				var needsSubGrid = false;
				
				if ((valueWidget = CustomWidgetProvider?.Invoke(record, _object)) != null)
				{
				}
				else if (CustomValuesProvider != null && (customValues = CustomValuesProvider(_object, record)) != null)
				{
					if (customValues.Values.Length == 0)
					{
						continue;
					}

					valueWidget = CreateCustomValuesEditor(record, customValues, hasSetter);
					if (value != null && !value.GetType().IsPrimitive && value.GetType() != typeof(string))
					{
						needsSubGrid = true;
					}
				} /*
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
				}*/
				else if (propertyType == typeof(SpriteFontBase))
				{
					valueWidget = CreateFileEditor(record, hasSetter, "*.fnt", name => Settings.AssetManager.LoadFont(name));
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
#if !STRIDE
				else if (propertyType == typeof(TextureCube))
				{
					valueWidget = CreateFileEditor(record, hasSetter, "*.dds", name => Settings.AssetManager.LoadTexture2D(MyraEnvironment.GraphicsDevice, name));
				}
#endif
#endif
				else if (PropertyEditor.TryCreate(this, record, out PropertyEditor editor))
				{
					_editors.Add(editor);
					valueWidget = editor.Widget;
				}
				else
				{
					if (value == null)
					{
						var tb = new Label();
						tb.ApplyLabelStyle(PropertyGridStyle.LabelStyle);
						tb.Text = "null";

						valueWidget = tb;
					} 
					else
					{
						needsSubGrid = true;
					}
				}
				
				
				if (valueWidget != null) //Add single value display
				{
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

				if (needsSubGrid)
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
					}
				}
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
			_editors.Clear();
			_expandedCategories.Clear();

			if(!RecordAggregator(in _object, in _parentType, _recMemory, _doFancyLayout))
				return;
			
			// Sort by categories
			for (var i = 0; i < _recMemory.Count; ++i)
			{
				var record = _recMemory[i];

				List<Record> categoryRecords;
				if (!_records.TryGetValue(record.Category, out categoryRecords))
				{
					categoryRecords = new List<Record>();
					_records[record.Category] = categoryRecords;
				}

				categoryRecords.Add(record);
			}

			if (_doFancyLayout)
			{
				// Sort by names within categories
				foreach (var category in _records)
				{
					category.Value.Sort((a, b) => Comparer<string>.Default.Compare(a.Name, b.Name));
				}
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
				Grid.SetRow(subGrid, y);

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
		
		private static bool RecordAggregator(in object target, in Type parentType, List<Record> result, bool categorize = true)
		{
			result.Clear();
			if (target == null)
				return false;

			Type targetType = target.GetType();
			
			// Properties
			var properties = from p in targetType.GetProperties() select p;
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
				record.Category = (categoryAttr != null & categorize) ? categoryAttr.Category : DefaultCategoryName;

				result.Add(record);
			}

			// Fields
			var fields = from f in targetType.GetFields() select f;
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
					Category = (categoryAttr != null & categorize) ? categoryAttr.Category : DefaultCategoryName
				};

				result.Add(record);
			}

			// Attached properties
			var asWidget = target as Widget;
			if (asWidget != null && parentType != null)
			{
				var attachedProperties = AttachedPropertiesRegistry.GetPropertiesOfType(parentType);
				foreach (var attachedProperty in attachedProperties)
				{
					var record = new AttachedPropertyRecord(attachedProperty)
					{
						Category = attachedProperty.OwnerType.Name
					};

					result.Add(record);
				}
			}

			return true;
		}

		private void RecordSorter(ref List<Record> collection)
		{
			
		}

		public void ApplyPropertyGridStyle(TreeStyle style)
		{
			ApplyWidgetStyle(style);

			PropertyGridStyle = style;
		}
		
		public override void InternalRender(RenderContext context)
		{
			foreach (var editor in _editors)
			{
				editor.UpdateDisplay();
			}
			base.InternalRender(context);
		}
	}
}
