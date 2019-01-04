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

			_checkBoxWidgetFrames.IsToggled = MyraEnvironment.DrawWidgetsFrames;
			_checkBoxWidgetFrames.ToggledChanged += (s, a) =>
			{
				MyraEnvironment.DrawWidgetsFrames = _checkBoxWidgetFrames.IsToggled;
			};

			_checkBoxFocusedWidgetFrame.IsToggled = MyraEnvironment.DrawFocusedWidgetFrame;
			_checkBoxFocusedWidgetFrame.ToggledChanged += (s, a) =>
			{
				MyraEnvironment.DrawFocusedWidgetFrame = _checkBoxFocusedWidgetFrame.IsToggled;
			};

			_checkBoxGlyphFrames.IsToggled = MyraEnvironment.DrawTextGlyphsFrames;
			_checkBoxGlyphFrames.ToggledChanged += (s, a) =>
			{
				MyraEnvironment.DrawTextGlyphsFrames = _checkBoxGlyphFrames.IsToggled;
			};

			_checkBoxDisableClipping.IsToggled = MyraEnvironment.DisableClipping;
			_checkBoxDisableClipping.ToggledChanged += (s, a) =>
			{
				MyraEnvironment.DisableClipping = _checkBoxDisableClipping.IsToggled;
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
			optionsCheckBox.GridPositionY = InternalChild.Widgets.Count;
			optionsCheckBox.Visible = true;
			optionsCheckBox.ToggledChanged += (s, a) =>
			{
				if (optionsCheckBox.IsToggled)
				{
					onEnabled();
				}
				else
				{
					onDisabled();
				}
			};
			optionsCheckBox.GridPositionY = Root.RowsProportions.Count;

			Root.RowsProportions.Add(new Proportion
			{
				Type = ProportionType.Auto
			});

			Root.Widgets.Add(optionsCheckBox);
		}
	}
}