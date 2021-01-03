using FontStashSharp.Interfaces;
using Myra.Graphics2D.UI;
using System.Drawing;

namespace Myra.Platform
{
	public interface IMyraPlatform: ITexture2DManager
	{
		Point ViewSize { get; }

		IMyraRenderer CreateRenderer();

		MouseInfo GetMouseInfo();

		void SetKeysDown(bool[] keys);
	}
}
