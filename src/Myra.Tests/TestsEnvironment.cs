using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;

namespace Myra.Tests
{
	[SetUpFixture]
	public class TestsEnvironment
	{
		private TestGame _game;

		public GraphicsDevice GraphicsDevice => _game.GraphicsDevice;

		[OneTimeSetUp]
		public void SetUp()
		{
			_game = new TestGame();
			MyraEnvironment.Game = _game;
		}
	}
}
