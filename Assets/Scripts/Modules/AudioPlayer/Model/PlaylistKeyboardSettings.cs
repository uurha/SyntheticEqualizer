using Modules.InputManagement.Model;
using UnityEngine;

namespace Modules.AudioPlayer.Model
{
    [CreateAssetMenu(menuName = "Playlist/Keyboard Settings", fileName = "KeyboardSettings", order = 0)]
    public class PlaylistKeyboardSettings : ScriptableObject
    {
        [SerializeField] private KeyPreset nextClipKey;
        [SerializeField] private KeyPreset previousClipKey;
        [SerializeField] private KeyPreset playClipKey;
        [SerializeField] private KeyPreset stopClipKey;
        [SerializeField] private KeyPreset pauseClipKey;
        [SerializeField] private KeyPreset muteClipKey;

        public KeyPreset NextClipKey => nextClipKey;

        public KeyPreset PreviousClipKey => previousClipKey;

        public KeyPreset PlayClipKey => playClipKey;

        public KeyPreset StopClipKey => stopClipKey;

        public KeyPreset PauseClipKey => pauseClipKey;

        public KeyPreset MuteClipKey => muteClipKey;
    }
}
