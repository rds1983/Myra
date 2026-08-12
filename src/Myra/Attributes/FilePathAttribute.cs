using Myra.Graphics2D.UI.File;
using System;

namespace Myra.Attributes
{
	/// <summary>
	/// Marks a string property as a file path to be edited via a file selection dialog.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class FilePathAttribute : Attribute
	{
		/// <summary>
		/// Gets the file dialog mode (open or save).
		/// </summary>
		public FileDialogMode DialogMode { get; private set; }

		/// <summary>
		/// Gets the file filter for the dialog.
		/// </summary>
		public string Filter { get; private set; }

		/// <summary>
		/// Gets a value indicating whether the file path should be displayed.
		/// </summary>
		public bool ShowPath { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="FilePathAttribute"/> class.
		/// </summary>
		/// <param name="dialogMode">The file dialog mode (open or save).</param>
		/// <param name="filter">The file filter for the dialog.</param>
		/// <param name="showPath">Whether the file path should be displayed.</param>
		public FilePathAttribute(FileDialogMode dialogMode, string filter = "", bool showPath = false)
		{
			DialogMode = dialogMode;
			Filter = filter;
			ShowPath = showPath;
		}
	}
}
