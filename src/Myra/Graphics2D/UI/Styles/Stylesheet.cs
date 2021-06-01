﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Linq;
using Myra.MML;
using System.Collections;
using AssetManagementBase;
using FontStashSharp;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.Brushes;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
using SolidBrush = Myra.Graphics2D.Brushes.SolidBrush;
#endif

namespace Myra.Graphics2D.UI.Styles
{
	[AssetLoader(typeof(StylesheetLoader))]
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
					_current = DefaultAssets.UIStylesheet;
				}

				return _current;
			}

			set
			{
				_current = value;
			}
		}

		private readonly Dictionary<string, LabelStyle> _labelStyles = new Dictionary<string, LabelStyle>();
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

		public TextureRegionAtlas Atlas { get; private set; }
		public Dictionary<string, SpriteFontBase> Fonts { get; private set; }

		public DesktopStyle DesktopStyle
		{
			get; set;
		}

		[XmlIgnore]
		public LabelStyle LabelStyle
		{
			get
			{
				return GetDefaultStyle(_labelStyles);
			}
			set
			{
				SetDefaultStyle(_labelStyles, value);
			}
		}

		[XmlIgnore]
		public TextBoxStyle TextBoxStyle
		{
			get
			{
				return GetDefaultStyle(_textBoxStyles);
			}
			set
			{
				SetDefaultStyle(_textBoxStyles, value);
			}
		}

		[XmlIgnore]
		public ButtonStyle ButtonStyle
		{
			get
			{
				return GetDefaultStyle(_buttonStyles);
			}
			set
			{
				SetDefaultStyle(_buttonStyles, value);
			}
		}

		[XmlIgnore]
		public ImageTextButtonStyle CheckBoxStyle
		{
			get
			{
				return GetDefaultStyle(_checkBoxStyles);
			}
			set
			{
				SetDefaultStyle(_checkBoxStyles, value);
			}
		}

		[XmlIgnore]
		public ImageTextButtonStyle RadioButtonStyle
		{
			get
			{
				return GetDefaultStyle(_radioButtonStyles);
			}
			set
			{
				SetDefaultStyle(_radioButtonStyles, value);
			}
		}

		[XmlIgnore]
		public SpinButtonStyle SpinButtonStyle
		{
			get
			{
				return GetDefaultStyle(_spinButtonStyles);
			}
			set
			{
				SetDefaultStyle(_spinButtonStyles, value);
			}
		}

		[XmlIgnore]
		public SliderStyle HorizontalSliderStyle
		{
			get
			{
				return GetDefaultStyle(_horizontalSliderStyles);
			}
			set
			{
				SetDefaultStyle(_horizontalSliderStyles, value);
			}
		}

		[XmlIgnore]
		public SliderStyle VerticalSliderStyle
		{
			get
			{
				return GetDefaultStyle(_verticalSliderStyles);
			}
			set
			{
				SetDefaultStyle(_verticalSliderStyles, value);
			}
		}

		[XmlIgnore]
		public ProgressBarStyle HorizontalProgressBarStyle
		{
			get
			{
				return GetDefaultStyle(_horizontalProgressBarStyles);
			}
			set
			{
				SetDefaultStyle(_horizontalProgressBarStyles, value);
			}
		}

		[XmlIgnore]
		public ProgressBarStyle VerticalProgressBarStyle
		{
			get
			{
				return GetDefaultStyle(_verticalProgressBarStyles);
			}
			set
			{
				SetDefaultStyle(_verticalProgressBarStyles, value);
			}
		}

		[XmlIgnore]
		public SeparatorStyle HorizontalSeparatorStyle
		{
			get
			{
				return GetDefaultStyle(_horizontalSeparatorStyles);
			}
			set
			{
				SetDefaultStyle(_horizontalSeparatorStyles, value);
			}
		}

		[XmlIgnore]
		public SeparatorStyle VerticalSeparatorStyle
		{
			get
			{
				return GetDefaultStyle(_verticalSeparatorStyles);
			}
			set
			{
				SetDefaultStyle(_verticalSeparatorStyles, value);
			}
		}

		[XmlIgnore]
		public ComboBoxStyle ComboBoxStyle
		{
			get
			{
				return GetDefaultStyle(_comboBoxStyles);
			}
			set
			{
				SetDefaultStyle(_comboBoxStyles, value);
			}
		}

		[XmlIgnore]
		public ListBoxStyle ListBoxStyle
		{
			get
			{
				return GetDefaultStyle(_listBoxStyles);
			}
			set
			{
				SetDefaultStyle(_listBoxStyles, value);
			}
		}

		[XmlIgnore]
		public TabControlStyle TabControlStyle
		{
			get
			{
				return GetDefaultStyle(_tabControlStyles);
			}
			set
			{
				SetDefaultStyle(_tabControlStyles, value);
			}
		}

		[XmlIgnore]
		public TreeStyle TreeStyle
		{
			get
			{
				return GetDefaultStyle(_treeStyles);
			}
			set
			{
				SetDefaultStyle(_treeStyles, value);
			}
		}

		[XmlIgnore]
		public SplitPaneStyle HorizontalSplitPaneStyle
		{
			get
			{
				return GetDefaultStyle(_horizontalSplitPaneStyles);
			}
			set
			{
				SetDefaultStyle(_horizontalSplitPaneStyles, value);
			}
		}

		[XmlIgnore]
		public SplitPaneStyle VerticalSplitPaneStyle
		{
			get
			{
				return GetDefaultStyle(_verticalSplitPaneStyles);
			}
			set
			{
				SetDefaultStyle(_verticalSplitPaneStyles, value);
			}
		}

		[XmlIgnore]
		public ScrollViewerStyle ScrollViewerStyle
		{
			get
			{
				return GetDefaultStyle(_scrollViewerStyles);
			}
			set
			{
				SetDefaultStyle(_scrollViewerStyles, value);
			}
		}

		[XmlIgnore]
		public MenuStyle HorizontalMenuStyle
		{
			get
			{
				return GetDefaultStyle(_horizontalMenuStyles);
			}
			set
			{
				SetDefaultStyle(_horizontalMenuStyles, value);
			}
		}

		[XmlIgnore]
		public MenuStyle VerticalMenuStyle
		{
			get
			{
				return GetDefaultStyle(_verticalMenuStyles);
			}
			set
			{
				SetDefaultStyle(_verticalMenuStyles, value);
			}
		}

		[XmlIgnore]
		public WindowStyle WindowStyle
		{
			get
			{
				return GetDefaultStyle(_windowStyles);
			}
			set
			{
				SetDefaultStyle(_windowStyles, value);
			}
		}

		public Dictionary<string, LabelStyle> LabelStyles
		{
			get
			{
				return _labelStyles;
			}
		}

		public Dictionary<string, TextBoxStyle> TextBoxStyles
		{
			get
			{
				return _textBoxStyles;
			}
		}

		public Dictionary<string, ButtonStyle> ButtonStyles
		{
			get
			{
				return _buttonStyles;
			}
		}

		public Dictionary<string, ImageTextButtonStyle> CheckBoxStyles
		{
			get
			{
				return _checkBoxStyles;
			}
		}

		public Dictionary<string, ImageTextButtonStyle> RadioButtonStyles
		{
			get
			{
				return _radioButtonStyles;
			}
		}

		public Dictionary<string, SpinButtonStyle> SpinButtonStyles
		{
			get
			{
				return _spinButtonStyles;
			}
		}

		public Dictionary<string, SliderStyle> HorizontalSliderStyles
		{
			get
			{
				return _horizontalSliderStyles;
			}
		}

		public Dictionary<string, SliderStyle> VerticalSliderStyles
		{
			get
			{
				return _verticalSliderStyles;
			}
		}

		public Dictionary<string, ProgressBarStyle> HorizontalProgressBarStyles
		{
			get
			{
				return _horizontalProgressBarStyles;
			}
		}

		public Dictionary<string, ProgressBarStyle> VerticalProgressBarStyles
		{
			get
			{
				return _verticalProgressBarStyles;
			}
		}

		public Dictionary<string, SeparatorStyle> HorizontalSeparatorStyles
		{
			get
			{
				return _horizontalSeparatorStyles;
			}
		}

		public Dictionary<string, SeparatorStyle> VerticalSeparatorStyles
		{
			get
			{
				return _verticalSeparatorStyles;
			}
		}

		public Dictionary<string, ComboBoxStyle> ComboBoxStyles
		{
			get
			{
				return _comboBoxStyles;
			}
		}

		public Dictionary<string, ListBoxStyle> ListBoxStyles
		{
			get
			{
				return _listBoxStyles;
			}
		}

		public Dictionary<string, TabControlStyle> TabControlStyles
		{
			get
			{
				return _tabControlStyles;
			}
		}

		public Dictionary<string, TreeStyle> TreeStyles
		{
			get
			{
				return _treeStyles;
			}
		}

		public Dictionary<string, SplitPaneStyle> HorizontalSplitPaneStyles
		{
			get
			{
				return _horizontalSplitPaneStyles;
			}
		}

		public Dictionary<string, SplitPaneStyle> VerticalSplitPaneStyles
		{
			get
			{
				return _verticalSplitPaneStyles;
			}
		}

		public Dictionary<string, ScrollViewerStyle> ScrollViewerStyles
		{
			get
			{
				return _scrollViewerStyles;
			}
		}

		public Dictionary<string, MenuStyle> HorizontalMenuStyles
		{
			get
			{
				return _horizontalMenuStyles;
			}
		}

		public Dictionary<string, MenuStyle> VerticalMenuStyles
		{
			get
			{
				return _verticalMenuStyles;
			}
		}

		public Dictionary<string, WindowStyle> WindowStyles
		{
			get
			{
				return _windowStyles;
			}
		}

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


		public static Stylesheet LoadFromSource(
			string stylesheetXml,
			TextureRegionAtlas textureRegionAtlas,
			Dictionary<string, SpriteFontBase> fonts,
			MMLDiagnosticAction onDiagnostic)
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

					onDiagnostic?.Invoke(new MMLDiagnostic(MMLDiagnosticSeverity.Error, "Assets", "Brush", $"Could not find parse IBrush '{name}'"));
					return null;
				}
				else if (t == typeof(SpriteFontBase))
				{
					if (fonts.ContainsKey(name))
					{
						return fonts[name];
					}

					if (fonts == null || fonts.Count == 0)
                    {
						onDiagnostic?.Invoke(new MMLDiagnostic(MMLDiagnosticSeverity.Error, "Assets", "Font", "There are no fonts registered."));

					}
					else
                    {
						onDiagnostic?.Invoke(new MMLDiagnostic(MMLDiagnosticSeverity.Error, "Assets", "Font", $"Font '{name}' has not been registered."));
					}

					return null;
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

			loadContext.Load<object>(result, xDoc.Root, onDiagnostic, null);

			return result;
		}

		public static Stylesheet LoadFromSource(
			string stylesheetXml,
			TextureRegionAtlas textureRegionAtlas,
			Dictionary<string, SpriteFontBase> fonts)
		{
			return LoadFromSource(stylesheetXml, textureRegionAtlas, fonts, null);
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

		private void CloneStylesTo<T>(Stylesheet destStylesheet, Func<Stylesheet, Dictionary<string, T>> stylesGetter) where T: WidgetStyle
		{
			var src = stylesGetter(this);
			var dest = stylesGetter(destStylesheet);

			dest.Clear();
			foreach(var pair in src)
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

			foreach(var pair in Fonts)
			{
				result.Fonts[pair.Key] = pair.Value;
			}

			return result;
		}
	}
}