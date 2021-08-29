using Myra.Assets;

namespace Myra.Tests
{
	[AssetLoader(typeof(UserProfileLoader))]
	public class UserProfile
	{
		public string Name;
		public int Score;
	}
}
