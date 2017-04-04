using Myra.Assets;
using Myra.Attributes;

namespace Myra.Graphics2D.UI
{
	[AssetLoader(typeof(UILoader))]
	public class Project
	{
		public Grid Root { get; set; }

		public Project()
		{
			Root = new Grid();
		}
	}
}
