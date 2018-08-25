using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Newtonsoft.Json.Linq;

namespace Myra.Graphics2D.UI.Styles
{
	public class Stylesheet
	{
		public static readonly string DefaultStyleName = string.Empty;

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

			set { _current = value; }
		}

		private readonly Dictionary<string, TextBlockStyle> _textBlockStyles = new Dictionary<string, TextBlockStyle>();
		private readonly Dictionary<string, TextFieldStyle> _textFieldStyles = new Dictionary<string, TextFieldStyle>();
		private readonly Dictionary<string, ButtonStyle> _buttonStyles = new Dictionary<string, ButtonStyle>();
		private readonly Dictionary<string, ButtonStyle> _checkBoxStyles = new Dictionary<string, ButtonStyle>();
		private readonly Dictionary<string, ImageButtonStyle> _imageButtonStyles = new Dictionary<string, ImageButtonStyle>();
		private readonly Dictionary<string, SpinButtonStyle> _spinButtonStyles = new Dictionary<string, SpinButtonStyle>();
		private readonly Dictionary<string, TextButtonStyle> _textButtonStyles = new Dictionary<string, TextButtonStyle>();
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
		private readonly Dictionary<string, GridStyle> _gridStyles = new Dictionary<string, GridStyle>();
		private readonly Dictionary<string, TreeStyle> _treeStyles = new Dictionary<string, TreeStyle>();

		private readonly Dictionary<string, SplitPaneStyle> _horizontalSplitPaneStyles =
			new Dictionary<string, SplitPaneStyle>();

		private readonly Dictionary<string, SplitPaneStyle> _verticalSplitPaneStyles =
			new Dictionary<string, SplitPaneStyle>();

		private readonly Dictionary<string, ScrollPaneStyle> _scrollPaneStyles = new Dictionary<string, ScrollPaneStyle>();
		private readonly Dictionary<string, MenuStyle> _horizontalMenuStyles = new Dictionary<string, MenuStyle>();
		private readonly Dictionary<string, MenuStyle> _verticalMenuStyles = new Dictionary<string, MenuStyle>();
		private readonly Dictionary<string, WindowStyle> _windowStyles = new Dictionary<string, WindowStyle>();

		public TextBlockStyle TextBlockStyle
		{
			get { return _textBlockStyles[DefaultStyleName]; }
			set { _textBlockStyles[DefaultStyleName] = value; }
		}

		public TextFieldStyle TextFieldStyle
		{
			get { return _textFieldStyles[DefaultStyleName]; }
			set { _textFieldStyles[DefaultStyleName] = value; }
		}

		public ButtonStyle ButtonStyle
		{
			get { return _buttonStyles[DefaultStyleName]; }
			set { _buttonStyles[DefaultStyleName] = value; }
		}

		public ButtonStyle CheckBoxStyle
		{
			get { return _checkBoxStyles[DefaultStyleName]; }
			set { _checkBoxStyles[DefaultStyleName] = value; }
		}

		public ImageButtonStyle ImageButtonStyle
		{
			get { return _imageButtonStyles[DefaultStyleName]; }
			set { _imageButtonStyles[DefaultStyleName] = value; }
		}

		public SpinButtonStyle SpinButtonStyle
		{
			get { return _spinButtonStyles[DefaultStyleName]; }
			set { _spinButtonStyles[DefaultStyleName] = value; }
		}


		public TextButtonStyle TextButtonStyle
		{
			get { return _textButtonStyles[DefaultStyleName]; }
			set { _textButtonStyles[DefaultStyleName] = value; }
		}

		public SliderStyle HorizontalSliderStyle
		{
			get { return _horizontalSliderStyles[DefaultStyleName]; }
			set { _horizontalSliderStyles[DefaultStyleName] = value; }
		}

		public SliderStyle VerticalSliderStyle
		{
			get { return _verticalSliderStyles[DefaultStyleName]; }
			set { _verticalSliderStyles[DefaultStyleName] = value; }
		}

		public ProgressBarStyle HorizontalProgressBarStyle
		{
			get { return _horizontalProgressBarStyles[DefaultStyleName]; }
			set { _horizontalProgressBarStyles[DefaultStyleName] = value; }
		}

		public ProgressBarStyle VerticalProgressBarStyle
		{
			get { return _verticalProgressBarStyles[DefaultStyleName]; }
			set { _verticalProgressBarStyles[DefaultStyleName] = value; }
		}

		public SeparatorStyle HorizontalSeparatorStyle
		{
			get { return _horizontalSeparatorStyles[DefaultStyleName]; }
			set { _horizontalSeparatorStyles[DefaultStyleName] = value; }
		}

		public SeparatorStyle VerticalSeparatorStyle
		{
			get { return _verticalSeparatorStyles[DefaultStyleName]; }
			set { _verticalSeparatorStyles[DefaultStyleName] = value; }
		}

		public ComboBoxStyle ComboBoxStyle
		{
			get { return _comboBoxStyles[DefaultStyleName]; }
			set { _comboBoxStyles[DefaultStyleName] = value; }
		}

		public ListBoxStyle ListBoxStyle
		{
			get { return _listBoxStyles[DefaultStyleName]; }
			set { _listBoxStyles[DefaultStyleName] = value; }
		}

		public GridStyle GridStyle
		{
			get { return _gridStyles[DefaultStyleName]; }
			set { _gridStyles[DefaultStyleName] = value; }
		}
		
		public TreeStyle TreeStyle
		{
			get { return _treeStyles[DefaultStyleName]; }
			set { _treeStyles[DefaultStyleName] = value; }
		}

		public SplitPaneStyle HorizontalSplitPaneStyle
		{
			get { return _horizontalSplitPaneStyles[DefaultStyleName]; }
			set { _horizontalSplitPaneStyles[DefaultStyleName] = value; }
		}

		public SplitPaneStyle VerticalSplitPaneStyle
		{
			get { return _verticalSplitPaneStyles[DefaultStyleName]; }
			set { _verticalSplitPaneStyles[DefaultStyleName] = value; }
		}

		public ScrollPaneStyle ScrollPaneStyle
		{
			get { return _scrollPaneStyles[DefaultStyleName]; }
			set { _scrollPaneStyles[DefaultStyleName] = value; }
		}

		public MenuStyle HorizontalMenuStyle
		{
			get { return _horizontalMenuStyles[DefaultStyleName]; }
			set { _horizontalMenuStyles[DefaultStyleName] = value; }
		}

		public MenuStyle VerticalMenuStyle
		{
			get { return _verticalMenuStyles[DefaultStyleName]; }
			set { _verticalMenuStyles[DefaultStyleName] = value; }
		}

		public WindowStyle WindowStyle
		{
			get { return _windowStyles[DefaultStyleName]; }
			set { _windowStyles[DefaultStyleName] = value; }
		}

		public Dictionary<string, TextBlockStyle> TextBlockStyles
		{
			get { return _textBlockStyles; }
		}

		public Dictionary<string, TextFieldStyle> TextFieldStyles
		{
			get { return _textFieldStyles; }
		}

		public Dictionary<string, ButtonStyle> ButtonStyles
		{
			get { return _buttonStyles; }
		}

		public Dictionary<string, ButtonStyle> CheckBoxStyles
		{
			get { return _checkBoxStyles; }
		}

		public Dictionary<string, ImageButtonStyle> ImageButtonStyles
		{
			get { return _imageButtonStyles; }
		}

		public Dictionary<string, SpinButtonStyle> SpinButtonStyles
		{
			get { return _spinButtonStyles; }
		}

		public Dictionary<string, TextButtonStyle> TextButtonStyles
		{
			get { return _textButtonStyles; }
		}

		public Dictionary<string, SliderStyle> HorizontalSliderStyles
		{
			get { return _horizontalSliderStyles; }
		}

		public Dictionary<string, SliderStyle> VerticalSliderStyles
		{
			get { return _verticalSliderStyles; }
		}

		public Dictionary<string, ProgressBarStyle> HorizontalProgressBarStyles
		{
			get { return _horizontalProgressBarStyles; }
		}

		public Dictionary<string, ProgressBarStyle> VerticalProgressBarStyles
		{
			get { return _verticalProgressBarStyles; }
		}

		public Dictionary<string, SeparatorStyle> HorizontalSeparatorStyles
		{
			get { return _horizontalSeparatorStyles; }
		}

		public Dictionary<string, SeparatorStyle> VerticalSeparatorStyles
		{
			get { return _verticalSeparatorStyles; }
		}

		public Dictionary<string, ComboBoxStyle> ComboBoxStyles
		{
			get { return _comboBoxStyles; }
		}

		public Dictionary<string, ListBoxStyle> ListBoxStyles
		{
			get { return _listBoxStyles; }
		}

		public Dictionary<string, TreeStyle> TreeStyles
		{
			get { return _treeStyles; }
		}

		public Dictionary<string, GridStyle> GridStyles
		{
			get { return _gridStyles; }
		}

		public Dictionary<string, SplitPaneStyle> HorizontalSplitPaneStyles
		{
			get { return _horizontalSplitPaneStyles; }
		}

		public Dictionary<string, SplitPaneStyle> VerticalSplitPaneStyles
		{
			get { return _verticalSplitPaneStyles; }
		}

		public Dictionary<string, ScrollPaneStyle> ScrollPaneStyles
		{
			get { return _scrollPaneStyles; }
		}

		public Dictionary<string, MenuStyle> HorizontalMenuStyles
		{
			get { return _horizontalMenuStyles; }
		}

		public Dictionary<string, MenuStyle> VerticalMenuStyles
		{
			get { return _verticalMenuStyles; }
		}

		public Dictionary<string, WindowStyle> WindowStyles
		{
			get { return _windowStyles; }
		}

		public Stylesheet()
		{
			TextBlockStyle = new TextBlockStyle();
			TextFieldStyle = new TextFieldStyle();
			ButtonStyle = new ButtonStyle();
			CheckBoxStyle = new ButtonStyle();
			ImageButtonStyle = new ImageButtonStyle();
			SpinButtonStyle = new SpinButtonStyle();
			TextButtonStyle = new TextButtonStyle();
			HorizontalSliderStyle = new SliderStyle();
			VerticalSliderStyle = new SliderStyle();
			HorizontalProgressBarStyle = new ProgressBarStyle();
			VerticalProgressBarStyle = new ProgressBarStyle();
			HorizontalSeparatorStyle = new SeparatorStyle();
			VerticalSeparatorStyle = new SeparatorStyle();
			ComboBoxStyle = new ComboBoxStyle();
			ListBoxStyle = new ListBoxStyle();
			GridStyle = new GridStyle();
			TreeStyle = new TreeStyle();
			HorizontalSplitPaneStyle = new SplitPaneStyle();
			VerticalSplitPaneStyle = new SplitPaneStyle();
			ScrollPaneStyle = new ScrollPaneStyle();
			HorizontalMenuStyle = new MenuStyle();
			VerticalMenuStyle = new MenuStyle();
			WindowStyle = new WindowStyle();
		}

		public static Stylesheet CreateFromSource(string s,
			Func<string, TextureRegion> textureGetter,
			Func<string, SpriteFont> fontGetter)
		{
			var root = JObject.Parse(s);

			var loader = new StylesheetLoader(root, textureGetter, fontGetter);
			return loader.Load();
		}
	}
}