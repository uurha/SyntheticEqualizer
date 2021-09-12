using System;
using CorePlugin.Attributes.Headers;
using Modules.AudioPlayer.Model;
using Modules.InputManagement;
using UnityEngine;

namespace Modules.AudioPlayer.SubSystems.Playlist
{
    [RequireComponent(typeof(PlaylistComponent))]
    public class PlaylistKeyboardSigner : MonoBehaviour
    {
        [ReferencesHeader]
        [SerializeField] private PlaylistComponent playlistComponent;

        [SettingsHeader]
        [SerializeField] private PlaylistKeyboardSettings settings;

        private void Awake()
        {
            playlistComponent ??= GetComponent<PlaylistComponent>();
        }

        private void Start()
        {
            InputHandler.AddKeyEvent(settings.NextClipKey, playlistComponent.PlayNext);
            InputHandler.AddKeyEvent(settings.PreviousClipKey, playlistComponent.PlayPrevious);
            InputHandler.AddKeyEvent(settings.PlayClipKey, playlistComponent.Play);
            InputHandler.AddKeyEvent(settings.StopClipKey, playlistComponent.Stop);
            InputHandler.AddKeyEvent(settings.PauseClipKey, playlistComponent.SwitchPause);
            InputHandler.AddKeyEvent(settings.MuteClipKey, playlistComponent.SwitchMute);
        }

        private void Reset()
        {
            playlistComponent ??= GetComponent<PlaylistComponent>();
        }
    }
}
