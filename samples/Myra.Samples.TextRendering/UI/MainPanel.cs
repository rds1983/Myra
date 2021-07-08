using FontStashSharp;
using Microsoft.Xna.Framework;

namespace Myra.Samples.TextRendering.UI
{
	public partial class MainPanel
	{
		public MainPanel()
		{
			BuildUI();

			Update();

			_checkBoxSmoothText.PressedChanged += (s, a) =>
			{
				MyraEnvironment.SmoothText = _checkBoxSmoothText.IsChecked;
			};

			_textBoxText.TextChanged += (s, a) => Update();
			_sliderScale.ValueChanged += (s, a) => Update();
			_spinButtonFontSize.ValueChanged += (s, a) => Update();

			_buttonReset.Click += (s, a) => _sliderScale.Value = 1.0f;
		}

		private void Update()
		{
			var game = TextRenderingGame.Instance;

			game.LabelText.Text = _textBoxText.Text;

			var scale = _sliderScale.Value;
			game.TopDesktop.Transform = Matrix.CreateScale(scale, scale, 0);
			_labelScaleValue.Text = scale.ToString("0.00");

			var fontSystem = ((DynamicSpriteFont)game.LabelText.Font).FontSystem;
			game.LabelText.Font = fontSystem.GetFont((int)_spinButtonFontSize.Value.Value);
		}
	}
}