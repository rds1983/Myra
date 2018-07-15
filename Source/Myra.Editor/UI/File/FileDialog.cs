using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;

namespace Myra.Editor.UI.File
{
	public partial class FileDialog : Dialog
	{
		private const int ImageTextSpacing = 4;

		private static readonly string[] Folders =
		{
			"Desktop", "Downloads"
		};

		private readonly List<string> _paths = new List<string>();
		private readonly FileDialogMode _mode;

		internal string Folder
		{
			get { return _textFieldPath.Text; }

			set
			{
				if (value == _textFieldPath.Text)
				{
					return;
				}

				_textFieldPath.Text = value;
				UpdateFolder();
			}
		}

		/// <summary>
		/// File filter that is used as 2nd parameter for Directory.EnumerateFiles call
		/// </summary>
		public string Filter
		{
			get; set;
		}

		internal string SelectedPath
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
				return System.IO.Path.Combine(Folder, SelectedPath);
			}

			set
			{
				Folder = System.IO.Path.GetDirectoryName(value);
				SelectedPath = System.IO.Path.GetFileName(value);
			}
		}

		public event EventHandler Selected;
		public event EventHandler Cancelled;

		public FileDialog(FileDialogMode mode)
		{
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

			_mode = mode;

			BuildUI();

			_splitPane.SetSplitterPosition(0, 0.3f);

			_buttonBack.Background = null;
			_buttonForward.Background = null;
			_buttonParent.Background = null;

			_listBoxPlaces.Background = null;

			_buttonBack.Image = DefaultAssets.UISpritesheet["icon-arrow-left"];
			_buttonForward.Image = DefaultAssets.UISpritesheet["icon-arrow-right"];
			_buttonParent.Image = DefaultAssets.UISpritesheet["icon-folder-parent"];

			var homePath = (Environment.OSVersion.Platform == PlatformID.Unix ||
							Environment.OSVersion.Platform == PlatformID.MacOSX)
				? Environment.GetEnvironmentVariable("HOME")
				: Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");

			var iconFolder = DefaultAssets.UISpritesheet["icon-folder"];

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
				_listBoxPlaces.Items.Add(new ListItem(Path.GetFileName(p), null, p)
				{
					Image = iconFolder,
					ImageTextSpacing = ImageTextSpacing
				});
			}

			_listBoxPlaces.Items.Add(new ListItem
			{
				IsSeparator = true
			});

			var drives = DriveInfo.GetDrives();

			var iconDrive = DefaultAssets.UISpritesheet["icon-drive"];
			foreach (var d in drives)
			{
				if (d.DriveType == DriveType.Ram)
				{
					continue;
				}

				var s = d.RootDirectory.Name;

				if (!string.IsNullOrEmpty(d.VolumeLabel) && d.VolumeLabel != d.RootDirectory.Name)
				{
					s += " (" + d.VolumeLabel + ")";
				}

				_listBoxPlaces.Items.Add(new ListItem(s, null, d.RootDirectory.Name)
				{
					Image = iconDrive,
					ImageTextSpacing = ImageTextSpacing
				});
			}

			_listBoxPlaces.SelectedIndexChanged += OnPlacesSelectedIndexChanged;
			_listBoxPlaces.SelectedIndex = 0;

			_gridFiles.SelectedIndexChanged += OnGridFilesSelectedIndexChanged;
			_gridFiles.DoubleClick += OnGridFilesDoubleClick;

			_textFieldFileName.TextChanged += (s, a) => UpdateEnabled();

			ButtonOk.Down += OnButtonOk;
			ButtonCancel.Down += (s, a) =>
			{
				var ev = Cancelled;
				if (ev != null)
				{
					ev(this, EventArgs.Empty);
				}
			};

			UpdateEnabled();
		}

		private void UpdateEnabled()
		{
			ButtonOk.Enabled = !string.IsNullOrEmpty(SelectedPath);
		}

		private void OnButtonOk(object sender, EventArgs args)
		{
			var ev = Selected;

			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}

		private void OnGridFilesDoubleClick(object sender, EventArgs args)
		{
			if (_gridFiles.SelectedIndex == null)
			{
				return;
			}


			var path = _paths[_gridFiles.SelectedIndex.Value];

			if (!Directory.Exists(path))
			{
				return;
			}

			_listBoxPlaces.SelectedItem = null;
			Folder = path;
		}

		private void OnGridFilesSelectedIndexChanged(object sender, EventArgs args)
		{
			if (_gridFiles.SelectedIndex == null)
			{
				return;
			}

			_listBoxPlaces.SelectedItem = null;

			var path = _paths[_gridFiles.SelectedIndex.Value];

			if (!System.IO.File.Exists(path))
			{
				return;
			}

			var fi = new FileInfo(path);

			var choose = (fi.Attributes.HasFlag(FileAttributes.Directory) && _mode == FileDialogMode.ChooseFolder) ||
						 (!fi.Attributes.HasFlag(FileAttributes.Directory) && _mode != FileDialogMode.ChooseFolder);

			if (choose)
			{
				SelectedPath = Path.GetFileName(path);
			}
		}

		private void OnPlacesSelectedIndexChanged(object sender, EventArgs args)
		{
			if (_listBoxPlaces.SelectedItem == null)
			{
				return;
			}

			var path = (string)_listBoxPlaces.Items[_listBoxPlaces.SelectedIndex].Tag;
			Folder = path;
		}

		private void UpdateFolder()
		{
			_gridFiles.RowsProportions.Clear();
			_gridFiles.Widgets.Clear();
			_paths.Clear();

			_scrollPane.ScrollPosition = Point.Zero;

			var path = _textFieldPath.Text;
			var folders = Directory.EnumerateDirectories(path);

			var iconFolder = DefaultAssets.UISpritesheet["icon-folder"];

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
					TextureRegion = iconFolder,
					GridPositionY = gridY,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center
				};

				_gridFiles.Widgets.Add(image);

				var name = new TextBlock
				{
					Text = Path.GetFileName(f),
					GridPositionX = 1,
					GridPositionY = gridY
				};

				_gridFiles.Widgets.Add(name);

				_paths.Add(f);

				++gridY;
			}

			IEnumerable<string> files;

			if (string.IsNullOrEmpty(Filter))
			{
				files = Directory.EnumerateFiles(path);
			}
			else
			{
				files = Directory.EnumerateFiles(path, Filter);
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

				var name = new TextBlock
				{
					Text = Path.GetFileName(f),
					GridPositionX = 1,
					GridPositionY = gridY
				};

				_gridFiles.Widgets.Add(name);

				_paths.Add(f);

				++gridY;
			}
		}
	}
}