using Myra.Attributes;

namespace Myra.Graphics2D.UI
{
	public class ContentControl: SingleItemContainer<Widget>, IContent
	{
		[Content]
		public virtual Widget Content
		{
			get => InternalChild;
			set => InternalChild = value;
		}
	}
}
