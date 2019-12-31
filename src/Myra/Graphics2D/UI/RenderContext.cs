using System;

#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Xenko.Core.Mathematics;
using Xenko.Graphics;
#endif

namespace Myra.Graphics2D.UI
{
	public class RenderContext
	{
		private SpriteBatchBeginParams _spriteBatchBeginParams = new SpriteBatchBeginParams
		{
			SpriteSortMode = SpriteSortMode.Deferred,
#if !XENKO
			RasterizerState = DefaultAssets.UIRasterizerState,
			BlendState = BlendState.NonPremultiplied,
			SamplerState = SamplerState.PointClamp,
			DepthStencilState = null
#else
			BlendState = BlendStates.AlphaBlend,
			SamplerState = MyraEnvironment.Game.GraphicsDevice.SamplerStates.PointClamp,
			DepthStencilState = DepthStencilStates.Default
#endif
		};

		public SpriteBatch Batch { get; set; }
		public Rectangle View { get; set; }
		public float Opacity { get; set; }

		internal SpriteBatchBeginParams SpriteBatchBeginParams
		{
			get
			{
				return _spriteBatchBeginParams;
			}

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}

				_spriteBatchBeginParams = value;
			}
		}

#if XENKO
		internal RenderContext()
		{
			var rs = new RasterizerStateDescription();
			rs.SetDefault();
			rs.ScissorTestEnable = true;

			_spriteBatchBeginParams.RasterizerState = rs;
		}
#endif

		/// <summary>
		/// Draws texture region taking into account the context transformations
		/// </summary>
		/// <param name="brush"></param>
		/// <param name="rectangle"></param>
		/// <param name="color"></param>
		public void Draw(IBrush brush, Rectangle rectangle, Color? color = null)
		{
			var c = color != null ? color.Value : Color.White;
			brush.Draw(Batch, rectangle, c * Opacity);
		}

		/// <summary>
		/// Draws texture region taking into account the context transformations
		/// </summary>
		/// <param name="image"></param>
		/// <param name="rectangle"></param>
		/// <param name="color"></param>
		public void Draw(IImage image, Point location, Color? color = null)
		{
			var c = color != null ? color.Value : Color.White;
			image.Draw(Batch, new Rectangle(location.X, location.Y, image.Size.X, image.Size.Y), c * Opacity);
		}

		/// <summary>
		/// Draws rectangle taking into account the context transformations
		/// </summary>
		/// <param name="rectangle"></param>
		/// <param name="color"></param>
		public void DrawRectangle(Rectangle rectangle, Color color)
		{
			Batch.DrawRectangle(rectangle, color);
		}

		/// <summary>
		/// Draws a filled rectangle taking into account the context transformations
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="rectangle">The rectangle to draw</param>
		/// <param name="color">The color to draw the rectangle in</param>
		public void FillRectangle(Rectangle rectangle, Color color)
		{
			Batch.FillRectangle(rectangle, color * Opacity);
		}

		internal void Begin()
		{
#if MONOGAME
			Batch.Begin(SpriteBatchBeginParams.SpriteSortMode,
				SpriteBatchBeginParams.BlendState,
				SpriteBatchBeginParams.SamplerState,
				SpriteBatchBeginParams.DepthStencilState,
				SpriteBatchBeginParams.RasterizerState,
				SpriteBatchBeginParams.Effect,
				SpriteBatchBeginParams.TransformMatrix);
#elif FNA
			Batch.Begin(SpriteBatchBeginParams.SpriteSortMode,
				SpriteBatchBeginParams.BlendState,
				SpriteBatchBeginParams.SamplerState,
				SpriteBatchBeginParams.DepthStencilState,
				SpriteBatchBeginParams.RasterizerState,
				SpriteBatchBeginParams.Effect,
				SpriteBatchBeginParams.TransformMatrix != null ? SpriteBatchBeginParams.TransformMatrix.Value : Matrix.Identity);
#elif XENKO
			Batch.Begin(MyraEnvironment.Game.GraphicsContext,
				SpriteBatchBeginParams.SpriteSortMode,
				SpriteBatchBeginParams.BlendState,
				SpriteBatchBeginParams.SamplerState,
				SpriteBatchBeginParams.DepthStencilState,
				SpriteBatchBeginParams.RasterizerState);
#endif
		}

		internal void End()
		{
			Batch.End();
		}

		internal void Flush()
		{
			End();
			Begin();
		}
	}
}