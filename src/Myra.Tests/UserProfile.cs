using Myra.Assets;

namespace AssetManagementBase.Tests
{
	[AssetLoader(typeof(UserProfileLoader))]
	public class UserProfile
	{
		public string Name;
		public int Score;
	}
}
