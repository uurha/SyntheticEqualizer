using System;
using Base;
using CorePlugin.Cross.Events.Interface;
using Modules.AudioAnalyse.Model;
using UnityEngine;

namespace Modules.AudioAnalyse.SubSystems.DataProcessors
{
    public class BeatDataProcessor : MonoBehaviour, IEventSubscriber
    {
        [SerializeField] private int maxBeatsBeforeCheck = 10;
        private int _beatCount;
        private float _timeSinceLast;

        private void OnAudioClipChanged()
        {
            _timeSinceLast = Time.time;
            _beatCount = 0;
        }

        private void OnBeatDetected(BeatAnalyzeData beatAnalyzeData)
        {
            if (beatAnalyzeData.IsBass || beatAnalyzeData.IsLow)
            {
                OnBeat();
            }
        }

        private void OnBeat()
        {
            if (_beatCount < maxBeatsBeforeCheck)
            {
                _beatCount++;
                Debug.Log("Beat");
                return;
            }
            var difference = Time.time - _timeSinceLast;
            _beatCount++;
            var bpm = Mathf.RoundToInt(_beatCount * 60f / difference);
            _beatCount = 0;
            _timeSinceLast = Time.time;
            Debug.Log(bpm);
        }

        public Delegate[] GetSubscribers()
        {
            return new Delegate[]
                   {
                       (BeatDetectionEvents.BeatDetectorEvent) OnBeatDetected,
                       (AudioPlayerEvents.AudioClipChangedEvent) OnAudioClipChanged
                   };
        }
    }
}
