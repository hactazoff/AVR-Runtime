using System.Collections.Generic;

namespace AVR
{
    namespace Base
    {
        public class Manager<T>
        {
            public static List<T> Cache = new();
            public static T[] ToArray() => Cache.ToArray();
        }
    }
}