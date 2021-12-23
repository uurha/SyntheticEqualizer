using Base.BehaviourModel.Interfaces;
using CorePlugin.Attributes.EditorAddons;
using CorePlugin.Attributes.EditorAddons.SelectAttributes;
using CorePlugin.Logger;
using Modules.Carting.Interfaces;
using Modules.Grid.Interfaces;
using Modules.Grid.Model;
using UnityEngine;

namespace Modules.Grid.Systems.ChunkEntity
{
    public class DefaultChunkComponent : MonoBehaviour, IChunkComponent
    {
        [SerializeField] private Vector3 chunkSize;

        [SelectImplementation(typeof(IChunkVisual))] [SerializeReference] private IChunkVisual behaviour;
        [SelectImplementation(typeof(ICartingRoad))] [SerializeReference] private ICartingRoad cartingRoadComponent;
        private bool _isRoad;

        public Vector3 ChunkSize => chunkSize;

        public bool IsRoad => _isRoad;

        public ICartingRoad CartingRoadComponent => cartingRoadComponent;

        public string Name
        {
            get => gameObject.name;
            set => gameObject.name = value;
        }

        public IInstantiable CreateInstance(Transform parent)
        {
            return Instantiate(this, Vector3.zero, Quaternion.identity, parent);
        }

        public bool TryGetVisualBehaviour(out IChunkVisual visual)
        {
            visual = behaviour;
            return visual != null;
        }

        private void OnDisable()
        {
            behaviour?.OnDisable();
            cartingRoadComponent?.OnDisable();
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public Orientation GetOrientation(bool isLocal = false)
        {
            return new Orientation(transform, isLocal);
        }

        public IChunkComponent Initialize()
        {
            if (behaviour == null)
                DebugLogger.LogWarning($"No Visual behaviour attached to {transform.name}", this);
            _isRoad = cartingRoadComponent != null;
            return this;
        }

        public IChunkComponent Initialize(string objectName)
        {
            Name = objectName;
            return Initialize();
        }

        public void SetActive(bool state)
        {
            if (gameObject.activeSelf != state) gameObject.SetActive(state);
        }

        public IChunkComponent SetOrientation(Orientation orientation)
        {
            var thisTransform = transform;
            if (orientation.IsLocal)
            {
                thisTransform.localPosition = orientation.Position;
                thisTransform.localRotation = orientation.Rotation;
            }
            else
            {
                thisTransform.position = orientation.Position;
                thisTransform.rotation = orientation.Rotation;
            }
            return this;
        }
    }
}
