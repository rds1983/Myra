using GdxSkinImport.MonoGdx;
using info.lundin.math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI.Styles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TextureRegion = Myra.Graphics2D.TextureAtlases.TextureRegion;

namespace GdxSkinImport;

public static class Converter
{
	private class TintedDrawable
	{
		public string Name { get; set; }
		public Color Color { get; set; }

		public override string ToString() => $"{Name}:{Color}";
	}

	private static TextureRegionAtlas ToMyraAtlas(TextureAtlas gdxAtlas)
	{
		if (gdxAtlas.Textures.Count > 1)
		{
			throw new NotSupportedException("Only atlases with a single texture are supported");
		}

		var result = new TextureRegionAtlas();

		var sourceTexture = gdxAtlas.Textures.First();

		result.Image = Path.GetFileName(sourceTexture.TexturePath);

		foreach(var sourceRegion in gdxAtlas.Regions)
		{
			var bounds = new Rectangle(sourceRegion.RegionX, sourceRegion.RegionY, sourceRegion.RegionWidth, sourceRegion.RegionHeight);

			TextureRegion region;
			if (sourceRegion.Splits == null)
			{
				region = new TextureRegion(sourceTexture.Texture, bounds);
			}
			else
			{
				var parts = sourceRegion.Splits;
				var thickness = new Thickness
				{
					Left = parts[0],
					Right = parts[1],
					Top = parts[2],
					Bottom = parts[3]
				};

				region = new NinePatchRegion(sourceTexture.Texture, bounds, thickness);
			}

			result.Regions[sourceRegion.Name] = region;
		}

		return result;
	}

	private static Stylesheet ToMyraStylesheet(Dictionary<string, object> data)
	{
		var colors = new Dictionary<string, Color>();

		object obj;
		if (data.TryGetValue("com.badlogic.gdx.graphics.Color", out obj))
		{
			var colorsData = (Dictionary<string, object>)obj;
			foreach (var pair in colorsData)
			{
				var asRGB = pair.Value as Dictionary<string, object>;
				if (asRGB != null)
				{
					var color = asRGB.ParseColor();
					colors[pair.Key] = color;
				} else
				{
					colors[pair.Key] = colors[(string)pair.Value];
				}
			}
		}

		var tintedDrawables = new Dictionary<string, TintedDrawable>();
		if (data.TryGetValue("com.badlogic.gdx.scenes.scene2d.ui.Skin$TintedDrawable", out obj))
		{
			var tintedData = (Dictionary<string, object>)obj;
			foreach (var pair in tintedData)
			{
				var v = (Dictionary<string, object>)pair.Value;

				var td = new TintedDrawable
				{
					Name = (string)v["name"],
					Color = colors[(string)v["color"]]
				};

				tintedDrawables[pair.Key] = td;
			}
		}

		return null;
	}

	public static Stylesheet ImportGdx(GraphicsDevice device, string path)
	{
		TextureAtlas gdxAtlas = null;
		var gdxAtlasFile = Path.ChangeExtension(path, "atlas");
		if (File.Exists(gdxAtlasFile))
		{
			gdxAtlas = new TextureAtlas(device, gdxAtlasFile);
		}

		var myraAtlas = ToMyraAtlas(gdxAtlas);

		Dictionary<string, object> data;
		using (TextReader reader = new StreamReader(path))
		{
			data = Json.Deserialize(reader) as Dictionary<string, object>;
		}
		
		return ToMyraStylesheet(data);
	}
}
