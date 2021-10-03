using System;
using Base;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.Extensions;
using Modules.AudioAnalyse.Model;
using Modules.AudioAnalyse.Systems.BeatDetector;
using Modules.AudioPlayerModule.Model;
using UnityEngine;

namespace Modules.AudioAnalyseUI.Systems.BeatUI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class BeatUIHolder : MonoBehaviour, IEventSubscriber
    {
        [SerializeField] private BeatUI lowBeat;
        [SerializeField] private BeatUI bassBeat;

        private CanvasGroup _canvasGroup;
        private const int Bass = (int) BeatAnalyzer.BeatType.Bass;
        private const int Low = (int) BeatAnalyzer.BeatType.Low;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.ChangeGroupState(false);
            ResetData();
        }

        private void BeatDataReceived(BeatDetectorOutput beatAnalyzeData)
        {
            bassBeat.UpdateData(beatAnalyzeData.FreqSpectrum[Bass], beatAnalyzeData.AvgSpectrum[Bass],
                                beatAnalyzeData.IsBass);

            lowBeat.UpdateData(beatAnalyzeData.FreqSpectrum[Low], beatAnalyzeData.AvgSpectrum[Low],
                               beatAnalyzeData.IsLow);
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
