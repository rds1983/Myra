using AssetManagementBase;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using System.Xml.Linq;
using Xunit;

namespace Myra.Tests
{
	[Collection("Myra Tests")]
	public class StylesheetTests
	{
		[Theory]
		[InlineData("Stylesheets/Default/default_ui_skin.xmms", "allControlsBasic.xmmp")]
		[InlineData("Stylesheets/Default/default_ui_skin_no_existing_texture.xmms", "allControlsBasic.xmmp")]
		[InlineData("Stylesheets/Default/default_ui_skin_another_font.xmms", "allControlsBasic.xmmp")]
		[InlineData("Stylesheets/Default/default_ui_skin_2x.xmms", "allControlsBasic.xmmp")]
		[InlineData("Stylesheets/Commodore64/ui_stylesheet.xmms", "allControlsBasic.xmmp")]
		[InlineData("Stylesheets/LibGDX/ui_stylesheet.xmms", "allControlsBasic.xmmp")]
		public void AllControlsFormStylesheetApplication(string stylesheetPath, string formPath)
		{
			var assetManager = Utility.CreateAssetManager();

			// Load the stylesheet first
			var stylesheet = assetManager.LoadStylesheet(stylesheetPath);
			Assert.NotNull(stylesheet);

			// Load the UI form XML and apply the stylesheet
			var projectXml = assetManager.ReadAsString(formPath);
			var project = Project.LoadFromXml(projectXml, assetManager, stylesheet);

			// Verify form loaded successfully
			Assert.NotNull(project.Root);

			// Get the style categories from the stylesheet
			var labelStyle = stylesheet.LabelStyles[Stylesheet.DefaultStyleName];
			var buttonStyle = stylesheet.ButtonStyles[Stylesheet.DefaultStyleName];
			var textBoxStyle = stylesheet.TextBoxStyles[Stylesheet.DefaultStyleName];
			var sliderStyle = stylesheet.HorizontalSliderStyles[Stylesheet.DefaultStyleName];

			// Verify Label widgets use stylesheet font
			var labels = Utility.CollectWidgetsByType<Label>(project.Root);
			Assert.True(labels.Count > 0);
			// Check fonts on all labels (fonts are less likely to be overridden)
			foreach (var label in labels)
			{
				Assert.Equal(labelStyle.Font, label.Font);
			}

			// Check text color on labels that don't have explicit colors
			// Labels in ComboView and ListView have explicit TextColor in the xmmp file
			var mainGridLabel = (Label)project.Root.FindChildById("_textButtonLabel");
			if (mainGridLabel != null)
			{
				Assert.Equal(labelStyle.TextColor, mainGridLabel.TextColor);
			}

			// Verify Button widgets have stylesheet background and padding
			var button = (Button)project.Root.FindChildById("_button");
			if (button != null)
			{
				Assert.Equal(buttonStyle.Background, button.Background);
				Assert.Equal(buttonStyle.PressedBackground, button.PressedBackground);
				Assert.Equal(buttonStyle.OverBackground, button.OverBackground);
				Assert.Equal(buttonStyle.Padding, button.Padding);
			}

			var textButton = (Button)project.Root.FindChildById("_textButton");
			if (textButton != null)
			{
				Assert.Equal(buttonStyle.Background, textButton.Background);
				Assert.Equal(buttonStyle.PressedBackground, textButton.PressedBackground);
				Assert.Equal(buttonStyle.Padding, textButton.Padding);
			}

			var imageButton = (Button)project.Root.FindChildById("_imageButton");
			if (imageButton != null)
			{
				Assert.Equal(buttonStyle.Background, imageButton.Background);
				Assert.Equal(buttonStyle.PressedBackground, imageButton.PressedBackground);
			}
		}

		[Fact]
		public void AllControlsC64ImageRenderables()
		{
			var assetManager = Utility.CreateAssetManager();

			// Load the UI form
			var project = assetManager.LoadProject("allControlsC64.xmmp"); ;
			Assert.NotNull(project);
			Assert.NotNull(project.Root);

			// Verify Image 1: music-off in button with id "_image"
			var image1 = (Image)project.Root.FindChildById("_image");
			Assert.NotNull(image1);
			Assert.NotNull(image1.Renderable);
			Assert.Equal("music-off", image1.Renderable.ToString());

			// Verify Image 2: sound-off in image button
			// Find the image button first
			var imageButton = (Button)project.Root.FindChildById("_imageButton");
			Assert.NotNull(imageButton);

			// Get the image from the button content
			var images = Utility.CollectWidgetsByType<Image>(imageButton);
			Assert.True(images.Count > 0);
			var image2 = images[0];
			Assert.NotNull(image2.Renderable);
			Assert.Equal("sound-off", image2.Renderable.ToString());
		}

		[Theory]
		[InlineData("Stylesheets/Default/default_ui_skin.xmms")]
		[InlineData("Stylesheets/Default/default_ui_skin_no_existing_texture.xmms")]
		[InlineData("Stylesheets/Default/default_ui_skin_another_font.xmms")]
		[InlineData("Stylesheets/Default/default_ui_skin_2x.xmms")]
		[InlineData("Stylesheets/Commodore64/ui_stylesheet.xmms")]
		[InlineData("Stylesheets/LibGDX/ui_stylesheet.xmms")]
		public void XmlRoundTrip(string fileName)
		{
			var assetManager = Utility.CreateAssetManager();

			// Read the original XML string
			var originalXml = assetManager.ReadAsString(fileName);

			// Load the stylesheet from XML
			var stylesheet = assetManager.LoadStylesheet(fileName);

			// Convert back to XML
			var resultXml = stylesheet.ToXml();

			// Parse both as XDocuments
			var originalDoc = XDocument.Parse(originalXml);
			var resultDoc = XDocument.Parse(resultXml);

			// Assert XML is semantically equal (ignoring attribute order)
			Utility.AssertXmlEqual(originalDoc, resultDoc);
		}

		private FontSystem GetFontSystem(Stylesheet stylesheet, string id)
		{
			var baseFont = stylesheet.Fonts[id].Font;
			Assert.IsType<DynamicSpriteFont>(baseFont);
			return ((DynamicSpriteFont)baseFont).FontSystem;
		}

		private void AssertFontUsesExistingTexture(Stylesheet stylesheet, string id)
		{
			var fontSystem = GetFontSystem(stylesheet, id);

			Assert.Equal(stylesheet.Fonts.AtlasUsedSpace, fontSystem.ExistingTextureUsedSpace);
			Assert.Same(stylesheet.Atlas.Texture, fontSystem.ExistingTexture);
		}

		private void AssertFontDoesNotUseExistingTexture(Stylesheet stylesheet, string id)
		{
			var fontSystem = GetFontSystem(stylesheet, id);
			Assert.Null(fontSystem.ExistingTexture);
		}

		[Fact]
		public void ExistingTexture()
		{
			var assetManager = Utility.CreateAssetManager();

			// Load the stylesheet first
			var stylesheet = assetManager.LoadStylesheet("Stylesheets/Default/default_ui_skin.xmms");

			// Make sure the texture atlas and the font system use the same texture
			AssertFontUsesExistingTexture(stylesheet, "default-font");
			AssertFontUsesExistingTexture(stylesheet, "tooltip-font");
		}

		[Fact]
		public void NoExistingTexture()
		{
			var assetManager = Utility.CreateAssetManager();

			// Load the stylesheet first
			var stylesheet = assetManager.LoadStylesheet("Stylesheets/Default/default_ui_skin_no_existing_texture.xmms");

			// Make sure the texture atlas and the font system do not use the same texture
			AssertFontDoesNotUseExistingTexture(stylesheet, "default-font");
			AssertFontDoesNotUseExistingTexture(stylesheet, "tooltip-font");
		}

		[Fact]
		public void ExistingTextureAnotherFont()
		{
			var assetManager = Utility.CreateAssetManager();

			// Load the stylesheet first
			var stylesheet = assetManager.LoadStylesheet("Stylesheets/Default/default_ui_skin_another_font.xmms");

			// Make sure some font systems use existing texture and some do not
			AssertFontUsesExistingTexture(stylesheet, "default-font");
			AssertFontUsesExistingTexture(stylesheet, "tooltip-font");
			AssertFontDoesNotUseExistingTexture(stylesheet, "another-font");
		}
	}
}
