using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Myra;

namespace MyraPad
{
	public class State
	{
		public const string StateFileName = "MyraPad.config";

		public static string StateFilePath
		{
			get
			{
				var result = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), StateFileName);
				return result;
			}
		}

		public Point Size { get; set; }
		public float TopSplitterPosition { get; set; }
		public float LeftSplitterPosition { get; set; }
		public string EditedFile { get; set; }
		public string LastFolder { get; set; }
		public uint[] UserColors { get; set; }
		public Options Options;

		public State()
		{
			Options = new Options();
		}

		public void Save()
		{
			using (var stream = new StreamWriter(StateFilePath, false))
			{
				var serializer = new XmlSerializer(typeof(State));
				serializer.Serialize(stream, this);
			}
		}

		public static State Load()
		{
			if (!File.Exists(StateFilePath))
			{
				return null;
			}

			State state;
			using (var stream = new StreamReader(StateFilePath))
			{
				var serializer = new XmlSerializer(typeof(State));
				state = (State)serializer.Deserialize(stream);
			}

			return state;
		}

		public override string ToString()
		{
			var colors = string.Empty;
			if (UserColors != null)
			{
				colors = string.Join(", ", from c in UserColors select new Color(c).ToHexString());
			}
			return string.Format("Size = {0}\n" +
								 "TopSplitter = {1:0.##}\n" +
								 "LeftSplitter= {2:0.##}\n" +
								 "EditedFile = {3}\n" +
								 "LastFolder = {4}\n" +
								 "UserColors = {5}",
				Size,
				TopSplitterPosition,
				LeftSplitterPosition,
				EditedFile,
				LastFolder,
				colors);
		}
	}
}