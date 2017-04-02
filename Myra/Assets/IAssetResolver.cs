using System.IO;

namespace Myra.Assets
{
	public interface IAssetResolver
	{
		Stream Open(string path);
	}
}
