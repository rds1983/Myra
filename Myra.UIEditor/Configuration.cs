using System;
using System.Configuration;

namespace Myra.UIEditor
{
	public static class Configuration
	{
		private static readonly string _pluginPath;

		public static string PluginPath
		{
			get { return _pluginPath; }
		}

		static Configuration()
		{
			_pluginPath = GetAppSettings<string>("PluginPath");
		}

		internal static T GetAppSettings<T>(string key, T def = default(T))
		{
			T result = def;
			try
			{
				var reader = new AppSettingsReader();
				result = (T)reader.GetValue(key, typeof(T));
			}
			catch (Exception)
			{
			}

			return result;
		}

	}
}
