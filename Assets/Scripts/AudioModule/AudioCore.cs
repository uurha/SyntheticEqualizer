using CorePlugin.Core.Interface;
using UnityEngine;

namespace AudioModule
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
