using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Myra
{
	public static class MyraEnvironment
	{
		public static bool IsWindowsDX { get; set; }

		public static Game Game { get; set; }

		public static GraphicsDevice GraphicsDevice
		{
			get
			{
				return Game.GraphicsDevice;
			}
		}
	}
}