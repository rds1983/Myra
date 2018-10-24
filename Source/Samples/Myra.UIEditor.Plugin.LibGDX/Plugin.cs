using Myra.Editor.Plugin;
using Myra.Graphics2D;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;

namespace Myra.UIEditor.Plugin.LibGDX
{
	public class Plugin: UIEditorPlugin
	{
		public override void OnLoad()
		{
			// Create resource asset resolver
			var assetResolver = new ResourceAssetResolver(GetType().Assembly, "Myra.Samples.Plugin.LibGDX.Resources.");

			// Load image containing font & ui spritesheet
			var colorBuffer = ColorBuffer.FromStream(assetResolver.Open("ui_stylesheet_image.png"));
			colorBuffer.PremultiplyAlpha();

			var texture = colorBuffer.CreateTexture2D();

			// Load ui text atlas
			var textureAtlas = TextureRegionAtlas.FromGDX(assetResolver.ReadAsString("ui_stylesheet_atlas.atlas"),
				s => texture);

			// Load ui font(s)
			var font = SpriteFontHelper.LoadFromFnt(assetResolver.ReadAsString("ui_font.fnt"),
				textureAtlas["default"]);

			// Load stylesheet
			var stylesheet = Stylesheet.CreateFromSource(assetResolver.ReadAsString("ui_stylesheet.json"),
				s => textureAtlas[s],
				s => font);

			Stylesheet.Current = stylesheet;
		}

		public override void FillCustomWidgetTypes(WidgetTypesSet widgetTypes)
		{
			base.FillCustomWidgetTypes(widgetTypes);
			
			widgetTypes.AddWidgetType<CustomWidget>();
		}
	}
}
