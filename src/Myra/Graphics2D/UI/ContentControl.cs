using Myra.Attributes;
using System.ComponentModel;

namespace Myra.Graphics2D.UI
{
	public abstract class ContentControl: Widget, IContent
	{
		[Content]
		[DefaultValue(null)]
		public abstract Widget Content { get; set; }
	}
}
