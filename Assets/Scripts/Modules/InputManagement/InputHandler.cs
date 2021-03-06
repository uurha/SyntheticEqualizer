using System;
using Base.BaseTypes;
using CorePlugin.Logger;
using CorePlugin.Singletons;
using Modules.InputManagement.Model;
using UnityEngine;

namespace Modules.InputManagement
{
    public enum ActionType
    {
        Down,
        Held
    }

    public class InputHandler : StaticObjectSingleton<InputHandler>
    {
        #if UNITY_EDITOR
        [SerializeField] private bool showEditorLogs;
        #endif

        private readonly Array _keyCodes = Enum.GetValues(typeof(KeyCode));

        private readonly Dictionary<KeyCode, ActionType, Action> _keyEventDictionary =
            new Dictionary<KeyCode, ActionType, Action>();

        public static event Action<Vector2> OnMouseScrollDelta;

        private void Update()
        {
            if (Input.anyKeyDown) OnKey(ActionType.Down, Input.GetKeyDown);
            if (Input.anyKey) OnKey(ActionType.Held, Input.GetKey);
            if (Input.mouseScrollDelta != Vector2.zero) OnMouseScrollDelta?.Invoke(Input.mouseScrollDelta);
        }

        public static void AddKeyEvent(KeyCode keyCode, ActionType actionType, Action keyAction)
        {
            if (Contains(keyCode, actionType))
                _instance._keyEventDictionary[keyCode, actionType] += keyAction;
            else
                _instance._keyEventDictionary.Add(keyCode, actionType, keyAction);
        }

        public static void AddKeyEvent(KeyPreset keyPreset, Action keyAction)
        {
            AddKeyEvent(keyPreset.Code, keyPreset.Type, keyAction);
        }

        private static bool Contains(KeyCode keyCode, ActionType actionType)
        {
            return _instance._keyEventDictionary.ContainsKey(keyCode, actionType);
        }

        [RuntimeInitializeOnLoadMethod]
        public static void InitializeOnLoad()
        {
            Initialize();
            DontDestroyOnLoad(_instance);
        }

        private void OnKey(ActionType actionType, Func<KeyCode, bool> predicate)
        {
            foreach (KeyCode keyCode in _keyCodes)
            {
                if (!predicate.Invoke(keyCode)) continue;
                #if UNITY_EDITOR
                if (showEditorLogs)
                    #endif
                    DebugLogger.Log($"KeyCode {actionType}: " + keyCode);
                if (_keyEventDictionary.TryGetValue(keyCode, actionType, out var action)) action?.Invoke();
                break;
            }
        }

        public static void RemoveKeyEvent(KeyCode keyCode, ActionType actionType, Action keyAction)
        {
            if (!Contains(keyCode, actionType)) return;
            _instance._keyEventDictionary[keyCode, actionType] -= keyAction;
        }

        public static void RemoveKeyEvent(KeyPreset keyPreset, Action keyAction)
        {
            RemoveKeyEvent(keyPreset.Code, keyPreset.Type, keyAction);
        }
    }
}
