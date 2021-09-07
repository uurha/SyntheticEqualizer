using System;

namespace Modules.AudioPlayer.UISystem
{
    public interface IClickable
    {
        public event Action OnClick;
    }
}
