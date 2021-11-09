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
        private float _passedPath;
        private float _time;
        
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
                InteractMove();
            }
        }

        private void InteractMove()
        {
            _passedPath = Mathf.InverseLerp(0, speed, _time);
            _time += Time.deltaTime;
            while (true)
            {
                if (_currentCartingRoad != null && _currentCartingRoad.GetPoint(_passedPath, out var movePoint))
                {
                    transform.position = movePoint;
                }
                else
                {
                    _currentCartingRoad = OnRequestNextRoadEntity?.Invoke();
                    _passedPath = 0f;
                    _time = 0f;
                    continue;
                }
                break;
            }
        }
    }
}
