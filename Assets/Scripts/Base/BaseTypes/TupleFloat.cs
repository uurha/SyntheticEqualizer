using System;
using System.Collections.Generic;
using UnityEngine;

namespace Base.BaseTypes
{
    [Serializable]
    public class TupleFloat : TupleItems<float, float>
    {
        public class TupleValueComparer : EqualityComparer<TupleFloat>
        {
            public override bool Equals(TupleFloat x, TupleFloat y)
            {
                return y != null && x != null && Math.Abs(x.Item1 - y.Item1) < Mathf.Epsilon && Math.Abs(x.Item2 - y.Item2) < Mathf.Epsilon;
            }

            public override int GetHashCode(TupleFloat obj)
            {
                return obj.GetHashCode();
            }
        }

        public TupleFloat()
        {
        }

        public TupleFloat(float first, float second) : base(first, second)
        {
        }

        public TupleFloat(TupleFloat items) : base(items)
        {
        }
    }
}
