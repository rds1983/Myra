using System;

namespace Myra.Graphics2D.UI.Data
{
	/// <summary>
	/// A DataGrid column that displays image values (<see cref="IImage"/>) as thumbnail images.
	/// Supports neither filtering nor sorting.
	/// </summary>
	public class DataGridImageColumn : DataGridColumnBase
	{
		/// <inheritdoc/>
		public override DataGridColumnFlags Flags => DataGridColumnFlags.None;

		/// <summary>
		/// Initializes a new instance of the <see cref="DataGridImageColumn"/> class with the specified property, header, and width.
		/// </summary>
		/// <param name="property">The name of the property on the data object to bind to.</param>
		/// <param name="header">The text displayed in the column header.</param>
		/// <param name="width">The width of the column in pixels.</param>
		public DataGridImageColumn(string property, string header, int width = 100) : base(property, header, width)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataGridImageColumn"/> class with the specified property and width,
		/// using the property name as the header text.
		/// </summary>
		/// <param name="property">The name of the property on the data object to bind to.</param>
		/// <param name="width">The width of the column in pixels.</param>
		public DataGridImageColumn(string property, int width = 100) : base(property, width)
		{
		}

		/// <inheritdoc/>
		public override Widget CreateWidget(object value)
		{
			var asImage = value as IImage;
			if (asImage == null)
			{
				throw new Exception("DataGridImageColumn can only be used with image values.");
			}

			return new Image
			{
				Width = 32,
				Height = 32,
				Renderable = asImage
			};
		}
	}
}
