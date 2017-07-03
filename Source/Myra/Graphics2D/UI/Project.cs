using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Myra.Graphics2D.UI
{
	public class Project
	{
		public Grid Root { get; set; }

		public Project()
		{
			Root = new Grid();
		}

		public string Save()
		{
			var result = JsonConvert.SerializeObject(this, Formatting.Indented,
				new JsonSerializerSettings
				{
					PreserveReferencesHandling = PreserveReferencesHandling.Objects,
					TypeNameHandling = TypeNameHandling.Objects
				});

			return result;
		}


		public static Project LoadFromData(string data)
		{
			var result = (Project)JsonConvert.DeserializeObject(data, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Objects
			});

			return result;
		}

		public static void PopulateFromData<T>(string data, T target) where T: Widget
		{
			var project = JObject.Parse(data);
			var root = project["Root"];

			using (var sr = root.CreateReader())
			{
				var serializer = JsonSerializer.CreateDefault(new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.Objects
				});

				serializer.Populate(sr, target);
			}
		}
	}
}
