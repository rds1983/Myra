using MonoGame.Utilities;
using System;

namespace TextCopy
{
	/// <summary>
	/// Provides access to system clipboard functionality with optional local clipboard fallback.
	/// </summary>
	public static class Clipboard
	{
		/// <summary>
		/// Gets or sets a value indicating whether to use a local clipboard instead of the system clipboard.
		/// </summary>
		public static bool UseLocalClipboard = false;
		private static string _localTextClipboard;

		static Action<string> setAction = CreateSet();
		static Func<string> getFunc = CreateGet();

		/// <summary>
		/// Sets the text content of the clipboard.
		/// </summary>
		/// <param name="text">The text to set in the clipboard.</param>
		public static void SetText(string text)
		{
			if (text == null)
			{
				throw new ArgumentNullException(nameof(text));
			}

			if (UseLocalClipboard)
			{
				_localTextClipboard = text;
				return;
			}

			setAction(text);
		}

		/// <summary>
		/// Gets the text content from the clipboard.
		/// </summary>
		/// <returns>The text content of the clipboard.</returns>
		public static string GetText()
		{
			if (UseLocalClipboard)
			{
				return _localTextClipboard;
			}

			return getFunc();
		}

		static Action<string> CreateSet()
		{
			if (CurrentPlatform.OS == OS.Windows)
			{
				return WindowsClipboard.SetText;
			}

			if (CurrentPlatform.OS == OS.MacOSX)
			{
				return OsxClipboard.SetText;
			}

			if (CurrentPlatform.OS == OS.Linux)
			{
				if (LinuxClipboard.SystemIsCompatible())
					return LinuxClipboard.SetText;
				Console.WriteLine(LinuxClipboard.GetCompatibilityProblem());
				UseLocalClipboard = true;
			}

			return s => throw new NotSupportedException();
		}

		static Func<string> CreateGet()
		{
			if (CurrentPlatform.OS == OS.Windows)
			{
				return WindowsClipboard.GetText;
			}

			if (CurrentPlatform.OS == OS.MacOSX)
			{
				return OsxClipboard.GetText;
			}

			if (CurrentPlatform.OS == OS.Linux)
			{
				if (LinuxClipboard.SystemIsCompatible())
					return LinuxClipboard.GetText;
				Console.WriteLine(LinuxClipboard.GetCompatibilityProblem());
				UseLocalClipboard = true;
			}

			return () => throw new NotSupportedException();
		}
	}
}