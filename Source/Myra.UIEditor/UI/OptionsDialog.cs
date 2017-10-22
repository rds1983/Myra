using Myra.Graphics2D.UI;

namespace Myra.UIEditor.UI
{
	public class OptionsDialog : Window
	{
		private readonly OptionsWidget _optionsWidget = new OptionsWidget();

		public OptionsDialog()
		{
			Title = "Options";
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

			_optionsWidget._checkBoxShowDebugInfo.IsPressed = Studio.Instance.ShowDebugInfo;
			_optionsWidget._checkBoxShowDebugInfo.Down += (s, a) =>
			{
				Studio.Instance.ShowDebugInfo = true;
			};
			_optionsWidget._checkBoxShowDebugInfo.Up += (s, a) =>
			{
				Studio.Instance.ShowDebugInfo = false;
			};

			_optionsWidget._checkBoxDisableScissors.IsPressed = Studio.Instance.ShowDebugInfo;
			_optionsWidget._checkBoxDisableScissors.Down += (s, a) =>
			{
				MyraEnvironment.DisableClipping = true;
			};
			_optionsWidget._checkBoxDisableScissors.Up += (s, a) =>
			{
				MyraEnvironment.DisableClipping = false;
			};
		}
	}
}