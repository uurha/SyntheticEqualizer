using CorePlugin.Attributes.Headers;
using CorePlugin.ReferenceDistribution;
using Modules.AudioPlayer.Model;
using Modules.AudioPlayer.SubSystems.ExternActions;
using Modules.InputManagement;
using UnityEngine;

namespace Modules.AudioPlayerUserInput
{
    public class AudioPlayerKeyboardSigner : MonoBehaviour
    {
        [SettingsHeader]
        [SerializeField] private PlaylistKeyboardSettings settings;

        private AudioPlayerExternAction _externAction;

        private void Start()
        {
            _externAction = ReferenceDistributor.GetReference<AudioPlayerExternAction>();
            InputHandler.AddKeyEvent(settings.NextClipKey, _externAction.PlayNext);
            InputHandler.AddKeyEvent(settings.PreviousClipKey, _externAction.PlayPrevious);
            InputHandler.AddKeyEvent(settings.PlayClipKey, _externAction.SwitchPlay);
            InputHandler.AddKeyEvent(settings.StopClipKey, _externAction.Stop);
            InputHandler.AddKeyEvent(settings.PauseClipKey, _externAction.SwitchPause);
            InputHandler.AddKeyEvent(settings.MuteClipKey, _externAction.SwitchMute);
        }
    }
}
