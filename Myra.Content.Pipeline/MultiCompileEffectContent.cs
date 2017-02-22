using System.Collections.Generic;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Myra.Content.Pipeline
{
	public class MultiCompileEffectContent : ContentItem
	{
		private readonly Dictionary<string, byte[]> _variants = new Dictionary<string, byte[]>();

		public string DefaultVariantKey { get; set; }

		public Dictionary<string, byte[]> Variants
		{
			get { return _variants; }
		}

		public void AddVariant(string defines, byte[] effectCode)
		{
			_variants[defines] = effectCode;
		}
	}
}
