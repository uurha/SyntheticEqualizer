using System;
using Unity.Burst;
using Unity.Mathematics;

namespace Cells.Behaviours
{
    [BurstCompile]
    public readonly struct BehaviourData : IEquatable<BehaviourData>
    {
        public readonly float3 _data;

        public BehaviourData(float3 data)
        {
            _data = data;
        }

        public bool Equals(BehaviourData other)
        {
            return _data.Equals(other._data);
        }

        public override bool Equals(object obj)
        {
            return obj is BehaviourData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _data.GetHashCode();
        }
    }
}
