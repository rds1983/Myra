using System;
using System.Collections.Generic;

namespace Myra.Samples.SplitPaneContainer
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            using (var game = new SplitPaneGame())
                game.Run();      
        }
    }
}