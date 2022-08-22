using System.Globalization;
using Myra.Graphics2D;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
using System.Numerics;
#endif

namespace Myra.MML
{
	public interface ITypeSerializer
	{
		string Serialize(object obj);
		object Deserialize(string str);
	}

	public abstract class TypeSerializer<T> : ITypeSerializer
	{
		public object Deserialize(string str) => DeserializeT(str);

		public string Serialize(object obj) => SerializeT((T)obj);

		public abstract T DeserializeT(string str);
		public abstract string SerializeT(T obj);
	}

	internal sealed class Vector2Serializer : TypeSerializer<Vector2>
	{
		public override Vector2 DeserializeT(string str)
		{
			var parts = str.Split(',');
			var x = float.Parse(parts[0].Trim(), CultureInfo.InvariantCulture);
			var y = float.Parse(parts[1].Trim(), CultureInfo.InvariantCulture);
			return new Vector2(x, y);
		}

		public override string SerializeT(Vector2 obj)
		{
			return obj.X.ToString(CultureInfo.InvariantCulture) + ", " + obj.Y.ToString(CultureInfo.InvariantCulture);
		}
	}

	internal sealed class ThicknessSerializer : TypeSerializer<Thickness>
	{
		public override Thickness DeserializeT(string str)
		{
			return Thickness.FromString(str);
		}

		public override string SerializeT(Thickness obj)
		{
			return obj.ToString();
		}
	}
}
