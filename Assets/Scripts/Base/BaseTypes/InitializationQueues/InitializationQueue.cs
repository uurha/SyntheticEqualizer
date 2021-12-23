using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Base.BaseTypes.InitializationQueues
{
    public class InitializationQueue<T>
    {
        private readonly Func<T, Task> _initializeAction;
        private readonly Queue<T> _queue = new Queue<T>();
        private bool _isInProgress;

        public InitializationQueue(Func<T, Task> initializeAction)
        {
            _initializeAction = initializeAction;
        }

        public async void Initialize(T item)
        {
            if (_isInProgress)
            {
                _queue.Enqueue(item);
                return;
            }
            _isInProgress = true;
            await _initializeAction.Invoke(item);

            if (_queue.Count > 0)
            {
                var nextItem = _queue.Dequeue();
                Initialize(nextItem);
            }
            _isInProgress = false;
        }
    }
}
