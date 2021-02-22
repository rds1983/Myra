using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;

namespace Myra.Tests
{
	[TestFixture]
	public class BaseTests
	{
		private TestGame _game;

		public GraphicsDevice GraphicsDevice => _game.GraphicsDevice;

		[SetUp]
		public void SetUp()
		{
			_game = new TestGame();
			MyraEnvironment.Game = _game;
		}
	}
}
