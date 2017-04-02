using System;
using System.Collections.Generic;

namespace Myra.Graphics2D
{
	public partial class SpriteSheet
	{
		private readonly Dictionary<string, ImageDrawable> _drawables;

		public Dictionary<string, ImageDrawable> Drawables
		{
			get { return _drawables; }
		}

		public SpriteSheet(Dictionary<string, ImageDrawable> drawables)
		{
			if (drawables == null)
			{
				throw new ArgumentNullException("drawables");
			}

			_drawables = drawables;
		}

		public ImageDrawable EnsureDrawable(string id)
		{
			ImageDrawable result;
			if (!_drawables.TryGetValue(id, out result))
			{
				throw new ArgumentNullException(string.Format("Could not resolve drawable '{0}'", id));
			}

			return result;
		}
	}
}