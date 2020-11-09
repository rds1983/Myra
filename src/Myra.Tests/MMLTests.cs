using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.UI;
using NUnit.Framework;
using XNAssets;

namespace Myra.Tests
{
	[TestFixture]
	public class MMLTests
	{
		private TestGame _game;

		public GraphicsDevice GraphicsDevice => _game.GraphicsDevice;

		[SetUp]
		public void SetUp()
		{
			_game = new TestGame();
			MyraEnvironment.Game = _game;
		}

		[Test]
		public void LoadMMLWithExternalAssets()
		{
			ResourceAssetResolver assetResolver = new ResourceAssetResolver(typeof(MMLTests).Assembly, "Resources.");
			AssetManager assetManager = new AssetManager(GraphicsDevice, assetResolver);

			var mml = assetManager.Load<string>("GridWithExternalResources.xmmp");

			var project = Project.LoadFromXml(mml, assetManager);

			var imageButton1 = (ImageButton)project.Root.FindWidgetById("spawnUnit1");
			Assert.IsNotNull(imageButton1);
			Assert.IsNotNull(imageButton1.Image);
			Assert.AreEqual(imageButton1.Image.Size, new Point(64, 64));

			var label = (Label)project.Root.FindWidgetById("label");
			Assert.IsNotNull(label);
			Assert.IsNotNull(label.Font);
			#if MONOGAME
			var texture = label.Font.Texture;
			#else
			var fi = typeof(SpriteFont).GetField("textureValue", BindingFlags.NonPublic | BindingFlags.Instance);
			var texture = (Texture2D) fi.GetValue(label.Font);
			#endif
			Assert.AreEqual(new Point(texture.Width, texture.Height), new Point(512, 512));
			Assert.AreEqual(label.Font.Characters.Count, 191);
		}
	}
}
