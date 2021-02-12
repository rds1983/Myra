using System;
using System.Drawing;
using System.Numerics;
using Myra.Platform;
using TrippyGL;

namespace Myra.Samples.AllWidgets
{
	internal class TrippyRenderer: IMyraRenderer
	{
		private bool _beginCalled;
		
		private readonly GraphicsDevice _device;
		private readonly SimpleShaderProgram _shaderProgram;
		private readonly TextureBatcher _batch;

		private Matrix3x2? _transform;
		private bool _oldScissorEnabled;

		public Rectangle Scissor
		{
			get
			{
				var rect = _device.ScissorRectangle;

				rect.X -= _device.Viewport.X;
				rect.Y -= _device.Viewport.Y;

				return rect.ToSystemDrawing();
			}

			set
			{
				Flush();
				value.X += _device.Viewport.X;
				value.Y += _device.Viewport.Y;
				_device.ScissorRectangle = value.ToTrippy();
			}
		}


		public TrippyRenderer(GraphicsDevice graphicsDevice)
		{
			if (graphicsDevice == null)
			{
				throw new ArgumentNullException(nameof(graphicsDevice));
			}

			_device = graphicsDevice;

			_shaderProgram = SimpleShaderProgram.Create<VertexColorTexture>(graphicsDevice, 0, 0, true);
			_batch = new TextureBatcher(_device);
			_batch.SetShaderProgram(_shaderProgram);

			AllWidgetsTest.Instance.SizeChanged += Instance_SizeChanged;
		}

		private void Instance_SizeChanged(object sender, EventArgs e)
		{
			_shaderProgram.Projection = Matrix4x4.CreateOrthographicOffCenter(0, _device.Viewport.Width, _device.Viewport.Height, 0, 0, 1);
		}

		public void Begin(Matrix3x2? transform)
		{
			_batch.Begin();

			_oldScissorEnabled = _device.ScissorTestEnabled;
//			_device.ScissorTestEnabled = true;

			_beginCalled = true;
			_transform = transform;
		}

		public void End()
		{
			_batch.End();
			_beginCalled = false;

			_device.DepthState = DepthState.None;
			_device.FaceCullingEnabled = false;
			_device.BlendState = BlendState.AlphaBlend;
			//			_device.ScissorTestEnabled = _oldScissorEnabled;

		}

		public void Draw(object texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, float depth)
		{
			var xnaTexture = (Texture2D)texture;

			_batch.Draw(xnaTexture,
				position,
				sourceRectangle,
				color.ToTrippy(),
				scale,
				rotation,
				origin,
				depth);
		}

		public void Draw(object texture, Rectangle dest, Rectangle? src, Color color)
		{
			var xnaTexture = (Texture2D)texture;

			Vector2 srcSize = src != null ? new Vector2(src.Value.Width, src.Value.Height) : new Vector2(xnaTexture.Width, xnaTexture.Height);

			Vector2 scale = new Vector2(dest.Width / srcSize.X,
				dest.Height / srcSize.Y);

			_batch.Draw(xnaTexture,
				new Vector2(dest.X, dest.Y),
				src,
				color.ToTrippy(),
				scale, 
				0);
		}

		private void Flush()
		{
			if (_beginCalled)
			{
				End();
				Begin(_transform);
			}
		}
	}
}
