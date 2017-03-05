using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.UI;
using Myra.Graphics3D;
using Myra.Utility;

namespace Myra.Samples.Utility
{
	public class StatisticsGrid3d : GridBased
	{
		private readonly FPSCounter _fpsCounter = new FPSCounter();

		private readonly CheckBox _lightsOn;
		private readonly CheckBox _drawNormals;
		private readonly TextBlock _gcMemoryLabel;
		private readonly TextBlock _processMemoryLabel;
		private readonly TextBlock _fpsLabel;
		private readonly TextBlock _drawCallsLabel;
		private readonly TextBlock _modelsCountLabel;
		private readonly TextBlock _primitiveCountLabel;
		private readonly TextBlock _textureCountLabel;
		private readonly TextBlock _vertexShaderCountLabel;
		private readonly TextBlock _pixelShaderCountLabel;

		public bool IsLightningOn
		{
			get { return _lightsOn.IsPressed; }
		}

		public RenderFlags RenderFlags
		{
			get
			{
				var result = RenderFlags.None;

				if (_drawNormals.IsPressed)
				{
					result |= RenderFlags.DrawNormals;
				}

				return result;
			}
		}

		public StatisticsGrid3d()
		{
			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Bottom;
			XHint = 10;
			YHint = -10;

			RowsProportions.Add(new Proportion());
			RowsProportions.Add(new Proportion());
			RowsProportions.Add(new Proportion());
			RowsProportions.Add(new Proportion());
			RowsProportions.Add(new Proportion());
			RowsProportions.Add(new Proportion());
			RowsProportions.Add(new Proportion());
			RowsProportions.Add(new Proportion());
			RowsProportions.Add(new Proportion());
			RowsProportions.Add(new Proportion());
			RowsProportions.Add(new Proportion());

			_lightsOn = new CheckBox
			{
				IsPressed = true,
				Text = "Lighting On"
			};
			Widgets.Add(_lightsOn);

			_drawNormals = new CheckBox
			{
				IsPressed = false,
				Text = "Draw Normals",
				GridPositionY = 1
			};
			Widgets.Add(_drawNormals);

			_gcMemoryLabel = new TextBlock
			{
				Text = "GC Memory: ",
				Font = DefaultAssets.FontSmall,
				GridPositionY = 2
			};
			Widgets.Add(_gcMemoryLabel);

			_processMemoryLabel = new TextBlock
			{
				Text = "Process Memory: ",
				Font = DefaultAssets.FontSmall,
				GridPositionY = 3
			};
			Widgets.Add(_processMemoryLabel);

			_fpsLabel = new TextBlock
			{
				Text = "FPS: ",
				Font = DefaultAssets.FontSmall,
				GridPositionY = 4
			};

			Widgets.Add(_fpsLabel);

			_drawCallsLabel = new TextBlock
			{
				Text = "Draw Calls: ",
				Font = DefaultAssets.FontSmall,
				GridPositionY = 5
			};

			Widgets.Add(_drawCallsLabel);

			_modelsCountLabel = new TextBlock
			{
				Text = "Models Count: ",
				Font = DefaultAssets.FontSmall,
				GridPositionY = 6
			};

			Widgets.Add(_modelsCountLabel);

			_primitiveCountLabel = new TextBlock
			{
				Text = "Draw Calls: ",
				Font = DefaultAssets.FontSmall,
				GridPositionY = 7
			};

			Widgets.Add(_primitiveCountLabel);

			_textureCountLabel = new TextBlock
			{
				Text = "Draw Calls: ",
				Font = DefaultAssets.FontSmall,
				GridPositionY = 8
			};

			Widgets.Add(_textureCountLabel);

			_vertexShaderCountLabel = new TextBlock
			{
				Text = "Draw Calls: ",
				Font = DefaultAssets.FontSmall,
				GridPositionY = 9
			};

			Widgets.Add(_vertexShaderCountLabel);

			_pixelShaderCountLabel = new TextBlock
			{
				Text = "Draw Calls: ",
				Font = DefaultAssets.FontSmall,
				GridPositionY = 10
			};

			Widgets.Add(_pixelShaderCountLabel);
		}

		public void Update(GraphicsDevice device, Scene scene)
		{
			_fpsCounter.Update();

			_gcMemoryLabel.Text = string.Format("GC Memory: {0} kb", GC.GetTotalMemory(false) / 1024);
			_processMemoryLabel.Text = string.Format("Process Memory: {0} kb",
				Process.GetCurrentProcess().PrivateMemorySize64 / 1024);
			_fpsLabel.Text = string.Format("FPS: {0:0.##}", _fpsCounter.FPS);
			_drawCallsLabel.Text = string.Format("Draw Calls: {0}", device.Metrics.DrawCount);
			_modelsCountLabel.Text = string.Format("Models Count: {0}", scene.ModelsRendered);
			_primitiveCountLabel.Text = string.Format("Primitive Count: {0}", device.Metrics.PrimitiveCount);
			_textureCountLabel.Text = string.Format("Texture Count: {0}", device.Metrics.TextureCount);
			_vertexShaderCountLabel.Text = string.Format("Vertex Shader Count: {0}", device.Metrics.VertexShaderCount);
			_pixelShaderCountLabel.Text = string.Format("Pixel Shader Count: {0}", device.Metrics.PixelShaderCount);			
		}
	}
}