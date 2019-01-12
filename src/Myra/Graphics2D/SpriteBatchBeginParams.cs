#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Xenko.Core.Mathematics;
using Xenko.Graphics;
using BlendState = Xenko.Graphics.BlendStateDescription;
using DepthStencilState = Xenko.Graphics.DepthStencilStateDescription;
using RasterizerState = Xenko.Graphics.RasterizerStateDescription;
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
