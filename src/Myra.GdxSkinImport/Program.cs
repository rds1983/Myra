using System;

namespace GdxSkinImport;

class Program
{
	// See system error codes: http://msdn.microsoft.com/en-us/library/windows/desktop/ms681382.aspx
	private const int ERROR_SUCCESS = 0;
	private const int ERROR_UNHANDLED_EXCEPTION = 574;  // 0x23E

	static void Log(string message)
	{
		Console.WriteLine(message);
	}

	static void ShowUsage()
	{
		Console.WriteLine($"Myra Gdx Import {Utility.Version}");
		Console.WriteLine("Usage: gdx2myra <inputFile.json>");
	}

	static int Process(string[] args)
	{
		if (args == null || args.Length == 0)
		{
			ShowUsage();
			return ERROR_SUCCESS;
		}

		using (var game = new Game1(args[0]))
		{
			game.Run();
		}

		return ERROR_SUCCESS;
	}

	static int Main(string[] args)
	{
		try
		{
			return Process(args);
		}
		catch (Exception ex)
		{
			Log(ex.ToString());
			return ERROR_UNHANDLED_EXCEPTION;
		}
	}
}