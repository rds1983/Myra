using AssetManagementBase;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using Myra.MML;
using System.Xml.Linq;
using Xunit;

namespace Myra.Tests
{
	[Collection("Myra Tests")]
	public class MMLTests
	{
		[Fact]
		public void LoadMMLWithExternalAssets()
		{
			var assetManager = Utility.CreateAssetManager();

			var project = assetManager.LoadProject("GridWithExternalResources.xmmp");

			var imageButton1 = (Button)project.Root.FindChildById("spawnUnit1");
			Assert.NotNull(imageButton1);

			var image = (Image)imageButton1.Content;
			Assert.NotNull(image);
			Assert.NotNull(image.Renderable);
			Assert.Equal(image.Renderable.Size, new Point(64, 64));

			var label = (Label)project.Root.FindChildById("label");
			Assert.NotNull(label);
			Assert.NotNull(label.Font);
		}

		[Fact]
		public void CheckGridAttachedProperties()
		{
			var properties = AttachedPropertiesRegistry.GetPropertiesOfType(typeof(Grid));
			Assert.Equal(4, properties.Length);
		}

		[Theory]
		[InlineData("checkButton.xmmp")]
		[InlineData("comboView.xmmp")]
		[InlineData("GridTests/SimpleAutoFill.xmmp")]
		[InlineData("GridTests/SimpleProportionsPart.xmmp")]
		[InlineData("GridWithExternalResources.xmmp")]
		[InlineData("labelWithPaddings.xmmp")]
		[InlineData("listView.xmmp")]
		[InlineData("marginBorderPadding.xmmp")]
		[InlineData("newButtons.xmmp")]
		[InlineData("scrolledTextField.xmmp")]
		[InlineData("allControls.xmmp")]
		[InlineData("allControlsC64.xmmp")]
		public void XmlRoundTrip(string fileName)
		{
			var assetManager = Utility.CreateAssetManager();

			// Read the original XML string
			var originalXml = assetManager.ReadAsString(fileName);

			// Load the project from XML
			var project = assetManager.LoadProject(fileName);

			// Convert back to XML
			var resultXml = project.ToXml();

			// Parse both as XDocuments
			var originalDoc = XDocument.Parse(originalXml);
			var resultDoc = XDocument.Parse(resultXml);

			// Assert XML is semantically equal (ignoring attribute order)
			Utility.AssertXmlEqual(originalDoc, resultDoc);
		}
	}
}



