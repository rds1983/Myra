using info.lundin.math;
using System;

namespace Myra.Samples.Layout2D
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
           using (var game = new MyraSamplesLayout2D())
                game.Run();
        }
    }
}
