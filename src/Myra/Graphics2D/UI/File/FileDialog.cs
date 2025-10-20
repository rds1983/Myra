using System;
using System.Collections.Generic;
using System.IO;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;

namespace Myra.Graphics2D.UI.File
{
	public partial class FileDialog
	{
		protected class PathInfo
		{
			public string Path { get; }
			public bool IsDrive { get; }

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
			public Location(string volume, string label, string path, bool isDrive)
			{
				VolumeLabel = volume;
				Label = label;
				Path = path;
				IsDrive = isDrive;
			}
            
			public readonly string VolumeLabel;
			public readonly string Label;
			public readonly string Path;
			public readonly bool IsDrive;
		}

		private const int ImageTextSpacing = 4;
		
		private readonly List<string> _paths = new List<string>();
		private readonly List<string> _history = new List<string>();
		private int _historyPosition;
		private readonly FileDialogMode _mode;

		public string Folder
		{
			get => _textFieldPath.Text;
			set => SetFolder(value, true);
		}

		/// <summary>
		/// File filter that is used as 2nd parameter for Directory.EnumerateFiles call
		/// </summary>
		public string Filter { get; set; }

		internal string FileName
		{
			get => _textFieldFileName.Text;
			set => _textFieldFileName.Text = value;
		}

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

		public bool AutoAddFilterExtension { get; set; }
		public bool ShowHiddenFiles { get; set; }
		
		public IImage IconFolder { get; set; }
		public IImage IconDrive { get; set; }

		public FileDialog(FileDialogMode mode) : base(null)
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

			SetStyle(Stylesheet.DefaultStyleName);
		}

		protected override void OnPlacedChanged()
		{
			base.OnPlacedChanged();

			UpdateFolder();
		}

		/// <summary>
		/// Create the navigation menu of places we can visit.
		/// </summary>
		protected virtual void PopulatePlacesListUI(ListView listView)
		{
			List<Location> placeList = new List<Location>(8);
			int index = 0;
			
			//Add user directories
			Platform.AppendUserPlacesOnSystem(placeList, Platform.SystemUserPlacePaths);
			for (; index < placeList.Count; index++)
				listView.Widgets.Add( CreateListItem(placeList[index]) );
			
			if (_listPlaces.Widgets.Count > 0)
				listView.Widgets.Add(new HorizontalSeparator());
			
			//Add file system drives
			Platform.AppendDrivesOnSystem(placeList);
			for (; index < placeList.Count; index++)
				listView.Widgets.Add( CreateListItem(placeList[index]) );
		}
		
		/// <summary>
		/// Create a display widget for the given location
		/// </summary>
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
		/// Return true if <paramref name="path"/> is a valid directory, and we have permissions to access it.
		/// </summary>
		protected bool TryAccessDirectory(string path)
		{
			if (!Directory.Exists(path))
				return false;
			if (!TryGetFileAttributes(path, out FileAttributes att))
				return false;
			return FileAttributeFilter(att, _mode, ShowHiddenFiles);
		}
		
		protected virtual bool FileAttributeFilter(FileAttributes attributes, FileDialogMode mode, bool showHidden)
		{
			bool discard =
				attributes.HasFlag(FileAttributes.System)  |
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

		private void OnButtonParent(object sender, EventArgs args)
		{
			if (string.IsNullOrEmpty(Folder))
			{
				return;
			}

			var parentFolder = Path.GetDirectoryName(Folder);

			Folder = parentFolder;
		}

		private void OnButtonBack(object sender, EventArgs args)
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

		private void OnButtonForward(object sender, EventArgs args)
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

		private void OnGridFilesDoubleClick(object sender, EventArgs args)
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

		private void OnGridFilesSelectedIndexChanged(object sender, EventArgs args)
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

		private void OnPlacesSelectedIndexChanged(object sender, EventArgs args)
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
				success = TryGetFileAttributes(folder, out FileAttributes att);
				if (success)
					success &= FileAttributeFilter(att, _mode, ShowHiddenFiles);
				if (!success)
					continue;
				
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
				success = TryGetFileAttributes(file, out FileAttributes att);
				if (success)
					success &= FileAttributeFilter(att, _mode, ShowHiddenFiles);
				if (!success)
					continue;
				
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

		public void ApplyFileDialogStyle(FileDialogStyle style)
		{
			ApplyWindowStyle(style);

			_buttonBack.ApplyImageButtonStyle(style.BackButtonStyle);
			_buttonForward.ApplyImageButtonStyle(style.ForwardButtonStyle);
			_buttonParent.ApplyImageButtonStyle(style.ParentButtonStyle);

			_gridFiles.SelectionBackground = style.SelectionBackground;
			_gridFiles.SelectionHoverBackground = style.SelectionHoverBackground;

			IconDrive = style.IconDrive;
			IconFolder = style.IconFolder;

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
		
		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyFileDialogStyle(stylesheet.FileDialogStyles.SafelyGetStyle(name));
		}
	}
}