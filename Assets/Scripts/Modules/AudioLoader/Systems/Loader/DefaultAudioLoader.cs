using System;
using System.IO;
using Base;
using CorePlugin.Attributes.EditorAddons;
using CorePlugin.Attributes.Headers;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.Extensions;
using Extensions;
using Modules.AudioLoader.Model;
using Modules.AudioPlayer.Systems.Playlist;
using UnityEngine;

namespace Modules.AudioLoader.Systems.Loader
{
    [RequireComponent(typeof(PlaylistComponent))]
    [CoreManagerElement]
    public class DefaultAudioLoader : MonoBehaviour, IEventHandler
    {
        [ReferencesHeader]
        [SerializeField] private PlaylistComponent playlistComponent;

        [SettingsHeader]
        [SerializeField] private bool isStreaming = true;

        [SerializeField] private AudioType type = AudioType.OGGVORBIS;

        private readonly string _defaultPath = Application.streamingAssetsPath;

        private event AudioPlayerEvents.AudioLoadRequested RequestAudioLoad;

        private void Awake()
        {
            playlistComponent ??= GetComponent<PlaylistComponent>();
        }

        private void Start()
        {
            if (!Directory.Exists(_defaultPath)) Directory.CreateDirectory(_defaultPath);
            var files = Directory.GetFiles(_defaultPath, $"*{type.GetExtension()}", SearchOption.AllDirectories);

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

        public void Subscribe(params Delegate[] subscribers)
        {
            EventExtensions.Subscribe(ref RequestAudioLoad, subscribers);
        }

        public void Unsubscribe(params Delegate[] unsubscribers)
        {
            EventExtensions.Unsubscribe(ref RequestAudioLoad, unsubscribers);
        }
    }
}
