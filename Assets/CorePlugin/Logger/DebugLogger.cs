#region license

// Copyright 2021 - 2021 Arcueid Elizabeth D'athemon
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

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CorePlugin.Logger
{
    /// <summary>
    /// Custom logger solution for logs.
    /// <remarks>
    /// Logs in this class are dependant on <see langword="DEBUG"/> and <see langword="ENABLE_RELEASE_LOGS"/>. 
    /// If <see langword="ENABLE_RELEASE_LOGS"/> defined logs will displayed in Release Build.
    /// Otherwise only Editor and Developer Build will display logs.
    /// For defining preprocessor open CoreManager or write down in PlayerSettings in field "Scripting Define Symbols".
    /// </remarks>
    /// <seealso cref="CorePlugin.Core.CoreManager"/>
    /// </summary>

    //TODO: Strip logger from RELEASE builds
    public static class DebugLogger
    {
        public static void Log(string message)
        {
            #if DEBUG || ENABLE_RELEASE_LOGS
            Debug.Log(message);
            #endif
        }

        public static void Log(string message, Object context)
        {
            #if DEBUG || ENABLE_RELEASE_LOGS
            Debug.Log(message, context);
            #endif
        }

        public static void LogError(string message)
        {
            #if DEBUG || ENABLE_RELEASE_LOGS
            Debug.LogError(message);
            #endif
        }

        public static void LogWarning(string message)
        {
            #if DEBUG || ENABLE_RELEASE_LOGS
            Debug.LogWarning(message);
            #endif
        }

        public static void LogWarning(string message, Object context)
        {
            #if DEBUG || ENABLE_RELEASE_LOGS
            Debug.LogWarning(message, context);
            #endif
        }

        public static void LogError(string message, Object context)
        {
            #if DEBUG || ENABLE_RELEASE_LOGS
            Debug.LogError(message, context);
            #endif
        }

        public static void LogError(Exception exception, Object context)
        {
            #if DEBUG || ENABLE_RELEASE_LOGS
            Debug.LogError(exception, context);
            #endif
        }

        public static void LogException(Exception exception)
        {
            #if DEBUG || ENABLE_RELEASE_LOGS
            Debug.LogException(exception);
            #endif
        }

        public static void LogException(Exception exception, Object context)
        {
            #if DEBUG || ENABLE_RELEASE_LOGS
            Debug.LogException(exception, context);
            #endif
        }
    }
}
