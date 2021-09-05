using System;

namespace AudioPlayerModule.UISystem
{
    public interface IClickable
    {
        public event Action OnClick;
    }
}