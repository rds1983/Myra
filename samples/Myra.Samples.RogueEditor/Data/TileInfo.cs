using Microsoft.Xna.Framework;

namespace Myra.Samples.RogueEditor.Data
{
	public class TileInfo: ItemWithId
	{
		public char Image { get; set; }
		public Color Color { get; set; }
		public bool Passable { get; set; }

		public override string ToString()
		{
			return string.Format("Id: {0}, Color: {1}, Passable: {2}",
				Id, Color, Passable);
		}
	}
}
