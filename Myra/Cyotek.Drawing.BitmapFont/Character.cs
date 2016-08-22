/* AngelCode bitmap font parsing using C#
 * http://www.cyotek.com/blog/angelcode-bitmap-font-parsing-using-csharp
 *
 * Copyright © 2012-2015 Cyotek Ltd.
 *
 * Licensed under the MIT License. See license.txt for the full text.
 */

using Microsoft.Xna.Framework;

namespace Cyotek.Drawing.BitmapFont
{
	/// <summary>
	/// Represents the definition of a single character in a <see cref="BitmapFont"/>
	/// </summary>
	public struct Character
	{
		#region Properties

		/// <summary>
		/// Gets or sets the bounds of the character image in the source texture.
		/// </summary>
		/// <value>
		/// The bounds of the character image in the source texture.
		/// </value>
		public Rectangle Bounds { get; set; }

		/// <summary>
		/// Gets or sets the texture channel where the character image is found.
		/// </summary>
		/// <value>
		/// The texture channel where the character image is found.
		/// </value>
		/// <remarks>
		/// 1 = blue, 2 = green, 4 = red, 8 = alpha, 15 = all channels
		/// </remarks>
		public int Channel { get; set; }

		/// <summary>
		/// Gets or sets the character.
		/// </summary>
		/// <value>
		/// The character.
		/// </value>
		public char Char { get; set; }

		/// <summary>
		/// Gets or sets the offset when copying the image from the texture to the screen.
		/// </summary>
		/// <value>
		/// The offset when copying the image from the texture to the screen.
		/// </value>
		public Point Offset { get; set; }

		/// <summary>
		/// Gets or sets the texture page where the character image is found.
		/// </summary>
		/// <value>
		/// The texture page where the character image is found.
		/// </value>
		public int TexturePage { get; set; }

		/// <summary>
		/// Gets or sets the value used to advance the current position after drawing the character.
		/// </summary>
		/// <value>
		/// How much the current position should be advanced after drawing the character.
		/// </value>
		public int XAdvance { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Returns the fully qualified type name of this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String" /> containing a fully qualified type name.
		/// </returns>
		/// <seealso cref="M:System.ValueType.ToString()"/>
		public override string ToString()
		{
			return Char.ToString();
		}

		#endregion
	}
}
