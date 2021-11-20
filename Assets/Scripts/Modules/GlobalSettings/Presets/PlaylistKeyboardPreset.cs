using Modules.InputManagement.Model;
using UnityEngine;

namespace Modules.GlobalSettings.Presets
{
    [CreateAssetMenu(menuName = "Presets/Playlist Keymap", fileName = "PlaylistKeymap", order = 0)]
    public class PlaylistKeyboardPreset : ScriptableObject
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
