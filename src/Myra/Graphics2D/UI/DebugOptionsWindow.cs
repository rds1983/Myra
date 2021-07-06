using System;

namespace Myra.Graphics2D.UI
{
	public partial class DebugOptionsWindow
	{
		public bool ShowDebugInfo { get; set; }

		public DebugOptionsWindow()
		{
			Title = "UI Debug Options";

			BuildUI();

			_checkBoxWidgetFrames.IsPressed = MyraEnvironment.DrawWidgetsFrames;
			_checkBoxWidgetFrames.PressedChanged += (s, a) =>
			{
				MyraEnvironment.DrawWidgetsFrames = _checkBoxWidgetFrames.IsPressed;
			};

			_checkBoxKeyboardFocusedWidgetFrame.IsPressed = MyraEnvironment.DrawKeyboardFocusedWidgetFrame;
			_checkBoxKeyboardFocusedWidgetFrame.PressedChanged += (s, a) =>
			{
				MyraEnvironment.DrawKeyboardFocusedWidgetFrame = _checkBoxKeyboardFocusedWidgetFrame.IsPressed;
			};

			_checkBoxMouseWheelFocusedWidgetFrame.IsPressed = MyraEnvironment.DrawMouseWheelFocusedWidgetFrame;
			_checkBoxMouseWheelFocusedWidgetFrame.PressedChanged += (s, a) =>
			{
				MyraEnvironment.DrawMouseWheelFocusedWidgetFrame = _checkBoxMouseWheelFocusedWidgetFrame.IsPressed;
			};

			_checkBoxGlyphFrames.IsPressed = MyraEnvironment.DrawTextGlyphsFrames;
			_checkBoxGlyphFrames.PressedChanged += (s, a) =>
			{
				MyraEnvironment.DrawTextGlyphsFrames = _checkBoxGlyphFrames.IsPressed;
			};

			_checkBoxDisableClipping.IsPressed = MyraEnvironment.DisableClipping;
			_checkBoxDisableClipping.PressedChanged += (s, a) =>
			{
				MyraEnvironment.DisableClipping = _checkBoxDisableClipping.IsPressed;
			};

			_checkBoxSmoothText.IsPressed = MyraEnvironment.SmoothText;
			_checkBoxSmoothText.PressedChanged += (s, a) =>
			{
				MyraEnvironment.SmoothText = _checkBoxSmoothText.IsPressed;
			};
		}

		public void AddOption(string text, Action onEnabled, Action onDisabled)
		{
			var optionsCheckBox = new CheckBox
			{
				Text = text,
				ContentHorizontalAlignment = HorizontalAlignment.Stretch,
				ContentVerticalAlignment = VerticalAlignment.Stretch,
				Enabled = true,
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Top,
				GridRow = InternalChild.Widgets.Count,
				Visible = true
			};

			optionsCheckBox.PressedChanged += (s, a) =>
			{
				if (optionsCheckBox.IsPressed)
				{
					onEnabled();
				}
				else
				{
					onDisabled();
				}
			};

			Root.Widgets.Add(optionsCheckBox);
		}
	}
}