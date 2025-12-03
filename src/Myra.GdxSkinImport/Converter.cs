using FontStashSharp;
using GdxSkinImport.MonoGdx;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra;
using Myra.Graphics2D;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI.Styles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TextureRegion = Myra.Graphics2D.TextureAtlases.TextureRegion;

namespace GdxSkinImport;

public class Converter
{
	private class TintedDrawable
	{
		public string Name { get; set; }
		public Color Color { get; set; }

		public override string ToString() => $"{Name}:{Color}";
	}

	private readonly GraphicsDevice _device;
	private readonly string _path;
	private readonly string _folder;
	private readonly Stylesheet _result = new Stylesheet();
	private TextureRegionAtlas _atlas;
	private readonly ColorsCache _colors = new ColorsCache();
	private readonly Dictionary<string, SpriteFontBase> _fonts = new Dictionary<string, SpriteFontBase>();
	private readonly Dictionary<string, TintedDrawable> _tintedDrawables = new Dictionary<string, TintedDrawable>();

	public Converter(GraphicsDevice device, string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			throw new ArgumentNullException(nameof(path));
		}

		_device = device ?? throw new ArgumentNullException(nameof(device));
		_path = path;
		_folder = Path.GetDirectoryName(this._path);
	}

	public Stylesheet Process()
	{
		TextureAtlas gdxAtlas = null;
		var gdxAtlasFile = Path.ChangeExtension(_path, "atlas");
		if (File.Exists(gdxAtlasFile))
		{
			gdxAtlas = new TextureAtlas(_device, gdxAtlasFile);
		}

		_atlas = ToMyraAtlas(gdxAtlas);

		Dictionary<string, object> data;
		using (TextReader reader = new StreamReader(_path))
		{
			data = Json.Deserialize(reader) as Dictionary<string, object>;
		}

		var folder = Path.GetDirectoryName(_path);

		return ToMyraStylesheet(data);
	}

	private SpriteFontBase GetFont(string key)
	{
		SpriteFontBase result;
		if (!_fonts.TryGetValue(key, out result))
		{
			throw new Exception($"Could not find font '{key}'");
		}

		return result;
	}

	private Color GetColor(Dictionary<string, object> data, string key) => _colors.Get(data[key]);

	private void LoadLabelStyle(Dictionary<string, object> data, LabelStyle style)
	{
		style.Font =  GetFont(data.GetString("font"));
		style.TextColor = GetColor(data, "fontColor");
	}

	private Stylesheet ToMyraStylesheet(Dictionary<string, object> data)
	{
		object obj;
		if (data.TryGetValue("com.badlogic.gdx.graphics.Color", out obj))
		{
			var colorsData = (Dictionary<string, object>)obj;
			foreach (var pair in colorsData)
			{
				_colors.Add(pair.Key, pair.Value);
			}
		}

		if (data.TryGetValue("com.badlogic.gdx.graphics.g2d.BitmapFont", out obj))
		{
			var fontsData = (Dictionary<string, object>)obj;
			foreach (var pair in fontsData)
			{
				var f = (Dictionary<string, object>)pair.Value;

				var file = f["file"].ToString();
				var path = Path.Combine(_folder, file);

				var fontData = File.ReadAllText(path);
				var font = StaticSpriteFont.FromBMFont(fontData, s =>
				{
					var region = _atlas[Path.GetFileNameWithoutExtension(s)];

					return new TextureWithOffset(region.Texture, region.Bounds.Location);
				});

				_fonts[pair.Key] = font;
			}
		}

		if (data.TryGetValue("com.badlogic.gdx.scenes.scene2d.ui.Skin$TintedDrawable", out obj))
		{
			var tintedData = (Dictionary<string, object>)obj;
			foreach (var pair in tintedData)
			{
				var v = (Dictionary<string, object>)pair.Value;

				var td = new TintedDrawable
				{
					Name = (string)v["name"],
					Color = GetColor(v, "color")
				};

				_tintedDrawables[pair.Key] = td;
			}
		}

		if (data.TryGetValue("com.badlogic.gdx.scenes.scene2d.ui.Label$LabelStyle", out obj))
		{
			var widgetData = (Dictionary<string, object>)obj;
			foreach (var pair in widgetData)
			{
				var labelStyle = new LabelStyle();
				LoadLabelStyle((Dictionary<string, object>)pair.Value, labelStyle);

				_result.LabelStyles[pair.Key] = labelStyle;
			}
		}

		return null;
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

		foreach (var sourceRegion in gdxAtlas.Regions)
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
}
