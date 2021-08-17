using System;

namespace Base.Deque
{
    public class Conveyor<T> : Deque<T>
    {
        private readonly int _capacity;
        private Action<T> _onExciteCapacity;

        public Conveyor(int capacity)
        {
            _capacity = capacity;
            _onExciteCapacity = null;
        }
        
        public Conveyor(int capacity, Action<T> onExciteCapacity)
        {
            _capacity = capacity;
            _onExciteCapacity = onExciteCapacity;
        }

        public int Capacity => _capacity;

        public bool IsFull => _capacity == _count;

        public override void AddFirst(T data)
        {
            if (_count >= _capacity)
            {
                _onExciteCapacity?.Invoke(RemoveLast());
            }
            base.AddFirst(data);
        }

        public override void AddLast(T data)
        {
            if (_count >= _capacity)
            {
                _onExciteCapacity?.Invoke(RemoveFirst());
            }
            base.AddLast(data);
        }
    }
}