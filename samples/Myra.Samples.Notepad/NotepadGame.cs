using System;
using System.IO;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.File;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using AssetManagementBase.Utility;

namespace Myra.Samples.Notepad
{
	public class NotepadGame : Game
	{
		private readonly GraphicsDeviceManager graphics;

		private string _filePath;
		private bool _dirty = true;
		private TextBox _textField;
		private Desktop _desktop;

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
			Window.AllowUserResizing = true;
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			MyraEnvironment.Game = this;

			UpdateTitle();

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

			ui.menuItemDebugOptions.Selected += DebugOptionsOnDown;

			// File/Quit
			var quitItem = ui.menuItemQuit;
			quitItem.Selected += QuitItemOnDown;

			var aboutItem = ui.menuItemAbout;
			aboutItem.Selected += AboutItemOnDown;

			_textField = ui.textArea;

			_textField.Text = typeof(NotepadGame).Assembly.ReadResourceAsString("Myra.Samples.Notepad.hobbits.txt");

			_textField.TextChanged += TextBoxOnTextChanged;

			_desktop = new Desktop
			{
				FocusedKeyboardWidget = _textField,
				Root = ui
			};

			_desktop.KeyDown += (s, a) =>
			{
				if (_desktop.HasModalWidget || ui._mainMenu.IsOpen)
				{
					return;
				}

				if (_desktop.IsKeyDown(Keys.LeftControl) || _desktop.IsKeyDown(Keys.RightControl))
				{
					if (_desktop.IsKeyDown(Keys.N))
					{
						NewItemOnDown(this, EventArgs.Empty);
					}
					else if (_desktop.IsKeyDown(Keys.O))
					{
						OpenItemOnDown(this, EventArgs.Empty);
					}
					else if (_desktop.IsKeyDown(Keys.S))
					{
						SaveItemOnDown(this, EventArgs.Empty);
					}
					else if (_desktop.IsKeyDown(Keys.Q))
					{
						Exit();
					}
				}
			};
		}


		private void DebugOptionsOnDown(object sender, EventArgs e)
		{
			var dlg = new DebugOptionsWindow();

			dlg.ShowModal(_desktop);
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

		private void TextBoxOnTextChanged(object sender, EventArgs eventArgs)
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

				dlg.ShowModal(_desktop);
			}
			else
			{
				ProcessSave(FilePath);
			}
		}

		private void AboutItemOnDown(object sender, EventArgs eventArgs)
		{
			var messageBox = Dialog.CreateMessageBox("Notepad", "Myra Notepad Sample " + MyraEnvironment.Version);
			messageBox.ShowModal(_desktop);
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

			dlg.ShowModal(_desktop);
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

			GraphicsDevice.Clear(Color.Black);

			_desktop.Render();
		}
	}
}