using System;
using System.IO;
using Microsoft.Xna.Framework;
using Myra.Editor.UI.File;
using Myra.Graphics2D.UI;

namespace Myra.Samples.Notepad
{
	public class NotepadGame : Game
	{
		private readonly GraphicsDeviceManager graphics;

		private string _filePath;
		private bool _dirty = true;
		private Desktop _host;
		private TextField _textField;

		public string FilePath
		{
			get { return _filePath; }

			set
			{
				if (value == _filePath)
				{
					return;
				}

				_filePath = value;

				UpdateTitle();
			}
		}

		public bool Dirty
		{
			get { return _dirty; }

			set
			{
				if (value == _dirty)
				{
					return;
				}

				_dirty = value;
				UpdateTitle();
			}
		}

		public NotepadGame()
		{
			graphics = new GraphicsDeviceManager(this);
			IsMouseVisible = true;
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			MyraEnvironment.Game = this;

			UpdateTitle();

			_host = new Desktop();

			// Load UI
			var ui = new Notepad();

			var newItem = ui.menuItemNew;
			newItem.Selected += NewItemOnDown;

			// File/Open
			var openItem = ui.menuItemOpen;
			openItem.Selected += OpenItemOnDown;

			// File/Save
			var saveItem = ui.menuItemSave;
			saveItem.Selected += SaveItemOnDown;

			// File/Save As...
			var saveAsItem = ui.menuItemSaveAs;
			saveAsItem.Selected += SaveAsItemOnDown;

			// File/Quit
			var quitItem = ui.menuItemQuit;
			quitItem.Selected += QuitItemOnDown;

			var aboutItem = ui.menuItemAbout;
			aboutItem.Selected += AboutItemOnDown;

			_textField = ui.textArea;

			_textField.Text =
				"Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum!";

			_textField.TextChanged += TextFieldOnTextChanged;

			_host.Widgets.Add(ui);
		}

		private void UpdateTitle()
		{
			Window.Title = CalculateTitle();
		}

		private string CalculateTitle()
		{
			if (string.IsNullOrEmpty(_filePath))
			{
				return "Notepad";
			}

			if (!Dirty)
			{
				return _filePath;
			}

			return _filePath + " *";
		}

		private void TextFieldOnTextChanged(object sender, EventArgs eventArgs)
		{
			Dirty = true;
		}

		private void ProcessSave(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				return;
			}

			File.WriteAllText(filePath, _textField.Text);

			FilePath = filePath;
			Dirty = false;
		}

		private void Save(bool setFileName)
		{
			if (string.IsNullOrEmpty(FilePath) || setFileName)
			{
				var dlg = new FileDialog(FileDialogMode.SaveFile)
				{
					Filter = "*.txt"
				};

				if (!string.IsNullOrEmpty(FilePath))
				{
					dlg.FilePath = FilePath;
				}

				dlg.Closed += (s, a) =>
				{
					if (dlg.Result)
					{
						ProcessSave(dlg.FilePath);
					}
				};

				dlg.ShowModal(_host);
			}
			else
			{
				ProcessSave(FilePath);
			}
		}

		private void AboutItemOnDown(object sender, EventArgs eventArgs)
		{
			var messageBox = Dialog.CreateMessageBox("Notepad", "Myra Notepad Sample " + MyraEnvironment.Version);
			messageBox.ShowModal(_host);
		}

		private void SaveAsItemOnDown(object sender, EventArgs eventArgs)
		{
			Save(true);
		}

		private void SaveItemOnDown(object sender, EventArgs eventArgs)
		{
			Save(false);
		}

		private void OpenItemOnDown(object sender, EventArgs eventArgs)
		{
			var dlg = new FileDialog(FileDialogMode.OpenFile)
			{
				Filter = "*.txt"
			};

			if (!string.IsNullOrEmpty(FilePath))
			{
				dlg.FilePath = FilePath;
			}

			dlg.Closed += (s, a) =>
			{
				if (!dlg.Result)
				{
					return;
				}

				var filePath = dlg.FilePath;
				if (string.IsNullOrEmpty(filePath))
				{
					return;
				}

				_textField.Text = File.ReadAllText(filePath);
				FilePath = filePath;
				Dirty = false;
			};

			dlg.ShowModal(_host);
		}

		private void NewItemOnDown(object sender, EventArgs eventArgs)
		{
			FilePath = string.Empty;
			_textField.Text = string.Empty;
		}

		private void QuitItemOnDown(object sender, EventArgs genericEventArgs)
		{
			Exit();
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			if (graphics.PreferredBackBufferWidth != Window.ClientBounds.Width ||
			    graphics.PreferredBackBufferHeight != Window.ClientBounds.Height)
			{
				graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
				graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
				graphics.ApplyChanges();
			}

			GraphicsDevice.Clear(Color.Black);

			_host.Bounds = new Rectangle(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth,
				GraphicsDevice.PresentationParameters.BackBufferHeight);
			_host.Render();
		}

	}
}