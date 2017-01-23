using System;

namespace LevelSpline
{
    public class EdgeNotExist : Exception
    {
        public EdgeNotExist() { }

        public EdgeNotExist(string message) : base(message) { }

        public EdgeNotExist(string message, Exception inner) : base(message, inner) { }
    }
}
