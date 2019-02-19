using System;
using System.Collections.Generic;

namespace Myra.Samples.Notepad
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            using (var game = new NotepadGame())
                game.Run();            
        }
    }
}