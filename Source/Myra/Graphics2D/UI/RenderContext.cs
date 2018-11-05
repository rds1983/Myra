using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Myra.Graphics2D.UI
{
	public class RenderContext
	{
		private SpriteBatchBeginParams _spriteBatchBeginParams = new SpriteBatchBeginParams
		{
			SpriteSortMode = SpriteSortMode.Deferred,
			BlendState = BlendState.AlphaBlend,
			SamplerState = SamplerState.PointClamp,
			DepthStencilState = null,
			RasterizerState = DefaultAssets.UIRasterizerState
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

		/// <summary>
		/// Draws texture region taking into account the context transformations
		/// </summary>
		/// <param name="renderable"></param>
		/// <param name="rectangle"></param>
		/// <param name="color"></param>
		public void Draw(IRenderable renderable, Rectangle rectangle, Color? color = null)
		{
			var c = color != null ? color.Value : Color.White;
			renderable.Draw(Batch, rectangle, c * Opacity);
		}

		/// <summary>
		/// Draws texture region taking into account the context transformations
		/// </summary>
		/// <param name="renderable"></param>
		/// <param name="rectangle"></param>
		/// <param name="color"></param>
		public void Draw(IRenderable renderable, Point location, Color? color = null)
		{
			var c = color != null ? color.Value : Color.White;
			renderable.Draw(Batch, new Rectangle(location, renderable.Size), c * Opacity);
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
#if !FNA
			Batch.Begin(SpriteBatchBeginParams.SpriteSortMode,
				SpriteBatchBeginParams.BlendState,
				SpriteBatchBeginParams.SamplerState,
				SpriteBatchBeginParams.DepthStencilState,
				SpriteBatchBeginParams.RasterizerState,
				SpriteBatchBeginParams.Effect,
				SpriteBatchBeginParams.TransformMatrix);
#else
			Batch.Begin(SpriteBatchBeginParams.SpriteSortMode,
				SpriteBatchBeginParams.BlendState,
				SpriteBatchBeginParams.SamplerState,
				SpriteBatchBeginParams.DepthStencilState,
				SpriteBatchBeginParams.RasterizerState,
				SpriteBatchBeginParams.Effect,
				SpriteBatchBeginParams.TransformMatrix != null ? SpriteBatchBeginParams.TransformMatrix.Value : Matrix.Identity);
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