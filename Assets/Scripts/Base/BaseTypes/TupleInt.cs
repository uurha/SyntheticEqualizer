using System.Collections.Generic;

namespace Base.BaseTypes
{
    public class TupleInt : TupleItems<int, int>
    {
        public class TupleValueComparer : EqualityComparer<TupleInt>
        {
            public override bool Equals(TupleInt x, TupleInt y)
            {
                return y != null && x != null && x.Item1 == y.Item1 && x.Item2 == y.Item2;
            }

            public override int GetHashCode(TupleInt obj)
            {
                return obj.GetHashCode();
            }
        }

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
