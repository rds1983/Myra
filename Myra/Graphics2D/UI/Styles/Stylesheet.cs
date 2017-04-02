using System;
using Myra.Graphics2D.Text;
using Newtonsoft.Json.Linq;

namespace Myra.Graphics2D.UI.Styles
{
	public class Stylesheet
	{
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

		public static Stylesheet CreateFromSource(string s, Func<string, BitmapFont> fontGetter, SpriteSheet spriteSheet)
		{
			var root = JObject.Parse(s);

			var loader = new StylesheetLoader(root, fontGetter, spriteSheet);
			return loader.Load();
		}
	}
}