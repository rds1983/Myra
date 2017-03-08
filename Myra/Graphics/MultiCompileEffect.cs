using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace Myra.Graphics
{
	public class MultiCompileEffect
	{
		public const string DefineSeparator = ";";

		private readonly Dictionary<string, Effect> _effects = new Dictionary<string, Effect>();

		public string DefaultVariantKey { get; internal set; }

		public IEnumerable<string> AllKeys
		{
			get { return _effects.Keys; }
		}

		public static string BuildKey(string[] defines)
		{
			return string.Join(DefineSeparator,
				(from d in defines where !string.IsNullOrEmpty(d.Trim()) orderby d select d.ToUpper()));
		}

		internal void AddEffect(string defines, Effect effect)
		{
			_effects[defines] = effect;
		}

		private Effect InternalGetEffect(string key)
		{
			Effect result;
			_effects.TryGetValue(key, out result);

			return result;
		}

		public Effect GetEffect(string[] defines)
		{
			var key = BuildKey(defines);
			return InternalGetEffect(key);
		}

		public Effect GetDefaultEffect()
		{
			return InternalGetEffect(DefaultVariantKey);
		}

		public static MultiCompileEffect CreateFromReader(GraphicsDevice device, BinaryReader input)
		{
			var count = input.ReadInt32();

			var result = new MultiCompileEffect();

			for (var i = 0; i < count; ++i)
			{
				var key = input.ReadString();

				var effectCodeLength = input.ReadInt32();
				var effectCode = input.ReadBytes(effectCodeLength);

				var effect = new Effect(device, effectCode);

				result.AddEffect(key, effect);
			}

			result.DefaultVariantKey = input.ReadString();

			return result;
		}

		public static MultiCompileEffect CreateFromBytecode(GraphicsDevice device, byte[] bytecode)
		{
			using (var stream = new MemoryStream(bytecode))
			{
				using (var input = new BinaryReader(stream))
				{
					return CreateFromReader(device, input);
				}
			}
		}
	}
}
