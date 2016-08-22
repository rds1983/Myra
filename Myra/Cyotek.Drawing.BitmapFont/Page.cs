/* AngelCode bitmap font parsing using C#
 * http://www.cyotek.com/blog/angelcode-bitmap-font-parsing-using-csharp
 *
 * Copyright © 2012-2015 Cyotek Ltd.
 *
 * Licensed under the MIT License. See license.txt for the full text.
 */

using System.IO;

namespace Cyotek.Drawing.BitmapFont
{
  /// <summary>
  /// Represents a texture page.
  /// </summary>
  public struct Page
  {
    #region Constructors

    /// <summary>
    /// Creates a texture page using the specified ID and source file name.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="fileName">Filename of the texture image.</param>
    public Page(int id, string fileName)
      : this()
    {
      this.FileName = fileName;
      this.Id = id;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the filename of the source texture image.
    /// </summary>
    /// <value>
    /// The name of the file containing the source texture image.
    /// </value>
    public string FileName { get; set; }

    /// <summary>
    /// Gets or sets the page identifier.
    /// </summary>
    /// <value>
    /// The page identifier.
    /// </value>
    public int Id { get; set; }

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
      return string.Format("{0} ({1})", this.Id, Path.GetFileName(this.FileName));
    }

    #endregion
  }
}
