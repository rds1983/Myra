using Myra.Attributes;

namespace Myra.Graphics2D.UI
{
	public class ContentControl: SingleItemContainer<Widget>, IContent
	{
		[Content]
		public Widget Content
		{
			get => InternalChild;
			set => InternalChild = value;
		}
	}
}
