using System;
using System.Collections.Generic;
using System.IO;

namespace Myra.Graphics2D.UI.File
{
	// === Static file IO utility methods
	public partial class FileDialog
	{
		/// <summary>
		/// Attempt to retrieve folders from the given directory <paramref name="path"/>. Result <paramref name="folders"/> is null if return value is false.
		/// </summary>
		private static bool TryEnumerateDirectoryFolders(string path, out IEnumerable<string> folders, out string exceptionMsg)
		{
			List<string> result = new List<string>(8);
			try
			{
				result.AddRange(Directory.EnumerateDirectories(path));
			}
			catch (Exception e)
			{
				exceptionMsg = e.Message;
				folders = null;
				return false;
			}
			result.Sort();
			folders = result;
			exceptionMsg = null;
			return true;
		}

		/// <summary>
		/// Attempt to retrieve files from the given directory <paramref name="path"/>. Result <paramref name="files"/> is null if return value is false.
		/// <paramref name="searchPattern"/> can be null for a wildcard.
		/// </summary>
		private static bool TryEnumerateDirectoryFiles(string path, string searchPattern, out IEnumerable<string> files, out string exceptionMsg)
		{
			List<string> result = new List<string>(8);
			try
			{
				if (string.IsNullOrEmpty(searchPattern))
					result.AddRange(Directory.EnumerateFiles(path));
				else
				{
					foreach (string pattern in searchPattern.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
						result.AddRange(Directory.EnumerateFiles(path, pattern));
				}
			}
			catch (Exception e)
			{
				exceptionMsg = e.Message;
				files = null;
				return false;
			}
			result.Sort();
			files = result;
			exceptionMsg = null;
			return true;
		}

		/// <summary>
		/// Attempt to retreive permissions about a file or directory. Return value is indicative of success and a basic attribute filter.
		/// </summary>
		private static bool TryGetFileAttributes(string path, out FileAttributes attributes)
		{
			try
			{
				attributes = new FileInfo(path).Attributes;
			}
			catch
			{
				attributes = 0;
				return false;
			}

			bool discard =
				attributes.HasFlag(FileAttributes.System) |
				attributes.HasFlag(FileAttributes.Offline);

			return !discard;
		}

		private static bool FileAttributeFilter(FileAttributes attributes, FileDialogMode mode, bool showHidden)
		{
			bool discard =
				attributes.HasFlag(FileAttributes.System) |
				attributes.HasFlag(FileAttributes.Offline) |
				(attributes.HasFlag(FileAttributes.Hidden) & !showHidden);

			switch (mode)
			{
				case FileDialogMode.SaveFile:
					discard |= attributes.HasFlag(FileAttributes.ReadOnly);
					break;
				case FileDialogMode.ChooseFolder:
					discard |= !attributes.HasFlag(FileAttributes.Directory);
					break;
				default:
					break;
			}
			return !discard;
		}
	}
}