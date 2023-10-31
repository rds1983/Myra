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

			_checkBoxWidgetFrames.IsChecked = MyraEnvironment.DrawWidgetsFrames;
			_checkBoxWidgetFrames.IsCheckedChanged += (s, a) =>
			{
				MyraEnvironment.DrawWidgetsFrames = _checkBoxWidgetFrames.IsChecked;
			};

			_checkBoxKeyboardFocusedWidgetFrame.IsChecked = MyraEnvironment.DrawKeyboardFocusedWidgetFrame;
			_checkBoxKeyboardFocusedWidgetFrame.IsCheckedChanged += (s, a) =>
			{
				MyraEnvironment.DrawKeyboardFocusedWidgetFrame = _checkBoxKeyboardFocusedWidgetFrame.IsChecked;
			};

			_checkBoxMouseInsideWidgetFrame.IsChecked = MyraEnvironment.DrawMouseHoveredWidgetFrame;
			_checkBoxMouseInsideWidgetFrame.IsCheckedChanged += (s, a) =>
			{
				MyraEnvironment.DrawMouseHoveredWidgetFrame = _checkBoxMouseInsideWidgetFrame.IsChecked;
			};

			_checkBoxGlyphFrames.IsChecked = MyraEnvironment.DrawTextGlyphsFrames;
			_checkBoxGlyphFrames.IsCheckedChanged += (s, a) =>
			{
				MyraEnvironment.DrawTextGlyphsFrames = _checkBoxGlyphFrames.IsChecked;
			};

			_checkBoxDisableClipping.IsChecked = MyraEnvironment.DisableClipping;
			_checkBoxDisableClipping.IsCheckedChanged += (s, a) =>
			{
				MyraEnvironment.DisableClipping = _checkBoxDisableClipping.IsChecked;
			};

			_checkBoxSmoothText.IsChecked = MyraEnvironment.SmoothText;
			_checkBoxSmoothText.IsCheckedChanged += (s, a) =>
			{
				MyraEnvironment.SmoothText = _checkBoxSmoothText.IsChecked;
			};
		}

		public void AddOption(string text, Action onEnabled, Action onDisabled)
		{
			var optionsCheckBox = new CheckButton
			{
				Enabled = true,
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Top,
				Visible = true,
				Content = new Label
				{
					HorizontalAlignment = HorizontalAlignment.Stretch,
					VerticalAlignment = VerticalAlignment.Stretch,
					Text = text
				},
			};
			Grid.SetRow(optionsCheckBox, Children.Count);

			optionsCheckBox.IsCheckedChanged += (s, a) =>
			{
				if (optionsCheckBox.IsChecked)
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