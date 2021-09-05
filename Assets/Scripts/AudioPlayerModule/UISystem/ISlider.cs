using System;

namespace AudioPlayerModule.UISystem
{
    public interface ISlider<T>
    {
        public event Action<T> OnValueChanged;

        public T Value { get; set; }

        public void SetValueWithoutNotify(T value);
    }
}