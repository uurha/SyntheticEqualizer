using System;

namespace Base.Deque
{
    public class Conveyor<T> : Deque<T>
    {
        private readonly Action<T> _onExciteCapacity;

        public Conveyor(int capacity)
        {
            Capacity = capacity;
            _onExciteCapacity = null;
        }

        public Conveyor(int capacity, Action<T> onExciteCapacity)
        {
            Capacity = capacity;
            _onExciteCapacity = onExciteCapacity;
        }

        public int Capacity { get; }

        public bool IsFull => Capacity == _count;

        public override void AddFirst(T data)
        {
            if (_count >= Capacity) _onExciteCapacity?.Invoke(RemoveLast());
            base.AddFirst(data);
        }

        public override void AddLast(T data)
        {
            if (_count >= Capacity) _onExciteCapacity?.Invoke(RemoveFirst());
            base.AddLast(data);
        }
    }
}
