using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.ComponentModel;
using Myra.MML;
using Myra.Graphics2D.TextureAtlases;
using Myra.Attributes;

namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Manages a collection of UI styles used throughout the application.
	/// Provides access to default styles and named style variants for all UI widgets.
	/// </summary>
	public class Stylesheet
	{
		internal static readonly Dictionary<string, string> LegacyClassNames = new Dictionary<string, string>();
		internal static readonly Dictionary<string, string> LegacyPropertyNames = new Dictionary<string, string>();

		/// <summary>
		/// The default style identifier used when no specific style name is provided.
		/// </summary>
		public const string DefaultStyleName = "";

		internal static Stylesheet _current;

		/// <summary>
		/// Gets or sets the current active stylesheet used globally.
		/// If not explicitly set, returns the default stylesheet from DefaultAssets.
		/// </summary>
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

		private TextureRegion _whiteRegion;

		/// <summary>
		/// Gets the texture atlas containing all texture regions used in the stylesheet.
		/// Skip load, since it is loaded manually
		/// </summary>
		[XmlName("TextureRegionAtlas")]
		[SkipLoad]
		public TextureRegionAtlas Atlas { get; internal set; }

		/// <summary>
		/// Gets a white texture region from the atlas, used for solid color rendering.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public TextureRegion WhiteRegion
		{
			get
			{
				if (Atlas == null)
				{
					// Since we switch atlases sometimes
					// In async context, Atlas could be null at the moment of access, so we need to check it every time
					return DefaultAssets.WhiteRegion;
				}

				if (_whiteRegion == null)
				{
					if (!Atlas.Regions.TryGetValue("white", out _whiteRegion))
					{
						_whiteRegion = DefaultAssets.WhiteRegion;
					}
				}

				return _whiteRegion;
			}
		}

		/// <summary>
		/// Gets the collection of fonts used in the stylesheet.
		/// </summary>
		[SkipLoad]
		public StylesheetFontsCollection Fonts { get; } = new StylesheetFontsCollection();

		/// <summary>
		/// Gets or sets the style applied to the desktop background.
		/// </summary>
		public DesktopStyle DesktopStyle { get; set; }

		/// <summary>
		/// Gets or sets the default style for label widgets.
		/// </summary>
		[XmlIgnore]
		public LabelStyle LabelStyle
		{
			get => GetDefaultStyle(LabelStyles);
			set => SetDefaultStyle(LabelStyles, value);
		}

		/// <summary>
		/// Gets or sets the default style for tooltip labels.
		/// </summary>
		[XmlIgnore]
		public LabelStyle TooltipStyle
		{
			get => GetDefaultStyle(TooltipStyles);
			set => SetDefaultStyle(TooltipStyles, value);
		}

		/// <summary>
		/// Gets or sets the default style for text box widgets.
		/// </summary>
		[XmlIgnore]
		public TextBoxStyle TextBoxStyle
		{
			get => GetDefaultStyle(TextBoxStyles);
			set => SetDefaultStyle(TextBoxStyles, value);
		}

		/// <summary>
		/// Gets or sets the default style for button widgets.
		/// </summary>
		[XmlIgnore]
		public ButtonStyle ButtonStyle
		{
			get => GetDefaultStyle(ButtonStyles);
			set => SetDefaultStyle(ButtonStyles, value);
		}

		/// <summary>
		/// Gets or sets the default style for checkbox widgets.
		/// </summary>
		[XmlIgnore]
		public CheckButtonStyle CheckBoxStyle
		{
			get => GetDefaultStyle(CheckBoxStyles);
			set => SetDefaultStyle(CheckBoxStyles, value);
		}

		/// <summary>
		/// Gets or sets the default style for radio button widgets.
		/// </summary>
		[XmlIgnore]
		public CheckButtonStyle RadioButtonStyle
		{
			get => GetDefaultStyle(RadioButtonStyles);
			set => SetDefaultStyle(RadioButtonStyles, value);
		}

		/// <summary>
		/// Gets or sets the default style for spin button widgets.
		/// </summary>
		[XmlIgnore]
		public SpinButtonStyle SpinButtonStyle
		{
			get => GetDefaultStyle(SpinButtonStyles);
			set => SetDefaultStyle(SpinButtonStyles, value);
		}

		/// <summary>
		/// Gets or sets the default style for horizontal slider widgets.
		/// </summary>
		[XmlIgnore]
		public SliderStyle HorizontalSliderStyle
		{
			get => GetDefaultStyle(HorizontalSliderStyles);
			set => SetDefaultStyle(HorizontalSliderStyles, value);
		}

		/// <summary>
		/// Gets or sets the default style for vertical slider widgets.
		/// </summary>
		[XmlIgnore]
		public SliderStyle VerticalSliderStyle
		{
			get => GetDefaultStyle(VerticalSliderStyles);
			set => SetDefaultStyle(VerticalSliderStyles, value);
		}

		/// <summary>
		/// Gets or sets the default style for horizontal progress bar widgets.
		/// </summary>
		[XmlIgnore]
		public ProgressBarStyle HorizontalProgressBarStyle
		{
			get => GetDefaultStyle(HorizontalProgressBarStyles);
			set => SetDefaultStyle(HorizontalProgressBarStyles, value);
		}

		/// <summary>
		/// Gets or sets the default style for vertical progress bar widgets.
		/// </summary>
		[XmlIgnore]
		public ProgressBarStyle VerticalProgressBarStyle
		{
			get => GetDefaultStyle(VerticalProgressBarStyles);
			set => SetDefaultStyle(VerticalProgressBarStyles, value);
		}

		/// <summary>
		/// Gets or sets the default style for horizontal separator widgets.
		/// </summary>
		[XmlIgnore]
		public SeparatorStyle HorizontalSeparatorStyle
		{
			get => GetDefaultStyle(HorizontalSeparatorStyles);
			set => SetDefaultStyle(HorizontalSeparatorStyles, value);
		}

		/// <summary>
		/// Gets or sets the default style for vertical separator widgets.
		/// </summary>
		[XmlIgnore]
		public SeparatorStyle VerticalSeparatorStyle
		{
			get => GetDefaultStyle(VerticalSeparatorStyles);
			set => SetDefaultStyle(VerticalSeparatorStyles, value);
		}

		/// <summary>
		/// Gets or sets the default style for combo box widgets.
		/// </summary>
		[XmlIgnore]
		public ComboBoxStyle ComboBoxStyle
		{
			get => GetDefaultStyle(ComboBoxStyles);
			set => SetDefaultStyle(ComboBoxStyles, value);
		}

		/// <summary>
		/// Gets or sets the default style for list box widgets.
		/// </summary>
		[XmlIgnore]
		public ListBoxStyle ListBoxStyle
		{
			get => GetDefaultStyle(ListBoxStyles);
			set => SetDefaultStyle(ListBoxStyles, value);
		}

		/// <summary>
		/// Gets or sets the default style for tab control widgets.
		/// </summary>
		[XmlIgnore]
		public TabControlStyle TabControlStyle
		{
			get => GetDefaultStyle(TabControlStyles);
			set => SetDefaultStyle(TabControlStyles, value);
		}

		/// <summary>
		/// Gets or sets the default style for tree widgets.
		/// </summary>
		[XmlIgnore]
		public TreeStyle TreeStyle
		{
			get => GetDefaultStyle(TreeStyles);
			set => SetDefaultStyle(TreeStyles, value);
		}

		/// <summary>
		/// Gets or sets the default style for horizontal split pane widgets.
		/// </summary>
		[XmlIgnore]
		public SplitPaneStyle HorizontalSplitPaneStyle
		{
			get => GetDefaultStyle(HorizontalSplitPaneStyles);
			set => SetDefaultStyle(HorizontalSplitPaneStyles, value);
		}

		/// <summary>
		/// Gets or sets the default style for vertical split pane widgets.
		/// </summary>
		[XmlIgnore]
		public SplitPaneStyle VerticalSplitPaneStyle
		{
			get => GetDefaultStyle(VerticalSplitPaneStyles);
			set => SetDefaultStyle(VerticalSplitPaneStyles, value);
		}

		/// <summary>
		/// Gets or sets the default style for scroll viewer widgets.
		/// </summary>
		[XmlIgnore]
		public ScrollViewerStyle ScrollViewerStyle
		{
			get => GetDefaultStyle(ScrollViewerStyles);
			set => SetDefaultStyle(ScrollViewerStyles, value);
		}

		/// <summary>
		/// Gets or sets the default style applied to DataGrid widgets.
		/// </summary>
		[XmlIgnore]
		public DataGridStyle DataGridStyle
		{
			get => GetDefaultStyle(DataGridStyles);
			set => SetDefaultStyle(DataGridStyles, value);
		}

		/// <summary>
		/// Gets or sets the default style for horizontal menu widgets.
		/// </summary>
		[XmlIgnore]
		public MenuStyle HorizontalMenuStyle
		{
			get => GetDefaultStyle(HorizontalMenuStyles);
			set => SetDefaultStyle(HorizontalMenuStyles, value);
		}

		/// <summary>
		/// Gets or sets the default style for vertical menu widgets.
		/// </summary>
		[XmlIgnore]
		public MenuStyle VerticalMenuStyle
		{
			get => GetDefaultStyle(VerticalMenuStyles);
			set => SetDefaultStyle(VerticalMenuStyles, value);
		}

		/// <summary>
		/// Gets or sets the default style for window widgets.
		/// </summary>
		[XmlIgnore]
		public WindowStyle WindowStyle
		{
			get => GetDefaultStyle(WindowStyles);
			set => SetDefaultStyle(WindowStyles, value);
		}

		/// <summary>
		/// Gets or sets the default style for file dialog widgets.
		/// </summary>
		[XmlIgnore]
		public FileDialogStyle FileDialogStyle
		{
			get => GetDefaultStyle(FileDialogStyles);
			set => SetDefaultStyle(FileDialogStyles, value);
		}

		/// <summary>
		/// Gets or sets the default style for color picker dialog widgets.
		/// </summary>
		[XmlIgnore]
		public ColorPickerDialogStyle ColorPickerDialogStyle
		{
			get => GetDefaultStyle(ColorPickerDialogStyles);
			set => SetDefaultStyle(ColorPickerDialogStyles, value);
		}

		/// <summary>
		/// Gets or sets the default style for panel widgets.
		/// </summary>
		[XmlIgnore]
		public WidgetStyle PanelStyle
		{
			get => GetDefaultStyle(PanelStyles);
			set => SetDefaultStyle(PanelStyles, value);
		}

		/// <summary>
		/// Gets or sets the default style for horizontal stack panels.
		/// </summary>
		[XmlIgnore]
		public WidgetStyle HorizontalStackPanelStyle
		{
			get => GetDefaultStyle(HorizontalStackPanelStyles);

			set => SetDefaultStyle(HorizontalStackPanelStyles, value);
		}

		/// <summary>
		/// Gets or sets the default style for vertical stack panels.
		/// </summary>
		[XmlIgnore]
		public WidgetStyle VerticalStackPanelStyle
		{
			get => GetDefaultStyle(VerticalStackPanelStyles);

			set => SetDefaultStyle(VerticalStackPanelStyles, value);
		}

		/// <summary>
		/// Gets the dictionary of named label styles, keyed by style identifier.
		/// </summary>
		public Dictionary<string, LabelStyle> LabelStyles { get; } = new Dictionary<string, LabelStyle>();

		/// <summary>
		/// Gets the dictionary of named tooltip label styles, keyed by style identifier.
		/// </summary>
		public Dictionary<string, LabelStyle> TooltipStyles { get; } = new Dictionary<string, LabelStyle>();

		/// <summary>
		/// Gets the dictionary of named text box styles, keyed by style identifier.
		/// </summary>
		public Dictionary<string, TextBoxStyle> TextBoxStyles { get; } = new Dictionary<string, TextBoxStyle>();

		/// <summary>
		/// Gets the dictionary of named button styles, keyed by style identifier.
		/// </summary>
		public Dictionary<string, ButtonStyle> ButtonStyles { get; } = new Dictionary<string, ButtonStyle>();

		/// <summary>
		/// Gets the dictionary of named checkbox styles, keyed by style identifier.
		/// </summary>
		public Dictionary<string, CheckButtonStyle> CheckBoxStyles { get; } = new Dictionary<string, CheckButtonStyle>();

		/// <summary>
		/// Gets the dictionary of named radio button styles, keyed by style identifier.
		/// </summary>
		public Dictionary<string, CheckButtonStyle> RadioButtonStyles { get; } = new Dictionary<string, CheckButtonStyle>();

		/// <summary>
		/// Gets the dictionary of named spin button styles, keyed by style identifier.
		/// </summary>
		public Dictionary<string, SpinButtonStyle> SpinButtonStyles { get; } = new Dictionary<string, SpinButtonStyle>();

		/// <summary>
		/// Gets the dictionary of named horizontal slider styles, keyed by style identifier.
		/// </summary>
		public Dictionary<string, SliderStyle> HorizontalSliderStyles { get; } = new Dictionary<string, SliderStyle>();

		/// <summary>
		/// Gets the dictionary of named vertical slider styles, keyed by style identifier.
		/// </summary>
		public Dictionary<string, SliderStyle> VerticalSliderStyles { get; } = new Dictionary<string, SliderStyle>();

		/// <summary>
		/// Gets the dictionary of named horizontal progress bar styles, keyed by style identifier.
		/// </summary>
		public Dictionary<string, ProgressBarStyle> HorizontalProgressBarStyles { get; } = new Dictionary<string, ProgressBarStyle>();

		/// <summary>
		/// Gets the dictionary of named vertical progress bar styles, keyed by style identifier.
		/// </summary>
		public Dictionary<string, ProgressBarStyle> VerticalProgressBarStyles { get; } = new Dictionary<string, ProgressBarStyle>();

		/// <summary>
		/// Gets the dictionary of named horizontal separator styles, keyed by style identifier.
		/// </summary>
		public Dictionary<string, SeparatorStyle> HorizontalSeparatorStyles { get; } = new Dictionary<string, SeparatorStyle>();

		/// <summary>
		/// Gets the dictionary of named vertical separator styles, keyed by style identifier.
		/// </summary>
		public Dictionary<string, SeparatorStyle> VerticalSeparatorStyles { get; } = new Dictionary<string, SeparatorStyle>();

		/// <summary>
		/// Gets the dictionary of named combo box styles, keyed by style identifier.
		/// </summary>
		public Dictionary<string, ComboBoxStyle> ComboBoxStyles { get; } = new Dictionary<string, ComboBoxStyle>();

		/// <summary>
		/// Gets the dictionary of named list box styles, keyed by style identifier.
		/// </summary>
		public Dictionary<string, ListBoxStyle> ListBoxStyles { get; } = new Dictionary<string, ListBoxStyle>();

		/// <summary>
		/// Gets the dictionary of named tab control styles, keyed by style identifier.
		/// </summary>
		public Dictionary<string, TabControlStyle> TabControlStyles { get; } = new Dictionary<string, TabControlStyle>();

		/// <summary>
		/// Gets the dictionary of named tree styles, keyed by style identifier.
		/// </summary>
		public Dictionary<string, TreeStyle> TreeStyles { get; } = new Dictionary<string, TreeStyle>();

		/// <summary>
		/// Gets the dictionary of named horizontal split pane styles, keyed by style identifier.
		/// </summary>
		public Dictionary<string, SplitPaneStyle> HorizontalSplitPaneStyles { get; } = new Dictionary<string, SplitPaneStyle>();

		/// <summary>
		/// Gets the dictionary of named vertical split pane styles, keyed by style identifier.
		/// </summary>
		public Dictionary<string, SplitPaneStyle> VerticalSplitPaneStyles { get; } = new Dictionary<string, SplitPaneStyle>();

		/// <summary>
		/// Gets the dictionary of named scroll viewer styles, keyed by style identifier.
		/// </summary>
		public Dictionary<string, ScrollViewerStyle> ScrollViewerStyles { get; } = new Dictionary<string, ScrollViewerStyle>();

		/// <summary>
		/// Gets the dictionary of named DataGrid styles, keyed by style identifier.
		/// </summary>
		public Dictionary<string, DataGridStyle> DataGridStyles { get; } = new Dictionary<string, DataGridStyle>();

		/// <summary>
		/// Gets the dictionary of named horizontal menu styles, keyed by style identifier.
		/// </summary>
		public Dictionary<string, MenuStyle> HorizontalMenuStyles { get; } = new Dictionary<string, MenuStyle>();

		/// <summary>
		/// Gets the dictionary of named vertical menu styles, keyed by style identifier.
		/// </summary>
		public Dictionary<string, MenuStyle> VerticalMenuStyles { get; } = new Dictionary<string, MenuStyle>();

		/// <summary>
		/// Gets the dictionary of named window styles, keyed by style identifier.
		/// </summary>
		public Dictionary<string, WindowStyle> WindowStyles { get; } = new Dictionary<string, WindowStyle>();

		/// <summary>
		/// Gets the dictionary of named file dialog styles, keyed by style identifier.
		/// </summary>
		public Dictionary<string, FileDialogStyle> FileDialogStyles { get; } = new Dictionary<string, FileDialogStyle>();

		/// <summary>
		/// Gets the dictionary of named color picker dialog styles, keyed by style identifier.
		/// </summary>
		public Dictionary<string, ColorPickerDialogStyle> ColorPickerDialogStyles { get; } = new Dictionary<string, ColorPickerDialogStyle>();

		/// <summary>
		/// Gets the dictionary of named grid styles, keyed by style identifier.
		/// </summary>
		public Dictionary<string, GridStyle> GridStyles { get; } = new Dictionary<string, GridStyle>();

		/// <summary>
		/// Gets the dictionary of named panel styles, keyed by style identifier.
		/// </summary>
		public Dictionary<string, WidgetStyle> PanelStyles { get; } = new Dictionary<string, WidgetStyle>();

		/// <summary>
		/// Gets the dictionary of named horizontal stack panel styles, keyed by style identifier.
		/// </summary>
		public Dictionary<string, WidgetStyle> HorizontalStackPanelStyles { get; } = new Dictionary<string, WidgetStyle>();

		/// <summary>
		/// Gets the dictionary of named vertical stack panel styles, keyed by style identifier.
		/// </summary>
		public Dictionary<string, WidgetStyle> VerticalStackPanelStyles { get; } = new Dictionary<string, WidgetStyle>();

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

		/// <summary>
		/// Converts the stylesheet to an XML string representation.
		/// </summary>
		/// <returns>An XML string containing all styles defined in this stylesheet.</returns>
		public string ToXml()
		{
			var saveContext = new SaveContext();
			saveContext.PrependNamespace = false;

			var root = saveContext.Save(this);

			var xDoc = new XDocument(root);

			return xDoc.ToString();
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

		/// <summary>
		/// Creates a deep copy of this stylesheet including all styles and fonts.
		/// </summary>
		/// <returns>A new Stylesheet instance with cloned styles and fonts.</returns>
		public Stylesheet Clone()
		{
			var result = new Stylesheet
			{
				Atlas = Atlas,
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
			CloneStylesTo(result, s => s.DataGridStyles);
			CloneStylesTo(result, s => s.GridStyles);
			CloneStylesTo(result, s => s.PanelStyles);
			CloneStylesTo(result, s => s.WindowStyles);

			if (Fonts != null)
			{
				foreach (var font in Fonts)
				{
					result.Fonts[font.Id] = font.Clone();
				}
			}

			return result;
		}
	}
}