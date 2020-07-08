using System;
using System.Collections.Generic;
using System.Text;

namespace Barotrauma_Circuit_Resolver.Util
{
    public class Pair<T1, T2>
    {
        public Pair()
        {

        }

        public Pair(T1 before, T2 after)
        {
            this.Before = before;
            this.After = after;
        }

        private T1 before;
        private T2 after;
        public T1 Before { get => before; set => before = value; }
        public T2 After { get => after; set => after = value; }
    }
}
