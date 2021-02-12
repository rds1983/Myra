using System;
using System.Drawing;
using Silk.NET.Input;
using Silk.NET.Input.Common;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Common;
using TrippyGL;

namespace Myra.Samples.AllWidgets
{
	/// <summary>
	/// A base for all test projects that contains shared code.
	/// </summary>
	public abstract class TestBase
	{
		/// <summary>This application's <see cref="IWindow"/></summary>
		public IWindow Window { private set; get; }

		/// <summary>This application window's <see cref="IInputContext"/>.</summary>
		public IInputContext InputContext { private set; get; }

		/// <summary>Whether to allow the user to toggle fullscreen mode by pressing F11.</summary>
		public bool AllowToggleFullscreen = true;

		private Size preFullscreenSize;
		private Point preFullscreenPosition;
		private WindowState preFullscreenState;

		/// <summary>Gets or sets whether the window is currently on fullscreen mode.</summary>
		public bool IsFullscreen
		{
			get => Window.WindowState == WindowState.Fullscreen;
			set
			{
				if (value == IsFullscreen)
					return;

				Window.Resize -= OnResized;
				if (value)
				{
					Size screenSize;
					if (Window.Monitor.VideoMode.Resolution.HasValue)
						screenSize = Window.Monitor.VideoMode.Resolution.Value;
					else
						screenSize = new Size(Window.Monitor.Bounds.Width, Window.Monitor.Bounds.Height);
					preFullscreenSize = Window.Size;
					preFullscreenPosition = Window.Position;
					preFullscreenState = Window.WindowState;
					Window.WindowState = WindowState.Fullscreen;
					Window.Size = screenSize;
				}
				else
				{
					if (preFullscreenSize.Width < 10 || preFullscreenSize.Height < 10)
					{
						preFullscreenSize = GetNewWindowSize(Window.Monitor);
						preFullscreenPosition = new Point(Window.Monitor.Bounds.X + 50, Window.Monitor.Bounds.Y + 50);
						preFullscreenState = WindowState.Normal;
					}

					Window.WindowState = preFullscreenState;
					Window.Size = preFullscreenSize;
					Window.Position = preFullscreenPosition;
				}

				Window.Resize += OnResized;
				OnResized(Window.Size);
			}
		}

		/// <summary>The <see cref="GraphicsDevice"/> whose drawing commands go to the application's window.</summary>
		public GraphicsDevice graphicsDevice;

		public TestBase(string title = null, int preferredDepthBufferBits = 0, bool isSingleThreaded = true)
		{
			title ??= System.Reflection.Assembly.GetEntryAssembly()?.GetName()?.Name ?? "Title";
			Console.WriteLine("Starting up: \"" + title + "\"...");

			IMonitor mainMonitor = Monitor.GetMainMonitor();
			Size windowSize = GetNewWindowSize(mainMonitor);
			WindowOptions windowOpts = new WindowOptions()
			{
				API = new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.Debug, new APIVersion(3, 3)),
				VSync = VSyncMode.On,
				UpdatesPerSecond = 60,
				FramesPerSecond = 60,
				UseSingleThreadedWindow = isSingleThreaded,
				RunningSlowTolerance = 30,
				Size = windowSize,
				VideoMode = new VideoMode(windowSize),
				PreferredDepthBufferBits = preferredDepthBufferBits,
				ShouldSwapAutomatically = false,
				Title = title,
				Position = new Point(mainMonitor.Bounds.X + 50, mainMonitor.Bounds.Y + 50)
			};

			Window = Silk.NET.Windowing.Window.Create(windowOpts);
		}

		/// <summary>
		/// Shows the window and starts the event loop.
		/// </summary>
		public void Run()
		{
			Console.WriteLine("Running project...");

#if !DEBUG
            try
            {
#endif
			Window.Load += Window_Load;
			Window.Update += OnUpdate;
			Window.Render += Window_Render;
			Window.Resize += OnResized;
			Window.Closing += Window_Closing;

			Window.Run();
#if !DEBUG
            }
            catch (Exception e)
            {
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("----------------------------------------------------");
                Console.WriteLine("The application close due to an unhandled exception:");
                Console.ResetColor();
                Console.WriteLine(e.Message);
                Console.WriteLine("-------- Stack Trace: --------");
                Console.WriteLine(e.StackTrace);
                Console.WriteLine("------------------------------");

                if (e.InnerException == null)
                    Console.WriteLine("The exception doesn't have an inner exception.");
                else
                {
                    Console.WriteLine("Inner exception message:");
                    Console.WriteLine(e.InnerException.Message);
                }

                if (!string.IsNullOrWhiteSpace(e.HelpLink))
                {
                    Console.WriteLine("The exception has an associated help link:");
                    Console.WriteLine(e.HelpLink);
                }

                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
            }
#endif
		}

		private void Window_Load()
		{
			Console.WriteLine("Loading window...");
			InputContext = Window.CreateInput();
			InputContext.ConnectionChanged += OnInputContextConnectionChanged;
			foreach (IKeyboard keyboard in InputContext.Keyboards)
				OnInputContextConnectionChanged(keyboard, keyboard.IsConnected);
			foreach (IMouse mouse in InputContext.Mice)
				OnInputContextConnectionChanged(mouse, mouse.IsConnected);
			foreach (IGamepad gamepad in InputContext.Gamepads)
				OnInputContextConnectionChanged(gamepad, gamepad.IsConnected);

			graphicsDevice = new GraphicsDevice(GL.GetApi(Window))
			{
				DebugMessagingEnabled = true
			};
			graphicsDevice.DebugMessageReceived += OnDebugMessage;
			graphicsDevice.ShaderCompiled += GraphicsDevice_ShaderCompiled;

			Console.WriteLine(string.Concat("GL Version: ", graphicsDevice.GLMajorVersion, ".", graphicsDevice.GLMinorVersion));
			Console.WriteLine("GL Version String: " + graphicsDevice.GLVersion);
			Console.WriteLine("GL Vendor: " + graphicsDevice.GLVendor);
			Console.WriteLine("GL Renderer: " + graphicsDevice.GLRenderer);
			Console.WriteLine("GL ShadingLanguageVersion: " + graphicsDevice.GLShadingLanguageVersion);
			Console.WriteLine("GL TextureUnits: " + graphicsDevice.MaxTextureImageUnits);
			Console.WriteLine("GL MaxTextureSize: " + graphicsDevice.MaxTextureSize);
			Console.WriteLine("GL MaxSamples: " + graphicsDevice.MaxSamples);

			OnLoad();
			OnResized(Window.Size);
		}

		private void OnInputContextConnectionChanged(IInputDevice device, bool status)
		{
			if (device is IKeyboard keyboard)
			{
				if (keyboard.IsConnected)
				{
					keyboard.KeyDown += Keyboard_KeyDown;
					keyboard.KeyUp += OnKeyUp;
					keyboard.KeyChar += OnKeyChar;
				}
				else
				{
					keyboard.KeyDown -= Keyboard_KeyDown;
					keyboard.KeyUp -= OnKeyUp;
					keyboard.KeyChar -= OnKeyChar;
				}
			}
			else if (device is IMouse mouse)
			{
				if (mouse.IsConnected)
				{
					mouse.MouseDown += OnMouseDown;
					mouse.MouseMove += OnMouseMove;
					mouse.MouseUp += OnMouseUp;
					mouse.Scroll += OnMouseScroll;
				}
				else
				{
					mouse.MouseDown -= OnMouseDown;
					mouse.MouseMove -= OnMouseMove;
					mouse.MouseUp -= OnMouseUp;
					mouse.Scroll -= OnMouseScroll;
				}
			}
			else if (device is IGamepad gamepad)
			{
				if (gamepad.IsConnected)
				{
					gamepad.ButtonDown += OnGamepadButtonDown;
					gamepad.ButtonUp += OnGamepadButtonUp;
					gamepad.ThumbstickMoved += OnGamepadThumbstickMoved;
					gamepad.TriggerMoved += OnGamepadTriggerMoved;
				}
				else
				{
					gamepad.ButtonDown -= OnGamepadButtonDown;
					gamepad.ButtonUp -= OnGamepadButtonUp;
					gamepad.ThumbstickMoved -= OnGamepadThumbstickMoved;
					gamepad.TriggerMoved -= OnGamepadTriggerMoved;
				}
			}
		}

		private void Keyboard_KeyDown(IKeyboard sender, Key key, int n)
		{
			if (key == Key.F11 && AllowToggleFullscreen)
				IsFullscreen = !IsFullscreen;

			if (key == Key.Escape)
				Window.Close();

			OnKeyDown(sender, key, n);
		}

		private void Window_Render(double dt)
		{
			if (!Window.IsClosing)
				OnRender(dt);
		}

		private void Window_Closing()
		{
			OnUnload();
			graphicsDevice.Dispose();
		}

		protected abstract void OnLoad();
		protected abstract void OnRender(double dt);
		protected abstract void OnResized(Size size);
		protected abstract void OnUnload();

		protected virtual void OnUpdate(double dt)
		{
			GLEnum c;
			while ((c = graphicsDevice.GL.GetError()) != GLEnum.NoError)
			{
				Console.WriteLine("GL Error found: " + c);
			}
		}

		protected virtual void OnKeyDown(IKeyboard sender, Key key, int n) { }
		protected virtual void OnKeyUp(IKeyboard sender, Key key, int n) { }
		protected virtual void OnKeyChar(IKeyboard sender, char key) { }

		protected virtual void OnMouseDown(IMouse sender, MouseButton button) { }
		protected virtual void OnMouseMove(IMouse sender, PointF position) { }
		protected virtual void OnMouseUp(IMouse sender, MouseButton button) { }
		protected virtual void OnMouseScroll(IMouse sender, ScrollWheel scroll) { }

		protected virtual void OnGamepadButtonDown(IGamepad sender, Button button) { }
		protected virtual void OnGamepadButtonUp(IGamepad sender, Button button) { }
		protected virtual void OnGamepadThumbstickMoved(IGamepad sender, Thumbstick thumbstick) { }
		protected virtual void OnGamepadTriggerMoved(IGamepad sender, Trigger trigger) { }

		private void GraphicsDevice_ShaderCompiled(GraphicsDevice sender, in ShaderProgramBuilder programBuilder, bool success)
		{
			bool hasVsLog = !string.IsNullOrEmpty(programBuilder.VertexShaderLog);
			bool hasGsLog = !string.IsNullOrEmpty(programBuilder.GeometryShaderLog);
			bool hasFsLog = !string.IsNullOrEmpty(programBuilder.FragmentShaderLog);
			bool hasProgramLog = !string.IsNullOrEmpty(programBuilder.ProgramLog);
			bool printLogs = false;

			if (success)
			{
				if (hasVsLog || hasGsLog || hasFsLog || hasProgramLog)
				{
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.WriteLine("Shader compiled with possible warnings:");
					printLogs = true;
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("Shader compiled succesfully.");
				}
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Shader compilation error:");
				printLogs = true;
			}

			if (printLogs)
			{
				if (hasVsLog)
				{
					Console.WriteLine("VertexShader log:");
					Console.WriteLine(programBuilder.VertexShaderLog);
				}

				if (hasGsLog)
				{
					Console.WriteLine("GeometryShader log:");
					Console.WriteLine(programBuilder.GeometryShaderLog);
				}

				if (hasFsLog)
				{
					Console.WriteLine("FragmentShader log:");
					Console.WriteLine(programBuilder.FragmentShaderLog);
				}

				if (hasProgramLog)
				{
					Console.WriteLine("Program log:");
					Console.WriteLine(programBuilder.ProgramLog);
				}
			}

			Console.ResetColor();
		}

		/// <summary>
		/// Calculates the size to use for a new window as two thirds the size of the main monitor.
		/// </summary>
		/// <param name="monitor">The monitor in which the window will be located.</param>
		private static Size GetNewWindowSize(IMonitor monitor)
		{
			if (monitor.VideoMode.Resolution.HasValue)
			{
				Size s = monitor.VideoMode.Resolution.Value;
				return new Size(s.Width * 2 / 3, s.Height * 2 / 3);
			}
			return new Size(monitor.Bounds.Width * 2 / 3, monitor.Bounds.Height * 2 / 3);
		}

		private static void OnDebugMessage(DebugSource debugSource, DebugType debugType, int messageId, DebugSeverity debugSeverity, string message)
		{
			if (messageId != 131185 && messageId != 131186)
				Console.WriteLine(string.Concat("Debug message: source=", debugSource.ToString(), " type=", debugType.ToString(), " id=", messageId.ToString(), " severity=", debugSeverity.ToString(), " message=\"", message, "\""));
		}
	}
}