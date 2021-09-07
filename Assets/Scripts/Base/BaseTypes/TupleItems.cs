namespace Base.BaseTypes
{
    public class TupleItems<T, TV>
    {
        public TupleItems()
        {
            Item1 = default;
            Item2 = default;
        }

        public TupleItems(T first, TV second)
        {
            Item1 = first;
            Item2 = second;
        }

        public TupleItems(TupleItems<T, TV> items)
        {
            Item1 = items.Item1;
            Item2 = items.Item2;
        }

        public T Item1 { get; set; }
        public TV Item2 { get; set; }
    }
}
