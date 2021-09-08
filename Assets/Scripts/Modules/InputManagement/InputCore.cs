using System;
using CorePlugin.Core.Interface;
using Modules.InputManagement.Handlers;
using UnityEngine;

namespace Modules.InputManagement
{
    public class InputCore : MonoBehaviour, ICore
    {
        [SerializeField] private bool showEditorLogs;
        
        public void InitializeElements()
        {
            #if UNITY_EDITOR
            InputHandler.Initialize(showEditorLogs);
            #else
            InputHandler.Initialize();
            #endif
        }
    }
}
