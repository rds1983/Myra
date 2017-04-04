using System.Drawing;

namespace Myra.UIEditor.Utils
{
	public static class GraphicsExtensions
	{
		public static Color ToSystemDrawing(this Microsoft.Xna.Framework.Color color)
		{
			return Color.FromArgb(color.A, color.R, color.G, color.B);
		}

		public static Microsoft.Xna.Framework.Color ToXna(this Color color)
		{
			return new Microsoft.Xna.Framework.Color(color.R, color.G, color.B, color.A);
		}
	}
}
