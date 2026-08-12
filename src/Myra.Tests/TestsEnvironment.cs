using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Xunit;

namespace Myra.Tests
{
	public class TestsEnvironment : IAsyncLifetime
	{
		private TestGame _game;

		public GraphicsDevice GraphicsDevice => _game.GraphicsDevice;

		async Task IAsyncLifetime.InitializeAsync()
		{
			_game = new TestGame();
			MyraEnvironment.Game = _game;
			await Task.CompletedTask;
		}

		async Task IAsyncLifetime.DisposeAsync()
		{
			_game?.Dispose();
			await Task.CompletedTask;
		}
	}

	[CollectionDefinition("Myra Tests")]
	public class TestsEnvironmentCollection : ICollectionFixture<TestsEnvironment>
	{
	}
}
