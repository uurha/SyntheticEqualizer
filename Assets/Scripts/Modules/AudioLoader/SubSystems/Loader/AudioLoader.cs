using System;
using System.Collections;
using System.Collections.Generic;
using Base;
using CorePlugin.Cross.Events.Interface;
using Modules.AudioLoader.Model;
using UnityEngine;
using UnityEngine.Networking;

namespace Modules.AudioLoader.SubSystems.Loader
{
    public class AudioLoader : MonoBehaviour, IEventSubscriber
    {
        private IEnumerator FetchAudioClip(AudioLoaderSettings loaderSettings, Action<AudioDataProgress> dataProgress)
        {
            var handler = new DownloadHandlerAudioClip(loaderSettings.Path, loaderSettings.Type)
                          {
                              streamAudio = loaderSettings.IsStreaming,
                              compressed = !loaderSettings.IsStreaming
                          };

            var request = new UnityWebRequest(loaderSettings.Path)
                          {
                              downloadHandler = handler,
                              timeout = 30,
                              useHttpContinue = true
                          };
            var op = request.SendWebRequest();
            var data = new AudioDataProgress();
            dataProgress?.Invoke(data);
            data.State = AudioDataProgress.StateProgress.InProgress;

            while (!op.isDone)
            {
                data.Progress = op.progress;
                dataProgress?.Invoke(data);
                yield return new WaitForEndOfFrame();
            }
            data.Progress = op.progress;
            data.State = AudioDataProgress.StateProgress.Done;
            data.Clip = handler.audioClip;
            dataProgress?.Invoke(data);
            request.Dispose();
        }

        public IEnumerable<Delegate> GetSubscribers()
        {
            return new[] {(CrossEventsType.OnAudioLoadRequested) LoadRequested};
        }

        private void LoadRequested(AudioLoaderSettings loaderSettings, Action<AudioDataProgress> action)
        {
            StartCoroutine(FetchAudioClip(loaderSettings, action));
        }
    }
}
