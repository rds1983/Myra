namespace Myra.Samples.FormattedTextSample
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            using (var game = new FormattedTextGame())
                game.Run();
        }
    }
}