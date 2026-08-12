using System;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A window that provides options for debugging the UI framework.
	/// </summary>
	public partial class DebugOptionsWindow
	{
		/// <summary>
		/// Gets or sets a value indicating whether debug information should be displayed.
		/// </summary>
		public bool ShowDebugInfo { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="DebugOptionsWindow"/> class.
		/// </summary>
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

		/// <summary>
		/// Adds a debug option with callbacks for when it's enabled or disabled.
		/// </summary>
		/// <param name="text">The text label for the option.</param>
		/// <param name="onEnabled">The action to call when the option is enabled.</param>
		/// <param name="onDisabled">The action to call when the option is disabled.</param>
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