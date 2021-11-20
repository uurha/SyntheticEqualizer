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
        private bool _readyMove;
        private ICartingRoadComponent _currentCartingRoad;
        private PathOverTime _pathOverTime;
        private Transform _thisTransform;
        
        public void Initialize(RoadEvents.RequestNextRoadEntity onRequestNextRoad)
        {
            OnRequestNextRoadEntity = onRequestNextRoad ?? throw new ArgumentNullException();
            _isInitialized = true;
            _currentCartingRoad = OnRequestNextRoadEntity.Invoke();
            _pathOverTime = new PathOverTime();
            _thisTransform = transform;
        }

        public void SetReadyMove(bool state)
        {
            _readyMove = state;
        }

        private void Update()
        {
            if (_isInitialized && _readyMove) InteractMove();
        }

        private class PathOverTime
        {
            private float _time;
            private float _passedPath;

            public float PassedPath => _passedPath;

            public void UpdateTime()
            {
                _time += Time.deltaTime;
            }

            public void UpdatePath(float speed)
            {
                _passedPath = Mathf.InverseLerp(0, speed, _time);
            }

            public void Reset()
            {
                _time = 0;
                _passedPath = 0;
            }
        }

        private void InteractMove()
        {
            _pathOverTime.UpdatePath(speed);
            _pathOverTime.UpdateTime();
            while (true)
            {
                if (_currentCartingRoad != null && _currentCartingRoad.GetPoint(_pathOverTime.PassedPath, out var movePoint))
                {
                    _thisTransform.position = movePoint.position;
                    _thisTransform.forward = movePoint.tangent;
                }
                else
                {
                    _currentCartingRoad = OnRequestNextRoadEntity?.Invoke();
                    _pathOverTime.Reset();
                    _pathOverTime.UpdateTime();
                    continue;
                }
                break;
            }
        }
    }
}
