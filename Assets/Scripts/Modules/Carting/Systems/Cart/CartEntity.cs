using System;
using Base;
using CorePlugin.Attributes.EditorAddons;
using Modules.Carting.Interfaces;
using UnityEngine;

namespace Modules.Carting.Systems.Cart
{
    [CoreManagerElement]
    public class CartEntity : MonoBehaviour
    {
        [SerializeField] private float speed;
        private event RoadEvents.RequestNextRoadEntity OnRequestNextRoadEntity;
        private bool _isInitialized;
        private ICartingRoadComponent _currentCartingRoad;
        
        public void Initialize(RoadEvents.RequestNextRoadEntity onRequestNextRoad)
        {
            OnRequestNextRoadEntity = onRequestNextRoad ?? throw new ArgumentNullException();
            _isInitialized = true;
             _currentCartingRoad = OnRequestNextRoadEntity.Invoke();
        }

        private void Update()
        {
            if (_isInitialized)
            {
                
            }
        }
    }
}
