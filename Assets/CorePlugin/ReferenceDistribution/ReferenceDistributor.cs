using System.Collections.Generic;
using System.Linq;
using CorePlugin.Attributes.Validation;
using CorePlugin.Core;
using CorePlugin.Extensions;
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
        public static T GetReference<T>() where T : MonoBehaviour, IDistributingReference
        {
            return _instance._isInitialized ? _instance._distributingReferences.OfType<T>().First() : null;
        }

        /// <summary>
        /// Getting reference by type from list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetInterfaceReference<T>() where T : IDistributingReference
        {
            return _instance._isInitialized ? _instance._distributingReferences.OfType<T>().First() : default;
        }

        /// <summary>
        /// Getting reference by type from list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetInterfaceReferences<T>() where T : IDistributingReference
        {
            return _instance._isInitialized ? _instance._distributingReferences.OfType<T>() : default;
        }

        /// <summary>
        /// Finding reference if passed parameter is null.
        /// Use this if you need reference not in Start() and/or reference should be received in some event
        /// </summary>
        /// <param name="reference"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool AskReference<T>(ref T reference) where T : MonoBehaviour, IDistributingReference
        {
            reference ??= GetReference<T>();
            return ReferenceEquals(reference, null);
        }

        /// <summary>
        /// Getting references by type from list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetReferences<T>() where T : MonoBehaviour, IDistributingReference
        {
            return _instance._isInitialized ? _instance._distributingReferences.OfType<T>() : null;
        }
    }
}
