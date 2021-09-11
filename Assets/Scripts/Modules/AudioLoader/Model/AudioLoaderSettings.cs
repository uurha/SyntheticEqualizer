using System;
using UnityEngine;

namespace Modules.AudioLoader.Model
{
    [Serializable]
    public class AudioLoaderSettings
    {
        public bool IsStreaming { get; }

        public AudioType Type { get; }

        public string Path { get; }

        public AudioLoaderSettings(string path, AudioType type, bool streaming)
        {
            Path = path;
            Type = type;
            IsStreaming = streaming;
        }
    }
}
