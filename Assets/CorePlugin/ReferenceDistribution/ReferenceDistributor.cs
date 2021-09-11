using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CorePlugin.Attributes.Validation;
using CorePlugin.Core;
using CorePlugin.Extensions;
using CorePlugin.Logger;
using CorePlugin.ReferenceDistribution.Interface;
using UnityEngine;

namespace CorePlugin.ReferenceDistribution
{
    /// <summary>
    /// Class responsible for reference distribution inside one scene.
    /// <remarks> Strongly recommended to use <see cref="CorePlugin.Cross.Events"/> and <see cref="CorePlugin.Cross.Events.Interface"/> instead of direct reference serialization.</remarks>
    /// </summary>
    [RequireComponent(typeof(CoreManager))]
    [OneAndOnly]
    public class ReferenceDistributor : MonoBehaviour
    {
        private static ReferenceDistributor _instance;
        private IEnumerable<IDistributingReference> _distributingReferences;
        private bool _isInitialized;
        private static readonly string[] warningCallers = {"Awake", "OnEnable"};

        private void OnDisable()
        {
            _instance = null;
        }

        /// <summary>
        /// Initializing distribution references
        /// </summary>
        public void Initialize()
        {
            _isInitialized = UnityExtensions.TryToFindObjectsOfType(out _distributingReferences);
            _instance = this;
        }

        /// <summary>
        /// Getting reference by type from list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetReference<T>([CallerMemberName] string callerName = "") where T : MonoBehaviour, IDistributingReference
        {
            ValidateCaller(callerName);
            return _instance._isInitialized ? _instance._distributingReferences.OfType<T>().First() : null;
        }

        /// <summary>
        /// Getting reference by type from list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetInterfaceReference<T>([CallerMemberName] string callerName = "") where T : IDistributingReference
        {
            ValidateCaller(callerName);
            return _instance._isInitialized ? _instance._distributingReferences.OfType<T>().First() : default;
        }

        /// <summary>
        /// Getting reference by type from list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetInterfaceReferences<T>([CallerMemberName] string callerName = "") where T : IDistributingReference
        {
            ValidateCaller(callerName);
            return _instance._isInitialized ? _instance._distributingReferences.OfType<T>() : default;
        }

        /// <summary>
        /// Getting references by type from list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetReferences<T>([CallerMemberName] string callerName = "") where T : MonoBehaviour, IDistributingReference
        {
            ValidateCaller(callerName);
            return _instance._isInitialized ? _instance._distributingReferences.OfType<T>() : null;
        }

        /// <summary>
        /// Finding reference if passed parameter is null.
        /// Use this if you need reference not in Start() and/or reference should be received in some event
        /// </summary>
        /// <param name="reference"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool AskReference<T>(ref T reference, [CallerMemberName] string callerName = "") where T : MonoBehaviour, IDistributingReference
        {
            ValidateCaller(callerName);
            reference ??= GetReference<T>();
            return ReferenceEquals(reference, null);
        }
        
        private static void ValidateCaller(string callerName)
        {
            if (warningCallers.Contains(callerName))
                DebugLogger.LogError($"It's not safe to call {nameof(ReferenceDistributor)} from {nameof(UnityEngine)}.{callerName}",
                                     _instance);
        }
    }
}
