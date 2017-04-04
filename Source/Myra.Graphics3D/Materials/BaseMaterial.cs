using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Myra.Graphics3D.Materials
{
	public class BaseMaterial
	{
		public bool HasLight;
		public Color DiffuseColor;
		public Texture2D Texture;

		public BaseMaterial Clone()
		{
			return new BaseMaterial
					{
						HasLight = HasLight,
						DiffuseColor = DiffuseColor,
						Texture = Texture
					};
		}
	}
}