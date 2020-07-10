namespace Barotrauma_Circuit_Resolver
{
    public struct Triplet<T1, T2, T3>
    {
        public T1 First;
        public T2 Second;
        public T3 Third;

        public Triplet(T1 first, T2 second, T3 third)
        {
            First = first;
            Second = second;
            Third = third;
        }
    }
}
