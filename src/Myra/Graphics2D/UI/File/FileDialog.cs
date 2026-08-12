using MonoGame.Utilities;
using Myra.Events;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using System.Collections.Generic;
using System.IO;
using System.Collections;

namespace Myra.Graphics2D.UI.File
{
	/// <summary>
	/// A dialog for selecting files or folders from the file system.
	/// </summary>
	public partial class FileDialog
	{
		/// <summary>
		/// Represents the path and type of a browsable location (folder or drive).
		/// </summary>
		protected class PathInfo
		{
			/// <summary>
			/// Gets the full path to the location.
			/// </summary>
			public string Path { get; }

			/// <summary>
			/// Gets a value indicating whether this location is a drive.
			/// </summary>
			public bool IsDrive { get; }

			/// <summary>
			/// Initializes a new instance of the PathInfo class.
			/// </summary>
			/// <param name="path">The full path to the location.</param>
			/// <param name="isDrive">True if this location is a drive; otherwise, false.</param>
			public PathInfo(string path, bool isDrive)
			{
				Path = path;
				IsDrive = isDrive;
			}
		}
		/// <summary>
		/// Container for info about a browsable file system or device.
		/// </summary>
		protected class Location
		{
			/// <summary>
			/// Initializes a new instance of the Location class.
			/// </summary>
			/// <param name="volume">The volume label of the location.</param>
			/// <param name="label">The display label of the location.</param>
			/// <param name="path">The full path to the location.</param>
			/// <param name="isDrive">True if this location is a drive; otherwise, false.</param>
			public Location(string volume, string label, string path, bool isDrive)
			{
				VolumeLabel = volume;
				Label = label;
				Path = path;
				IsDrive = isDrive;
			}

			/// <summary>
			/// Gets the volume label of the location.
			/// </summary>
			public readonly string VolumeLabel;

			/// <summary>
			/// Gets the display label for the location.
			/// </summary>
			public readonly string Label;

			/// <summary>
			/// Gets the full path to the location.
			/// </summary>
			public readonly string Path;

			/// <summary>
			/// Gets a value indicating whether this location is a drive.
			/// </summary>
			public readonly bool IsDrive;
		}

		private const int ImageTextSpacing = 4;

		private readonly List<string> _paths = new List<string>();
		private readonly List<string> _history = new List<string>();
		private int _historyPosition;
		private readonly FileDialogMode _mode;

		/// <summary>
		/// Gets or sets the current folder path being browsed.
		/// </summary>
		public string Folder
		{
			get => _textFieldPath.Text;
			set => SetFolder(value, true);
		}

		/// <summary>
		/// Gets or sets the file filter used when enumerating files (e.g., "*.txt" or "*.*").
		/// </summary>
		public string Filter { get; set; }

		/// <summary>
		/// Gets or sets the name of the file being selected or saved.
		/// </summary>
		internal string FileName
		{
			get => _textFieldFileName.Text;
			set => _textFieldFileName.Text = value;
		}

		/// <summary>
		/// Gets or sets the full path of the selected file or folder.
		/// </summary>
		public string FilePath
		{
			get
			{
				if (_mode == FileDialogMode.ChooseFolder)
				{
					return Folder;
				}

				if (string.IsNullOrEmpty(Folder))
				{
					return FileName;
				}

				if (string.IsNullOrEmpty(FileName))
				{
					return Folder;
				}

				return Path.Combine(Folder, FileName);
			}

			set
			{
				Folder = Path.GetDirectoryName(value);
				FileName = Path.GetFileName(value);

				if (!string.IsNullOrEmpty(FileName))
				{
					foreach (var widget in _gridFiles.Widgets)
					{
						var asLabel = widget as Label;

						if (asLabel == null)
						{
							continue;
						}

						if (asLabel.Text == FileName)
						{
							_gridFiles.SelectedRowIndex = Grid.GetRow(asLabel);
							break;
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether to automatically add the file extension based on the active filter.
		/// </summary>
		public bool AutoAddFilterExtension { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether hidden files and folders are shown in the file list.
		/// </summary>
		public bool ShowHiddenFiles { get; set; }

		/// <summary>
		/// Gets or sets the image displayed for folder items in the file list.
		/// </summary>
		public IImage IconFolder { get; set; }

		/// <summary>
		/// Gets or sets the image displayed for drive items in the file list.
		/// </summary>
		public IImage IconDrive { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="FileDialog"/> class with the specified mode, stylesheet, and style.
		/// </summary>
		/// <param name="mode">The file dialog mode (open file, save file, or choose folder).</param>
		/// <param name="stylesheet">The stylesheet to use for applying the style.</param>
		/// <param name="styleName">The name of the style to apply. Defaults to the default stylesheet style.</param>
		public FileDialog(FileDialogMode mode, Stylesheet stylesheet, string styleName = Stylesheet.DefaultStyleName) : base(stylesheet, null)
		{
			_mode = mode;

			BuildUI();

			switch (mode)
			{
				case FileDialogMode.OpenFile:
					Title = "Open File...";
					break;
				case FileDialogMode.SaveFile:
					Title = "Save File...";
					break;
				case FileDialogMode.ChooseFolder:
					Title = "Choose Folder...";
					break;
			}

			AutoAddFilterExtension = true;

			if (mode == FileDialogMode.ChooseFolder)
			{
				_textBlockFileName.Visible = false;
				_textFieldFileName.Visible = false;
			}

			_splitPane.SetSplitterPosition(0, 0.3f);

			_buttonBack.Background = null;
			_buttonForward.Background = null;
			_buttonParent.Background = null;
			_listPlaces.Background = null;

			PopulatePlacesListUI(_listPlaces);

			if (_listPlaces.Widgets.Count > 0) //Set starting folder
			{
				var pathInfo = (PathInfo)_listPlaces.Widgets[0].Tag;
				SetFolder(pathInfo.Path, false);
			}

			_listPlaces.SelectedIndexChanged += OnPlacesSelectedIndexChanged;

			_gridFiles.SelectedIndexChanged += OnGridFilesSelectedIndexChanged;
			_gridFiles.TouchDoubleClick += OnGridFilesDoubleClick;

			_buttonParent.Click += OnButtonParent;

			_textFieldFileName.TextChanged += (s, a) => UpdateEnabled();

			_buttonBack.Click += OnButtonBack;
			_buttonForward.Click += OnButtonForward;

			_textFieldFileName.Readonly = !(mode == FileDialogMode.SaveFile);

			UpdateEnabled();

			SetStyle(stylesheet, styleName);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FileDialog"/> class with the specified mode.
		/// </summary>
		/// <param name="mode">The file dialog mode (open file, save file, or choose folder).</param>
		/// <param name="styleName">The name of the style to apply. Defaults to the default stylesheet style.</param>
		public FileDialog(FileDialogMode mode, string styleName = Stylesheet.DefaultStyleName) : this(mode, Stylesheet.Current, styleName)
		{
		}

		/// <summary>
		/// Handles the placement of the dialog window and updates the folder view.
		/// </summary>
		protected override void OnPlacedChanged()
		{
			base.OnPlacedChanged();

			UpdateFolder();
		}

		/// <summary>
		/// Populates the navigation menu with places that can be visited (user directories and drives).
		/// </summary>
		/// <param name="listView">The list view to populate with place items.</param>
		protected virtual void PopulatePlacesListUI(ListView listView)
		{
			List<Location> placeList = new List<Location>(8);
			int index = 0;

			//Add user directories
			Platform.AppendUserPlacesOnSystem(placeList, Platform.SystemUserPlacePaths, _mode, ShowHiddenFiles);
			for (; index < placeList.Count; index++)
				listView.Widgets.Add(CreateListItem(placeList[index]));

			if (_listPlaces.Widgets.Count > 0)
				listView.Widgets.Add(new HorizontalSeparator());

			//Add file system drives
			Platform.AppendDrivesOnSystem(placeList);
			for (; index < placeList.Count; index++)
				listView.Widgets.Add(CreateListItem(placeList[index]));
		}

		/// <summary>
		/// Creates a display widget for the given file system location (drive or folder).
		/// </summary>
		/// <param name="location">The location to create a display widget for.</param>
		/// <returns>A widget displaying the location with icon and label.</returns>
		protected virtual Widget CreateListItem(Location location)
		{
			var item = new HorizontalStackPanel
			{
				Spacing = ImageTextSpacing,
				Tag = new PathInfo(location.Path, location.IsDrive)
			};

			string label = string.IsNullOrEmpty(location.VolumeLabel)
				? location.Label
				: $"[{location.VolumeLabel}] {location.Label}";

			item.Widgets.Add(new Image());
			item.Widgets.Add(new Label { Text = label });
			return item;
		}

		/// <summary>
		/// Determines whether a path is a valid accessible directory that can be browsed.
		/// </summary>
		/// <param name="path">The directory path to check.</param>
		/// <returns>True if the path is a valid directory with appropriate access permissions; otherwise, false.</returns>
		protected bool TryAccessDirectory(string path)
		{
			if (!Directory.Exists(path))
				return false;

			// For Windows, existance check is enough
			if (CurrentPlatform.OS == OS.Windows)
			{
				return true;
			}

			return CheckAccess(path, _mode, ShowHiddenFiles);
		}

		/// <summary>
		/// Displays an error message dialog for I/O operations that failed.
		/// </summary>
		/// <param name="path">The file path where the error occurred.</param>
		/// <param name="exceptionMsg">The error message describing what went wrong.</param>
		protected void ShowIOError(string path, string exceptionMsg)
		{
			CreateMessageBox("I/O Error", exceptionMsg);
		}

		private void UpdateEnabled()
		{
			var enabled = false;
			switch (_mode)
			{
				case FileDialogMode.OpenFile:
					enabled = !string.IsNullOrEmpty(FileName) && System.IO.File.Exists(FilePath);
					break;
				case FileDialogMode.SaveFile:
					enabled = !string.IsNullOrEmpty(FileName);
					break;
				case FileDialogMode.ChooseFolder:
					enabled = !string.IsNullOrEmpty(Folder);
					break;
			}

			ButtonOk.Enabled = enabled;
		}

		private void OnButtonParent(object sender, MyraEventArgs args)
		{
			if (string.IsNullOrEmpty(Folder))
			{
				return;
			}

			var parentFolder = Path.GetDirectoryName(Folder);

			Folder = parentFolder;
		}

		private void OnButtonBack(object sender, MyraEventArgs args)
		{
			if (_historyPosition <= 0)
			{
				return;
			}

			--_historyPosition;
			if (_historyPosition >= 0 && _historyPosition < _history.Count)
			{
				SetFolder(_history[_historyPosition], false);
			}
		}

		private void OnButtonForward(object sender, MyraEventArgs args)
		{
			if (_historyPosition >= _history.Count - 1)
			{
				return;
			}

			++_historyPosition;
			if (_historyPosition >= 0 && _historyPosition < _history.Count)
			{
				SetFolder(_history[_historyPosition], false);
			}
		}

		private void SetFolder(string value, bool storeInHistory)
		{
			if (!TryAccessDirectory(value))
			{
				return;
			}

			_textFieldPath.Text = value;
			UpdateFolder();
			UpdateEnabled();

			if (!storeInHistory)
			{
				return;
			}

			while (_history.Count > 0 && _historyPosition < _history.Count - 1)
			{
				_history.RemoveAt(_history.Count - 1);
			}

			_history.Add(Folder);

			_historyPosition = _history.Count - 1;
		}

		private void OnGridFilesDoubleClick(object sender, MyraEventArgs args)
		{
			if (_gridFiles.SelectedRowIndex == null)
			{
				return;
			}

			var path = _paths[_gridFiles.SelectedRowIndex.Value];

			if (Directory.Exists(path))
			{
				_listPlaces.SelectedIndex = null;
				Folder = path;
			}
			else
			{
				OnOk();
			}
		}

		private void OnGridFilesSelectedIndexChanged(object sender, MyraEventArgs args)
		{
			if (_gridFiles.SelectedRowIndex == null)
			{
				return;
			}

			_listPlaces.SelectedIndex = null;

			var path = _paths[_gridFiles.SelectedRowIndex.Value];
			var fi = new FileInfo(path);
			if (fi.Attributes.HasFlag(FileAttributes.Directory) && _mode == FileDialogMode.ChooseFolder)
			{
				_textFieldPath.Text = path;
			}
			else if (!fi.Attributes.HasFlag(FileAttributes.Directory) && _mode != FileDialogMode.ChooseFolder)
			{
				FileName = Path.GetFileName(path);
			}
		}

		private void OnPlacesSelectedIndexChanged(object sender, MyraEventArgs args)
		{
			if (_listPlaces.SelectedIndex == null)
			{
				return;
			}

			var pathInfo = (PathInfo)_listPlaces.Widgets[_listPlaces.SelectedIndex.Value].Tag;
			Folder = pathInfo.Path;
		}

		private void UpdateFolder()
		{
			_gridFiles.RowsProportions.Clear();
			_gridFiles.Widgets.Clear();
			_paths.Clear();

			_scrollPane.ScrollPosition = Mathematics.PointZero;

			if (Desktop == null)
			{
				return;
			}

			var path = _textFieldPath.Text;

			// Enumerate folders in directory
			bool success = TryEnumerateDirectoryFolders(path, out IEnumerable<string> collection, out string exceptionMsg);
			if (!success)
			{
				ShowIOError(path, exceptionMsg);
				return;
			}

			var gridY = 0;
			foreach (string folder in collection)
			{
				if (!CheckAccess(folder, _mode, ShowHiddenFiles))
				{
					continue;
				}

				_gridFiles.RowsProportions.Add(new Proportion());

				var image = new Image
				{
					Renderable = IconFolder,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center
				};
				Grid.SetRow(image, gridY);

				_gridFiles.Widgets.Add(image);

				var name = new Label
				{
					Text = Path.GetFileName(folder),
				};
				Grid.SetColumn(name, 1);
				Grid.SetRow(name, gridY);

				_gridFiles.Widgets.Add(name);

				_paths.Add(folder);

				++gridY;
			}

			if (_mode == FileDialogMode.ChooseFolder)
			{
				return;
			}

			// Enumerate files in directory
			success = TryEnumerateDirectoryFiles(path, Filter, out collection, out exceptionMsg);
			if (!success)
			{
				ShowIOError(path, exceptionMsg);
				return;
			}

			foreach (string file in collection)
			{
				if (!CheckAccess(file, _mode, ShowHiddenFiles))
				{
					continue;
				}

				_gridFiles.RowsProportions.Add(new Proportion());

				var name = new Label
				{
					Text = Path.GetFileName(file),
				};
				Grid.SetColumn(name, 1);
				Grid.SetRow(name, gridY);

				_gridFiles.Widgets.Add(name);

				_paths.Add(file);

				++gridY;
			}
		}

		/// <summary>
		/// Determines whether the dialog can be closed by clicking OK. For save dialogs, confirms file overwrite if necessary.
		/// </summary>
		/// <returns>True if the dialog can be closed; otherwise, false.</returns>
		protected internal override bool CanCloseByOk()
		{
			if (_mode != FileDialogMode.SaveFile)
			{
				return true;
			}

			var fileName = FileName;

			if (AutoAddFilterExtension && !string.IsNullOrEmpty(Filter))
			{
				var idx = Filter.LastIndexOf('.');
				if (idx != -1)
				{
					var ext = Filter.Substring(idx);

					if (!fileName.EndsWith(ext))
					{
						fileName += ext;
					}
				}
			}

			if (System.IO.File.Exists(Path.Combine(Folder, fileName)))
			{
				var dlg = CreateMessageBox("Confirm Replace",
					string.Format("File named '{0}' already exists. Do you want to replace it?", fileName));

				dlg.Closed += (s, a) =>
				{
					if (!dlg.Result)
					{
						return;
					}

					FileName = fileName;

					Result = true;
					Close();
				};

				dlg.ShowModal(Desktop);
			}
			else
			{
				FileName = fileName;

				Result = true;
				Close();
			}

			return false;
		}

		internal override IDictionary GetStylesDictionary(Stylesheet stylesheet) => stylesheet.FileDialogStyles;

		/// <summary>
		/// Applies the specified widget style to this file dialog.
		/// </summary>
		/// <param name="style">The widget style to apply.</param>
		protected override void ApplyStyle(WidgetStyle style)
		{
			base.ApplyStyle(style);

			var fileDialogStyle = (FileDialogStyle)style;
			_buttonBack.ApplyImageButtonStyle(fileDialogStyle.BackButtonStyle);
			_buttonForward.ApplyImageButtonStyle(fileDialogStyle.ForwardButtonStyle);
			_buttonParent.ApplyImageButtonStyle(fileDialogStyle.ParentButtonStyle);

			_gridFiles.SelectionBackground = fileDialogStyle.SelectionBackground;
			_gridFiles.SelectionHoverBackground = fileDialogStyle.SelectionHoverBackground;

			IconDrive = fileDialogStyle.IconDrive;
			IconFolder = fileDialogStyle.IconFolder;

			foreach (var widget in _listPlaces.Widgets)
			{
				var container = widget as Container;
				if (container == null)
				{
					continue;
				}

				var pathInfo = (PathInfo)container.Tag;
				var image = (Image)container.Widgets[0];

				image.Renderable = pathInfo.IsDrive ? IconDrive : IconFolder;
			}
		}
	}
}