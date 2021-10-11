using System;
using UnityEngine;

namespace Extensions
{
    public static class AudioExtensions
    {
        //TODO: Rework as fabric method
        public enum BandType
        {
            FourBand = 0,
            FourBandVisual = 1,
            EightBand = 2,
            TenBand = 3,
            TwentySixBand = 4,
            ThirtyOneBand = 5
        }

        public static readonly float[][] MiddleFrequenciesForBands =
        {
            new[] {125.0f, 500, 1000, 2000},
            new[] {250.0f, 400, 600, 800},
            new[]
            {
                63.0f, 125, 500, 1000, 2000, 4000,
                6000, 8000
            },
            new[]
            {
                31.5f, 63, 125, 250, 500, 1000, 2000,
                4000, 8000, 16000
            },
            new[]
            {
                25.0f, 31.5f, 40, 50, 63, 80, 100, 125,
                160, 200, 250, 315, 400, 500, 630, 800,
                1000, 1250, 1600, 2000, 2500, 3150,
                4000, 5000, 6300, 8000
            },
            new[]
            {
                20.0f, 25, 31.5f, 40, 50, 63, 80, 100,
                125, 160, 200, 250, 315, 400, 500, 630,
                800, 1000, 1250, 1600, 2000, 2500,
                3150, 4000, 5000, 6300, 8000, 10000,
                12500, 16000, 20000
            }
        };

        public static readonly float[] BandWidthForBands =
        {
            1.414f, // 2^(1/2)
            1.260f, // 2^(1/3)
            1.414f, // 2^(1/2)
            1.414f, // 2^(1/2)
            1.122f, // 2^(1/6)
            1.122f  // 2^(1/6)
        };

        public static int GetBandLenght(this BandType bandType)
        {
            return MiddleFrequenciesForBands[bandType.BandToIndex()].Length;
        }

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

        public static int BandToIndex(this BandType bandType)
        {
            return (int) bandType;
        }

        public static int FrequencyToSpectrumMinIndex(int lenght, int bandIndex, BandType bandType)
        {
            var bandTypeIndex = bandType.BandToIndex();

            return FrequencyToSpectrumIndex(lenght,
                                            MiddleFrequenciesForBands[bandTypeIndex][bandIndex] /
                                            BandWidthForBands[bandTypeIndex]);
        }

        public static int FrequencyToSpectrumMaxIndex(int lenght, int bandIndex, BandType bandType)
        {
            var bandTypeIndex = bandType.BandToIndex();

            return FrequencyToSpectrumIndex(lenght,
                                            MiddleFrequenciesForBands[bandTypeIndex][bandIndex] *
                                            BandWidthForBands[bandTypeIndex]);
        }

        public static int FrequencyToSpectrumIndex(int lenght, float f)
        {
            var i = Mathf.FloorToInt(f / AudioSettings.outputSampleRate * 2.0f * lenght);
            return Mathf.Clamp(i, 0, lenght - 1);
        }
    }
}
