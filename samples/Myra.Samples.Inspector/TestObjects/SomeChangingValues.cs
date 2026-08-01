using System;
using System.ComponentModel;
using System.Numerics;
using Microsoft.Xna.Framework;
using Myra.Utility.Types;

namespace Myra.Samples.Inspector
{
    public class SomeChangingValues
    {
        public int tick;
        public double Time { get; private set; }
        public string EvenOrOdd { get; private set; }
        
        private bool _dirty;
        
        private TypeCode _type = TypeCode.Int32;
        public TypeCode Type
        {
            get => _type;
            set
            {
                if (value != _type)
                {
                    _type = value;
                    _dirty = true;
                }
            }
        }
        //[ReadOnly(true)]
        //public ulong TypeMaxValues { get; private set; }

        public void Update(GameTime time)
        {
            tick++;
            Time = time.ElapsedGameTime.TotalSeconds;
            if (_dirty)
            {
                _dirty = false;
                EvenOrOdd = time.TotalGameTime.Seconds % 2 == 0 ? "Even" : "Odd";
                //TypeMaxValues = FindMaxBitValue();
            }
        }
    }
}