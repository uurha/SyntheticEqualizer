using CorePlugin.Logger;
using UnityEngine;

namespace CorePlugin.Singletons
{
    /// <summary>
    /// Base for static objects singletons.
    /// Strongly recommended to use singletons as little as possible.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StaticObjectSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private protected static T _instance;

        public static bool IsInitialised => _instance != null;

        private protected static T GetInstance()
        {
            if (IsInitialised) return _instance;
            return _instance = new GameObject(typeof(T).Name).AddComponent<T>();
        }

        protected virtual void OnDestroy()
        {
            DebugLogger.Log("OnDestroy: " + typeof(T));
            if (_instance == this) _instance = null;
        }
    }
}