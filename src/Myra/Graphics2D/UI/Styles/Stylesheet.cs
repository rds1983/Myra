using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Linq;
using Myra.MML;
using System.Collections;
using Myra.Assets;

#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Xenko.Core.Mathematics;
using Xenko.Graphics;
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
		private readonly Dictionary<string, DialogStyle> _dialogStyles = new Dictionary<string, DialogStyle>();

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

		[XmlIgnore]
		public DialogStyle DialogStyle
		{
			get
			{
				return GetDefaultStyle(_dialogStyles);
			}
			set
			{
				SetDefaultStyle(_dialogStyles, value);
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

		public Dictionary<string, DialogStyle> DialogStyles
		{
			get
			{
				return _dialogStyles;
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
			styles.TryGetValue(DefaultStyleName, out result);
			return result;
		}

		private static void SetDefaultStyle<T>(Dictionary<string, T> styles, T value) where T : WidgetStyle
		{
			styles[DefaultStyleName] = value;
		}

		public static Stylesheet LoadFromSource(string stylesheetXml,
			Func<string, IBrush> textureGetter,
			Func<string, SpriteFont> fontGetter)
		{
			var xDoc = XDocument.Parse(stylesheetXml);

			var colors = new Dictionary<string, Color>();
			var colorsNode = xDoc.Root.Element("Colors");
			if (colorsNode != null)
			{
				foreach(var el in colorsNode.Elements())
				{
					var color = ColorStorage.FromName(el.Attribute("Value").Value);
					if (color != null)
					{
						colors[el.Attribute(BaseContext.IdName).Value] = color.Value;
					}
				}
			}

			Func<Type, string, object> resourceGetter = (t, s) =>
			{
				if (typeof(IBrush).IsAssignableFrom(t))
				{
					return textureGetter(s);
				} else if (t == typeof(SpriteFont))
				{
					return fontGetter(s);
				}

				throw new Exception(string.Format("Type {0} isn't supported", t.Name));
			};

			var result = new Stylesheet();

			var loadContext = new LoadContext
			{
				Namespace = typeof(WidgetStyle).Namespace,
				ResourceGetter = resourceGetter,
				NodesToIgnore = new HashSet<string>(new[] { "Designer", "Colors", "Fonts" }),
				LegacyClassNames = LegacyClassNames,
				LegacyPropertyNames = LegacyPropertyNames,
				Colors = colors
			};

			loadContext.Load(result, xDoc.Root);

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
			foreach(var k in dict.Keys)
			{
				result.Add((string)k);
			}

			return result.ToArray();
		}
	}
}