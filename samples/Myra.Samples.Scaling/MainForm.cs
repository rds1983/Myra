using FontStashSharp;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Samples;

public partial class MainForm
{
	private AllWidgets _allWidgets;

	public MainForm()
	{
		BuildUI();

		_sliderScale.ValueChangedByUser += (s, a) => Update();
		_comboTextScaling.SelectedIndexChanged += (s, a) => RecreateAllWidgets();

		_comboTextScaling.SelectedIndex = 0;
		Update();
	}

	private void RecreateAllWidgets()
	{
		switch (_comboTextScaling.SelectedIndex)
		{
			case 0:
				FontSystemDefaults.FontRasterizationMode = FontRasterizationMode.Standard;
				FontSystemDefaults.FontResolutionFactor = null;
				FontSystemDefaults.KernelWidth = 0;
				FontSystemDefaults.KernelHeight = 0;
				break;

			case 1:
				FontSystemDefaults.FontRasterizationMode = FontRasterizationMode.Standard;
				FontSystemDefaults.FontResolutionFactor = 4.0f;
				FontSystemDefaults.KernelWidth = 4;
				FontSystemDefaults.KernelHeight = 4;
				break;

			case 2:
				FontSystemDefaults.FontRasterizationMode = FontRasterizationMode.SDF;
				FontSystemDefaults.FontResolutionFactor = null;
				FontSystemDefaults.KernelWidth = 0;
				FontSystemDefaults.KernelHeight = 0;
				FontSystemDefaults.FixedSDFFontSize = 64.0f;
				break;
		}

		DefaultAssets.Reset();
		Stylesheet.Current = DefaultAssets.DefaultStylesheet;

		_allWidgets = new AllWidgets
		{
			TransformOrigin = Vector2.Zero
		};

		_panelContainer.Content = _allWidgets;

		Update();
	}

	private void Update()
	{
		_labelScale.Text = _sliderScale.Value.ToString("0.00");
		_allWidgets.Scale = new Vector2((float)_sliderScale.Value);
	}
}