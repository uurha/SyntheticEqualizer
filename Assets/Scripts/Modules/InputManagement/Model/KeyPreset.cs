using System;
using UnityEngine;

namespace Modules.InputManagement.Model
{
    [Serializable]
    public class KeyPreset
    {
        [SerializeField] private KeyCode keyCode;
        [SerializeField] private ActionType actionType;

        public KeyCode Code => keyCode;

        public ActionType Type => actionType;

        public KeyPreset()
        {
        }

        public KeyPreset(KeyCode code, ActionType type)
        {
            keyCode = code;
            actionType = type;
        }

        public void Set(KeyCode code)
        {
            keyCode = code;
        }

        public void Set(ActionType type)
        {
            actionType = type;
        }
    }
}
