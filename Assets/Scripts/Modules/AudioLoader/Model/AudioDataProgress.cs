using System;
using UnityEngine;

namespace Modules.AudioLoader.Model
{
    [Serializable]
    public class AudioDataProgress
    {
        public enum StateProgress
        {
            Start,
            InProgress,
            Done
        }

        public StateProgress State { get; set; } = StateProgress.Start;

        public float Progress { get; set; } = 0f;

        public AudioClip Clip { get; set; } = null;
    }
}
