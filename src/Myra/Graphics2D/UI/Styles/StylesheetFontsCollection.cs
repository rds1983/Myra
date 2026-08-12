using Myra.Attributes;
using System.Collections.Generic;
using System.Collections;
using System.Xml.Serialization;


#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
#endif

namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Represents a collection of <see cref="StylesheetFont"/> objects used by a stylesheet.
	/// </summary>
	[XmlName("Fonts")]
	public class StylesheetFontsCollection : IEnumerable<StylesheetFont>
	{
		/// <summary>
		/// Gets or sets the atlas used space rectangle for the font collection.
		/// </summary>
		[XmlName("UsedSpace")]
		public Rectangle? AtlasUsedSpace { get; set; }

		/// <summary>
		/// Gets the number of fonts in the collection.
		/// </summary>
		[XmlIgnore]
		public int Count => Fonts.Count;

		/// <summary>
		/// Gets or sets a font by its identifier.
		/// </summary>
		/// <param name="id">The font identifier.</param>
		[XmlIgnore]
		public StylesheetFont this[string id]
		{
			get => Fonts[id];

			set => Fonts[id] = value;
		}

		/// <summary>
		/// Gets the underlying dictionary of fonts keyed by identifier.
		/// </summary>
		[Content]
		public Dictionary<string, StylesheetFont> Fonts { get; } = new Dictionary<string, StylesheetFont>();

		/// <summary>
		/// Attempts to retrieve a font by its identifier.
		/// </summary>
		/// <param name="id">The font identifier.</param>
		/// <param name="font">When this method returns, contains the font associated with the specified identifier, if found.</param>
		/// <returns><c>true</c> if a font with the specified identifier exists; otherwise, <c>false</c>.</returns>
		public bool TryGetValue(string id, out StylesheetFont font) => Fonts.TryGetValue(id, out font);

		/// <inheritdoc/>
		public IEnumerator<StylesheetFont> GetEnumerator() => Fonts.Values.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => Fonts.Values.GetEnumerator();
	}
}
