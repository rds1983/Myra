using System;
using System.Collections.Generic;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json.Linq;

namespace Myra.Graphics2D.UI.Styles
{
	public class Stylesheet
	{
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

		private readonly Dictionary<string, TextBlockStyle> _textBlockVariants = new Dictionary<string,TextBlockStyle>();
		private readonly Dictionary<string, TextFieldStyle> _textFieldVariants = new Dictionary<string, TextFieldStyle>();
		private readonly Dictionary<string, ButtonStyle> _buttonVariants = new Dictionary<string, ButtonStyle>();
		private readonly Dictionary<string, ButtonStyle> _checkBoxVariants = new Dictionary<string, ButtonStyle>();
		private readonly Dictionary<string, ImageButtonStyle> _imageButtonVariants = new Dictionary<string, ImageButtonStyle>();
		private readonly Dictionary<string, SpinButtonStyle> _spinButtonVariants = new Dictionary<string, SpinButtonStyle>();
		private readonly Dictionary<string, SliderStyle> _horizontalSliderVariants = new Dictionary<string, SliderStyle>();
		private readonly Dictionary<string, SliderStyle> _verticalSliderVariants = new Dictionary<string, SliderStyle>();
		private readonly Dictionary<string, ProgressBarStyle> _horizontalProgressBarVariants = new Dictionary<string, ProgressBarStyle>();
		private readonly Dictionary<string, ProgressBarStyle> _verticalProgressBarVariants = new Dictionary<string, ProgressBarStyle>();
		private readonly Dictionary<string, ComboBoxStyle> _comboBoxVariants = new Dictionary<string, ComboBoxStyle>();
		private readonly Dictionary<string, ListBoxStyle> _listBoxVariants = new Dictionary<string, ListBoxStyle>();
		private readonly Dictionary<string, TreeStyle> _treeVariants = new Dictionary<string, TreeStyle>();
		private readonly Dictionary<string, SplitPaneStyle> _horizontalSplitPaneVariants = new Dictionary<string, SplitPaneStyle>();
		private readonly Dictionary<string, SplitPaneStyle> _verticalSplitPaneVariants = new Dictionary<string, SplitPaneStyle>();
		private readonly Dictionary<string, ScrollAreaStyle> _scrollAreaVariants = new Dictionary<string, ScrollAreaStyle>();
		private readonly Dictionary<string, MenuStyle> _horizontalMenuVariants = new Dictionary<string, MenuStyle>();
		private readonly Dictionary<string, MenuStyle> _verticalMenuVariants = new Dictionary<string, MenuStyle>();
		private readonly Dictionary<string, WindowStyle> _windowVariants = new Dictionary<string, WindowStyle>();

		public TextBlockStyle TextBlockStyle { get; private set; }
		public TextFieldStyle TextFieldStyle { get; private set; }
		public ButtonStyle ButtonStyle { get; private set; }
		public ButtonStyle CheckBoxStyle { get; private set; }
		public ImageButtonStyle ImageButtonStyle { get; private set; }
		public SpinButtonStyle SpinButtonStyle { get; private set; }
		public SliderStyle HorizontalSliderStyle { get; private set; }
		public SliderStyle VerticalSliderStyle { get; private set; }
		public ProgressBarStyle HorizontalProgressBarStyle { get; private set; }
		public ProgressBarStyle VerticalProgressBarStyle { get; private set; }
		public ComboBoxStyle ComboBoxStyle { get; private set; }
		public ListBoxStyle ListBoxStyle { get; set; }
		public TreeStyle TreeStyle { get; private set; }
		public SplitPaneStyle HorizontalSplitPaneStyle { get; private set; }
		public SplitPaneStyle VerticalSplitPaneStyle { get; private set; }
		public ScrollAreaStyle ScrollAreaStyle { get; private set; }
		public MenuStyle HorizontalMenuStyle { get; private set; }
		public MenuStyle VerticalMenuStyle { get; private set; }
		public WindowStyle WindowStyle { get; private set; }

		public Dictionary<string, TextBlockStyle> TextBlockVariants
		{
			get { return _textBlockVariants; }
		}

		public Dictionary<string, TextFieldStyle> TextFieldVariants
		{
			get { return _textFieldVariants; }
		}

		public Dictionary<string, ButtonStyle> ButtonVariants
		{
			get { return _buttonVariants; }
		}

		public Dictionary<string, ButtonStyle> CheckBoxVariants
		{
			get { return _checkBoxVariants; }
		}

		public Dictionary<string, ImageButtonStyle> ImageButtonVariants
		{
			get { return _imageButtonVariants; }
		}

		public Dictionary<string, SpinButtonStyle> SpinButtonVariants
		{
			get { return _spinButtonVariants; }
		}

		public Dictionary<string, SliderStyle> HorizontalSliderVariants
		{
			get { return _horizontalSliderVariants; }
		}

		public Dictionary<string, SliderStyle> VerticalSliderVariants
		{
			get { return _verticalSliderVariants; }
		}

		public Dictionary<string, ProgressBarStyle> HorizontalProgressBarVariants
		{
			get { return _horizontalProgressBarVariants; }
		}

		public Dictionary<string, ProgressBarStyle> VerticalProgressBarVariants
		{
			get { return _verticalProgressBarVariants; }
		}

		public Dictionary<string, ComboBoxStyle> ComboBoxVariants
		{
			get { return _comboBoxVariants; }
		}

		public Dictionary<string, ListBoxStyle> ListBoxVariants
		{
			get { return _listBoxVariants; }
		}

		public Dictionary<string, TreeStyle> TreeVariants
		{
			get { return _treeVariants; }
		}

		public Dictionary<string, SplitPaneStyle> HorizontalSplitPaneVariants
		{
			get { return _horizontalSplitPaneVariants; }
		}

		public Dictionary<string, SplitPaneStyle> VerticalSplitPaneVariants
		{
			get { return _verticalSplitPaneVariants; }
		}

		public Dictionary<string, ScrollAreaStyle> ScrollAreaVariants
		{
			get { return _scrollAreaVariants; }
		}

		public Dictionary<string, MenuStyle> HorizontalMenuVariants
		{
			get { return _horizontalMenuVariants; }
		}

		public Dictionary<string, MenuStyle> VerticalMenuVariants
		{
			get { return _verticalMenuVariants; }
		}

		public Dictionary<string, WindowStyle> WindowVariants
		{
			get { return _windowVariants; }
		}

		public Stylesheet()
		{
			TextBlockStyle = new TextBlockStyle();
			TextFieldStyle = new TextFieldStyle();
			ButtonStyle = new ButtonStyle();
			CheckBoxStyle = new ButtonStyle();
			ImageButtonStyle = new ImageButtonStyle();
			SpinButtonStyle = new SpinButtonStyle();
			HorizontalSliderStyle = new SliderStyle();
			VerticalSliderStyle = new SliderStyle();
			HorizontalProgressBarStyle = new ProgressBarStyle();
			VerticalProgressBarStyle = new ProgressBarStyle();
			ComboBoxStyle = new ComboBoxStyle();
			ListBoxStyle = new ListBoxStyle();
			TreeStyle = new TreeStyle();
			HorizontalSplitPaneStyle = new SplitPaneStyle();
			VerticalSplitPaneStyle = new SplitPaneStyle();
			ScrollAreaStyle = new ScrollAreaStyle();
			HorizontalMenuStyle = new MenuStyle();
			VerticalMenuStyle = new MenuStyle();
			WindowStyle = new WindowStyle();
		}

		public static Stylesheet CreateFromSource(string s,
			Func<string, TextureRegion2D> textureGetter,
			Func<string, BitmapFont> fontGetter)
		{
			var root = JObject.Parse(s);

			var loader = new StylesheetLoader(root, textureGetter, fontGetter);
			return loader.Load();
		}
		
	}
}