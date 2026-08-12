using FontStashSharp;
using GdxSkinImport.MonoGdx;
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

public class Converter
{
	private class FontInfo
	{
		public SpriteFontBase Font { get; }
		public string Id { get; }
		public string File { get; }

		public FontInfo(SpriteFontBase font, string id, string file)
		{
			Font = font;
			Id = id;
			File = file;
		}
	}

	private readonly GraphicsDevice _device;
	private readonly string _path;
	private readonly string _folder;
	private readonly Stylesheet _result = new Stylesheet();
	private TextureRegionAtlas _atlas;
	private readonly ColorsCache _colors = new ColorsCache();
	private readonly Dictionary<string, FontInfo> _fonts = new Dictionary<string, FontInfo>();
	private readonly Dictionary<string, TintedRegion> _tintedDrawables = new Dictionary<string, TintedRegion>();

	public Converter(GraphicsDevice device, string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			throw new ArgumentNullException(nameof(path));
		}

		_device = device ?? throw new ArgumentNullException(nameof(device));
		_path = path;
		_folder = Path.GetDirectoryName(_path);
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

		ToMyraStylesheet(data);

		_result.Atlas = _atlas;

		foreach (var pair in _fonts)
		{
			_result.Fonts[pair.Key] = new StylesheetFont { Id = pair.Value.Id, File = pair.Value.File };
		}

		return _result;
	}

	private SpriteFontBase GetFont(string key)
	{
		FontInfo result;
		if (!_fonts.TryGetValue(key, out result))
		{
			return null;
		}

		return result.Font;
	}

	private IImage GetDrawable(string name)
	{
		if (_atlas.Regions.TryGetValue(name, out var region))
		{
			return region;
		}

		if (_tintedDrawables.TryGetValue(name, out var tinted))
		{
			return tinted;
		}

		throw new Exception($"Could not find drawable '{name}'");
	}

	private Color GetColor(Dictionary<string, object> data, string key) => _colors.Get(data[key]);

	private void LoadLabelStyle(Dictionary<string, object> data, LabelStyle style)
	{
		if (data.TryGetValue("font", out var fontObj))
		{
			var font = GetFont(fontObj.ToString());
			if (font != null)
				style.Font = font;
		}

		if (data.TryGetValue("fontColor", out var colorObj))
		{
			style.TextColor = GetColor(data, "fontColor");
		}
		else
		{
			style.TextColor = Color.White;
		}
	}

	private ButtonStyle LoadButtonStyle(Dictionary<string, object> data)
	{
		var style = new ButtonStyle();

		if (data.TryGetValue("up", out var upObj))
		{
			style.Background = GetDrawable(upObj.ToString());
		}

		if (data.TryGetValue("down", out var downObj))
		{
			style.PressedBackground = GetDrawable(downObj.ToString());
		}

		return style;
	}

	private ImageButtonStyle LoadImageButtonStyle(Dictionary<string, object> data)
	{
		var style = new ImageButtonStyle();

		if (data.TryGetValue("imageUp", out var upObj))
		{
			var d = GetDrawable(upObj.ToString());
			style.Background = d;
			style.Width = d.Size.X;
			style.Height = d.Size.Y;
		}

		if (data.TryGetValue("imageDown", out var downObj))
		{
			var d = GetDrawable(downObj.ToString());
			style.PressedBackground = d;
			style.Width = d.Size.X;
			style.Height = d.Size.Y;
		}

		if (data.TryGetValue("imageOver", out var overObj))
		{
			var d = GetDrawable(overObj.ToString());
			style.OverBackground = d;
			style.Width = d.Size.X;
			style.Height = d.Size.Y;
		}

		return style;
	}

	private CheckButtonStyle LoadCheckBoxStyle(Dictionary<string, object> data)
	{
		var style = new CheckButtonStyle();

		if (data.TryGetValue("checkboxOff", out var offObj))
		{
			var imageStyle = new ImageStyle();
			imageStyle.Image = GetDrawable(offObj.ToString());

			style.ImageStyle = imageStyle;
		}

		if (data.TryGetValue("checkboxOn", out var onObj))
		{
			if (style.ImageStyle == null)
				style.ImageStyle = new ImageStyle();
			style.ImageStyle.PressedImage = GetDrawable(onObj.ToString());
		}

		return style;
	}

	private TextBoxStyle LoadTextFieldStyle(Dictionary<string, object> data)
	{
		var style = new TextBoxStyle();

		if (data.TryGetValue("background", out var bgObj))
		{
			style.Background = GetDrawable(bgObj.ToString());
		}

		if (data.TryGetValue("cursor", out var cursorObj))
		{
			style.Cursor = GetDrawable(cursorObj.ToString());
		}

		if (data.TryGetValue("selection", out var selObj))
		{
			style.Selection = GetDrawable(selObj.ToString());
		}

		if (data.TryGetValue("font", out var fontObj))
		{
			style.Font = GetFont(fontObj.ToString());
		}

		if (data.TryGetValue("fontColor", out var colorObj))
		{
			style.TextColor = GetColor(data, "fontColor");
		}

		return style;
	}

	private SliderStyle LoadSliderStyle(Dictionary<string, object> data)
	{
		var style = new SliderStyle();

		if (data.TryGetValue("background", out var bgObj))
		{
			style.Background = GetDrawable(bgObj.ToString());
		}

		if (data.TryGetValue("knob", out var knobObj))
		{
			var knobButtonStyle = new ImageButtonStyle();
			var imageStyle = new ImageStyle();
			imageStyle.Image = GetDrawable(knobObj.ToString());
			knobButtonStyle.ImageStyle = imageStyle;
			style.KnobStyle = knobButtonStyle;
		}

		return style;
	}

	private WindowStyle LoadWindowStyle(Dictionary<string, object> data)
	{
		var style = new WindowStyle();

		if (data.TryGetValue("background", out var bgObj))
		{
			style.Background = GetDrawable(bgObj.ToString());
		}

		if (data.TryGetValue("titleFont", out var fontObj) || data.TryGetValue("title", out fontObj))
		{
			var labelStyle = new LabelStyle();
			labelStyle.Font = GetFont(fontObj.ToString());

			if (data.TryGetValue("titleFontColor", out var colorObj))
			{
				labelStyle.TextColor = GetColor(data, "titleFontColor");
			}
			else if (data.TryGetValue("fontColor", out colorObj))
			{
				labelStyle.TextColor = GetColor(data, "fontColor");
			}

			style.TitleStyle = labelStyle;
		}

		if (data.TryGetValue("closeButton", out var closeButtonObj))
		{
			var closeButtonStyle = new ImageButtonStyle();
			var imageStyle = new ImageStyle();
			imageStyle.Image = GetDrawable(closeButtonObj.ToString());
			closeButtonStyle.ImageStyle = imageStyle;
			style.CloseButtonStyle = closeButtonStyle;
		}

		return style;
	}

	private SplitPaneStyle LoadSplitPaneStyle(Dictionary<string, object> data)
	{
		var style = new SplitPaneStyle();

		if (data.TryGetValue("handle", out var handleObj))
		{
			var handleStyle = new SplitPanelButtonStyle();

			handleStyle.Background = GetDrawable(handleObj.ToString());

			style.HandleStyle = handleStyle;
		}

		return style;
	}

	private ListBoxStyle LoadListStyle(Dictionary<string, object> data)
	{
		var style = new ListBoxStyle();

		if (data.TryGetValue("background", out var overObj))
		{
			style.Background = GetDrawable(overObj.ToString());
		}

		// Load list item style
		var listItemStyle = new ButtonStyle();

		if (data.TryGetValue("selection", out var selectionObj))
		{
			listItemStyle.PressedBackground = GetDrawable(selectionObj.ToString());
		}

		if (data.TryGetValue("down", out var downObj))
		{
			listItemStyle.PressedBackground = GetDrawable(downObj.ToString());
		}

		if (data.TryGetValue("up", out var upObj))
		{
			listItemStyle.Background = GetDrawable(upObj.ToString());
		}

		style.ListItemStyle = listItemStyle;
		return style;
	}

	private ComboBoxStyle LoadComboBoxStyle(Dictionary<string, object> data)
	{
		var style = new ComboBoxStyle();

		// Load button properties (up, down, over states)
		if (data.TryGetValue("background", out var bg))
		{
			style.Background = GetDrawable(bg.ToString());

			var asImage = style.Background as IImage;
			if (asImage != null)
			{
				style.Height = asImage.Size.Y;
			}
		}

		if (data.TryGetValue("backgroundOpen", out var bgOpen))
		{
			style.PressedBackground = GetDrawable(bgOpen.ToString());

			var asImage = style.PressedBackground as IImage;
			if (asImage != null)
			{
				style.Height = asImage.Size.Y;
			}
		}

		// Load label properties (font, fontColor)
		var labelStyle = new LabelStyle();
		LoadLabelStyle(data, labelStyle);
		style.LabelStyle = labelStyle;

		// Load list style reference - listStyle is a string ID, not a nested object
		if (data.TryGetValue("listStyle", out var listStyleIdObj))
		{
			var listStyleId = listStyleIdObj.ToString();
			if (_result.ListBoxStyles.TryGetValue(listStyleId, out var listBoxStyle))
			{
				style.ListBoxStyle = (ListBoxStyle)listBoxStyle.Clone();
			}
		}

		return style;
	}

	private static string ResolveId(string id)
	{
		if (id.StartsWith("default"))
		{
			return Stylesheet.DefaultStyleName;
		}

		return id;
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
					// Try to use the file name without extension
					var regionName = Path.GetFileNameWithoutExtension(s);

					var region = _atlas[regionName];
					return new TextureWithOffset(region.Texture, region.Bounds.Location);
				});

				font.Name = pair.Key;

				var fontInfo = new FontInfo(font, font.Name, file);
				_fonts[pair.Key] = fontInfo;
			}
		}

		if (data.TryGetValue("com.badlogic.gdx.scenes.scene2d.ui.Skin$TintedDrawable", out obj))
		{
			var tintedData = (Dictionary<string, object>)obj;
			foreach (var pair in tintedData)
			{
				var v = (Dictionary<string, object>)pair.Value;

				var region = _atlas.Regions[(string)v["name"]];

				var td = new TintedRegion(region, GetColor(v, "color"));


				_tintedDrawables[pair.Key] = td;
			}
		}

		// Label styles
		if (data.TryGetValue("com.badlogic.gdx.scenes.scene2d.ui.Label$LabelStyle", out obj))
		{
			var widgetData = (Dictionary<string, object>)obj;
			foreach (var pair in widgetData)
			{
				var labelStyle = new LabelStyle();
				labelStyle.Id = ResolveId(pair.Key);
				LoadLabelStyle((Dictionary<string, object>)pair.Value, labelStyle);
				_result.LabelStyles[pair.Key] = labelStyle;
			}
		}

		// Button styles
		if (data.TryGetValue("com.badlogic.gdx.scenes.scene2d.ui.Button$ButtonStyle", out obj))
		{
			var widgetData = (Dictionary<string, object>)obj;
			foreach (var pair in widgetData)
			{
				var buttonStyle = LoadButtonStyle((Dictionary<string, object>)pair.Value);
				buttonStyle.Id = ResolveId(pair.Key);
				_result.ButtonStyles[pair.Key] = buttonStyle;
			}
		}

		// TextButton styles
		if (data.TryGetValue("com.badlogic.gdx.scenes.scene2d.ui.TextButton$TextButtonStyle", out obj))
		{
			var widgetData = (Dictionary<string, object>)obj;
			foreach (var pair in widgetData)
			{
				var styleData = (Dictionary<string, object>)pair.Value;
				var buttonStyle = LoadButtonStyle(styleData);
				buttonStyle.Id = ResolveId(pair.Key);

				_result.ButtonStyles[pair.Key] = buttonStyle;
			}
		}

		// ImageTextButton styles
		if (data.TryGetValue("com.badlogic.gdx.scenes.scene2d.ui.ImageTextButton$ImageTextButtonStyle", out obj))
		{
			var widgetData = (Dictionary<string, object>)obj;
			foreach (var pair in widgetData)
			{
				var styleData = (Dictionary<string, object>)pair.Value;
				var buttonStyle = LoadImageButtonStyle(styleData);
				buttonStyle.Id = ResolveId(pair.Key);

				_result.ButtonStyles[pair.Key] = buttonStyle;
			}
		}

		// CheckBox styles
		if (data.TryGetValue("com.badlogic.gdx.scenes.scene2d.ui.CheckBox$CheckBoxStyle", out obj))
		{
			var widgetData = (Dictionary<string, object>)obj;
			foreach (var pair in widgetData)
			{
				var checkBoxStyle = LoadCheckBoxStyle((Dictionary<string, object>)pair.Value);
				checkBoxStyle.Id = ResolveId(pair.Key);

				if (checkBoxStyle.Id != "radio")
				{
					_result.CheckBoxStyles[pair.Key] = checkBoxStyle;
				}
				else
				{
					checkBoxStyle.Id = string.Empty;
					_result.RadioButtonStyles[pair.Key] = checkBoxStyle;
				}
			}
		}

		// TextField styles
		if (data.TryGetValue("com.badlogic.gdx.scenes.scene2d.ui.TextField$TextFieldStyle", out obj))
		{
			var widgetData = (Dictionary<string, object>)obj;
			foreach (var pair in widgetData)
			{
				var textFieldStyle = LoadTextFieldStyle((Dictionary<string, object>)pair.Value);
				textFieldStyle.Id = ResolveId(pair.Key);
				_result.TextBoxStyles[pair.Key] = textFieldStyle;
			}
		}

		// Slider styles
		if (data.TryGetValue("com.badlogic.gdx.scenes.scene2d.ui.Slider$SliderStyle", out obj))
		{
			var widgetData = (Dictionary<string, object>)obj;
			foreach (var pair in widgetData)
			{
				var sliderStyle = LoadSliderStyle((Dictionary<string, object>)pair.Value);
				sliderStyle.Id = ResolveId(pair.Key);

				if (pair.Key.Contains("horizontal"))
				{
					_result.HorizontalSliderStyles[pair.Key] = sliderStyle;
				}
				else
				{
					_result.VerticalSliderStyles[pair.Key] = sliderStyle;
				}
			}
		}

		// ProgressBar styles
		if (data.TryGetValue("com.badlogic.gdx.scenes.scene2d.ui.ProgressBar$ProgressBarStyle", out obj))
		{
			var widgetData = (Dictionary<string, object>)obj;
			foreach (var pair in widgetData)
			{
				var pbStyle = new ProgressBarStyle();
				pbStyle.Id = ResolveId(pair.Key);
				var styleData = (Dictionary<string, object>)pair.Value;

				if (styleData.TryGetValue("background", out var bgObj))
				{
					pbStyle.Background = GetDrawable(bgObj.ToString());
				}

				if (styleData.TryGetValue("knobBefore", out var knobObj))
				{
					pbStyle.Filler = GetDrawable(knobObj.ToString());
				}

				if (pair.Key.Contains("horizontal"))
				{
					_result.HorizontalProgressBarStyles[pair.Key] = pbStyle;
				}
				else
				{
					_result.VerticalProgressBarStyles[pair.Key] = pbStyle;
				}
			}
		}

		// ScrollPane styles
		if (data.TryGetValue("com.badlogic.gdx.scenes.scene2d.ui.ScrollPane$ScrollPaneStyle", out obj))
		{
			var widgetData = (Dictionary<string, object>)obj;
			foreach (var pair in widgetData)
			{
				var svStyle = new ScrollViewerStyle();
				svStyle.Id = ResolveId(pair.Key);
				var styleData = (Dictionary<string, object>)pair.Value;

				if (styleData.TryGetValue("hScrollKnob", out var hKnobObj))
				{
					svStyle.HorizontalScrollKnob = GetDrawable(hKnobObj.ToString());
				}

				if (styleData.TryGetValue("vScrollKnob", out var vKnobObj))
				{
					svStyle.VerticalScrollKnob = GetDrawable(vKnobObj.ToString());
				}

				if (styleData.TryGetValue("hScroll", out var hScrollObj))
				{
					svStyle.HorizontalScrollBackground = GetDrawable(hScrollObj.ToString());
				}

				if (styleData.TryGetValue("vScroll", out var vScrollObj))
				{
					svStyle.VerticalScrollBackground = GetDrawable(vScrollObj.ToString());
				}

				_result.ScrollViewerStyles[pair.Key] = svStyle;
			}
		}

		// Window styles
		if (data.TryGetValue("com.badlogic.gdx.scenes.scene2d.ui.Window$WindowStyle", out obj))
		{
			var widgetData = (Dictionary<string, object>)obj;
			foreach (var pair in widgetData)
			{
				var windowStyle = LoadWindowStyle((Dictionary<string, object>)pair.Value);
				windowStyle.Id = ResolveId(pair.Key);
				_result.WindowStyles[pair.Key] = windowStyle;
			}
		}

		// SplitPane styles
		if (data.TryGetValue("com.badlogic.gdx.scenes.scene2d.ui.SplitPane$SplitPaneStyle", out obj))
		{
			var widgetData = (Dictionary<string, object>)obj;
			foreach (var pair in widgetData)
			{
				var splitPaneStyle = LoadSplitPaneStyle((Dictionary<string, object>)pair.Value);
				splitPaneStyle.Id = ResolveId(pair.Key);

				if (pair.Key.Contains("horizontal"))
				{
					_result.HorizontalSplitPaneStyles[pair.Key] = splitPaneStyle;
				}
				else
				{
					_result.VerticalSplitPaneStyles[pair.Key] = splitPaneStyle;
				}
			}
		}

		// List styles (must be loaded before ComboBox styles since they reference ListStyle)
		if (data.TryGetValue("com.badlogic.gdx.scenes.scene2d.ui.List$ListStyle", out obj))
		{
			var widgetData = (Dictionary<string, object>)obj;
			foreach (var pair in widgetData)
			{
				var listBoxStyle = LoadListStyle((Dictionary<string, object>)pair.Value);
				listBoxStyle.Id = ResolveId(pair.Key);
				_result.ListBoxStyles[pair.Key] = listBoxStyle;
			}
		}

		// ComboBox (SelectBox in libGDX) styles
		if (data.TryGetValue("com.badlogic.gdx.scenes.scene2d.ui.SelectBox$SelectBoxStyle", out obj))
		{
			var widgetData = (Dictionary<string, object>)obj;
			foreach (var pair in widgetData)
			{
				var comboBoxStyle = LoadComboBoxStyle((Dictionary<string, object>)pair.Value);
				comboBoxStyle.Id = ResolveId(pair.Key);
				_result.ComboBoxStyles[pair.Key] = comboBoxStyle;
			}
		}

		return _result;
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

			region.Name = sourceRegion.Name;

			result.Regions[sourceRegion.Name] = region;
		}

		return result;
	}
}
