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
  /// Represents padding or margin information associated with an element.
  /// </summary>
  public struct Padding
  {
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Padding"/> stricture using a separate padding size for each edge.
    /// </summary>
    /// <param name="left">The padding size, in pixels, for the left edge.</param>
    /// <param name="top">The padding size, in pixels, for the top edge.</param>
    /// <param name="right">The padding size, in pixels, for the right edge.</param>
    /// <param name="bottom">The padding size, in pixels, for the bottom edge.</param>
    public Padding(int left, int top, int right, int bottom)
      : this()
    {
      this.Top = top;
      this.Left = left;
      this.Right = right;
      this.Bottom = bottom;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the padding value for the bottom edge.
    /// </summary>
    /// <value>
    /// The padding, in pixels, for the bottom edge.
    /// </value>
    public int Bottom { get; set; }

    /// <summary>
    /// Gets or sets the padding value for the left edge.
    /// </summary>
    /// <value>
    /// The padding, in pixels, for the left edge.
    /// </value>
    public int Left { get; set; }

    /// <summary>
    /// Gets or sets the padding value for the right edge.
    /// </summary>
    /// <value>
    /// The padding, in pixels, for the right edge.
    /// </value>
    public int Right { get; set; }

    /// <summary>
    /// Gets or sets the padding value for the top edge.
    /// </summary>
    /// <value>
    /// The padding, in pixels, for the top edge.
    /// </value>
    public int Top { get; set; }

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
      return string.Format("{0}, {1}, {2}, {3}", this.Left, this.Top, this.Right, this.Bottom);
    }

    #endregion
  }
}
