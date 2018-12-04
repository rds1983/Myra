using System;
using static Myra.Graphics2D.UI.Grid;

namespace Myra.Graphics2D.UI
{
	public partial class DebugOptionsDialog : Dialog
	{
		public bool ShowDebugInfo { get; set; }

		public DebugOptionsDialog()
		{
			Title = "UI Debug Options";

			BuildUI();

			_checkBoxWidgetFrames.IsPressed = MyraEnvironment.DrawWidgetsFrames;
			_checkBoxWidgetFrames.Down += (s, a) =>
			{
				MyraEnvironment.DrawWidgetsFrames = true;
			};
			_checkBoxWidgetFrames.Up += (s, a) =>
			{
				MyraEnvironment.DrawWidgetsFrames = false;
			};

			_checkBoxFocusedWidgetFrame.IsPressed = MyraEnvironment.DrawFocusedWidgetFrame;
			_checkBoxFocusedWidgetFrame.Down += (s, a) =>
			{
				MyraEnvironment.DrawFocusedWidgetFrame = true;
			};
			_checkBoxFocusedWidgetFrame.Up += (s, a) =>
			{
				MyraEnvironment.DrawFocusedWidgetFrame = false;
			};

			_checkBoxGlyphFrames.IsPressed = MyraEnvironment.DrawTextGlyphsFrames;
			_checkBoxGlyphFrames.Down += (s, a) =>
			{
				MyraEnvironment.DrawTextGlyphsFrames = true;
			};
			_checkBoxGlyphFrames.Up += (s, a) =>
			{
				MyraEnvironment.DrawTextGlyphsFrames = false;
			};

			_checkBoxDisableClipping.IsPressed = MyraEnvironment.DisableClipping;
			_checkBoxDisableClipping.Down += (s, a) =>
			{
				MyraEnvironment.DisableClipping = true;
			};
			_checkBoxDisableClipping.Up += (s, a) =>
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
			optionsCheckBox.GridPositionY = Widget.Widgets.Count;
			optionsCheckBox.Visible = true;
			optionsCheckBox.Down += (s, a) => onEnabled();
			optionsCheckBox.Up += (s, a) => onDisabled();
			optionsCheckBox.GridPositionY = Root.RowsProportions.Count;

			Root.RowsProportions.Add(new Proportion
			{
				Type = ProportionType.Auto
			});

			Root.Widgets.Add(optionsCheckBox);
		}
	}
}