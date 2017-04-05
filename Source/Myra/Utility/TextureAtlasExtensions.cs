using System;
using System.Collections.Generic;
using System.Reflection;
using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;

namespace Myra.Utility
{
	public static class TextureAtlasExtensions
	{
		private static FieldInfo _regionsField;
		private static FieldInfo _regionMapField;

		internal static NinePatchRegion2D CreateNinePatchRegion(this TextureAtlas atlas, string name, int x, int y, int width,
			int height, Thickness thickness)
		{
			if (_regionsField == null)
			{
				_regionsField = atlas.GetType().GetTypeInfo().GetDeclaredField("_regions");
				_regionMapField = atlas.GetType().GetTypeInfo().GetDeclaredField("_regionMap");
			}

			var regionMap = (Dictionary<string, int>)_regionMapField.GetValue(atlas);
			var regions = (List<TextureRegion2D>) _regionsField.GetValue(atlas);
			if (regionMap.ContainsKey(name))
				throw new InvalidOperationException(string.Format("Region {0} already exists in the texture atlas", name));

			var textureRegion = new TextureRegion2D(name, atlas.Texture, x, y, width, height);
			var ninePatchRegion2D = new NinePatchRegion2D(textureRegion, thickness);

			var index = regions.Count;
			regions.Add(ninePatchRegion2D);
			regionMap.Add(ninePatchRegion2D.Name, index);

			return ninePatchRegion2D;
		}
	}
}