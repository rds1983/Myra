using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GdxSkinImport;

public class ColorsCache
{
	public Dictionary<string, Color> Colors { get; } = new Dictionary<string, Color>();

	public void Add(string key, object value)
	{
		var asRGB = value as Dictionary<string, object>;
		if (asRGB != null)
		{
			var color = asRGB.ParseColor();
			Colors[key] = color;
		}
		else
		{
			Colors[key] = Colors[(string)value];
		}
	}

	public Color Get(object value)
	{
		var asRGB = value as Dictionary<string, object>;
		if (asRGB != null)
		{
			return asRGB.ParseColor();
		}

		return Colors[(string)value];
	}
}
