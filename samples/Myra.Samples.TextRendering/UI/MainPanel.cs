using FontStashSharp;
using Microsoft.Xna.Framework;
using Myra.Graphics2D;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.File;
using System;
using System.IO;
using System.Linq;

namespace Myra.Samples.TextRendering.UI
{
	public partial class MainPanel
	{
		private FontSystem _fontSystem;

		private FontSystem FontSystem
		{
			get => _fontSystem;
			set
			{
				if (value == _fontSystem)
				{
					return;
				}

				_fontSystem = value;
				_imageTexture.Renderable = null;
			}
		}

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
			_checkBoxShowTexture.PressedChanged += (s, a) => Update();

			_spinButtonResolutionFactor.ValueChanged += _spinButtonResolutionFactor_ValueChanged;
			_spinButtonKernelWidth.ValueChanged += _spinButtonResolutionFactor_ValueChanged;
			_spinButtonKernelHeight.ValueChanged += _spinButtonResolutionFactor_ValueChanged;

			_buttonReset.Click += _buttonReset_Click;
			_buttonBrowseFont.Click += _buttonBrowseFont_Click;
		}

		private void _spinButtonResolutionFactor_ValueChanged(object sender, Utility.ValueChangedEventArgs<float?> e)
		{
			FontSystem = null;
			Update();
		}

		private void _buttonReset_Click(object sender, EventArgs e)
		{
			_sliderScale.Value = 1.0f;
			_spinButtonFontSize.Value = 32;
			_spinButtonResolutionFactor.Value = 1.0f;
			_spinButtonKernelWidth.Value = 0;
			_spinButtonKernelHeight.Value = 0;
			FontSystem = null;
			_textBoxFont.Text = "(default)";
			Update();
		}

		private FontSystem LoadFontSystem(byte[] data)
		{
			var settings = new FontSystemSettings
			{
				FontResolutionFactor = _spinButtonResolutionFactor.Value.Value,
				KernelWidth = (int)_spinButtonKernelWidth.Value.Value,
				KernelHeight = (int)_spinButtonKernelHeight.Value.Value
			};

			var result = new FontSystem(settings);
			result.AddFont(data);

			return result;
		}

		private void _buttonBrowseFont_Click(object sender, EventArgs e)
		{
			var dialog = new FileDialog(FileDialogMode.OpenFile)
			{
				Filter = "*.ttf|*.otf|*.ttc"
			};

			dialog.Closed += (s, a) =>
			{
				if (!dialog.Result)
				{
					return;
				}

				try
				{
					var fontSystem = LoadFontSystem(File.ReadAllBytes(dialog.FilePath));
					_textBoxFont.Text = dialog.FilePath;

					FontSystem = fontSystem;
					Update();
				}
				catch(Exception ex)
				{
					var messageBox = Dialog.CreateMessageBox("Error", ex.Message);
					messageBox.ShowModal(Desktop);
				}
			};


			dialog.ShowModal(Desktop);
		}

		private void Update()
		{
			var game = TextRenderingGame.Instance;

			if (FontSystem == null)
			{
				byte[] data;

				using(var stream = DefaultAssets.OpenDefaultFontDataStream())
				using(var ms = new MemoryStream())
				{
					stream.CopyTo(ms);
					data = ms.ToArray();
				}

				FontSystem = LoadFontSystem(data);
			}

			game.LabelText.Text = _textBoxText.Text;
			game.LabelText.Font = FontSystem.GetFont((int)_spinButtonFontSize.Value.Value);

			var scale = _sliderScale.Value;
			game.TopDesktop.Scale = new Vector2(scale, scale);
			_labelScaleValue.Text = scale.ToString("0.00");

			_imageTexture.Visible = _checkBoxShowTexture.IsChecked;
		}

		public override void InternalRender(RenderContext context)
		{
			base.InternalRender(context);

			if (_imageTexture.Renderable == null && _imageTexture.Visible)
			{
				var texture = FontSystem.EnumerateTextures().FirstOrDefault();
				if (texture != null)
				{
					_imageTexture.Renderable = new TextureRegion(texture);
				}
			}
		}
	}
}