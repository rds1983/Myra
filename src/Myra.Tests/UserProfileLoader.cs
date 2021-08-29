using Myra.Assets;
using System.Xml.Linq;

namespace AssetManagementBase.Tests
{
	internal class UserProfileLoader : IAssetLoader<UserProfile>
	{
		public UserProfile Load(AssetLoaderContext context, string assetName)
		{
			var data = context.Load<string>(assetName);

			var xDoc = XDocument.Parse(data);

			var result = new UserProfile
			{
				Name = xDoc.Root.Element("Name").Value,
				Score = int.Parse(xDoc.Root.Element("Score").Value)
			};

			return result;
		}
	}
}
