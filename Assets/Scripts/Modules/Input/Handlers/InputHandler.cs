using System;
using System.Collections.Generic;
using CorePlugin.Logger;
using CorePlugin.Singletons;
using UnityEngine;

namespace Modules.Input.Handlers
{
    public class InputHandler : StaticObjectSingleton<InputHandler>
    {
        private readonly Array _keyCodes = Enum.GetValues(typeof(KeyCode));
        private readonly Dictionary<KeyCode, Action> _keyEventDictionary = new Dictionary<KeyCode, Action>();

        private void Update()
        {
            if (UnityEngine.Input.anyKeyDown)
                foreach (KeyCode keyCode in _keyCodes)
                {
                    if (!UnityEngine.Input.GetKeyDown(keyCode)) continue;
                    DebugLogger.Log("KeyCode down: " + keyCode);
                    if (_keyEventDictionary.TryGetValue(keyCode, out var action)) action?.Invoke();
                    break;
                }
        }

        private static void AddKeyEvent(KeyCode keyCode, Action keyAction)
        {
            if (TryGetValue(keyCode, out var actions))
            {
                actions += keyAction;
                SetActions(keyCode, actions);
            }
            else
            {
                _instance._keyEventDictionary.Add(keyCode, keyAction);
            }
        }

        private static void RemoveKeyEvent(KeyCode keyCode, Action keyAction)
        {
            if (!TryGetValue(keyCode, out var actions)) return;
            actions -= keyAction;
            SetActions(keyCode, actions);
        }

        private static void SetActions(KeyCode keyCode, Action actions)
        {
            _instance._keyEventDictionary[keyCode] = actions;
        }

        private static bool TryGetValue(KeyCode keyCode, out Action action)
        {
            return _instance._keyEventDictionary.TryGetValue(keyCode, out action);
        }
    }
}
