#region license

// Copyright 2021 Arcueid Elizabeth D'athemon
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
//     http://www.apache.org/licenses/LICENSE-2.0 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.

#endregion

using System.Collections.Generic;
using System.Linq;
using CorePlugin.Attributes.Validation;
using CorePlugin.Extensions;
using CorePlugin.ReferenceDistribution.Interface;
using UnityEngine;

namespace CorePlugin.ReferenceDistribution
{
    /// <summary>
    /// Class responsible for reference distribution inside one scene.
    /// <remarks> Strongly recommended to use <see cref="CorePlugin.Cross.Events"/> and <see cref="CorePlugin.Cross.Events.Interface"/> instead of direct reference serialization.</remarks>
    /// </summary>
    [RequireComponent(typeof(Core.CoreManager))]
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
