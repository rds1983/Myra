using System;
using System.Collections.Generic;
using Myra.Utility;
using Myra.Graphics2D.UI.Styles;
using Newtonsoft.Json.Linq;
using Microsoft.Xna.Framework;
using System.Xml.Linq;

namespace Myra.StylesheetConverter
{
	internal class StylesheetLoader
	{
		public const string TypeName = "type";
		public const string ColorsName = "colors";
		public const string DrawablesName = "drawables";
		public const string DesktopName = "desktop";
		public const string TextBlockName = "textBlock";
		public const string TextFieldName = "textField";
		public const string ScrollAreaName = "scrollArea";
		public const string ButtonName = "button";
		public const string CheckBoxName = "checkBox";
		public const string RadioButtonName = "radioButton";
		public const string ImageButtonName = "imageButton";
		public const string SpinButtonName = "spinButton";
		public const string TextButtonName = "textButton";
		public const string ComboBoxName = "comboBox";
		public const string ComboBoxItemName = "comboBoxItem";
		public const string ListBoxName = "listBox";
		public const string TabControlName = "tabControl";
		public const string ListBoxItemName = "listBoxItem";
		public const string TabItemName = "tabItem";
		public const string GridName = "grid";
		public const string TreeName = "tree";
		public const string SplitPaneName = "splitPane";
		public const string HorizontalSplitPaneName = "horizontalSplitPane";
		public const string VerticalSplitPaneName = "verticalSplitPane";
		public const string HorizontalMenuName = "horizontalMenu";
		public const string VerticalMenuName = "verticalMenu";
		public const string WindowName = "window";
		public const string DialogName = "dialog";
		public const string LeftName = "left";
		public const string RightName = "right";
		public const string WidthName = "width";
		public const string HeightName = "height";
		public const string TopName = "top";
		public const string BottomName = "bottom";
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
		public const string OverTextColorName = "overTextColor";
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
		public const string HeaderSpacingName = "headerSpacing";
		public const string ButtonSpacingName = "buttonSpacing";
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
		public const string ContentName = "content";

		private readonly Dictionary<string, Color> _colors = new Dictionary<string, Color>();
		private readonly JObject _root;

		public StylesheetLoader(JObject root)
		{
			if (root == null)
			{
				throw new ArgumentNullException("root");
			}

			_root = root;
		}

		private void ParseColors(JObject colors)
		{
			_colors.Clear();

			foreach (var props in colors)
			{
				// Parse it
				var stringValue = props.Value.ToString();
				var parsedColor = stringValue.FromName();
				if (parsedColor == null)
				{
					throw new Exception(string.Format("Could not parse color '{0}'", stringValue));
				}

				_colors.Add(props.Key, parsedColor.Value);
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

		private void LoadWidgetStyleFromSource(JObject source, XElement result)
		{
			int i;
			if (source.GetStyle(WidthName, out i))
			{
				result.SetAttributeValue("Width", i);
			}

			if (source.GetStyle(HeightName, out i))
			{
				result.SetAttributeValue("Height", i);
			}

			string name;
			if (source.GetStyle(BackgroundName, out name))
			{
				result.SetAttributeValue("Background", name);
			}

			if (source.GetStyle(OverBackgroundName, out name))
			{
				result.SetAttributeValue("OverBackground", name);
			}

			if (source.GetStyle(DisabledBackgroundName, out name))
			{
				result.SetAttributeValue("DisabledBackground", name);
			}

			if (source.GetStyle(FocusedBackgroundName, out name))
			{
				result.SetAttributeValue("FocusedBackground", name);
			}

			if (source.GetStyle(BorderName, out name))
			{
				result.SetAttributeValue("Border", name);
			}

			if (source.GetStyle(OverBorderName, out name))
			{
				result.SetAttributeValue("OverBorder", name);
			}

			if (source.GetStyle(DisabledBorderName, out name))
			{
				result.SetAttributeValue("DisabledBorder", name);
			}

			if (source.GetStyle(FocusedBorderName, out name))
			{
				result.SetAttributeValue("FocusedBorder", name);
			}

			JObject padding;
			if (source.GetStyle(PaddingName, out padding))
			{
				int value;
				if (padding.GetStyle(LeftName, out value))
				{
					result.SetAttributeValue("PaddingLeft", value);
				}

				if (padding.GetStyle(RightName, out value))
				{
					result.SetAttributeValue("PaddingRight", value);
				}

				if (padding.GetStyle(TopName, out value))
				{
					result.SetAttributeValue("PaddingTop", value);
				}

				if (padding.GetStyle(BottomName, out value))
				{
					result.SetAttributeValue("PaddingBottom", value);
				}
			}
		}

		private void LoadTextBlockStyleFromSource(JObject source, XElement result)
		{
			LoadWidgetStyleFromSource(source, result);

			string name;
			if (source.GetStyle(FontName, out name))
			{
				result.SetAttributeValue("Font", name);
			}

			if (source.GetStyle(TextColorName, out name))
			{
				result.SetAttributeValue("TextColor", name);
			}

			if (source.GetStyle(DisabledTextColorName, out name))
			{
				result.SetAttributeValue("DisabledTextColor", name);
			}

			if (source.GetStyle(OverTextColorName, out name))
			{
				result.SetAttributeValue("OverTextColor", name);
			}

			if (source.GetStyle(PressedTextColorName, out name))
			{
				result.SetAttributeValue("PressedTextColor", name);
			}
		}

		private void LoadTextFieldStyleFromSource(JObject source, XElement result)
		{
			LoadWidgetStyleFromSource(source, result);

			string name;
			if (source.GetStyle(TextColorName, out name))
			{
				result.SetAttributeValue("TextColor", name);
			}

			if (source.GetStyle(DisabledTextColorName, out name))
			{
				result.SetAttributeValue("DisabledTextColor", name);
			}

			if (source.GetStyle(FocusedTextColorName, out name))
			{
				result.SetAttributeValue("FocusedTextColor", name);
			}

			if (source.GetStyle(FontName, out name))
			{
				result.SetAttributeValue("Font", name);
			}

			if (source.GetStyle(CursorName, out name))
			{
				result.SetAttributeValue("Cursor", name);
			}

			if (source.GetStyle(SelectionName, out name))
			{
				result.SetAttributeValue("Selection", name);
			}
		}

		private void LoadScrollAreaStyleFromSource(JObject source, XElement result)
		{
			LoadWidgetStyleFromSource(source, result);

			string name;
			if (source.GetStyle(HorizontalScrollName, out name))
			{
				result.SetAttributeValue("HorizontalScrollBackground", name);
			}

			if (source.GetStyle(HorizontalScrollKnobName, out name))
			{
				result.SetAttributeValue("HorizontalScrollKnob", name);
			}

			if (source.GetStyle(VerticalScrollName, out name))
			{
				result.SetAttributeValue("VerticalScrollBackground", name);
			}

			if (source.GetStyle(VerticalScrollKnobName, out name))
			{
				result.SetAttributeValue("VerticalScrollKnob", name);
			}
		}

		private void LoadImageStyleFromSource(JObject source, XElement result)
		{
			LoadWidgetStyleFromSource(source, result);

			string name;
			if (source.GetStyle(ImageName, out name))
			{
				result.SetAttributeValue("Image", name);
			}

			if (source.GetStyle(OverImageName, out name))
			{
				result.SetAttributeValue("OverImage", name);
			}
		}

		private void LoadPressableImageStyleFromSource(JObject source, XElement result)
		{
			LoadImageStyleFromSource(source, result);

			string name;
			if (source.GetStyle(PressedImageName, out name))
			{
				result.SetAttributeValue("PressedImage", name);
			}
		}

		private void LoadButtonStyleFromSource(JObject source, XElement result)
		{
			LoadWidgetStyleFromSource(source, result);

			string name;
			if (source.GetStyle(PressedBackgroundName, out name))
			{
				result.SetAttributeValue("PressedBackground", name);
			}
		}

		private void LoadImageTextButtonStyleFromSource(JObject source, XElement result)
		{
			LoadButtonStyleFromSource(source, result);

			JObject style;
			if (source.GetStyle(LabelStyleName, out style))
			{
				var labelStyle = new XElement("LabelStyle");
				LoadTextBlockStyleFromSource(style, labelStyle);
				result.Add(labelStyle);
			}

			if (source.GetStyle(ImageStyleName, out style))
			{
				var imageStyle = new XElement("ImageStyle");
				LoadPressableImageStyleFromSource(style, imageStyle);
				result.Add(imageStyle);
			}

			int spacing;
			if (source.GetStyle(SpacingName, out spacing))
			{
				result.SetAttributeValue("ImageTextSpacing", spacing);
			}
		}

		private void LoadTextButtonStyleFromSource(JObject source, XElement result)
		{
			LoadButtonStyleFromSource(source, result);

			JObject labelStyle;
			if (source.GetStyle(LabelStyleName, out labelStyle))
			{
				var textBlockStyle = new XElement("LabelStyle");
				LoadTextBlockStyleFromSource(labelStyle, textBlockStyle);
				result.Add(textBlockStyle);
			}
		}

		private void LoadImageButtonStyleFromSource(JObject source, XElement result)
		{
			LoadButtonStyleFromSource(source, result);

			JObject style;
			if (source.GetStyle(ImageStyleName, out style))
			{
				var pressableImageStyle = new XElement("ImageStyle");
				LoadPressableImageStyleFromSource(style, pressableImageStyle);
				result.Add(pressableImageStyle);
			}
		}

		private void LoadSpinButtonStyleFromSource(JObject source, XElement result)
		{
			LoadWidgetStyleFromSource(source, result);

			JObject style;
			if (source.GetStyle(TextFieldStyleName, out style))
			{
				var textFieldStyle = new XElement("TextFieldStyle");
				LoadTextFieldStyleFromSource(style, textFieldStyle);
				result.Add(textFieldStyle);
			}

			if (source.GetStyle(UpButtonStyleName, out style))
			{
				var upButtonStyle = new XElement("UpButtonStyle");
				LoadImageButtonStyleFromSource(style, upButtonStyle);
				result.Add(upButtonStyle);
			}

			if (source.GetStyle(DownButtonStyleName, out style))
			{
				var downButtonStyle = new XElement("DownButtonStyle");
				LoadImageButtonStyleFromSource(style, downButtonStyle);
				result.Add(downButtonStyle);
			}
		}

		private void LoadSliderStyleFromSource(JObject source, XElement result)
		{
			LoadWidgetStyleFromSource(source, result);

			JObject style;
			if (source.GetStyle(KnobName, out style))
			{
				var knobButtonStyle = new XElement("KnobStyle");
				LoadImageButtonStyleFromSource(style, knobButtonStyle);
				result.Add(knobButtonStyle);
			}
		}

		private void LoadProgressBarStyleFromSource(JObject source, XElement result)
		{
			LoadWidgetStyleFromSource(source, result);

			string style;
			if (source.GetStyle(FilledName, out style))
			{
				result.SetAttributeValue("Filled", style);
			}
		}

		private void LoadComboBoxStyleFromSource(JObject source, XElement result)
		{
			LoadImageTextButtonStyleFromSource(source, result);

			JObject style;
			if (source.GetStyle(ItemsContainerName, out style))
			{
				var itemsContainerStyle = new XElement("ItemsContainerStyle");
				LoadWidgetStyleFromSource(style, itemsContainerStyle);
				result.Add(itemsContainerStyle);
			}

			if (source.GetStyle(ComboBoxItemName, out style))
			{
				var listItemStyle = new XElement("ListItemStyle");
				LoadImageTextButtonStyleFromSource(style, listItemStyle);
				result.Add(listItemStyle);
			}
		}

		private void LoadListBoxStyleFromSource(JObject source, XElement result)
		{
			LoadWidgetStyleFromSource(source, result);

			JObject style;
			if (source.GetStyle(ListBoxItemName, out style))
			{
				var listItemStyle = new XElement("ListItemStyle");
				LoadImageTextButtonStyleFromSource(style, listItemStyle);
				result.Add(listItemStyle);
			}

			if (source.GetStyle(SeparatorName, out style))
			{
				var separatorStyle = new XElement("SeparatorStyle");
				LoadSeparatorStyleFromSource(style, separatorStyle);
				result.Add(separatorStyle);
			}
		}

		private void LoadTabControlStyleFromSource(JObject source, XElement result)
		{
			LoadWidgetStyleFromSource(source, result);

			JObject style;
			if (source.GetStyle(TabItemName, out style))
			{
				var tabItemStyle = new XElement("TabItemStyle");
				LoadImageTextButtonStyleFromSource(style, tabItemStyle);
				result.Add(tabItemStyle);
			}

			if (source.GetStyle(ContentName, out style))
			{
				var contentStyle = new XElement("ContentStyle");
				LoadWidgetStyleFromSource(style, contentStyle);
				result.Add(contentStyle);
			}

			int spacing;
			if (source.GetStyle(HeaderSpacingName, out spacing))
			{
				result.SetAttributeValue("HeaderSpacing", spacing);
			}

			if (source.GetStyle(ButtonSpacingName, out spacing))
			{
				result.SetAttributeValue("ButtonSpacing", spacing);
			}
		}

		private void LoadMenuItemStyleFromSource(JObject source, XElement result)
		{
			LoadImageTextButtonStyleFromSource(source, result);

			int value;
			if (source.GetStyle(IconWidthName, out value))
			{
				result.SetAttributeValue("IconWidth", value);
			}

			if (source.GetStyle(ShortcutWidthName, out value))
			{
				result.SetAttributeValue("ShortcutWidth", value);
			}
		}

		private void LoadSeparatorStyleFromSource(JObject source, XElement result)
		{
			LoadWidgetStyleFromSource(source, result);

			string name;
			if (source.GetStyle(ImageName, out name))
			{
				result.SetAttributeValue("Image", name);
			}

			if (source.GetStyle(ThicknessName, out name))
			{
				result.SetAttributeValue("Thickness", int.Parse(name));
			}
		}

		private void LoadSplitPaneStyleFromSource(JObject source, XElement result)
		{
			LoadWidgetStyleFromSource(source, result);

			JObject style;
			if (source.GetStyle(HandleName, out style))
			{
				var handleStyle = new XElement("HandleStyle");
				LoadButtonStyleFromSource(style, handleStyle);
				result.Add(handleStyle);
			}
		}

		private void LoadMenuStyleFromSource(JObject source, XElement result)
		{
			LoadWidgetStyleFromSource(source, result);

			JObject menuItem;
			if (source.GetStyle(MenuItemName, out menuItem))
			{
				var menuItemStyle = new XElement("MenuItemStyle");
				LoadMenuItemStyleFromSource(menuItem, menuItemStyle);
				result.Add(menuItemStyle);
			}

			if (source.GetStyle(SeparatorName, out menuItem))
			{
				var separatorStyle = new XElement("SeparatorStyle");
				LoadSeparatorStyleFromSource(menuItem, separatorStyle);
				result.Add(separatorStyle);
			}
		}

		private void LoadTreeStyleFromSource(JObject source, XElement result)
		{
			string name;
			if (source.GetStyle(SelectionBackgroundName, out name))
			{
				result.SetAttributeValue("SelectionBackground", name);
			}

			if (source.GetStyle(SelectionHoverBackgroundName, out name))
			{
				result.SetAttributeValue("SelectionHoverBackground", name);
			}

			JObject obj;
			if (source.GetStyle(MarkName, out obj))
			{
				var markStyle = new XElement("MarkStyle");
				LoadImageButtonStyleFromSource(obj, markStyle);
				result.Add(markStyle);

				if (obj.GetStyle(LabelStyleName, out obj))
				{
					var labelStyle = new XElement("LabelStyle");
					LoadTextBlockStyleFromSource(obj, labelStyle);
					result.Add(labelStyle);
				}
			}
		}

		private void LoadWindowStyleFromSource(JObject source, XElement result)
		{
			LoadWidgetStyleFromSource(source, result);

			JObject obj;
			if (source.GetStyle(TitleStyleName, out obj))
			{
				var titleStyle = new XElement("TitleStyle");
				LoadTextBlockStyleFromSource(obj, titleStyle);
				result.Add(titleStyle);
			}

			if (source.GetStyle(CloseButtonStyleName, out obj))
			{
				var closeButtonStyle = new XElement("CloseButtonStyle");
				LoadImageButtonStyleFromSource(obj, closeButtonStyle);
				result.Add(closeButtonStyle);
			}
		}

		private void LoadDialogStyleFromSource(JObject source, XElement result)
		{
			LoadWindowStyleFromSource(source, result);
		}

		private void FillStyles(string key, 
			XElement root,
			string dictName,
			Action<JObject, XElement> fillAction)
		{
			JObject source;
			if (!_root.GetStyle(key, out source) || source == null)
			{
				return;
			}

			var stylesElement = new XElement(dictName);
			root.Add(stylesElement);

			var styleName = dictName.Substring(0, dictName.Length - 1);
			var styleElement = new XElement(styleName);
			stylesElement.Add(styleElement);

			fillAction(source, styleElement);

			JObject styles;
			if (!source.GetStyle(VariantsName, out styles) || styles == null)
			{
				return;
			}

			foreach (var pair in styles)
			{
				var variant = new XElement(styleName);
				variant.SetAttributeValue("Id", pair.Key);
				fillAction((JObject)pair.Value, variant);
				stylesElement.Add(variant);
			}
		}

		private XElement LoadDesktopStyleFromSource()
		{
			JObject source;
			if (!_root.GetStyle(DesktopName, out source) || source == null)
			{
				return null;
			}

			var result = new XElement("Desktop");

			string name;
			if (source.GetStyle(BackgroundName, out name))
			{
				result.SetAttributeValue("Background", name);
			}

			return result;
		}

		public XDocument Load()
		{
			var result = new XDocument();
			var root = new XElement("Stylesheet");
			result.Add(root);

			JObject colors;
			if (_root.GetStyle(ColorsName, out colors))
			{
				ParseColors(colors);

				var colorsNode = new XElement("Colors");
				foreach(var pair in _colors)
				{
					var colorNode = new XElement("Color");
					colorNode.SetAttributeValue("Id", pair.Key);
					colorNode.SetAttributeValue("Value", pair.Value.ToHexString());
					colorsNode.Add(colorNode);
				}

				root.Add(colorsNode);
			}

			var desktopStyle = LoadDesktopStyleFromSource();
			if (desktopStyle != null)
			{
				root.Add(desktopStyle);
			}

			FillStyles(TextBlockName, root, "TextBlockStyles", 
				LoadTextBlockStyleFromSource);
			FillStyles(TextFieldName, root, "TextFieldStyles", 
				LoadTextFieldStyleFromSource);
			FillStyles(ScrollAreaName, root, "ScrollPaneStyles", 
				LoadScrollAreaStyleFromSource);
			FillStyles(ButtonName, root, "ButtonStyles", 
				LoadButtonStyleFromSource);
			FillStyles(CheckBoxName, root, "CheckBoxStyles", 
				LoadImageTextButtonStyleFromSource);
			FillStyles(RadioButtonName, root, "RadioButtonStyles", 
				LoadImageTextButtonStyleFromSource);
			FillStyles(SpinButtonName, root, "SpinButtonStyles", 
				LoadSpinButtonStyleFromSource);
			FillStyles(HorizontalSliderName, root, "HorizontalSliderStyles", 
				LoadSliderStyleFromSource);
			FillStyles(VerticalSliderName, root, "VerticalSliderStyles", 
				LoadSliderStyleFromSource);
			FillStyles(HorizontalProgressBarName, root, "HorizontalProgressBarStyles", 
				LoadProgressBarStyleFromSource);
			FillStyles(VerticalProgressBarName, root, "VerticalProgressBarStyles", 
				LoadProgressBarStyleFromSource);
			FillStyles(HorizontalSeparatorName, root, "HorizontalSeparatorStyles", 
				LoadSeparatorStyleFromSource);
			FillStyles(VerticalSeparatorName, root, "VerticalSeparatorStyles", 
				LoadSeparatorStyleFromSource);
			FillStyles(ComboBoxName, root, "ComboBoxStyles", 
				LoadComboBoxStyleFromSource);
			FillStyles(ListBoxName, root, "ListBoxStyles", 
				LoadListBoxStyleFromSource);
			FillStyles(TabControlName, root, "TabControlStyles", 
				LoadTabControlStyleFromSource);
			FillStyles(TreeName, root, "TreeStyles", 
				LoadTreeStyleFromSource);
			FillStyles(HorizontalSplitPaneName, root, "HorizontalSplitPaneStyles", 
				LoadSplitPaneStyleFromSource);
			FillStyles(VerticalSplitPaneName, root, "VerticalSplitPaneStyles", 
				LoadSplitPaneStyleFromSource);
			FillStyles(HorizontalMenuName, root, "HorizontalMenuStyles", 
				LoadMenuStyleFromSource);
			FillStyles(VerticalMenuName, root, "VerticalMenuStyles", 
				LoadMenuStyleFromSource);
			FillStyles(WindowName, root, "WindowStyles", 
				LoadWindowStyleFromSource);
			FillStyles(DialogName, root, "DialogStyles", 
				LoadDialogStyleFromSource);

			return result;
		}
	}
}