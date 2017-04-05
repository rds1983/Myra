using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.TextureAtlases;
using Myra.Utility;
using Newtonsoft.Json.Linq;

namespace Myra.Graphics2D.UI.Styles
{
	internal class StylesheetLoader
	{
		public const string ColorsName = "colors";
		public const string TextBlockName = "textBlock";
		public const string TextFieldName = "textField";
		public const string ScrollAreaName = "scrollArea";
		public const string ButtonName = "button";
		public const string CheckBoxName = "checkBox";
		public const string ImageButtonName = "imageButton";
		public const string SpinButtonName = "spinButton";
		public const string ComboBoxName = "comboBox";
		public const string ComboBoxItemName = "comboBoxItem";
		public const string ListBoxName = "listBox";
		public const string ListBoxItemName = "listBoxItem";
		public const string SelectedBackgroundName = "selectedBackground";
		public const string TreeName = "tree";
		public const string SplitPaneName = "splitPane";
		public const string HorizontalSplitPaneName = "horizontalSplitPane";
		public const string VerticalSplitPaneName = "verticalSplitPane";
		public const string HorizontalMenuName = "horizontalMenu";
		public const string VerticalMenuName = "verticalMenu";
		public const string WindowName = "window";
		public const string LeftName = "left";
		public const string RightName = "right";
		public const string TopName = "top";
		public const string BottomName = "bottom";
		public const string WidthHintName = "widthHint";
		public const string HeightHintName = "heightHint";
		public const string BackgroundName = "background";
		public const string OverBackgroundName = "overBackground";
		public const string DisabledBackgroundName = "disabledBackground";
		public const string FocusedBackgroundName = "disabledBackground";
		public const string PressedBackgroundName = "pressedBackground";
		public const string OpenBackgroundName = "openBackground";
		public const string BorderName = "border";
		public const string OverBorderName = "overBorder";
		public const string DisabledBorderName = "disabledBorder";
		public const string PaddingName = "padding";
		public const string FontName = "font";
		public const string MessageFontName = "font";
		public const string TextColorName = "textColor";
		public const string DisabledTextColorName = "disabledTextColor";
		public const string FocusedTextColorName = "focusedTextColor";
		public const string MessageTextColorName = "messageTextColor";
		public const string HorizontalScrollName = "horizontalScroll";
		public const string HorizontalScrollKnobName = "horizontalScrollKnob";
		public const string VerticalScrollName = "verticalScroll";
		public const string VerticalScrollKnobName = "verticalScrollKnob";
		public const string SelectionName = "selection";
		public const string SplitHorizontalHandleName = "horizontalHandle";
		public const string SplitHorizontalHandleOverName = "horizontalHandleOver";
		public const string SplitVerticalHandleName = "verticalHandle";
		public const string SplitVerticalHandleOverName = "verticalHandleOver";
		public const string TitleStyleName = "title";
		public const string CloseButtonStyleName = "closeButton";
		public const string UpButtonStyleName = "upButton";
		public const string DownButtonStyleName = "downButton";
		public const string CursorName = "cursor";
		public const string IconWidthName = "iconWidth";
		public const string ShortcutWidthName = "shortcutWidth";
		public const string SpacingName = "spacing";
		public const string LabelStyleName = "label";
		public const string ImageStyleName = "image";
		public const string TextFieldStyleName = "textField";
		public const string MenuItemName = "menuItem";
		public const string HandleName = "handle";
		public const string MarkName = "mark";
		public const string ImageName = "image";
		public const string OverImageName = "overImage";
		public const string PressedImageName = "pressedImage";
		public const string RowSelectionBackgroundName = "rowSelectionBackground";
		public const string RowSelectionBackgroundWithoutFocusName = "rowSelectionBackgroundWithoutFocus";
		public const string MenuSeparatorName = "separator";
		public const string ThicknessName = "thickness";
		public const string ItemsContainerName = "itemsContainer";
		public const string HorizontalSliderName = "horizontalSlider";
		public const string VerticalSliderName = "verticalSlider";
		public const string KnobName = "knob";
		public const string HorizontalProgressBarName = "horizontalProgressBar";
		public const string VerticalProgressBarName = "verticalProgressBar";
		public const string FilledName = "filled";

		private readonly Dictionary<string, Color> _colors = new Dictionary<string, Color>();
		private readonly JObject _root;
		private readonly Func<string, BitmapFont> _fontGetter;
		private readonly TextureAtlas _textureAtlas;

		public StylesheetLoader(JObject root,
			Func<string, BitmapFont> fontGetter,
			TextureAtlas textureAtlas)
		{
			if (root == null)
			{
				throw new ArgumentNullException("root");
			}

			if (textureAtlas == null)
			{
				throw new ArgumentNullException("textureAtlas");
			}

			_root = root;
			_fontGetter = fontGetter;
			_textureAtlas = textureAtlas;
		}

		private TextureRegion2D GetTextureRegion2D(string id)
		{
			return _textureAtlas.GetRegion(id);
		}

		private BitmapFont GetFont(string id)
		{
			if (_fontGetter == null || string.IsNullOrEmpty(id))
			{
				return null;
			}

			return _fontGetter(id);
		}

		private void ParseColors(JObject colors)
		{
			_colors.Clear();

			foreach (var props in colors.Properties())
			{
				// Parse it
				var stringValue = props.Value.ToString();
				var parsedColor = stringValue.FromName();
				if (parsedColor == null)
				{
					throw new Exception(string.Format("Could not parse color '{0}'", stringValue));
				}

				_colors.Add(props.Name, parsedColor.Value);
			}
		}

		private Color GetColor(string color)
		{
			Color result;
			if (_colors.TryGetValue(color, out result))
			{
				return result;
			}

			// Not found
			throw new Exception(string.Format("Unknown color '{0}'", color));
		}

		private static PaddingInfo GetPaddingInfo(JObject padding)
		{
			var result = new PaddingInfo();
			int value;
			if (padding.GetStyle(LeftName, out value))
			{
				result.Left = value;
			}

			if (padding.GetStyle(RightName, out value))
			{
				result.Right = value;
			}

			if (padding.GetStyle(TopName, out value))
			{
				result.Top = value;
			}

			if (padding.GetStyle(BottomName, out value))
			{
				result.Bottom = value;
			}

			return result;
		}

		private void LoadWidgetStyleFromSource(JObject source, WidgetStyle result)
		{

			int i;
			if (source.GetStyle(WidthHintName, out i))
			{
				result.WidthHint = i;
			}

			if (source.GetStyle(HeightHintName, out i))
			{
				result.HeightHint = i;
			}

			string TextureRegion2DName;
			if (source.GetStyle(BackgroundName, out TextureRegion2DName))
			{
				result.Background = GetTextureRegion2D(TextureRegion2DName);
			}

			if (source.GetStyle(OverBackgroundName, out TextureRegion2DName))
			{
				result.OverBackground = GetTextureRegion2D(TextureRegion2DName);
			}

			if (source.GetStyle(DisabledBackgroundName, out TextureRegion2DName))
			{
				result.DisabledBackground = GetTextureRegion2D(TextureRegion2DName);
			}

			if (source.GetStyle(BorderName, out TextureRegion2DName))
			{
				result.Border = GetTextureRegion2D(TextureRegion2DName);
			}

			if (source.GetStyle(OverBorderName, out TextureRegion2DName))
			{
				result.OverBorder = GetTextureRegion2D(TextureRegion2DName);
			}

			if (source.GetStyle(DisabledBorderName, out TextureRegion2DName))
			{
				result.DisabledBorder = GetTextureRegion2D(TextureRegion2DName);
			}

			JObject padding;
			if (source.GetStyle(PaddingName, out padding))
			{
				result.Padding = GetPaddingInfo(padding);
			}
		}

		private void LoadTextBlockStyleFromSource(JObject source, TextBlockStyle result)
		{
			LoadWidgetStyleFromSource(source, result);

			string name;
			if (source.GetStyle(FontName, out name))
			{
				result.Font = GetFont(name);
			}

			if (source.GetStyle(TextColorName, out name))
			{
				result.TextColor = GetColor(name);
			}

			if (source.GetStyle(DisabledTextColorName, out name))
			{
				result.DisabledTextColor = GetColor(name);
			}
		}

		private void LoadTextFieldStyleFromSource(JObject source, TextFieldStyle result)
		{
			LoadWidgetStyleFromSource(source, result);

			string name;
			if (source.GetStyle(TextColorName, out name))
			{
				result.TextColor = GetColor(name);
			}

			if (source.GetStyle(DisabledTextColorName, out name))
			{
				result.DisabledTextColor = GetColor(name);
			}

			if (source.GetStyle(FocusedTextColorName, out name))
			{
				result.FocusedTextColor = GetColor(name);
			}
			
			if (source.GetStyle(MessageTextColorName, out name))
			{
				result.MessageTextColor = GetColor(name);
			}

			if (source.GetStyle(FontName, out name))
			{
				result.Font = GetFont(name);
			}

			if (source.GetStyle(MessageFontName, out name))
			{
				result.MessageFont = GetFont(name);
			}

			if (source.GetStyle(FocusedBackgroundName, out name))
			{
				result.FocusedBackground = GetTextureRegion2D(name);
			}

			if (source.GetStyle(CursorName, out name))
			{
				result.Cursor = GetTextureRegion2D(name);
			}

			if (source.GetStyle(SelectionName, out name))
			{
				result.Selection = GetTextureRegion2D(name);
			}
		}

		private void LoadScrollAreaStyleFromSource(JObject source, ScrollAreaStyle result)
		{
			LoadWidgetStyleFromSource(source, result);

			string TextureRegion2DName;
			if (source.GetStyle(HorizontalScrollName, out TextureRegion2DName))
			{
				result.HorizontalScrollBackground = GetTextureRegion2D(TextureRegion2DName);
			}

			if (source.GetStyle(HorizontalScrollKnobName, out TextureRegion2DName))
			{
				result.HorizontalScrollKnob = GetTextureRegion2D(TextureRegion2DName);
			}

			if (source.GetStyle(VerticalScrollName, out TextureRegion2DName))
			{
				result.VerticalScrollBackground = GetTextureRegion2D(TextureRegion2DName);
			}

			if (source.GetStyle(VerticalScrollKnobName, out TextureRegion2DName))
			{
				result.VerticalScrollKnob = GetTextureRegion2D(TextureRegion2DName);
			}
		}

		private void LoadImageStyleFromSource(JObject source, ImageStyle result)
		{
			LoadWidgetStyleFromSource(source, result);

			string name;
			if (source.GetStyle(ImageName, out name))
			{
				result.Image = GetTextureRegion2D(name);
			}

			if (source.GetStyle(OverImageName, out name))
			{
				result.OverImage = GetTextureRegion2D(name);
			}
		}

		private void LoadPressableImageStyleFromSource(JObject source, PressableImageStyle result)
		{
			LoadImageStyleFromSource(source, result);

			string name;
			if (source.GetStyle(PressedImageName, out name))
			{
				result.PressedImage = GetTextureRegion2D(name);
			}
		}

		private void LoadButtonBaseStyleFromSource(JObject source, ButtonBaseStyle result)
		{
			LoadWidgetStyleFromSource(source, result);

			string name;
			if (source.GetStyle(PressedBackgroundName, out name))
			{
				result.PressedBackground = GetTextureRegion2D(name);
			}
		}

		private void LoadButtonStyleFromSource(JObject source, ButtonStyle result)
		{
			LoadButtonBaseStyleFromSource(source, result);

			JObject labelStyle;
			if (source.GetStyle(LabelStyleName, out labelStyle))
			{
				LoadTextBlockStyleFromSource(labelStyle, result.LabelStyle);
			}

			if (source.GetStyle(ImageStyleName, out labelStyle))
			{
				LoadPressableImageStyleFromSource(labelStyle, result.ImageStyle);
			}
		}


		private void LoadImageButtonStyleFromSource(JObject source, ImageButtonStyle result)
		{
			LoadButtonBaseStyleFromSource(source, result);

			JObject style;
			if (source.GetStyle(ImageStyleName, out style))
			{
				LoadPressableImageStyleFromSource(style, result.ImageStyle);
			}
		}

		private void LoadSpinButtonStyleFromSource(JObject source, SpinButtonStyle result)
		{
			LoadWidgetStyleFromSource(source, result);

			JObject style;
			if (source.GetStyle(TextFieldStyleName, out style))
			{
				LoadTextFieldStyleFromSource(style, result.TextFieldStyle);
			}

			if (source.GetStyle(UpButtonStyleName, out style))
			{
				LoadButtonStyleFromSource(style, result.UpButtonStyle);
			}

			if (source.GetStyle(DownButtonStyleName, out style))
			{
				LoadButtonStyleFromSource(style, result.DownButtonStyle);
			}
		}

		private void LoadSliderStyleFromSource(JObject source, SliderStyle result)
		{
			LoadWidgetStyleFromSource(source, result);

			JObject knobStyle;
			if (source.GetStyle(KnobName, out knobStyle))
			{
				LoadButtonStyleFromSource(knobStyle, result.KnobStyle);
			}
		}

		private void LoadProgressBarStyleFromSource(JObject source, ProgressBarStyle result)
		{
			LoadWidgetStyleFromSource(source, result);

			string style;
			if (source.GetStyle(FilledName, out style))
			{
				result.Filled = GetTextureRegion2D(style);
			}
		}

		private void LoadComboBoxItemStyleFromSource(JObject source, ListItemStyle result)
		{
			LoadButtonStyleFromSource(source, result);

			string name;
			if (source.GetStyle(SelectedBackgroundName, out name))
			{
				result.SelectedBackground = GetTextureRegion2D(name);
			}
		}

		private void LoadComboBoxStyleFromSource(JObject source, ComboBoxStyle result)
		{
			LoadButtonStyleFromSource(source, result);

			JObject subStyle;
			if (source.GetStyle(ItemsContainerName, out subStyle))
			{
				LoadWidgetStyleFromSource(subStyle, result.ItemsContainerStyle);
			}

			if (source.GetStyle(ComboBoxItemName, out subStyle))
			{
				LoadComboBoxItemStyleFromSource(subStyle, result.ListItemStyle);
			}
		}

		private void LoadListBoxStyleFromSource(JObject source, ListBoxStyle result)
		{
			LoadWidgetStyleFromSource(source, result);

			JObject subStyle;
			if (source.GetStyle(ListBoxItemName, out subStyle))
			{
				LoadComboBoxItemStyleFromSource(subStyle, result.ListItemStyle);
			}
		}

		private void LoadMenuItemStyleFromSource(JObject source, MenuItemStyle result)
		{
			LoadButtonStyleFromSource(source, result);

			string name;
			if (source.GetStyle(OpenBackgroundName, out name))
			{
				result.OpenBackground = GetTextureRegion2D(name);
			}

			int value;
			if (source.GetStyle(IconWidthName, out value))
			{
				result.IconWidth = value;
			}

			if (source.GetStyle(ShortcutWidthName, out value))
			{
				result.ShortcutWidth = value;
			}
		}

		private void LoadMenuSeparatorStyleFromSource(JObject source, MenuSeparatorStyle result)
		{
			LoadWidgetStyleFromSource(source, result);

			string name;
			if (source.GetStyle(ImageName, out name))
			{
				result.Image = GetTextureRegion2D(name);
			}

			if (source.GetStyle(ThicknessName, out name))
			{
				result.Thickness = int.Parse(name);
			}
		}

		private void LoadSplitPaneStyleFromSource(JObject source, SplitPaneStyle result)
		{
			LoadWidgetStyleFromSource(source, result);

			JObject handle;
			if (source.GetStyle(HandleName, out handle))
			{
				LoadImageButtonStyleFromSource(handle, result.HandleStyle);
			}
		}

		private void LoadMenuStyleFromSource(JObject source, MenuStyle result)
		{
			LoadWidgetStyleFromSource(source, result);

			JObject menuItem;
			if (source.GetStyle(MenuItemName, out menuItem))
			{
				LoadMenuItemStyleFromSource(menuItem, result.MenuItemStyle);
			}

			if (source.GetStyle(MenuSeparatorName, out menuItem))
			{
				LoadMenuSeparatorStyleFromSource(menuItem, result.SeparatorStyle);
			}
		}

		private void LoadTreeStyleFromSource(JObject source, TreeStyle result)
		{
			LoadWidgetStyleFromSource(source, result);

			string name;
			if (source.GetStyle(RowSelectionBackgroundWithoutFocusName, out name))
			{
				result.RowSelectionBackgroundWithoutFocus = GetTextureRegion2D(name);
			}

			if (source.GetStyle(RowSelectionBackgroundName, out name))
			{
				result.RowSelectionBackground = GetTextureRegion2D(name);
			}

			JObject obj;
			if (source.GetStyle(MarkName, out obj))
			{
				LoadImageButtonStyleFromSource(obj, result.MarkStyle);

				if (obj.GetStyle(LabelStyleName, out obj))
				{
					LoadTextBlockStyleFromSource(obj, result.LabelStyle);
				}
			}
		}

		private void LoadWindowStyleFromSource(JObject source, WindowStyle result)
		{
			LoadWidgetStyleFromSource(source, result);

			JObject obj;
			if (source.GetStyle(TitleStyleName, out obj))
			{
				LoadTextBlockStyleFromSource(obj, result.TitleStyle);
			}

			if (source.GetStyle(CloseButtonStyleName, out obj))
			{
				LoadImageButtonStyleFromSource(obj, result.CloseButtonStyle);
			}
		}

		private void FillStyles<T>(string key,
			T result,
			Action<JObject, T> fillAction) where T : new()
		{
			JObject source;
			if (!_root.GetStyle(key, out source) || source == null)
			{
				return;
			}

			fillAction(source, result);
		}

		public Stylesheet Load()
		{
			var result = new Stylesheet();

			JObject colors;
			if (_root.GetStyle(ColorsName, out colors))
			{
				ParseColors(colors);
			}
			FillStyles(TextBlockName, result.TextBlockStyle, LoadTextBlockStyleFromSource);
			FillStyles(TextFieldName, result.TextFieldStyle, LoadTextFieldStyleFromSource);
			FillStyles(ScrollAreaName, result.ScrollAreaStyle, LoadScrollAreaStyleFromSource);
			FillStyles(ButtonName, result.ButtonStyle, LoadButtonStyleFromSource);
			FillStyles(CheckBoxName, result.CheckBoxStyle, LoadButtonStyleFromSource);
			FillStyles(ImageButtonName, result.ImageButtonStyle, LoadImageButtonStyleFromSource);
			FillStyles(SpinButtonName, result.SpinButtonStyle, LoadSpinButtonStyleFromSource);
			FillStyles(HorizontalSliderName, result.HorizontalSliderStyle, LoadSliderStyleFromSource);
			FillStyles(VerticalSliderName, result.VerticalSliderStyle, LoadSliderStyleFromSource);
			FillStyles(HorizontalProgressBarName, result.HorizontalProgressBarStyle, LoadProgressBarStyleFromSource);
			FillStyles(VerticalProgressBarName, result.VerticalProgressBarStyle, LoadProgressBarStyleFromSource);
			FillStyles(ComboBoxName, result.ComboBoxStyle, LoadComboBoxStyleFromSource);
			FillStyles(ListBoxName, result.ListBoxStyle, LoadListBoxStyleFromSource);
			FillStyles(TreeName, result.TreeStyle, LoadTreeStyleFromSource);
			FillStyles(HorizontalSplitPaneName, result.HorizontalSplitPaneStyle, LoadSplitPaneStyleFromSource);
			FillStyles(VerticalSplitPaneName, result.VerticalSplitPaneStyle, LoadSplitPaneStyleFromSource);
			FillStyles(HorizontalMenuName, result.HorizontalMenuStyle, LoadMenuStyleFromSource);
			FillStyles(VerticalMenuName, result.VerticalMenuStyle, LoadMenuStyleFromSource);
			FillStyles(WindowName, result.WindowStyle, LoadWindowStyleFromSource);

			return result;
		}
	}
}
