using AssetManagementBase;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using Myra.MML;
using NUnit.Framework;

namespace Myra.Tests
{
	[TestFixture]
	public class MMLTests
	{
		[Test]
		public void LoadMMLWithExternalAssets()
		{
			var assetManager = AssetManager.CreateResourceAssetManager(Utility.Assembly, "Resources.");

			var mml = assetManager.ReadAsString("GridWithExternalResources.xmmp");

			var project = Project.LoadFromXml(mml, assetManager);

			var imageButton1 = (Button)project.Root.FindChildById("spawnUnit1");
			Assert.IsNotNull(imageButton1);

			var image = (Image)imageButton1.Content;
			Assert.IsNotNull(image);
			Assert.IsNotNull(image.Renderable);
			Assert.AreEqual(image.Renderable.Size, new Point(64, 64));

			var label = (Label)project.Root.FindChildById("label");
			Assert.IsNotNull(label);
			Assert.IsNotNull(label.Font);
		}

		[Test]
		public void CheckGridAttachedProperties()
		{
			var properties = AttachedPropertiesRegistry.GetPropertiesOfType(typeof(Grid));
			Assert.AreEqual(4, properties.Length);
		}
	}
}
