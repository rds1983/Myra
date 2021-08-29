using Myra.Assets;
using NUnit.Framework;
using System;
using System.Reflection;

namespace Myra.Tests
{
	[TestFixture]
	public class ResourceAssetResolverTests
	{
		private static readonly Assembly _assembly = typeof(ResourceAssetResolverTests).Assembly;

		[Test]
		public void TestWrongPath()
		{
			var resolver = new ResourceAssetResolver(_assembly, "WrongPath.Resources");
			var assetManager = new AssetManager(resolver);

			Assert.Throws<Exception>(() =>
			{
				var text = assetManager.Load<string>("test.txt");
			});
		}

		[Test]
		public void TestWithoutEndDot()
		{
			// Without dot at the end
			var resolver = new ResourceAssetResolver(_assembly, "Resources");
			var assetManager = new AssetManager(resolver);
			var text = assetManager.Load<string>("test.txt");
			Assert.AreEqual(text, "Test");
		}

		[Test]
		public void TestWithEndDot()
		{
			// With dot
			var resolver = new ResourceAssetResolver(_assembly, "Resources.");
			var assetManager = new AssetManager(resolver);
			var text = assetManager.Load<string>("test.txt");
			Assert.AreEqual(text, "Test");
		}

		[Test]
		public void TestWithoutPrependAssemblyName()
		{
			// Without dot at the end
			var resolver = new ResourceAssetResolver(_assembly, "AssetManagementBase.Tests.Resources", false);
			var assetManager = new AssetManager(resolver);
			var text = assetManager.Load<string>("test.txt");
			Assert.AreEqual(text, "Test");
		}
	}
}
