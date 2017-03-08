using System.IO;
using System.Reflection;

namespace Myra.Resources
{
	internal static class Resources
	{
		private const string Prefix = "Myra.Resources.";

		private static string _defaultFont;
		private static string _defaultFontSmall;
		private static string _defaultUISkinAtlas;
		private static string _defaultStyleSheet;

		public static string DefaultFont
		{
			get
			{
				EnsureStringResource(Prefix + "default_font.fnt", ref _defaultFont);
				return _defaultFont;
			}
		}

		public static string DefaultFontSmall
		{
			get
			{
				EnsureStringResource(Prefix + "default_font_small.fnt", ref _defaultFontSmall);
				return _defaultFontSmall;
			}
		}

		public static string DefaultUISkinAtlas
		{
			get
			{
				EnsureStringResource(Prefix + "default_uiskin.atlas", ref _defaultUISkinAtlas);
				return _defaultUISkinAtlas;
			}
		}

		public static string DefaultStyleSheet
		{
			get
			{
				EnsureStringResource(Prefix + "default_stylesheet.json", ref _defaultStyleSheet);
				return _defaultStyleSheet;
			}
		}

		private static void EnsureStringResource(string path, ref string s)
		{
			if (s != null)
			{
				return;
			}

			var assembly = typeof (Resources).GetTypeInfo().Assembly;

			// Once you figure out the name, pass it in as the argument here.
			using (var stream = assembly.GetManifestResourceStream(path))
			{
				using (var reader = new StreamReader(stream))
				{
					s = reader.ReadToEnd();
				}
			}
		}

		internal static Stream GetBinaryResourceStream(string name)
		{
			var assembly = typeof (Resources).GetTypeInfo().Assembly;

			// Once you figure out the name, pass it in as the argument here.
			var stream = assembly.GetManifestResourceStream(Prefix + name);

			return stream;
		}
	}
}