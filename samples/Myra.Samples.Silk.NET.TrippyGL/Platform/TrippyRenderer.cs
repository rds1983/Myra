using System;
using System.Drawing;
using System.Numerics;
using FontStashSharp;
using FontStashSharp.Interfaces;
using Myra.Graphics2D;
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

		private TextureFiltering _filtering;

		private DepthState _oldDepthState;
		private bool _oldFaceCullingEnabled;
		private BlendState _oldBlendState;
		private bool _oldScissorEnabled;
		private Rectangle _scissorRectangle;

		public RendererType RendererType => RendererType.Sprite;

		public Rectangle Scissor
		{
			get
			{
				return _scissorRectangle;
			}

			set
			{
				Flush();

				value.X += _device.Viewport.X;
				value.Y += _device.Viewport.Y;

				// TripplyGL Scissor Rect has y-axis facing upwards
				// Hence we require some transforms
				var result = new Viewport(value.X, (int)(_device.Viewport.Height - value.Height - value.Y), (uint)value.Width, (uint)value.Height);
				_device.ScissorRectangle = result;

				_scissorRectangle = value;
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
			OnViewportChanged();
			_batch = new TextureBatcher(_device);
			_batch.SetShaderProgram(_shaderProgram);
		}

		public void OnViewportChanged()
		{
			_shaderProgram.Projection = Matrix4x4.CreateOrthographicOffCenter(0, _device.Viewport.Width, _device.Viewport.Height, 0, 0, 1);
		}

		public void Begin(TextureFiltering textureFiltering)
		{
			// Save old state
			_oldDepthState = _device.DepthState;
			_oldFaceCullingEnabled = _device.FaceCullingEnabled;
			_oldBlendState = _device.BlendState;
			_oldScissorEnabled = _device.ScissorTestEnabled;

			// Set new state
			_device.DepthState = DepthState.None;
			_device.FaceCullingEnabled = false;
			_device.BlendState = BlendState.AlphaBlend;
			_device.ScissorTestEnabled = true;

			_batch.Begin();

			_beginCalled = true;
			_filtering = textureFiltering;
		}

		public void End()
		{
			_batch.End();
			_beginCalled = false;

			// Restore old state
			_device.DepthState = _oldDepthState;
			_device.FaceCullingEnabled = _oldFaceCullingEnabled;
			_device.BlendState = _oldBlendState;
			_device.ScissorTestEnabled = _oldScissorEnabled;
		}

		public void DrawQuad(object texture, ref VertexPositionColorTexture topLeft, ref VertexPositionColorTexture topRight, ref VertexPositionColorTexture bottomLeft, ref VertexPositionColorTexture bottomRight)
		{
			throw new NotImplementedException();
		}

		public void DrawSprite(object texture, Vector2 pos, Rectangle? src, FSColor color, float rotation, Vector2 scale, float depth)
		{
			var tex = (Texture2D)texture;

			_batch.Draw(tex,
				pos,
				src,
				color.ToTrippy(),
				scale,
				rotation,
				Vector2.Zero,
				depth);
		}

		private void Flush()
		{
			if (_beginCalled)
			{
				End();
				Begin(_filtering);
			}
		}
	}
}
