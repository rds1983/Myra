/* AngelCode bitmap font parsing using C#
 * http://www.cyotek.com/blog/angelcode-bitmap-font-parsing-using-csharp
 *
 * Copyright © 2012-2015 Cyotek Ltd.
 *
 * Licensed under the MIT License. See license.txt for the full text.
 */

namespace Cyotek.Drawing.BitmapFont
{
  /// <summary>
  /// Represents the font kerning between two characters.
  /// </summary>
  public struct Kerning
  {
    #region Constructors

    public Kerning(char firstCharacter, char secondCharacter, int amount)
      : this()
    {
      this.FirstCharacter = firstCharacter;
      this.SecondCharacter = secondCharacter;
      this.Amount = amount;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets how much the x position should be adjusted when drawing the second character immediately following the first.
    /// </summary>
    /// <value>
    /// How much the x position should be adjusted when drawing the second character immediately following the first.
    /// </value>
    public int Amount { get; set; }

    /// <summary>
    /// Gets or sets the first character.
    /// </summary>
    /// <value>
    /// The first character.
    /// </value>
    public char FirstCharacter { get; set; }

    /// <summary>
    /// Gets or sets the second character.
    /// </summary>
    /// <value>
    /// The second character.
    /// </value>
    public char SecondCharacter { get; set; }

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
      return string.Format("{0} to {1} = {2}", this.FirstCharacter, this.SecondCharacter, this.Amount);
    }

    #endregion
  }
}
