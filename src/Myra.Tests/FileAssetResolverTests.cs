using Myra.Assets;
using Myra.Utility;
using NUnit.Framework;
using System;

namespace Myra.Tests
{
	[TestFixture]
	public class FileAssetResolverTests
	{
		[Test]
		public void LoadUserProfile()
		{
			var resolver = new FileAssetResolver(PathUtils.ExecutingAssemblyDirectory);
			var assetManager = new AssetManager(resolver);
			var userProfile = assetManager.Load<UserProfile>("userProfile.xml");

			Assert.AreEqual(userProfile.Name, "AssetManagementBase");
			Assert.AreEqual(userProfile.Score, 10000);
		}

		[Test]
		public void WrongPath()
		{
			var resolver = new FileAssetResolver(PathUtils.ExecutingAssemblyDirectory);
			var assetManager = new AssetManager(resolver);

			Assert.Throws<Exception>(() =>
			{
				var userProfile = assetManager.Load<UserProfile>("userProfile2.xml");
			});
		}
	}
}
