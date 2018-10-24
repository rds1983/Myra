using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Myra.Utility;
using Newtonsoft.Json.Linq;

namespace Myra.Graphics2D.UI.Styles
{
	internal class StylesheetLoader
	{
		public const string TypeName = "type";
		public const string ColorsName = "colors";
		public const string DrawablesName = "drawables";
		public const string TextBlockName = "textBlock";
		public const string TextFieldName = "textField";
		public const string ScrollAreaName = "scrollArea";
		public const string ButtonName = "button";
		public const string CheckBoxName = "checkBox";
		public const string ImageButtonName = "imageButton";
		public const string SpinButtonName = "spinButton";
		public const string TextButtonName = "textButton";
		public const string ComboBoxName = "comboBox";
		public const string ComboBoxItemName = "comboBoxItem";
		public const string ListBoxName = "listBox";
		public const string ListBoxItemName = "listBoxItem";
		public const string GridName = "grid";
		public const string TreeName = "tree";
		public const string SplitPaneName = "splitPane";
		public const string HorizontalSplitPaneName = "horizontalSplitPane";
		public const string VerticalSplitPaneName = "verticalSplitPane";
		public const string HorizontalMenuName = "horizontalMenu";
		public const string VerticalMenuName = "verticalMenu";
		public const string WindowName = "window";
		public const string LeftName = "left";
		public const string RightName = "right";
		public const string WidthName = "width";
		public const string HeightName = "height";
		public const string TopName = "top";
		public const string BottomName = "bottom";
		public const string WidthHintName = "widthHint";
		public const string HeightHintName = "heightHint";
		public const string BackgroundName = "background";
		public const string OverBackgroundName = "overBackground";
		public const string DisabledBackgroundName = "disabledBackground";
		public const string FocusedBackgroundName = "focusedBackground";
		public const string PressedBackgroundName = "pressedBackground";
		public const string BorderName = "border";
		public const string OverBorderName = "overBorder";
		public const string DisabledBorderName = "disabledBorder";
		public const string FocusedBorderName = "focusedBorder";
		public const string BoundsName = "bounds";
		public const string PaddingName = "padding";
		public const string FontName = "font";
		public const string MessageFontName = "font";
		public const string TextColorName = "textColor";
		public const string DisabledTextColorName = "disabledTextColor";
		public const string FocusedTextColorName = "focusedTextColor";
		public const string PressedTextColorName = "pressedTextColor";
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
		public const string SelectionBackgroundName = "selectionBackground";
		public const string SelectionBackgroundWithoutFocusName = "selectionBackgroundWithoutFocus";
		public const string SelectionHoverBackgroundName = "selectionHoverBackground";
		public const string SeparatorName = "separator";
		public const string ThicknessName = "thickness";
		public const string ItemsContainerName = "itemsContainer";
		public const string HorizontalSliderName = "horizontalSlider";
		public const string VerticalSliderName = "verticalSlider";
		public const string KnobName = "knob";
		public const string HorizontalProgressBarName = "horizontalProgressBar";
		public const string VerticalProgressBarName = "verticalProgressBar";
		public const string HorizontalSeparatorName = "horizontalSeparator";
		public const string VerticalSeparatorName = "verticalSeparator";
		public const string FilledName = "filled";
		public const string VariantsName = "variants";

		private readonly Dictionary<string, Color> _colors = new Dictionary<string, Color>();
		private readonly Dictionary<string, Tuple<string, Color>> _drawables = new Dictionary<string, Tuple<string, Color>>();
		private readonly JObject _root;
		private readonly Func<string, SpriteFont> _fontLoader;
		private readonly Func<string, TextureRegion> _textureLoader;

		public StylesheetLoader(JObject root,
			Func<string, TextureRegion> textureLoader,
			Func<string, SpriteFont> fontLoader)
		{
			if (root == null)
			{
				throw new ArgumentNullException("root");
			}

			if (textureLoader == null)
			{
				throw new ArgumentNullException("textureLoader");
			}

			if (fontLoader == null)
			{
				throw new ArgumentNullException("fontLoader");
			}

			_root = root;
			_fontLoader = fontLoader;
			_textureLoader = textureLoader;
		}

		private Drawable GetDrawable(string id)
		{
			Tuple<string, Color> drawableFromTable;
			if (_drawables.TryGetValue(id, out drawableFromTable))
			{
				return new Drawable(_textureLoader(drawableFromTable.Item1))
				{
					Color = drawableFromTable.Item2
				};
			}


			return new Drawable(_textureLoader(id));
		}

		private void ParseDrawables(JObject drawables)
		{
			_drawables.Clear();

			foreach (var props in drawables.Properties())
			{
				// Parse it
				var name = props.Value["name"].ToString();
				var color = props.Value["color"].ToString();

				var drawableEntry = new Tuple<string, Color>(name, GetColor(color));
				_drawables.Add(props.Name, drawableEntry);
			}
		}

		private SpriteFont GetFont(string id)
		{
			if (_fontLoader == null || string.IsNullOrEmpty(id))
			{
				return null;
			}

			return _fontLoader(id);
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

			var fromName = color.FromName();
			if (fromName != null)
			{
				return fromName.Value;
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

			string name;
			if (source.GetStyle(BackgroundName, out name))
			{
				result.Background = GetDrawable(name);
			}

			if (source.GetStyle(OverBackgroundName, out name))
			{
				result.OverBackground = GetDrawable(name);
			}

			if (source.GetStyle(DisabledBackgroundName, out name))
			{
				result.DisabledBackground = GetDrawable(name);
			}

			if (source.GetStyle(FocusedBackgroundName, out name))
			{
				result.FocusedBackground = GetDrawable(name);
			}

			if (source.GetStyle(BorderName, out name))
			{
				result.Border = GetDrawable(name);
			}

			if (source.GetStyle(OverBorderName, out name))
			{
				result.OverBorder = GetDrawable(name);
			}

			if (source.GetStyle(DisabledBorderName, out name))
			{
				result.DisabledBorder = GetDrawable(name);
			}

			if (source.GetStyle(FocusedBorderName, out name))
			{
				result.FocusedBorder = GetDrawable(name);
			}

			JObject padding;
			if (source.GetStyle(PaddingName, out padding))
			{
				int value;
				if (padding.GetStyle(LeftName, out value))
				{
					result.PaddingLeft = value;
				}

				if (padding.GetStyle(RightName, out value))
				{
					result.PaddingRight = value;
				}

				if (padding.GetStyle(TopName, out value))
				{
					result.PaddingTop = value;
				}

				if (padding.GetStyle(BottomName, out value))
				{
					result.PaddingBottom = value;
				}
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

			if (source.GetStyle(PressedTextColorName, out name))
			{
				result.PressedTextColor = GetColor(name);
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

			if (source.GetStyle(FontName, out name))
			{
				result.Font = GetFont(name);
			}

			if (source.GetStyle(MessageFontName, out name))
			{
				result.MessageFont = GetFont(name);
			}

			if (source.GetStyle(CursorName, out name))
			{
				result.Cursor = GetDrawable(name);
			}

			if (source.GetStyle(SelectionName, out name))
			{
				result.Selection = GetDrawable(name);
			}
		}

		private void LoadScrollAreaStyleFromSource(JObject source, ScrollPaneStyle result)
		{
			LoadWidgetStyleFromSource(source, result);

			string TextureRegionName;
			if (source.GetStyle(HorizontalScrollName, out TextureRegionName))
			{
				result.HorizontalScrollBackground = GetDrawable(TextureRegionName);
			}

			if (source.GetStyle(HorizontalScrollKnobName, out TextureRegionName))
			{
				result.HorizontalScrollKnob = GetDrawable(TextureRegionName);
			}

			if (source.GetStyle(VerticalScrollName, out TextureRegionName))
			{
				result.VerticalScrollBackground = GetDrawable(TextureRegionName);
			}

			if (source.GetStyle(VerticalScrollKnobName, out TextureRegionName))
			{
				result.VerticalScrollKnob = GetDrawable(TextureRegionName);
			}
		}

		private void LoadImageStyleFromSource(JObject source, ImageStyle result)
		{
			LoadWidgetStyleFromSource(source, result);

			string name;
			if (source.GetStyle(ImageName, out name))
			{
				result.Image = GetDrawable(name);
			}

			if (source.GetStyle(OverImageName, out name))
			{
				result.OverImage = GetDrawable(name);
			}
		}

		private void LoadPressableImageStyleFromSource(JObject source, PressableImageStyle result)
		{
			LoadImageStyleFromSource(source, result);

			string name;
			if (source.GetStyle(PressedImageName, out name))
			{
				result.PressedImage = GetDrawable(name);
			}
		}

		private void LoadButtonBaseStyleFromSource(JObject source, ButtonBaseStyle result)
		{
			LoadWidgetStyleFromSource(source, result);

			string name;
			if (source.GetStyle(PressedBackgroundName, out name))
			{
				result.PressedBackground = GetDrawable(name);
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

		private void LoadTextButtonStyleFromSource(JObject source, TextButtonStyle result)
		{
			LoadButtonBaseStyleFromSource(source, result);

			JObject labelStyle;
			if (source.GetStyle(LabelStyleName, out labelStyle))
			{
				LoadTextBlockStyleFromSource(labelStyle, result.LabelStyle);
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
				LoadImageButtonStyleFromSource(style, result.UpButtonStyle);
			}

			if (source.GetStyle(DownButtonStyleName, out style))
			{
				LoadImageButtonStyleFromSource(style, result.DownButtonStyle);
			}
		}

		private void LoadSliderStyleFromSource(JObject source, SliderStyle result)
		{
			LoadWidgetStyleFromSource(source, result);

			JObject knobStyle;
			if (source.GetStyle(KnobName, out knobStyle))
			{
				LoadImageButtonStyleFromSource(knobStyle, result.KnobStyle);
			}
		}

		private void LoadProgressBarStyleFromSource(JObject source, ProgressBarStyle result)
		{
			LoadWidgetStyleFromSource(source, result);

			string style;
			if (source.GetStyle(FilledName, out style))
			{
				result.Filled = GetDrawable(style);
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
				LoadButtonStyleFromSource(subStyle, result.ListItemStyle);
			}
		}

		private void LoadListBoxStyleFromSource(JObject source, ListBoxStyle result)
		{
			LoadWidgetStyleFromSource(source, result);

			JObject subStyle;
			if (source.GetStyle(ListBoxItemName, out subStyle))
			{
				LoadButtonStyleFromSource(subStyle, result.ListItemStyle);
			}

			if (source.GetStyle(SeparatorName, out subStyle))
			{
				LoadSeparatorStyleFromSource(subStyle, result.SeparatorStyle);
			}

		}

		private void LoadMenuItemStyleFromSource(JObject source, MenuItemStyle result)
		{
			LoadButtonStyleFromSource(source, result);

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

		private void LoadSeparatorStyleFromSource(JObject source, SeparatorStyle result)
		{
			LoadWidgetStyleFromSource(source, result);

			string name;
			if (source.GetStyle(ImageName, out name))
			{
				result.Image = GetDrawable(name);
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

			if (source.GetStyle(SeparatorName, out menuItem))
			{
				LoadSeparatorStyleFromSource(menuItem, result.SeparatorStyle);
			}
		}

		private void LoadGridStyleFromSource(JObject source, GridStyle result)
		{
			LoadWidgetStyleFromSource(source, result);

			string name;
			if (source.GetStyle(SelectionBackgroundName, out name))
			{
				result.SelectionBackground = GetDrawable(name);
			}

			if (source.GetStyle(SelectionHoverBackgroundName, out name))
			{
				result.SelectionHoverBackground = GetDrawable(name);
			}
		}

		private void LoadTreeStyleFromSource(JObject source, TreeStyle result)
		{
			LoadGridStyleFromSource(source, result);

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
			Dictionary<string, T> stylesDict,
			Action<JObject, T> fillAction) where T : WidgetStyle, new()
		{
			JObject source;
			if (!_root.GetStyle(key, out source) || source == null)
			{
				return;
			}

			fillAction(source, stylesDict[Stylesheet.DefaultStyleName]);

			JObject styles;
			if (!source.GetStyle(VariantsName, out styles) || styles == null)
			{
				return;
			}

			foreach (var pair in styles)
			{
				var variant = (T) stylesDict[Stylesheet.DefaultStyleName].Clone();
				fillAction((JObject) pair.Value, variant);
				stylesDict[pair.Key] = variant;
			}
		}

		public Stylesheet Load()
		{
			var result = new Stylesheet();

			JObject colors;
			if (_root.GetStyle(ColorsName, out colors))
			{
				ParseColors(colors);
			}

			JObject drawables;
			if (_root.GetStyle(DrawablesName, out drawables))
			{
				ParseDrawables(drawables);
			}

			FillStyles(TextBlockName, result.TextBlockStyles, LoadTextBlockStyleFromSource);
			FillStyles(TextFieldName, result.TextFieldStyles, LoadTextFieldStyleFromSource);
			FillStyles(ScrollAreaName, result.ScrollPaneStyles, LoadScrollAreaStyleFromSource);
			FillStyles(ButtonName, result.ButtonStyles, LoadButtonStyleFromSource);
			FillStyles(CheckBoxName, result.CheckBoxStyles, LoadButtonStyleFromSource);
			FillStyles(ImageButtonName, result.ImageButtonStyles, LoadImageButtonStyleFromSource);
			FillStyles(SpinButtonName, result.SpinButtonStyles, LoadSpinButtonStyleFromSource);
			FillStyles(TextButtonName, result.TextButtonStyles, LoadTextButtonStyleFromSource);
			FillStyles(HorizontalSliderName, result.HorizontalSliderStyles, LoadSliderStyleFromSource);
			FillStyles(VerticalSliderName, result.VerticalSliderStyles, LoadSliderStyleFromSource);
			FillStyles(HorizontalProgressBarName, result.HorizontalProgressBarStyles, LoadProgressBarStyleFromSource);
			FillStyles(VerticalProgressBarName, result.VerticalProgressBarStyles, LoadProgressBarStyleFromSource);
			FillStyles(HorizontalSeparatorName, result.HorizontalSeparatorStyles, LoadSeparatorStyleFromSource);
			FillStyles(VerticalSeparatorName, result.VerticalSeparatorStyles, LoadSeparatorStyleFromSource);
			FillStyles(ComboBoxName, result.ComboBoxStyles, LoadComboBoxStyleFromSource);
			FillStyles(ListBoxName, result.ListBoxStyles, LoadListBoxStyleFromSource);
			FillStyles(GridName, result.GridStyles, LoadGridStyleFromSource);
			FillStyles(TreeName, result.TreeStyles, LoadTreeStyleFromSource);
			FillStyles(HorizontalSplitPaneName, result.HorizontalSplitPaneStyles, LoadSplitPaneStyleFromSource);
			FillStyles(VerticalSplitPaneName, result.VerticalSplitPaneStyles, LoadSplitPaneStyleFromSource);
			FillStyles(HorizontalMenuName, result.HorizontalMenuStyles, LoadMenuStyleFromSource);
			FillStyles(VerticalMenuName, result.VerticalMenuStyles, LoadMenuStyleFromSource);
			FillStyles(WindowName, result.WindowStyles, LoadWindowStyleFromSource);

			return result;
		}
	}
}