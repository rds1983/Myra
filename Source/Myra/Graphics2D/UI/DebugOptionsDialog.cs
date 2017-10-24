using System;
using Myra.Graphics2D.UI;

namespace Myra.Graphics2D.UI
{
	public class DebugOptionsDialog : Window
	{
		private readonly DebugOptionsWidget _optionsWidget = new DebugOptionsWidget();

		public bool ShowDebugInfo { get; set; }

		public DebugOptionsDialog()
		{
			Title = "UI Debug Options";
			Content = _optionsWidget;

			_optionsWidget._checkBoxWidgetFrames.IsPressed = MyraEnvironment.DrawWidgetsFrames;
			_optionsWidget._checkBoxWidgetFrames.Down += (s, a) =>
			{
				MyraEnvironment.DrawWidgetsFrames = true;
			};
			_optionsWidget._checkBoxWidgetFrames.Up += (s, a) =>
			{
				MyraEnvironment.DrawWidgetsFrames = false;
			};

			_optionsWidget._checkBoxFocusedWidgetFrame.IsPressed = MyraEnvironment.DrawFocusedWidgetFrame;
			_optionsWidget._checkBoxFocusedWidgetFrame.Down += (s, a) =>
			{
				MyraEnvironment.DrawFocusedWidgetFrame = true;
			};
			_optionsWidget._checkBoxFocusedWidgetFrame.Up += (s, a) =>
			{
				MyraEnvironment.DrawFocusedWidgetFrame = false;
			};

			_optionsWidget._checkBoxGlyphFrames.IsPressed = MyraEnvironment.DrawTextGlyphsFrames;
			_optionsWidget._checkBoxGlyphFrames.Down += (s, a) =>
			{
				MyraEnvironment.DrawTextGlyphsFrames = true;
			};
			_optionsWidget._checkBoxGlyphFrames.Up += (s, a) =>
			{
				MyraEnvironment.DrawTextGlyphsFrames = false;
			};

			_optionsWidget._checkBoxDisableClipping.IsPressed = MyraEnvironment.DisableClipping;
			_optionsWidget._checkBoxDisableClipping.Down += (s, a) =>
			{
				MyraEnvironment.DisableClipping = true;
			};
			_optionsWidget._checkBoxDisableClipping.Up += (s, a) =>
			{
				MyraEnvironment.DisableClipping = false;
			};
		}

		public void AddOption(string text, Action onEnabled, Action onDisabled)
		{
			var optionsCheckBox = new CheckBox();
			optionsCheckBox.Text = text;
			optionsCheckBox.ContentHorizontalAlignment = HorizontalAlignment.Stretch;
			optionsCheckBox.ContentVerticalAlignment = VerticalAlignment.Stretch;
			optionsCheckBox.Enabled = true;
			optionsCheckBox.HorizontalAlignment = HorizontalAlignment.Left;
			optionsCheckBox.VerticalAlignment = VerticalAlignment.Top;
			optionsCheckBox.GridPositionY = _optionsWidget.Widgets.Count;
			optionsCheckBox.Visible = true;
			optionsCheckBox.Down += (s, a) => onEnabled();
			optionsCheckBox.Up += (s, a) => onDisabled();

			_optionsWidget.RowsProportions.Add(new Proportion
			{
				Type = Myra.Graphics2D.UI.Grid.ProportionType.Auto
			});

			_optionsWidget.Widgets.Add(optionsCheckBox);
		}
	}
}