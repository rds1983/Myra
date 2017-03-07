using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using Myra.Utility;
using Menu = Myra.Graphics2D.UI.Menu;

namespace Myra.Samples
{
	public class Notepad: Game
	{
		private readonly GraphicsDeviceManager graphics;

		private string _filePath;
		private bool _dirty = true;
		private Desktop _host;
		private TextField _textField;

		public string FilePath
		{
			get
			{
				return _filePath;
			}

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
			get
			{
				return _dirty;
			}

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

		public Notepad()
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

			var data = File.ReadAllText("Assets/notepad.ui");
			var ui = Serialization.LoadFromData(data);

			var mainMenu = (Menu) ui.Root.FindWidgetById("mainMenu");
			var newItem = mainMenu.FindMenuItemById("menuItemNew");
			newItem.Selected += NewItemOnDown;

			// File/Open
			var openItem = mainMenu.FindMenuItemById("menuItemOpen");
			openItem.Selected += OpenItemOnDown;

			// File/Save
			var saveItem = mainMenu.FindMenuItemById("menuItemSave");
			saveItem.Selected += SaveItemOnDown;

			// File/Save As...
			var saveAsItem = mainMenu.FindMenuItemById("menuItemSaveAs");
			saveAsItem.Selected += SaveAsItemOnDown;

			// File/Quit
			var quitItem = mainMenu.FindMenuItemById("menuItemQuit");
			quitItem.Selected += QuitItemOnDown;

			var aboutItem = mainMenu.FindMenuItemById("menuItemAbout");
			aboutItem.Selected += AboutItemOnDown;

			_textField = (TextField)ui.Root.FindWidgetById("textArea");

			_textField.Text =
				"Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum!";

			_textField.TextChanged += TextFieldOnTextChanged;

			_host.Widgets.Add(ui.Root);
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

		private void Save(bool setFileName)
		{
			if (string.IsNullOrEmpty(FilePath) || setFileName)
			{
				using (var dlg = new SaveFileDialog())
				{
					dlg.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
					if (dlg.ShowDialog() == DialogResult.OK)
					{
						FilePath = dlg.FileName;
					}
					else
					{
						return;
					}
				}
			}

			using (var writer = new StreamWriter(_filePath))
			{
				writer.Write(_textField.Text);
			}

			Dirty = false;
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
			string filePath;
			using (var dlg = new OpenFileDialog())
			{
				dlg.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					filePath = dlg.FileName;
				}
				else
				{
					return;
				}
			}

			using (var reader = new StreamReader(filePath))
			{
				_textField.Text = reader.ReadToEnd();
			}

			FilePath = _filePath;
			Dirty = false;
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

			_host.Bounds = new Rectangle(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
			_host.Render();
		}
	}
}
