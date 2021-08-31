using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UIModule
{
    [Serializable]
    public class TextedButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text textField;
        
        public event UnityAction onClick
        {
            add => button.onClick.AddListener(value);
            remove => button.onClick.RemoveListener(value);
        }

        public string text
        {
            get => textField.text;
            set => textField.text = value;
        }
    }
}
