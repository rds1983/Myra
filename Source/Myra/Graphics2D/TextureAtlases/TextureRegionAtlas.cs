using System;
using System.Collections.Generic;

namespace Myra.Graphics2D.TextureAtlases
{
	public partial class TextureRegionAtlas
	{
		private readonly Dictionary<string, TextureRegion> _drawables;

		public Dictionary<string, TextureRegion> Drawables
		{
			get { return _drawables; }
		}

		public TextureRegion this[string name]
		{
			get { return Drawables[name]; }
		}

		public TextureRegionAtlas(Dictionary<string, TextureRegion> drawables)
		{
			if (drawables == null)
			{
				throw new ArgumentNullException("drawables");
			}

			_drawables = drawables;
		}

		public TextureRegion EnsureDrawable(string id)
		{
			TextureRegion result;
			if (!_drawables.TryGetValue(id, out result))
			{
				throw new ArgumentNullException(string.Format("Could not resolve drawable '{0}'", id));
			}

			return result;
		}
	}
}