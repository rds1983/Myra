#if !STRIDE
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Stride.Core.Mathematics;
using Stride.Graphics;
using BlendState = Stride.Graphics.BlendStateDescription;
using DepthStencilState = Stride.Graphics.DepthStencilStateDescription;
using RasterizerState = Stride.Graphics.RasterizerStateDescription;
#endif

namespace Myra.Graphics2D
{
	public class SpriteBatchBeginParams
	{
		private Matrix? _transformMatrix;

		public SpriteSortMode SpriteSortMode { get; set; }
		public BlendState BlendState { get; set; }
		public SamplerState SamplerState { get; set; }
		public DepthStencilState DepthStencilState { get; set; }
		public RasterizerState RasterizerState { get; set; }
		public Effect Effect { get; set; }
		public Matrix? TransformMatrix
		{
			get
			{
				return _transformMatrix;
			}

			set
			{
				if (value == _transformMatrix)
				{
					return;
				}

				_transformMatrix = value;

				if (_transformMatrix != null)
				{
					InverseTransform = Matrix.Invert(_transformMatrix.Value);
				}
			}
		}

		internal Matrix InverseTransform { get; set; }

	}
}
