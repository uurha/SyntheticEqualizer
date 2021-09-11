using System;
using UnityEngine;

namespace Extensions
{
    public static class AudioClipExtensions
    {
        public static string GetExtension(this AudioType type)
        {
            var extension = type switch
                            {
                                AudioType.MPEG => ".mp3",
                                AudioType.OGGVORBIS => ".ogg",
                                AudioType.WAV => ".wav",
                                _ => throw new
                                         NotSupportedException($"Supplied {nameof(AudioType)}: {type} not supported")
                            };
            return extension;
        }
    }
}
