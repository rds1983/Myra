using AssetManagementBase;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Myra.Events;
using Myra.Graphics2D;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.File;
using System;
using System.IO;

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

			_comboRasterizer.SelectedIndex = 0;
			_comboRasterizer.SelectedIndexChanged += (s, a) =>
			{
				FontSystem = null;
				Update();
			};

			_comboRenderer.SelectedIndex = 0;
			_comboRenderer.SelectedIndexChanged += (s, a) =>
			{
				FontSystem = null;
				Update();
			};
		}

		private void _spinButtonResolutionFactor_ValueChanged(object sender, ValueChangedEventArgs<float?> e)
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

			switch (_comboRasterizer.SelectedIndex)
			{
				case 1:
					settings.StbTrueTypeUseOldRasterizer = true;
					break;
			}

			switch (_comboRenderer.SelectedIndex)
			{
				case 1:
					settings.GlyphRenderer = (input, output, options) =>
					{
						var size = options.Size.X * options.Size.Y;

						for (var i = 0; i < size; i++)
						{
							var c = input[i];
							var ci = i * 4;

							if (c == 0)
							{
								output[ci] = output[ci + 1] = output[ci + 2] = output[ci + 3] = 0;
							}
							else
							{
								output[ci] = output[ci + 1] = output[ci + 2] = output[ci + 3] = 255;
							}
						}
					};
					break;
			}

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
				catch (Exception ex)
				{
					var messageBox = Dialog.CreateMessageBox("Error", ex.Message);
					messageBox.ShowModal(Desktop);
				}
			};


			dialog.ShowModal(Desktop);
		}

		private void Update()
		{
			if (FontSystem == null)
			{
				var assembly = typeof(MainPanel).Assembly;
				var assetManager = AssetManager.CreateResourceAssetManager(assembly, "Resources");

				byte[] data;

				if (string.IsNullOrEmpty(_textBoxFont.Text) || _textBoxFont.Text == "(default)")
				{
					using (var stream = assetManager.Open("Inter-Regular.ttf"))
					using (var ms = new MemoryStream())
					{
						stream.CopyTo(ms);
						data = ms.ToArray();
					}
				}
				else
				{
					data = File.ReadAllBytes(_textBoxFont.Text);
				}

				FontSystem = LoadFontSystem(data);
			}

			_labelText.Text = _textBoxText.Text;
			_labelText.Font = FontSystem.GetFont((int)_spinButtonFontSize.Value.Value);

			var scale = _sliderScale.Value;
			_labelText.Scale = new Vector2(scale, scale);
			_labelScaleValue.Text = scale.ToString("0.00");

			_imageTexture.Visible = _checkBoxShowTexture.IsChecked;
		}

		public override void InternalRender(RenderContext context)
		{
			base.InternalRender(context);

			if (_imageTexture.Renderable == null && _imageTexture.Visible && FontSystem.Atlases.Count > 0)
			{
				var texture = FontSystem.Atlases[0].Texture;
				if (texture != null)
				{
					_imageTexture.Renderable = new TextureRegion(texture);
				}
			}
		}
	}
}