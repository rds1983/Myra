using AssetManagementBase;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using NUnit.Framework;

namespace Myra.Tests
{
	[TestFixture]
	public class MMLTests
	{
		[Test]
		public void LoadMMLWithExternalAssets()
		{
			var assembly = typeof(MMLTests).Assembly;
			ResourceAssetResolver assetResolver = new ResourceAssetResolver(assembly, "Resources.");
			AssetManager assetManager = new AssetManager(assetResolver);

			var mml = assetManager.Load<string>("GridWithExternalResources.xmmp");

			var project = Project.LoadFromXml(mml, assetManager);

			var imageButton1 = (ImageButton)project.Root.FindWidgetById("spawnUnit1");
			Assert.IsNotNull(imageButton1);
			Assert.IsNotNull(imageButton1.Image);
			Assert.AreEqual(imageButton1.Image.Size, new Point(64, 64));

			var label = (Label)project.Root.FindWidgetById("label");
			Assert.IsNotNull(label);
			Assert.IsNotNull(label.Font);
		}
	}
}
