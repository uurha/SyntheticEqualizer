using System;
using System.Collections;
using System.Collections.Generic;

namespace Base.Deque
{
    public class Deque<T> : IEnumerable<T>
    {
        private DoublyNode<T> _head;
        private DoublyNode<T> _tail;
        private protected int _count;
        
        public virtual void AddLast(T data)
        {
            var node = new DoublyNode<T>(data);

            if (_head == null)
                _head = node;
            else
            {
                _tail.Next = node;
                node.Previous = _tail;
            }
            _tail = node;
            _count++;
        }

        public virtual void AddFirst(T data)
        {
            var node = new DoublyNode<T>(data);
            var temp = _head;
            node.Next = temp;
            _head = node;

            if (_count == 0)
                _tail = _head;
            else
                temp.Previous = node;
            _count++;
        }

        public T RemoveFirst()
        {
            if (_count == 0) throw new InvalidOperationException();
            var output = _head.Data;

            if (_count == 1)
            {
                _head = _tail = null;
            }
            else
            {
                _head = _head.Next;
                _head.Previous = null;
            }
            _count--;
            return output;
        }

        public T RemoveLast()
        {
            if (_count == 0) throw new InvalidOperationException();
            var output = _tail.Data;

            if (_count == 1)
            {
                _head = _tail = null;
            }
            else
            {
                _tail = _tail.Previous;
                _tail.Next = null;
            }
            _count--;
            return output;
        }

        public T First
        {
            get
            {
                if (IsEmpty) throw new InvalidOperationException();
                return _head.Data;
            }
        }

        public T Last
        {
            get
            {
                if (IsEmpty) throw new InvalidOperationException();
                return _tail.Data;
            }
        }

        public int Count => _count;

        public bool IsEmpty => _count == 0;

        public void Clear()
        {
            _head = null;
            _tail = null;
            _count = 0;
        }

        public bool Contains(T data)
        {
            var current = _head;

            while (current != null)
            {
                if (current.Data.Equals(data)) return true;
                current = current.Next;
            }
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) this).GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            var current = _head;

            while (current != null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }
    }

    public class Conveyor<T> : Deque<T>
    {
        private readonly int _capacity;

        public Conveyor(int capacity)
        {
            _capacity = capacity;
        }

        public int Capacity => _capacity;

        public bool IsFull => _capacity == _count;

        public override void AddFirst(T data)
        {
            if (_count >= _capacity)
            {
                RemoveLast();
            }
            base.AddFirst(data);
        }

        public override void AddLast(T data)
        {
            if (_count >= _capacity)
            {
                RemoveFirst();
            }
            base.AddLast(data);
        }
    }
}
