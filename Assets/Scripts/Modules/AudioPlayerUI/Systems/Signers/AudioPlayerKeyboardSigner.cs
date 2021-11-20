using CorePlugin.Attributes.Headers;
using Modules.AudioPlayer.Systems;
using Modules.GlobalSettings.Presets;
using Modules.InputManagement;
using UnityEngine;

namespace Modules.AudioPlayerUI.Systems.Signers
{
    public class AudioPlayerKeyboardSigner : MonoBehaviour
    {
        [SettingsHeader]
        [SerializeField] private PlaylistKeyboardPreset settings;

        private void Start()
        {
            InputHandler.AddKeyEvent(settings.NextClipKey, AudioPlayerStateCaller.PlayNext);
            InputHandler.AddKeyEvent(settings.PreviousClipKey, AudioPlayerStateCaller.PlayPrevious);
            InputHandler.AddKeyEvent(settings.PlayClipKey, AudioPlayerStateCaller.SwitchPlay);
            InputHandler.AddKeyEvent(settings.StopClipKey, AudioPlayerStateCaller.Stop);
            InputHandler.AddKeyEvent(settings.PauseClipKey, AudioPlayerStateCaller.SwitchPause);
            InputHandler.AddKeyEvent(settings.MuteClipKey, AudioPlayerStateCaller.SwitchMute);
        }
    }
}
