using CorePlugin.Attributes.Headers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.AudioAnalyseUI.Systems.BeatUI
{
    public class BeatUI : MonoBehaviour
    {
        [ReferencesHeader]
        [SerializeField] private TMP_Text currentSpectrum;

        [SerializeField] private TMP_Text averageSpectrum;
        [SerializeField] private Image visualRepresentation;

        [SettingsHeader]
        [SerializeField] private Color defaultColor = Color.white;

        [SerializeField] private Color beatColor = Color.red;
        [SerializeField] private float colorDecayTime = 1f;

        private float _colorTime;

        private void AnalyzeBeat(bool beat)
        {
            if (beat)
            {
                visualRepresentation.color = beatColor;
                _colorTime = 0;
            }
            else
            {
                if (_colorTime < colorDecayTime)
                {
                    _colorTime += Time.deltaTime;
                    var t = Mathf.InverseLerp(0f, colorDecayTime, _colorTime);
                    visualRepresentation.color = Color.Lerp(visualRepresentation.color, defaultColor, t);
                }
                else
                {
                    visualRepresentation.color = defaultColor;
                }
            }
        }

        private static string FormatText(float value)
        {
            return $"{value:F4}";
        }

        public void ResetData()
        {
            currentSpectrum.text = FormatText(0f);
            currentSpectrum.text = FormatText(0f);
            visualRepresentation.fillAmount = 0f;
            visualRepresentation.color = defaultColor;
            _colorTime = 0;
        }

        public void UpdateData(float current, float average, bool beat)
        {
            currentSpectrum.text = FormatText(current);
            averageSpectrum.text = FormatText(average);
            visualRepresentation.fillAmount = Mathf.Lerp(0f, 1f, current);
            AnalyzeBeat(beat);
        }
    }
}
