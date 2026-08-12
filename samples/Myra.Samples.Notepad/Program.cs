global using Myra.Events;

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