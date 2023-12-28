using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Linq;
using Myra.MML;
using System.Collections;
using FontStashSharp;
using Myra.Graphics2D.TextureAtlases;
using FontStashSharp.RichText;
using Myra.Graphics2D.Brushes;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using Color = FontStashSharp.FSColor;
using SolidBrush = Myra.Graphics2D.Brushes.SolidBrush;
#endif

namespace Myra.Graphics2D.UI.Styles
{
	public class Stylesheet
	{
		private static readonly Dictionary<string, string> LegacyClassNames = new Dictionary<string, string>();
		private static readonly Dictionary<string, string> LegacyPropertyNames = new Dictionary<string, string>();

		public const string DefaultStyleName = "";

		private static Stylesheet _current;

		public static Stylesheet Current
		{
			get
			{
				if (_current == null)
				{
					_current = DefaultAssets.DefaultStylesheet;
				}

				return _current;
			}

			set
			{
				_current = value;
			}
		}

		private readonly Dictionary<string, LabelStyle> _labelStyles = new Dictionary<string, LabelStyle>();
		private readonly Dictionary<string, LabelStyle> _tooltipStyles = new Dictionary<string, LabelStyle>();
		private readonly Dictionary<string, TextBoxStyle> _textBoxStyles = new Dictionary<string, TextBoxStyle>();
		private readonly Dictionary<string, ButtonStyle> _buttonStyles = new Dictionary<string, ButtonStyle>();
		private readonly Dictionary<string, ImageTextButtonStyle> _checkBoxStyles = new Dictionary<string, ImageTextButtonStyle>();
		private readonly Dictionary<string, ImageTextButtonStyle> _radioButtonStyles = new Dictionary<string, ImageTextButtonStyle>();
		private readonly Dictionary<string, SpinButtonStyle> _spinButtonStyles = new Dictionary<string, SpinButtonStyle>();
		private readonly Dictionary<string, SliderStyle> _horizontalSliderStyles = new Dictionary<string, SliderStyle>();
		private readonly Dictionary<string, SliderStyle> _verticalSliderStyles = new Dictionary<string, SliderStyle>();
		private readonly Dictionary<string, ProgressBarStyle> _horizontalProgressBarStyles =
			new Dictionary<string, ProgressBarStyle>();
		private readonly Dictionary<string, ProgressBarStyle> _verticalProgressBarStyles =
			new Dictionary<string, ProgressBarStyle>();
		private readonly Dictionary<string, SeparatorStyle> _horizontalSeparatorStyles =
			new Dictionary<string, SeparatorStyle>();
		private readonly Dictionary<string, SeparatorStyle> _verticalSeparatorStyles =
			new Dictionary<string, SeparatorStyle>();
		private readonly Dictionary<string, ComboBoxStyle> _comboBoxStyles = new Dictionary<string, ComboBoxStyle>();
		private readonly Dictionary<string, ListBoxStyle> _listBoxStyles = new Dictionary<string, ListBoxStyle>();
		private readonly Dictionary<string, TabControlStyle> _tabControlStyles = new Dictionary<string, TabControlStyle>();
		private readonly Dictionary<string, TreeStyle> _treeStyles = new Dictionary<string, TreeStyle>();
		private readonly Dictionary<string, SplitPaneStyle> _horizontalSplitPaneStyles =
			new Dictionary<string, SplitPaneStyle>();
		private readonly Dictionary<string, SplitPaneStyle> _verticalSplitPaneStyles =
			new Dictionary<string, SplitPaneStyle>();
		private readonly Dictionary<string, ScrollViewerStyle> _scrollViewerStyles = new Dictionary<string, ScrollViewerStyle>();
		private readonly Dictionary<string, MenuStyle> _horizontalMenuStyles = new Dictionary<string, MenuStyle>();
		private readonly Dictionary<string, MenuStyle> _verticalMenuStyles = new Dictionary<string, MenuStyle>();
		private readonly Dictionary<string, WindowStyle> _windowStyles = new Dictionary<string, WindowStyle>();
		private readonly Dictionary<string, FileDialogStyle> _fileDialogStyles = new Dictionary<string, FileDialogStyle>();
		private readonly Dictionary<string, ColorPickerDialogStyle> _colorPickerDialogStyles = new Dictionary<string, ColorPickerDialogStyle>();

		private TextureRegion _whiteRegion;

		public TextureRegionAtlas Atlas { get; private set; }

		public TextureRegion WhiteRegion
		{
			get
			{
				if (_whiteRegion == null)
				{
					_whiteRegion = Atlas["white"];
				}

				return _whiteRegion;
			}
		}

		public Dictionary<string, SpriteFontBase> Fonts { get; private set; }

		public DesktopStyle DesktopStyle { get; set; }

		[XmlIgnore]
		public LabelStyle LabelStyle
		{
			get => GetDefaultStyle(_labelStyles);
			set => SetDefaultStyle(_labelStyles, value);
		}

		[XmlIgnore]
		public LabelStyle TooltipStyle
		{
			get => GetDefaultStyle(_tooltipStyles);
			set => SetDefaultStyle(_tooltipStyles, value);
		}

		[XmlIgnore]
		public TextBoxStyle TextBoxStyle
		{
			get => GetDefaultStyle(_textBoxStyles);
			set => SetDefaultStyle(_textBoxStyles, value);
		}

		[XmlIgnore]
		public ButtonStyle ButtonStyle
		{
			get => GetDefaultStyle(_buttonStyles);
			set => SetDefaultStyle(_buttonStyles, value);
		}

		[XmlIgnore]
		public ImageTextButtonStyle CheckBoxStyle
		{
			get => GetDefaultStyle(_checkBoxStyles);
			set => SetDefaultStyle(_checkBoxStyles, value);
		}

		[XmlIgnore]
		public ImageTextButtonStyle RadioButtonStyle
		{
			get => GetDefaultStyle(_radioButtonStyles);
			set => SetDefaultStyle(_radioButtonStyles, value);
		}

		[XmlIgnore]
		public SpinButtonStyle SpinButtonStyle
		{
			get => GetDefaultStyle(_spinButtonStyles);
			set => SetDefaultStyle(_spinButtonStyles, value);
		}

		[XmlIgnore]
		public SliderStyle HorizontalSliderStyle
		{
			get => GetDefaultStyle(_horizontalSliderStyles);
			set => SetDefaultStyle(_horizontalSliderStyles, value);
		}

		[XmlIgnore]
		public SliderStyle VerticalSliderStyle
		{
			get => GetDefaultStyle(_verticalSliderStyles);
			set => SetDefaultStyle(_verticalSliderStyles, value);
		}

		[XmlIgnore]
		public ProgressBarStyle HorizontalProgressBarStyle
		{
			get => GetDefaultStyle(_horizontalProgressBarStyles);
			set => SetDefaultStyle(_horizontalProgressBarStyles, value);
		}

		[XmlIgnore]
		public ProgressBarStyle VerticalProgressBarStyle
		{
			get => GetDefaultStyle(_verticalProgressBarStyles);
			set => SetDefaultStyle(_verticalProgressBarStyles, value);
		}

		[XmlIgnore]
		public SeparatorStyle HorizontalSeparatorStyle
		{
			get => GetDefaultStyle(_horizontalSeparatorStyles);
			set => SetDefaultStyle(_horizontalSeparatorStyles, value);
		}

		[XmlIgnore]
		public SeparatorStyle VerticalSeparatorStyle
		{
			get => GetDefaultStyle(_verticalSeparatorStyles);
			set => SetDefaultStyle(_verticalSeparatorStyles, value);
		}

		[XmlIgnore]
		public ComboBoxStyle ComboBoxStyle
		{
			get => GetDefaultStyle(_comboBoxStyles);
			set => SetDefaultStyle(_comboBoxStyles, value);
		}

		[XmlIgnore]
		public ListBoxStyle ListBoxStyle
		{
			get => GetDefaultStyle(_listBoxStyles);
			set => SetDefaultStyle(_listBoxStyles, value);
		}

		[XmlIgnore]
		public TabControlStyle TabControlStyle
		{
			get => GetDefaultStyle(_tabControlStyles);
			set => SetDefaultStyle(_tabControlStyles, value);
		}

		[XmlIgnore]
		public TreeStyle TreeStyle
		{
			get => GetDefaultStyle(_treeStyles);
			set => SetDefaultStyle(_treeStyles, value);
		}

		[XmlIgnore]
		public SplitPaneStyle HorizontalSplitPaneStyle
		{
			get => GetDefaultStyle(_horizontalSplitPaneStyles);
			set => SetDefaultStyle(_horizontalSplitPaneStyles, value);
		}

		[XmlIgnore]
		public SplitPaneStyle VerticalSplitPaneStyle
		{
			get => GetDefaultStyle(_verticalSplitPaneStyles);
			set => SetDefaultStyle(_verticalSplitPaneStyles, value);
		}

		[XmlIgnore]
		public ScrollViewerStyle ScrollViewerStyle
		{
			get => GetDefaultStyle(_scrollViewerStyles);
			set => SetDefaultStyle(_scrollViewerStyles, value);
		}

		[XmlIgnore]
		public MenuStyle HorizontalMenuStyle
		{
			get => GetDefaultStyle(_horizontalMenuStyles);
			set => SetDefaultStyle(_horizontalMenuStyles, value);
		}

		[XmlIgnore]
		public MenuStyle VerticalMenuStyle
		{
			get => GetDefaultStyle(_verticalMenuStyles);
			set => SetDefaultStyle(_verticalMenuStyles, value);
		}

		[XmlIgnore]
		public WindowStyle WindowStyle
		{
			get => GetDefaultStyle(_windowStyles);
			set => SetDefaultStyle(_windowStyles, value);
		}

		public FileDialogStyle FileDialogStyle
		{
			get => GetDefaultStyle(_fileDialogStyles);
			set => SetDefaultStyle(_fileDialogStyles, value);
		}

		public ColorPickerDialogStyle ColorPickerDialogStyle
		{
			get => GetDefaultStyle(_colorPickerDialogStyles);
			set => SetDefaultStyle(_colorPickerDialogStyles, value);
		}

		public Dictionary<string, LabelStyle> LabelStyles => _labelStyles;

		public Dictionary<string, LabelStyle> TooltipStyles => _tooltipStyles;

		public Dictionary<string, TextBoxStyle> TextBoxStyles => _textBoxStyles;

		public Dictionary<string, ButtonStyle> ButtonStyles => _buttonStyles;

		public Dictionary<string, ImageTextButtonStyle> CheckBoxStyles => _checkBoxStyles;

		public Dictionary<string, ImageTextButtonStyle> RadioButtonStyles => _radioButtonStyles;

		public Dictionary<string, SpinButtonStyle> SpinButtonStyles => _spinButtonStyles;

		public Dictionary<string, SliderStyle> HorizontalSliderStyles => _horizontalSliderStyles;

		public Dictionary<string, SliderStyle> VerticalSliderStyles => _verticalSliderStyles;

		public Dictionary<string, ProgressBarStyle> HorizontalProgressBarStyles => _horizontalProgressBarStyles;

		public Dictionary<string, ProgressBarStyle> VerticalProgressBarStyles => _verticalProgressBarStyles;

		public Dictionary<string, SeparatorStyle> HorizontalSeparatorStyles => _horizontalSeparatorStyles;

		public Dictionary<string, SeparatorStyle> VerticalSeparatorStyles => _verticalSeparatorStyles;

		public Dictionary<string, ComboBoxStyle> ComboBoxStyles => _comboBoxStyles;

		public Dictionary<string, ListBoxStyle> ListBoxStyles => _listBoxStyles;

		public Dictionary<string, TabControlStyle> TabControlStyles => _tabControlStyles;

		public Dictionary<string, TreeStyle> TreeStyles => _treeStyles;

		public Dictionary<string, SplitPaneStyle> HorizontalSplitPaneStyles => _horizontalSplitPaneStyles;

		public Dictionary<string, SplitPaneStyle> VerticalSplitPaneStyles => _verticalSplitPaneStyles;

		public Dictionary<string, ScrollViewerStyle> ScrollViewerStyles => _scrollViewerStyles;

		public Dictionary<string, MenuStyle> HorizontalMenuStyles => _horizontalMenuStyles;

		public Dictionary<string, MenuStyle> VerticalMenuStyles => _verticalMenuStyles;

		public Dictionary<string, WindowStyle> WindowStyles => _windowStyles;

		public Dictionary<string, FileDialogStyle> FileDialogStyles => _fileDialogStyles;

		public Dictionary<string, ColorPickerDialogStyle> ColorPickerDialogStyles => _colorPickerDialogStyles;

		static Stylesheet()
		{
			LegacyClassNames["TextBlockStyle"] = "LabelStyle";
			LegacyClassNames["TextFieldStyle"] = "TextBoxStyle";
			LegacyClassNames["ScrollPaneStyle"] = "ScrollViewerStyle";

			LegacyPropertyNames["TextBlockStyle"] = "LabelStyle";
			LegacyPropertyNames["TextFieldStyle"] = "TextBoxStyle";
			LegacyPropertyNames["ScrollPaneStyle"] = "ScrollViewerStyle";
			LegacyPropertyNames["TextBlockStyles"] = "LabelStyles";
			LegacyPropertyNames["TextFieldStyles"] = "TextBoxStyles";
			LegacyPropertyNames["ScrollPaneStyles"] = "ScrollViewerStyles";
		}

		private static T GetDefaultStyle<T>(Dictionary<string, T> styles) where T : WidgetStyle
		{
			T result = null;
			if (!styles.TryGetValue(DefaultStyleName, out result))
			{
				throw new Exception("Stylesheet doesnt define default style for " + typeof(T).Name + ".");
			}

			return result;
		}

		private static void SetDefaultStyle<T>(Dictionary<string, T> styles, T value) where T : WidgetStyle
		{
			styles[DefaultStyleName] = value;
		}

		public static Stylesheet LoadFromSource(string stylesheetXml,
			TextureRegionAtlas textureRegionAtlas,
			Dictionary<string, SpriteFontBase> fonts)
		{
			var xDoc = XDocument.Parse(stylesheetXml);

			var colors = new Dictionary<string, Color>();
			var colorsNode = xDoc.Root.Element("Colors");
			if (colorsNode != null)
			{
				foreach (var el in colorsNode.Elements())
				{
					var color = ColorStorage.FromName(el.Attribute("Value").Value);
					if (color != null)
					{
						colors[el.Attribute(BaseContext.IdName).Value] = color.Value;
					}
				}
			}

			Func<Type, string, object> resourceGetter = (t, name) =>
			{
				if (typeof(IBrush).IsAssignableFrom(t))
				{
					TextureRegion region;

					if (!textureRegionAtlas.Regions.TryGetValue(name, out region))
					{
						var color = ColorStorage.FromName(name);
						if (color != null)
						{
							return new SolidBrush(color.Value);
						}
					}
					else
					{
						return region;
					}

					throw new Exception(string.Format("Could not find parse IBrush '{0}'", name));
				}
				else if (t == typeof(SpriteFontBase))
				{
					return fonts[name];
				}

				throw new Exception(string.Format("Type {0} isn't supported", t.Name));
			};

			var result = new Stylesheet
			{
				Atlas = textureRegionAtlas,
				Fonts = fonts
			};

			var loadContext = new LoadContext
			{
				Namespaces = new[]
				{
					typeof(WidgetStyle).Namespace
				},
				ResourceGetter = resourceGetter,
				NodesToIgnore = new HashSet<string>(new[] { "Designer", "Colors", "Fonts" }),
				LegacyClassNames = LegacyClassNames,
				LegacyPropertyNames = LegacyPropertyNames,
				Colors = colors
			};

			loadContext.Load<object>(result, xDoc.Root, null);

			return result;
		}

		public string[] GetStylesByWidgetName(string name)
		{
			// Special case
			if (name.Contains("Button"))
			{
				name = "Button";
			}

			var propertyName = name + "Styles";
			var property = GetType().GetProperty(propertyName);
			if (property == null)
			{
				return null;
			}

			var dict = (IDictionary)property.GetValue(this);

			var result = new List<string>();
			foreach (var k in dict.Keys)
			{
				result.Add((string)k);
			}

			return result.ToArray();
		}

		private void CloneStylesTo<T>(Stylesheet destStylesheet, Func<Stylesheet, Dictionary<string, T>> stylesGetter) where T : WidgetStyle
		{
			var src = stylesGetter(this);
			var dest = stylesGetter(destStylesheet);

			dest.Clear();
			foreach (var pair in src)
			{
				dest[pair.Key] = (T)pair.Value.Clone();
			}
		}

		public Stylesheet Clone()
		{
			var result = new Stylesheet
			{
				Atlas = Atlas,
				Fonts = new Dictionary<string, SpriteFontBase>()
			};

			// Clone all dictionary properties
			CloneStylesTo(result, s => s.HorizontalSliderStyles);
			CloneStylesTo(result, s => s.VerticalSliderStyles);
			CloneStylesTo(result, s => s.HorizontalProgressBarStyles);
			CloneStylesTo(result, s => s.VerticalProgressBarStyles);
			CloneStylesTo(result, s => s.HorizontalSeparatorStyles);
			CloneStylesTo(result, s => s.VerticalSeparatorStyles);
			CloneStylesTo(result, s => s.HorizontalSplitPaneStyles);
			CloneStylesTo(result, s => s.VerticalSplitPaneStyles);
			CloneStylesTo(result, s => s.HorizontalMenuStyles);
			CloneStylesTo(result, s => s.VerticalMenuStyles);

			CloneStylesTo(result, s => s.LabelStyles);
			CloneStylesTo(result, s => s.TextBoxStyles);
			CloneStylesTo(result, s => s.ButtonStyles);
			CloneStylesTo(result, s => s.CheckBoxStyles);
			CloneStylesTo(result, s => s.RadioButtonStyles);
			CloneStylesTo(result, s => s.SpinButtonStyles);
			CloneStylesTo(result, s => s.ComboBoxStyles);
			CloneStylesTo(result, s => s.ListBoxStyles);
			CloneStylesTo(result, s => s.TabControlStyles);
			CloneStylesTo(result, s => s.TreeStyles);
			CloneStylesTo(result, s => s.ScrollViewerStyles);
			CloneStylesTo(result, s => s.WindowStyles);

			foreach (var pair in Fonts)
			{
				result.Fonts[pair.Key] = pair.Value;
			}

			return result;
		}
	}
}