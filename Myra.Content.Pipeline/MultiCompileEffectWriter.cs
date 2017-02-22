using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace Myra.Content.Pipeline
{
	[ContentTypeWriter]
	class MultiCompileEffectWriter : ContentTypeWriter<MultiCompileEffectContent>
	{
		protected override void Write(ContentWriter output, MultiCompileEffectContent value)
		{
			output.Write(value.Variants.Count);

			foreach (var v in value.Variants)
			{
				output.Write(v.Key);
				output.Write(v.Value.Length);
				output.Write(v.Value);
			}

			output.Write(value.DefaultVariantKey);
		}

		public override string GetRuntimeReader(Microsoft.Xna.Framework.Content.Pipeline.TargetPlatform targetPlatform)
		{
			var type = typeof(MultiCompileEffectReader);
			var readerType = type.Namespace + ".MultiCompileEffectReader, " + type.Assembly.FullName;
			return readerType;
		}
	}
}
