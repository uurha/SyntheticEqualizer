﻿using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using Base.BehaviourModel.Interfaces;
using Base.Deque;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.Extensions;
using Modules.AudioAnalyse.Model;
using Modules.AudioPlayer.Model;
using Modules.Grid.Model;
using SubModules.Cell.Model;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Modules.AudioAnalyse.SubSystems.DataProcessors
{
    public class SpectrumDataProcessor : MonoBehaviour, IEventSubscriber, IEventHandler
    {
        private JobHandle _handle;
        
        private event AudioPlayerEvents.RequestAudioPlayerData RequestAudioPlayerData;
        private event DataProcessorsEvents.SpectrumProcessorDataEvent SpectrumListenerDataPrecessed;
        
        private struct MultiplyJob : IJobParallelFor
        {
            private readonly float _multiplier;

            [ReadOnly] [DeallocateOnJobCompletion]
            private NativeArray<float> _input;

            [WriteOnly]
            private NativeArray<float> _output;

            public MultiplyJob(float[] input, float multiplier)
            {
                _multiplier = multiplier;
                _input = new NativeArray<float>(input, Allocator.TempJob);
                _output = new NativeArray<float>(input.Length, Allocator.TempJob);
            }

            public NativeArray<float> Output => _output;

            public void Execute(int index)
            {
                _output[index] = _input[index] * _multiplier;
            }
        }

        private void OnSpectrumDataReceived(SpectrumListenerData listenerData)
        {
            if (RequestAudioPlayerData == null) return;
            if (!_handle.IsCompleted) return;
            var channels = listenerData.Channels;
            var jobs = new MultiplyJob[channels];
            var audioPlayerData = RequestAudioPlayerData.Invoke();
            
            if(!audioPlayerData.IsValid) return;
            
            for (var channel = 0; channel < listenerData.Channels; channel++)
            {
                var volume = audioPlayerData.Volume;
                var multiplier = volume > 0 ? 1 / volume : 0f;
                jobs[channel] = new MultiplyJob(listenerData.RawSpectrumData[channel], multiplier);
                var handle = jobs[channel].Schedule(listenerData.NumberOfSamples, 1);
                JobHandle.CombineDependencies(_handle, handle);
                handle.Complete();
            }
            
            _handle.Complete();
            var spectrumListenerData = new List<float[]>();

            for (var channel = 0; channel < channels; channel++)
            {
                spectrumListenerData.Add(jobs[channel].Output.ToArray());
                jobs[channel].Output.Dispose();
            }
            
            SpectrumListenerDataPrecessed?.Invoke(new SpectrumProcessorData(listenerData, spectrumListenerData));
        }

        public Delegate[] GetSubscribers()
        {
            return new Delegate[]
                   {
                       (DataProcessorsEvents.SpectrumListenerDataEvent)OnSpectrumDataReceived
                   };
        }

        public void InvokeEvents()
        {
        }

        public void Subscribe(params Delegate[] subscribers)
        {
            EventExtensions.Subscribe(ref RequestAudioPlayerData, subscribers);
            EventExtensions.Subscribe(ref SpectrumListenerDataPrecessed, subscribers);
        }

        public void Unsubscribe(params Delegate[] unsubscribers)
        {
            EventExtensions.Unsubscribe(ref RequestAudioPlayerData, unsubscribers);
            EventExtensions.Unsubscribe(ref SpectrumListenerDataPrecessed, unsubscribers);
        }
    }
}