using Base.BehaviourModel.Interfaces;
using Modules.Carting.Interfaces;
using Modules.Grid.Model;
using UnityEngine;

namespace Modules.Grid.Interfaces
{
    public interface IChunkComponent : IInstantiable
    {
        public Vector3 ChunkSize { get; }
        
        public bool IsRoad { get; }
        
        public ICartingRoad CartingRoadComponent { get; }

        public bool TryGetVisualBehaviour(out IChunkVisual behaviour);

        public string Name { get; set; }

        public IChunkComponent Initialize();
        
        public IChunkComponent Initialize(string objectName);

        public void SetActive(bool state);

        public IChunkComponent SetOrientation(Orientation orientation);

        public void Destroy();

        public Orientation GetOrientation(bool isLocal = false);
    }
}
