using System;
using System.Collections;
using System.Threading.Tasks;
using System.Timers;
using CorePlugin.Dispatchers;
using UnityEngine;
using TaskExtensions = CorePlugin.Extensions.TaskExtensions;

namespace Base
{
    /// <summary>
    /// FPS timer for checking if frame skips are necessary
    /// </summary>
    public class FPSTimer : IDisposable
    {
        private const float FPSMeasurePeriod = 500f;
        private int _mFpsAccumulator = 0;
        private int _currentFPS;
        private int _targetFPS;
        private Timer _timer;
        private Timer _frameCounter;

        public static FPSTimer Create(int targetFPS)
        {
            return new FPSTimer().StartTimer(targetFPS);
        }

        public static FPSTimer Create()
        {
            return new FPSTimer().StartTimer(60);
        }

        /// <summary>
        /// Starts the fps timer, for target fps investigation
        /// </summary>
        /// <param name="targetFPS"></param>
        public FPSTimer StartTimer(int targetFPS)
        {
            MainThreadDispatcher.OnDestroyEvent += StopTimer;
            _targetFPS = targetFPS;
            _timer = new Timer(FPSMeasurePeriod);
            _timer.AutoReset = true;
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();
            _frameCounter = new Timer(1);
            _frameCounter.AutoReset = true;
            _frameCounter.Elapsed += FrameCounterOnElapsed;
            _frameCounter.Start();
            return this;
        }

        private async void FrameCounterOnElapsed(object sender, ElapsedEventArgs e)
        {
            _mFpsAccumulator++;
            var unscaledDeltaTime = 0f;
            await MainThreadDispatcher.EnqueueAsync(() => unscaledDeltaTime = Time.unscaledDeltaTime);
            if (_frameCounter != null) _frameCounter.Interval = unscaledDeltaTime;
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            _currentFPS = (int)(_mFpsAccumulator / (FPSMeasurePeriod / 100));
            _mFpsAccumulator = 0;
        }

        /// <summary>
        /// Starts the fps timer, for target fps investigation
        /// </summary>
        public void StopTimer()
        {
            Dispose();
        }

        /// <summary>
        /// Check if fps time is elapsed, for skip frames if necessary 
        /// </summary>
        public bool HasTimeForMaxFPSElapsed()
        {
            return _currentFPS >= _targetFPS;
        }

        public IEnumerator AwaitForTargetFPS()
        {
            yield return new WaitUntil(HasTimeForMaxFPSElapsed);
        }

        public async Task AwaitTargetFPS()
        {
            if (true/*HasTimeForMaxFPSElapsed()*/)
            {
                await Task.CompletedTask;
            }
            // else
            // {
            //     await TaskExtensions.WaitUntil(HasTimeForMaxFPSElapsed);
            // }
        }

        public void Dispose()
        {
            _frameCounter?.Stop();
            _frameCounter?.Dispose();
            _frameCounter = null;
            _timer?.Stop();
            _timer?.Dispose();
            _timer = null;
        }
    }
}
