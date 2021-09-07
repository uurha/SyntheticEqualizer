using System;
using Modules.AudioPlayer.UISystem;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Base.UI
{
    [Serializable]
    public class TextedButton : MonoBehaviour, IButton
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text textField;

        public string Text
        {
            get => textField.text;
            set => textField.text = value;
        }

        public event Action OnClick
        {
            add => button.onClick.AddListener(new UnityAction(value));
            remove => button.onClick.RemoveListener(new UnityAction(value));
        }
    }
}
