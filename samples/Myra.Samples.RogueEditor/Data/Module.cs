using System.Collections.Generic;
using Myra.Attributes;

namespace Myra.Samples.RogueEditor.Data
{
	public class Module
	{
		private readonly Dictionary<string, TileInfo> _tileInfos = new Dictionary<string, TileInfo>();
		private readonly Dictionary<string, CreatureInfo> _creatureInfos = new Dictionary<string, CreatureInfo>();
		private readonly Dictionary<string, Map> _maps = new Dictionary<string, Map>();

		[HiddenInEditor]
		public Dictionary<string, TileInfo> TileInfos
		{
			get { return _tileInfos; }
		}

		[HiddenInEditor]
		public Dictionary<string, CreatureInfo> CreatureInfos
		{
			get { return _creatureInfos; }
		}

		[HiddenInEditor]
		public Dictionary<string, Map> Maps
		{
			get { return _maps; }
		}

		public string ToJSON()
		{
			return string.Empty;
		}

		public static Module FromJSON(string data)
		{
			return null;
		}
	}
}