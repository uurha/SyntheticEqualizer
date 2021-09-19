﻿using System;
using System.IO;
using System.Linq;
using Base;
using CorePlugin.Attributes.Headers;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.Extensions;
using Extensions;
using Modules.AudioLoader.Model;
using Modules.AudioPlayer.SubSystems.Playlist;
using UnityEngine;

namespace Modules.AudioLoader.SubSystems.Loader
{
    [RequireComponent(typeof(PlaylistComponent))]
    public class DefaultAudioLoader : MonoBehaviour, IEventHandler
    {
        [ReferencesHeader]
        [SerializeField] private PlaylistComponent playlistComponent;

        [SettingsHeader]
        [SerializeField] private bool isStreaming = true;

        [SerializeField] private AudioType type = AudioType.OGGVORBIS;

        private string defaultPath = Application.streamingAssetsPath;

        private event CrossEventsType.OnAudioLoadRequested RequestAudioLoad;

        private void Awake()
        {
            playlistComponent ??= GetComponent<PlaylistComponent>();
        }

        private void Start()
        {
            if (!Directory.Exists(defaultPath)) Directory.CreateDirectory(defaultPath);
            var files = Directory.GetFiles(defaultPath, $"*{type.GetExtension()}", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var loadSettings = new AudioLoaderSettings(file, type, isStreaming);
                RequestAudioLoad?.Invoke(loadSettings, Loading);
            }
        }

        private void Loading(AudioDataProgress dataProgress)
        {
            if (dataProgress.State != AudioDataProgress.StateProgress.Done) return;
            if (dataProgress.Clip != null) playlistComponent.AddClip(dataProgress.Clip);
        }

        private void Reset()
        {
            playlistComponent ??= GetComponent<PlaylistComponent>();
        }

        public void InvokeEvents()
        {
        }

        public void Subscribe(Delegate[] subscribers)
        {
            RequestAudioLoad += subscribers.Combine<CrossEventsType.OnAudioLoadRequested>();
        }

        public void Unsubscribe(Delegate[] unsubscribers)
        {
            RequestAudioLoad -= unsubscribers.Combine<CrossEventsType.OnAudioLoadRequested>();
        }
    }
}
