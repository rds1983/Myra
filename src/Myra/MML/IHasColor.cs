#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.MML
{
	/// <summary>
	/// Represents an object that has a color property.
	/// </summary>
	public interface IHasColor
	{
		/// <summary>
		/// Gets the color of the object.
		/// </summary>
		Color Color { get; }
	}
}
