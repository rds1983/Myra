using Microsoft.Xna.Framework;

namespace Myra.Samples.Inspector
{
    public class SomeChangingValues
    {
        public int tick;
        public double Time { get; private set; }
        public string EvenOrOdd { get; private set; }
        
        public void Update(GameTime time)
        {
            tick++;
            Time = time.ElapsedGameTime.TotalSeconds;
            EvenOrOdd = time.ElapsedGameTime.Seconds % 2 == 0 ? "Even" : "Odd";
        }
    }
}