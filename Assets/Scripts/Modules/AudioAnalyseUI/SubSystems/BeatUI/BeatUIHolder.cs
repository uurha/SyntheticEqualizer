using System;
using Base;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.Extensions;
using Modules.AudioAnalyse.Model;
using Modules.AudioAnalyse.SubSystems.BeatDetector;
using Modules.AudioPlayer.Model;
using UnityEngine;

namespace Modules.AudioAnalyseUI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class BeatUIHolder : MonoBehaviour, IEventSubscriber
    {
        [SerializeField] private BeatUI lowBeat;
        [SerializeField] private BeatUI bassBeat;
    
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.ChangeGroupState(false);
            ResetData();
        }

        private void BeatDataReceived(BeatAnalyzeData beatAnalyzeData)
        {
            const int bass = (int)BeatDetector.BeatType.Bass;
            bassBeat.UpdateData(beatAnalyzeData.FreqSpectrum[bass], beatAnalyzeData.AvgSpectrum[bass], beatAnalyzeData.IsBass);
        
            const int low = (int)BeatDetector.BeatType.Low;
            lowBeat.UpdateData(beatAnalyzeData.FreqSpectrum[low], beatAnalyzeData.AvgSpectrum[low], beatAnalyzeData.IsLow);
        }

        private void OnPlayerStateChanged(AudioPlayerState state)
        {
            if (state.HasFlag(AudioPlayerState.Play) &&
                !state.HasFlag(AudioPlayerState.Pause))
            {
                _canvasGroup.ChangeGroupState(true);
            }
            else
            {
                _canvasGroup.ChangeGroupState(false);
                ResetData();
            }
        }

        private void ResetData()
        {
            lowBeat.ResetData();
            bassBeat.ResetData();
        }

        public Delegate[] GetSubscribers()
        {
            return new Delegate[]
                   {
                       (BeatDetectionEvents.BeatDetectorEvent) BeatDataReceived,
                       (AudioPlayerEvents.AudioPlayerStateEvent) OnPlayerStateChanged
                   };
        }
    }
}
