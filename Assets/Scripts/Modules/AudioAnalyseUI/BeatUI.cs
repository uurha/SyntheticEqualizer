using CorePlugin.Attributes.Headers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.AudioAnalyseUI
{
    public class BeatUI : MonoBehaviour
    {
        [ReferencesHeader]
        [SerializeField] private TMP_Text currentSpectrum;
        [SerializeField] private TMP_Text averageSpectrum;
        [SerializeField] private Image visualRepresentation;

        [SettingsHeader]
        [SerializeField] private float initialValue = 0f;

        public void UpdateData(float current, float average)
        {
            currentSpectrum.text = FormatText(current);
            averageSpectrum.text = FormatText(average);
            visualRepresentation.fillAmount = Mathf.Lerp(0f, 1f, current);
        }

        private static string FormatText(float value)
        {
            return $"{value:F4}";
        }

        public void ResetData()
        {
            currentSpectrum.text = FormatText(initialValue);
            currentSpectrum.text = FormatText(initialValue);
            visualRepresentation.fillAmount = Mathf.Lerp(0f, 1f, initialValue);
        }
    }
}