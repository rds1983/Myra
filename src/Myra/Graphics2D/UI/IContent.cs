using Myra.Graphics2D.UI;

namespace Myra.Graphics2D
{
	/// <summary>
	/// Represents a container that can hold a single content widget.
	/// </summary>
	public interface IContent
	{
		/// <summary>
		/// Gets or sets the content widget.
		/// </summary>
		Widget Content { get; set; }
	}
}
