using System;

namespace SubModules.UI
{
    public interface IClickable
    {
        public event Action OnClick;
    }
}
