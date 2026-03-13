using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Myra.Samples.Inspector
{
    public enum MyEnum
    {
        One = 1,
        Five = 5,
        Twenty = 20
    }
    
    public class SomeTypesInAClass
    {
        public MyEnum enumValue = MyEnum.Five;

        public bool checkbox = true;
        public bool? nullableBool = null;

        public string stringValue = "Hotdog";

        public Color colorValue = Color.Honeydew;

        public List<string> stringCollection = new List<string>();

        public byte @byte = 250;
    }
}