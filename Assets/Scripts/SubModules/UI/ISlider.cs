using System;

namespace SubModules.UI
{
    public interface ISlider<T>
    {
        public T Value { get; set; }
        public event Action<T> OnValueChanged;

        public void SetValueWithoutNotify(T value);
    }
}
