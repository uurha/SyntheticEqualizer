using Cells;
using UnityEngine;

namespace BehaviourSystem.Interfaces
{
    public interface ICellVisualBehaviour
    {
        public void RunBehaviour(Orientation[] data);

        public ICellVisualBehaviour Initialize();
        
        public ICellVisualBehaviour Initialize(IJobBehaviour jobBehaviour);
    }
}
