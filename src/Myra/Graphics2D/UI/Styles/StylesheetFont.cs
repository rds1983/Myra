using FontStashSharp;
using Myra.Attributes;
using Myra.MML;
using System;
using System.Xml.Serialization;

namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Represents a font definition within a stylesheet, including file path and optional size.
	/// </summary>
	[XmlName("Font")]
	public class StylesheetFont : IItemWithId
	{
		private SpriteFontBase _font;

		/// <summary>
		/// The character used to separate font file names from size values in font keys.
		/// </summary>
		internal const char Separator = ':';

		/// <summary>
		/// Gets or sets the identifier for this font.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the file path to the font file.
		/// </summary>
		public string File { get; set; }

		/// <summary>
		/// Gets or sets the font size (only used for TTF and OTF fonts).
		/// </summary>
		public float? Size { get; set; }

		/// <summary>
		/// Gets or sets the loaded sprite font instance.
		/// </summary>
		[XmlIgnore]
		public SpriteFontBase Font
		{
			get => _font;

			internal set
			{
				_font = value ?? throw new ArgumentNullException(nameof(value));
				_font.Name = Id;
			}
		}

		/// <summary>
		/// Validates that all required properties are set and valid for the font type.
		/// </summary>
		/// <exception cref="Exception">Thrown when required properties are missing or invalid.</exception>
		public void Validate()
		{
			if (string.IsNullOrEmpty(Id))
			{
				throw new Exception("Font Id is required");
			}
			if (string.IsNullOrEmpty(File))
			{
				throw new Exception($"Font '{Id}' File is required");
			}

			if (File.EndsWith(".ttf", StringComparison.InvariantCultureIgnoreCase) || File.EndsWith(".otf", StringComparison.InvariantCultureIgnoreCase))
			{
				if (Size == 0)
				{
					throw new Exception($"Font '{Id}' Size is required for TTF/OTF fonts");
				}
			}
		}

		/// <summary>
		/// Creates a deep copy of this stylesheet font.
		/// </summary>
		/// <returns>A new StylesheetFont instance with the same properties.</returns>
		public StylesheetFont Clone() => new StylesheetFont
		{
			Id = Id,
			File = File,
			Size = Size,
			Font = Font
		};

		/// <summary>
		/// Builds a font file key combining the file name and size.
		/// </summary>
		/// <returns>The font file key for caching and lookup.</returns>
		public string BuildFontFileKey()
		{
			if (Size == 0)
			{
				return File;
			}

			return File + Separator + Size;
		}

		/// <summary>
		/// Attempts to extract a numeric parameter from an asset name containing a separator.
		/// </summary>
		/// <param name="assetName">The asset name to parse. Will be modified to remove the parameter.</param>
		/// <param name="parameter">The extracted parameter if found; otherwise null.</param>
		/// <returns>True if a numeric parameter was found and extracted; otherwise false.</returns>
		internal static bool TryGetParameter(ref string assetName, out string parameter)
		{
			parameter = null;

			for (var i = 0; i < assetName.Length - 1; ++i)
			{
				if (assetName[i] == Separator && char.IsDigit(assetName[i + 1]))
				{
					// Found
					parameter = assetName.Substring(i + 1).Trim();
					assetName = assetName.Substring(0, i).Trim();
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Returns the font identifier.
		/// </summary>
		/// <returns>The font ID.</returns>
		public override string ToString() => Id;
	}
}
