using Myra.Graphics2D;
using Myra.Graphics2D.TextureAtlases;
using System;

namespace Myra.Samples;

/// <summary>
/// Represents a single texture atlas region as a data row for the <see cref="DataGrid"/>.
/// </summary>
public class Record
{
	private readonly TextureRegion _region;

	/// <summary>
	/// Gets the region itself, used to render the thumbnail via <see cref="DataGridImageColumn"/>.
	/// </summary>
	public IImage Image => _region;

	/// <summary>
	/// Gets the region size formatted as a string (e.g. <c>"32x32"</c>).
	/// </summary>
	public string Size => $"{_region.Size.X}x{_region.Size.Y}";

	/// <summary>
	/// Gets whether the region is a nine-patch region.
	/// </summary>
	public bool NP => _region is NinePatchRegion;

	/// <summary>
	/// Gets the name of the region.
	/// </summary>
	public string Name => _region.Name;

	/// <summary>
	/// Initializes a new instance of the <see cref="Record"/> class wrapping the specified region.
	/// </summary>
	/// <param name="region">The texture atlas region to display.</param>
	public Record(TextureRegion region)
	{
		_region = region ?? throw new ArgumentNullException(nameof(region));
	}
}
