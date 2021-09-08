using System;
using System.Collections.Generic;
using Base.BaseTypes;
using CorePlugin.Logger;
using CorePlugin.Singletons;
using UnityEngine;

namespace Modules.InputManagement.Handlers
{
    public enum ActionType
    {
        Held,
        Down
    }

    public class InputHandler : StaticObjectSingleton<InputHandler>
    {
        private readonly Array _keyCodes = Enum.GetValues(typeof(KeyCode));

        private readonly Dictionary<KeyCode, ActionType, Action> _keyEventDictionary =
            new Dictionary<KeyCode, ActionType, Action>();
        
        #if UNITY_EDITOR
        private bool _showEditorLogs;
        #endif

        public static event Action<Vector2> OnMouseScrollDelta; 
        
        #if UNITY_EDITOR
        /// <summary>
        /// Editor only initialization
        /// </summary>
        /// <param name="showEditorLogs"></param>
        public static void Initialize(bool showEditorLogs)
        {
            _instance = GetInstance();
            _instance._showEditorLogs = showEditorLogs;
        }
        #endif
        
        public static void Initialize()
        {
            _instance = GetInstance();
        }

        private void Update()
        {
            if (Input.anyKeyDown) OnKey(ActionType.Down, Input.GetKeyDown);
            if (Input.anyKey) OnKey(ActionType.Held, Input.GetKey);

            if (Input.mouseScrollDelta != Vector2.zero)
            {
                OnMouseScrollDelta?.Invoke(Input.mouseScrollDelta);
            }
        }

        private void OnKey(ActionType actionType, Func<KeyCode, bool> predicate)
        {
            foreach (KeyCode keyCode in _keyCodes)
            {
                if (!predicate.Invoke(keyCode)) continue;

                #if UNITY_EDITOR
                if (_showEditorLogs)
                    #endif
                    DebugLogger.Log($"KeyCode {actionType}: " + keyCode);

                if (_keyEventDictionary.TryGetValue(keyCode, actionType, out var action)) action?.Invoke();
                break;
            }
        }

        private static void AddKeyEvent(KeyCode keyCode, ActionType actionType, Action keyAction)
        {
            if (Contains(keyCode, actionType))
            {
                _instance._keyEventDictionary[keyCode, actionType] += keyAction;
            }
            else
            {
                _instance._keyEventDictionary.Add(keyCode, actionType, keyAction);
            }
        }

        private static void RemoveKeyEvent(KeyCode keyCode, ActionType actionType, Action keyAction)
        {
            if (!Contains(keyCode, actionType)) return;
            _instance._keyEventDictionary[keyCode, actionType] -= keyAction;
        }

        private static bool Contains(KeyCode keyCode, ActionType actionType)
        {
            return _instance._keyEventDictionary.ContainsKey(keyCode, actionType);
        }
    }
}
