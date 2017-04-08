using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Myra.Graphics2D.UI;
using Myra.Graphics3D;

namespace Myra.Samples.Graphics3D.Utility
{
	public class StatisticsGrid3d : GridBased
	{
		private readonly FramesPerSecondCounter _fpsCounter = new FramesPerSecondCounter();

		private readonly CheckBox _lightningOn;
		private readonly CheckBox _drawNormals;
		private readonly CheckBox _drawBoundingBoxes;
		private readonly TextBlock _gcMemoryLabel;
		private readonly TextBlock _fpsLabel;
		private readonly TextBlock _drawCallsLabel;
		private readonly TextBlock _modelsCountLabel;
		private readonly TextBlock _primitiveCountLabel;
		private readonly TextBlock _textureCountLabel;
		private readonly TextBlock _vertexShaderCountLabel;
		private readonly TextBlock _pixelShaderCountLabel;

		public bool IsLightningOn
		{
			get { return _lightningOn.IsPressed; }
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

				if (_drawBoundingBoxes.IsPressed)
				{
					result |= RenderFlags.DrawBoundingBoxes;
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

			_lightningOn = new CheckBox
			{
				IsPressed = true,
				Text = "Lighting On"
			};
			Widgets.Add(_lightningOn);

			_drawNormals = new CheckBox
			{
				IsPressed = false,
				Text = "Draw Normals",
				GridPositionY = 1
			};
			Widgets.Add(_drawNormals);

			_drawBoundingBoxes = new CheckBox
			{
				IsPressed = false,
				Text = "Draw Bounding Boxes",
				GridPositionY = 2
			};
			Widgets.Add(_drawBoundingBoxes);

			_gcMemoryLabel = new TextBlock
			{
				Text = "GC Memory: ",
				Font = DefaultAssets.FontSmall,
				GridPositionY = 3
			};
			Widgets.Add(_gcMemoryLabel);

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

		public void Update(GameTime gameTime)
		{
			_fpsCounter.Update(gameTime);
		}

		public void Draw(GameTime gameTime, GraphicsDevice device, Scene scene)
		{
			_fpsCounter.Draw(gameTime);

			_gcMemoryLabel.Text = string.Format("GC Memory: {0} kb", GC.GetTotalMemory(false) / 1024);
			_fpsLabel.Text = string.Format("FPS: {0}", _fpsCounter.FramesPerSecond);
			_drawCallsLabel.Text = string.Format("Draw Calls: {0}", device.Metrics.DrawCount);
			_modelsCountLabel.Text = string.Format("Models Count: {0}", scene.ModelsRendered);
			_primitiveCountLabel.Text = string.Format("Primitive Count: {0}", device.Metrics.PrimitiveCount);
			_textureCountLabel.Text = string.Format("Texture Count: {0}", device.Metrics.TextureCount);
			_vertexShaderCountLabel.Text = string.Format("Vertex Shader Count: {0}", device.Metrics.VertexShaderCount);
			_pixelShaderCountLabel.Text = string.Format("Pixel Shader Count: {0}", device.Metrics.PixelShaderCount);
		}

	}
}