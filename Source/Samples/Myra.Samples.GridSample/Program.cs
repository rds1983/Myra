namespace Myra.Samples.GridSample
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            using (var game = new GridGame())
                game.Run();
        }
    }
}