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

			_checkBoxMouseInsideWidgetFrame.IsPressed = MyraEnvironment.DrawMouseHoveredWidgetFrame;
			_checkBoxMouseInsideWidgetFrame.PressedChanged += (s, a) =>
			{
				MyraEnvironment.DrawMouseHoveredWidgetFrame = _checkBoxMouseInsideWidgetFrame.IsPressed;
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
				Visible = true
			};
			Grid.SetRow(optionsCheckBox, InternalChild.Widgets.Count);

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