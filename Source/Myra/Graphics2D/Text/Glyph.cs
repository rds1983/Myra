using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Myra.Graphics2D.Text
{
	public class Glyph
	{
		private readonly Dictionary<char, int> _kerning = new Dictionary<char, int>();

		public char Id { get; set; }
		public TextureRegion Region { get; set; }
		public Point Offset { get; set; }
		public int XAdvance { get; set; }

		public Dictionary<char, int> Kerning
		{
			get { return _kerning; }
		}

		public int GetKerning(char nextChar)
		{
			int result;

			if (!Kerning.TryGetValue(nextChar, out result))
			{
				result = 0;
			}

			return result;
		}
	}
}