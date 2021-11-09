namespace Base.BaseTypes
{
    public static class BaseTypesExtensions
    {
        public static void Deconstruct<T1, T2>(this TupleItems<T1, T2> value, out T1 item1, out T2 item2)
        {
            item1 = value.Item1;
            item2 = value.Item2;
        }
    }
}
