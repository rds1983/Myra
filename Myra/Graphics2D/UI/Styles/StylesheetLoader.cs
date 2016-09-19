using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.Text;
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
		public const string BackgroundName = "background";
		public const string OverBackgroundName = "overBackground";
		public const string DisabledBackgroundName = "disabledBackground";
		public const string FocusedBackgroundName = "disabledBackground";
		public const string PressedBackgroundName = "pressedBackground";
		public const string PaddingName = "padding";
		public const string BorderName = "border";
		public const string MarginName = "margin";
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
		public const string TitleFontName = "titleFont";
		public const string TitleTextColorName = "titleTextColor";
		public const string CursorName = "cursor";
		public const string IconWidthName = "iconWidth";
		public const string ShortcutWidthName = "shortcutWidth";
		public const string SpacingName = "spacing";
		public const string LabelStyleName = "label";
		public const string MenuItemName = "menuItem";
		public const string HandleName = "handle";
		public const string MarkName = "mark";
		public const string ImageName = "image";
		public const string PressedImageName = "pressedImage";
		public const string RowOverBackgroundName = "rowOverBackground";
		public const string RowSelectionBackgroundName = "rowSelectionBackground";
		public const string MenuSeparatorName = "separator";
		public const string ThicknessName = "thickness";

		private readonly Dictionary<string, Color> _colors = new Dictionary<string, Color>();
		private readonly JObject _root;
		private readonly Func<string, BitmapFont> _fontGetter;
		private readonly Func<string, Drawable> _drawableGetter;

		public StylesheetLoader(JObject root,
			Func<string, BitmapFont> fontGetter,
			Func<string, Drawable> drawableGetter)
		{
			if (root == null)
			{
				throw new ArgumentNullException("root");
			}

			_root = root;
			_fontGetter = fontGetter;
			_drawableGetter = drawableGetter;
		}

		private Drawable GetDrawable(string id)
		{
			if (_drawableGetter == null || string.IsNullOrEmpty(id))
			{
				return null;
			}

			return _drawableGetter(id);
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
			string drawableName;
			if (source.GetStyle(BackgroundName, out drawableName))
			{
				result.Background = GetDrawable(drawableName);
			}

			if (source.GetStyle(OverBackgroundName, out drawableName))
			{
				result.OverBackground = GetDrawable(drawableName);
			}

			if (source.GetStyle(DisabledBackgroundName, out drawableName))
			{
				result.DisabledBackground = GetDrawable(drawableName);
			}

			JObject padding;
			if (source.GetStyle(PaddingName, out padding))
			{
				result.FrameInfo.Padding = GetPaddingInfo(padding);
			}

			if (source.GetStyle(BorderName, out padding))
			{
				result.FrameInfo.Border = GetPaddingInfo(padding);
			}

			if (source.GetStyle(MarginName, out padding))
			{
				result.FrameInfo.Margin = GetPaddingInfo(padding);
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
				result.FocusedBackground = GetDrawable(name);
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

		private void LoadScrollAreaStyleFromSource(JObject source, ScrollAreaStyle result)
		{
			LoadWidgetStyleFromSource(source, result);

			string drawableName;
			if (source.GetStyle(HorizontalScrollName, out drawableName))
			{
				result.HorizontalScrollBackground = GetDrawable(drawableName);
			}

			if (source.GetStyle(HorizontalScrollKnobName, out drawableName))
			{
				result.HorizontalScrollKnob = GetDrawable(drawableName);
			}

			if (source.GetStyle(VerticalScrollName, out drawableName))
			{
				result.VerticalScrollBackground = GetDrawable(drawableName);
			}

			if (source.GetStyle(VerticalScrollKnobName, out drawableName))
			{
				result.VerticalScrollKnob = GetDrawable(drawableName);
			}
		}

		private void LoadButtonStyleFromSource(JObject source, ButtonStyle result)
		{
			LoadWidgetStyleFromSource(source, result);

			string name;
			if (source.GetStyle(PressedBackgroundName, out name))
			{
				result.PressedBackground = GetDrawable(name);
			}

			JObject labelStyle;
			if (source.GetStyle(LabelStyleName, out labelStyle))
			{
				LoadTextBlockStyleFromSource(labelStyle, result.LabelStyle);
			}

			if (source.GetStyle(ImageName, out name))
			{
				result.Image = GetDrawable(name);
			}

			if (source.GetStyle(PressedImageName, out name))
			{
				result.PressedImage = GetDrawable(name);
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

		private void LoadMenuSeparatorStyleFromSource(JObject source, MenuSeparatorStyle result)
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
				LoadButtonStyleFromSource(handle, result.HandleStyle);
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
			if (source.GetStyle(RowOverBackgroundName, out name))
			{
				result.RowOverBackground = GetDrawable(name);
			}

			if (source.GetStyle(RowSelectionBackgroundName, out name))
			{
				result.RowSelectionBackground = GetDrawable(name);
			}

			JObject obj;
			if (source.GetStyle(MarkName, out obj))
			{
				LoadButtonStyleFromSource(obj, result.MarkStyle);
			}

			if (source.GetStyle(LabelStyleName, out obj))
			{
				LoadTextBlockStyleFromSource(obj, result.LabelStyle);
			}
		}

		private void LoadWindowStyleFromSource(JObject source, WindowStyle result)
		{
			LoadWidgetStyleFromSource(source, result);

			string name;
			if (source.GetStyle(TitleFontName, out name))
			{
				result.TitleFont = GetFont(name);
			}

			if (source.GetStyle(TitleTextColorName, out name))
			{
				result.TitleTextColor = GetColor(name);
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

		public Stylesheet CreateWidgetStyleFromSource()
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
