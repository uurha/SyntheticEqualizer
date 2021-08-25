// Audio spectrum component
// By Keijiro Takahashi, 2013
// https://github.com/keijiro/unity-audio-spectrum

using UnityEditor;
using UnityEngine;

namespace AudioSystem.SpectrumAnalyzer
{
    [CustomEditor(typeof(AudioSpectrum))]
    public class AudioSpectrumInspector : Editor
    {
        private static readonly string[] SampleOptionStrings = {
                                                                   "256", "512", "1024", "2048", "4096"
                                                               };
        private static readonly int[] SampleOptions = {
                                                          256, 512, 1024, 2048, 4096
                                                      };
        private static readonly string[] BandOptionStrings = {
                                                                 "4 band", "4 band (visual)", "8 band", "10 band (ISO standard)", "26 band", "31 band (FBQ3102)"
                                                             };
        private static readonly int[] BandOptions = {
                                                        (int)AudioSpectrum.BandType.FourBand,
                                                        (int)AudioSpectrum.BandType.FourBandVisual,
                                                        (int)AudioSpectrum.BandType.EightBand,
                                                        (int)AudioSpectrum.BandType.TenBand,
                                                        (int)AudioSpectrum.BandType.TwentySixBand,
                                                        (int)AudioSpectrum.BandType.ThirtyOneBand
                                                    };
    
        private AnimationCurve _curve;

        private void UpdateCurve (AudioSpectrum spectrum)
        {
            // Create a new curve to update the UI.
            _curve = new AnimationCurve ();

            // Add keys for the each band.
            var bands = spectrum.Levels;
            for (var i = 0; i < bands.Length; i++) {
                _curve.AddKey (1.0f / bands.Length * i, bands [i]);
            }
        }
        public override void OnInspectorGUI ()
        {
            var spectrum = target as AudioSpectrum;
        
            if(spectrum == null) return;

            // Update the curve only when it's playing.
            if (EditorApplication.isPlaying) {
                UpdateCurve (spectrum);
            } else if (_curve == null) {
                _curve = new AnimationCurve ();
            }

            // Component properties.
            spectrum.Source = (AudioSource) EditorGUILayout.ObjectField("Audio Source", spectrum.Source, typeof(AudioSource), true);
            spectrum.NumberOfSamples = EditorGUILayout.IntPopup ("Number of samples", spectrum.NumberOfSamples, SampleOptionStrings, SampleOptions);
            spectrum.Type = (AudioSpectrum.BandType)EditorGUILayout.IntPopup ("Band type", (int)spectrum.Type, BandOptionStrings, BandOptions);
            spectrum.FallSpeed = EditorGUILayout.Slider ("Fall speed", spectrum.FallSpeed, 0.01f, 0.5f);
            spectrum.Sensibility = EditorGUILayout.Slider ("Sensibility", spectrum.Sensibility, 1.0f, 20.0f);

            // Shows the spectrum curve.
            EditorGUILayout.CurveField (_curve, Color.white, new Rect (0, 0, 1.0f, 0.1f), GUILayout.Height (64));

            // Update frequently while it's playing.
            if (EditorApplication.isPlaying) {
                EditorUtility.SetDirty (target);
            }
        }
    }
}