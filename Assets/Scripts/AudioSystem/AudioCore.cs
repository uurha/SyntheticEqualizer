using System;
using CorePlugin.Core.Interface;
using UnityEngine;

namespace AudioSystem
{
    public class AudioCore : MonoBehaviour, ICore
    {
        [SerializeField] private GameObject[] elements;
        public void InitializeElements()
        {
            foreach (var element in elements)
            {
                Instantiate(element, transform);
            }
        }
    }
}
