using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Content;
using MonoGame.Extended.TextureAtlases;

namespace Myra.Content.TextureAtlases
{
	public class TextureAtlasReader : ContentTypeReader<TextureAtlas>
	{
		protected override TextureAtlas Read(ContentReader reader, TextureAtlas existingInstance)
		{
			var assetName = reader.GetRelativeAssetName(reader.ReadString());
			var texture = reader.ContentManager.Load<Texture2D>(assetName);
			var atlas = new TextureAtlas(assetName, texture);

			var regionCount = reader.ReadInt32();

			for (var i = 0; i < regionCount; i++)
			{
				var name = reader.ReadString();
				var bounds = new Rectangle(reader.ReadInt32(),
					reader.ReadInt32(),
					reader.ReadInt32(),
					reader.ReadInt32());

				var isNinePatch = reader.ReadBoolean();
				if (!isNinePatch)
				{
					atlas.CreateRegion(name, bounds.X, bounds.Y, bounds.Width, bounds.Height);
				}
				else
				{
					var thickness = new Thickness
					{
						Left = reader.ReadInt32(),
						Right = reader.ReadInt32(),
						Top = reader.ReadInt32(),
						Bottom = reader.ReadInt32()
					};

					atlas.CreateNinePatchRegion(name, bounds.X, bounds.Y, bounds.Width, bounds.Height, thickness);
				}
			}

			return atlas;
		}
	}
}
