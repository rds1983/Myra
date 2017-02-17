using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Utilities.Png;
using Myra.Editor.Plugin;
using Myra.Graphics2D;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;

namespace Myra.UIEditor.Plugin.LibGDX
{
	public class Plugin: IUIEditorPlugin
	{
		private const string Prefix = "Myra.UIEditor.Plugin.LibGDX.Resources.";

		private static string GetStringResource(string name)
		{
			var assembly = typeof(Plugin).GetTypeInfo().Assembly;

			// Once you figure out the name, pass it in as the argument here.
			string s;
			using (var stream = assembly.GetManifestResourceStream(Prefix + name))
			{
				using (var reader = new StreamReader(stream))
				{
					s = reader.ReadToEnd();
				}
			}

			return s;
		}

		private static Stream GetBinaryResourceStream(string name)
		{
			var assembly = typeof(Plugin).GetTypeInfo().Assembly;

			// Once you figure out the name, pass it in as the argument here.
			var stream = assembly.GetManifestResourceStream(Prefix + name);

			return stream;
		}

		public void OnLoad()
		{
			// First step - load underlying image(s)
			var pngReader = new PngReader();

			Texture2D underlyingImage;
			using (var stream = GetBinaryResourceStream("uiskin.png"))
			{
				underlyingImage = pngReader.Read(stream, MyraEnvironment.GraphicsDevice);
			}

			// Making it alpha-premultiplied
			underlyingImage.PremultiplyAlpha();

			// Second step - load sprite sheet
			var data = GetStringResource("uiskin.atlas");
			var spriteSheet = SpriteSheet.LoadLibGDX(data, s => underlyingImage);

			// Retrieve sprite with font
			var fontSprite = spriteSheet.Drawables["default"];

			// Third step - create bitmap font(s)
			data = GetStringResource("default.fnt");
			var font = BitmapFont.CreateFromFNT(data, fontSprite.TextureRegion);

			// Final step - load UI style sheet
			data = GetStringResource("uiskin.json");
			var stylesheet = Stylesheet.CreateFromSource(data, s => font, s =>
			{
				ImageDrawable result;
				if (!spriteSheet.Drawables.TryGetValue(s, out result))
				{
					throw new Exception(string.Format("Could not find default drawable '{0}'", s));
				}

				return result;
			});

			DefaultAssets.UIStylesheet = stylesheet;
		}
	}
}
