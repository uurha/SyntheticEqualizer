using System;
using AudioPlayerModule.UISystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Base.UI
{
    [RequireComponent(typeof(Slider))]
    public class SliderHelper : MonoBehaviour, ISlider<float>
    {
        [SerializeField] private Slider slider;

        private void Awake()
        {
            slider ??= GetComponent<Slider>();
        }

        public event Action<float> OnValueChanged
        {
            add => slider.onValueChanged.AddListener(new UnityAction<float>(value));
            remove => slider.onValueChanged.RemoveListener(new UnityAction<float>(value));
        }

        public float Value
        {
            get => slider.value;
            set => slider.value = value;
        }

        public void SetValueWithoutNotify(float value)
        {
            slider.SetValueWithoutNotify(value);
        }
    
        #if UNITY_EDITOR
        private void Reset()
        {
            slider ??= GetComponent<Slider>();
        }
        #endif
    }
}
