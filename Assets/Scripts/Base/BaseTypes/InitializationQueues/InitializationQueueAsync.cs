using System;
using System.Collections.Generic;
using Base.BaseTypes.InitializationQueues.Interfaces;

namespace Base.BaseTypes.InitializationQueues
{
    public class InitializationQueueAsync<TInitializable, TParams> where TInitializable : IInitializable where TParams : IInitializeParams
    {
        private readonly Queue<Tuple<TInitializable, TParams>> _queue = new Queue<Tuple<TInitializable, TParams>>();
        private bool _isInProgress;

        public async void Initialize(TInitializable item, TParams initializeParams = default)
        {
            if (_isInProgress)
            {
                _queue.Enqueue(new Tuple<TInitializable,TParams>(item, initializeParams));
                return;
            }
            _isInProgress = true;
            
            if(initializeParams.UsageReady)
                await item.Initialize();
            else
                await item.Initialize(initializeParams);

            if (_queue.Count > 0)
            {
                var (initializable, parameters) = _queue.Dequeue();
                Initialize(initializable, parameters);
            }
            _isInProgress = false;
        }
    }
}
