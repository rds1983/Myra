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
			_checkBoxWidgetFrames.PressedChanged += (s, a) =>
			{
				MyraEnvironment.DrawWidgetsFrames = _checkBoxWidgetFrames.IsPressed;
			};

			_checkBoxFocusedWidgetFrame.IsPressed = MyraEnvironment.DrawFocusedWidgetFrame;
			_checkBoxFocusedWidgetFrame.PressedChanged += (s, a) =>
			{
				MyraEnvironment.DrawFocusedWidgetFrame = _checkBoxFocusedWidgetFrame.IsPressed;
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
			optionsCheckBox.GridRow = InternalChild.Widgets.Count;
			optionsCheckBox.Visible = true;
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
			optionsCheckBox.GridRow = Root.RowsProportions.Count;

			Root.RowsProportions.Add(new Proportion
			{
				Type = ProportionType.Auto
			});

			Root.Widgets.Add(optionsCheckBox);
		}
	}
}