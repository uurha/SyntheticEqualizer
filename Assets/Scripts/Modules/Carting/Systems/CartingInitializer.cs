using System;
using Base;
using CorePlugin.Attributes.EditorAddons;
using CorePlugin.Cross.Events.Interface;
using CorePlugin.Extensions;
using Modules.Carting.Systems.Cart;
using UnityEngine;

namespace Modules.Carting.Systems
{
    [CoreManagerElement]
    public class CartingInitializer : MonoBehaviour, IEventSubscriber, IEventHandler
    {
        [SerializeField] [CoreManagerElementsField(FieldType.EditorMode)]
        private CartEntity cartEntityPrefab;

        private event RoadEvents.RequestNextRoadEntity OnRequestNextRoadEntity;
        [CoreManagerElementsField(FieldType.PlayMode)] private CartEntity _instancedCart;
        private bool _isRoadReady;
        
        private void OnRoadReady(bool isReady)
        {
            if (isReady && !_isRoadReady)
            {
                _instancedCart = Instantiate(cartEntityPrefab);
                _instancedCart.Initialize(OnRequestNextRoadEntity);
            }
            _isRoadReady = isReady;
        }

        public Delegate[] GetSubscribers()
        {
            return new Delegate[]
                   {
                       (RoadEvents.OnRoadReadyEvent)OnRoadReady
                   };
        }

        public void InvokeEvents()
        {
            
        }

        public void Subscribe(params Delegate[] subscribers)
        {
            EventExtensions.Subscribe(ref OnRequestNextRoadEntity, subscribers);
        }

        public void Unsubscribe(params Delegate[] unsubscribers)
        {
            EventExtensions.Unsubscribe(ref OnRequestNextRoadEntity, unsubscribers);
        }
    }
}
