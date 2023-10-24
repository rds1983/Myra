using System;
using System.Collections.Generic;
using System.IO;
using Myra.Utility;

namespace Myra.Graphics2D.UI.File
{
	public partial class FileDialog
	{
		private const int ImageTextSpacing = 4;

		private static readonly string[] Folders =
		{
			"Desktop", "Downloads"
		};

		private readonly List<string> _paths = new List<string>();
		private readonly List<string> _history = new List<string>();
		private int _historyPosition;
		private readonly FileDialogMode _mode;

		public string Folder
		{
			get { return _textFieldPath.Text; }

			set
			{
				SetFolder(value, true);
			}
		}

		/// <summary>
		/// File filter that is used as 2nd parameter for Directory.EnumerateFiles call
		/// </summary>
		public string Filter
		{
			get; set;
		}

		internal string FileName
		{
			get { return _textFieldFileName.Text; }

			set
			{
				_textFieldFileName.Text = value;
			}
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

		public FileDialog(FileDialogMode mode)
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

			_listBoxPlaces.Background = null;

			_buttonBack.Image = DefaultAssets.UITextureRegionAtlas["icon-arrow-left"];
			_buttonForward.Image = DefaultAssets.UITextureRegionAtlas["icon-arrow-right"];
			_buttonParent.Image = DefaultAssets.UITextureRegionAtlas["icon-folder-parent"];

			var homePath = (Environment.OSVersion.Platform == PlatformID.Unix ||
							Environment.OSVersion.Platform == PlatformID.MacOSX)
				? Environment.GetEnvironmentVariable("HOME")
				: Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");

			var iconFolder = DefaultAssets.UITextureRegionAtlas["icon-folder"];

			var places = new List<string>
			{
				homePath
			};

			foreach (var f in Folders)
			{
				places.Add(Path.Combine(homePath, f));
			}

			foreach (var p in places)
			{
				if (!Directory.Exists(p))
				{
					continue;
				}

				_listBoxPlaces.Items.Add(new ListItem(Path.GetFileName(p), null, p)
				{
					Image = iconFolder,
					ImageTextSpacing = ImageTextSpacing
				});
			}

			if (_listBoxPlaces.Items.Count > 0)
			{
				SetFolder((string)_listBoxPlaces.Items[0].Tag, false);
			}

			_listBoxPlaces.Items.Add(new ListItem
			{
				IsSeparator = true
			});

			var drives = DriveInfo.GetDrives();
			var iconDrive = DefaultAssets.UITextureRegionAtlas["icon-drive"];
			foreach (var d in drives)
			{
				if (d.DriveType == DriveType.Ram || d.DriveType == DriveType.Unknown)
				{
					continue;
				}

				try
				{
					var s = d.RootDirectory.FullName;

					if (!string.IsNullOrEmpty(d.VolumeLabel) && d.VolumeLabel != d.RootDirectory.FullName)
					{
						s += " (" + d.VolumeLabel + ")";
					}

					_listBoxPlaces.Items.Add(new ListItem(s, null, d.RootDirectory.FullName)
					{
						Image = iconDrive,
						ImageTextSpacing = ImageTextSpacing
					});
				}
				catch (Exception)
				{
				}
			}

			_listBoxPlaces.SelectedIndexChanged += OnPlacesSelectedIndexChanged;

			_gridFiles.SelectionBackground = DefaultAssets.UITextureRegionAtlas["tree-selection"];
			_gridFiles.SelectionHoverBackground = DefaultAssets.UITextureRegionAtlas["button-over"];
			_gridFiles.SelectedIndexChanged += OnGridFilesSelectedIndexChanged;
			_gridFiles.TouchDoubleClick += OnGridFilesDoubleClick;

			_buttonParent.Click += OnButtonParent;

			_textFieldFileName.TextChanged += (s, a) => UpdateEnabled();

			_buttonBack.Click += OnButtonBack;
			_buttonForward.Click += OnButtonForward;

			_textFieldFileName.Readonly = !(mode == FileDialogMode.SaveFile);

			UpdateEnabled();
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
			if (!Directory.Exists(value))
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
				_listBoxPlaces.SelectedIndex = null;
				Folder = path;
			} else
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

			_listBoxPlaces.SelectedIndex = null;

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
			if (_listBoxPlaces.SelectedIndex == null)
			{
				return;
			}

			var path = (string)_listBoxPlaces.Items[_listBoxPlaces.SelectedIndex.Value].Tag;
			Folder = path;
		}

		private void UpdateFolder()
		{
			_gridFiles.RowsProportions.Clear();
			_gridFiles.Widgets.Clear();
			_paths.Clear();

			_scrollPane.ScrollPosition = Mathematics.PointZero;

			var path = _textFieldPath.Text;
			var folders = Directory.EnumerateDirectories(path);

			var iconFolder = DefaultAssets.UITextureRegionAtlas["icon-folder"];

			var gridY = 0;
			foreach (var f in folders)
			{
				var fileInfo = new FileInfo(f);
				if (fileInfo.Attributes.HasFlag(FileAttributes.Hidden))
				{
					continue;
				}

				var prop = new Proportion();

				_gridFiles.RowsProportions.Add(prop);

				var image = new Image
				{
					Renderable = iconFolder,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center
				};
				Grid.SetRow(image, gridY);

				_gridFiles.Widgets.Add(image);

				var name = new Label
				{
					Text = Path.GetFileName(f),
				};
				Grid.SetColumn(name, 1);
				Grid.SetRow(name, gridY);

				_gridFiles.Widgets.Add(name);

				_paths.Add(f);

				++gridY;
			}

			if (_mode == FileDialogMode.ChooseFolder)
			{
				return;
			}

			IEnumerable<string> files;

			if (string.IsNullOrEmpty(Filter))
			{
				files = Directory.EnumerateFiles(path);
			}
			else
			{
				var parts = Filter.Split('|');
				var result = new List<string>();

				foreach (var part in parts)
				{
					result.AddRange(Directory.EnumerateFiles(path, part));
				}

				files = result;
			}

			foreach (var f in files)
			{
				var fileInfo = new FileInfo(f);
				if (fileInfo.Attributes.HasFlag(FileAttributes.Hidden))
				{
					continue;
				}

				var prop = new Proportion();

				_gridFiles.RowsProportions.Add(prop);

				var name = new Label
				{
					Text = Path.GetFileName(f),
				};
				Grid.SetColumn(name, 1);
				Grid.SetRow(name, gridY);

				_gridFiles.Widgets.Add(name);

				_paths.Add(f);

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
	}
}