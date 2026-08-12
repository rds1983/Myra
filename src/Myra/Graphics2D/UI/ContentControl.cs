using Myra.Attributes;
using System.ComponentModel;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// Base class for widgets that contain a single child widget.
	/// </summary>
	public abstract class ContentControl: Widget, IContent
	{
		/// <summary>
		/// Gets or sets the content widget.
		/// </summary>
		[Content]
		[DefaultValue(null)]
		public abstract Widget Content { get; set; }

		/// <summary>
		/// Copies properties from another content control to this one, including the content widget.
		/// </summary>
		/// <param name="w">The source content control to copy from.</param>
		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var contentControl = (ContentControl)w;
			Content = contentControl.Content.Clone();
		}
	}
}
