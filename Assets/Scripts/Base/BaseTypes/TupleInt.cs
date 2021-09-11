namespace Base.BaseTypes
{
    public class TupleInt : TupleItems<int, int>
    {
        public TupleInt()
        {
        }

        public TupleInt(int first, int second) : base(first, second)
        {
        }

        public TupleInt(TupleInt items) : base(items)
        {
        }
    }
}
