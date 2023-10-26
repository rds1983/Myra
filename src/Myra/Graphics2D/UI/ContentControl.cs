using Myra.Attributes;
using System.ComponentModel;

namespace Myra.Graphics2D.UI
{
	public class ContentControl: SingleItemContainer<Widget>, IContent
	{
		[Content]
		[DefaultValue(null)]
		public virtual Widget Content
		{
			get => InternalChild;
			set => InternalChild = value;
		}
	}
}
