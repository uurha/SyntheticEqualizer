using Base.BehaviourModel;
using Base.BehaviourModel.Interfaces;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace Modules.Grid.Systems.CellEntity.Behaviours
{
    [BurstCompile]
    public struct DefaultRotateBehaviour : IJobBehaviour
    {
        private BehaviourData _data;

        public void Execute(int index, TransformAccess transform)
        {
            var position = _data.InitialData[index].Rotation;
            var additional = _data.AdditionalData[index % _data.AdditionalDataLenght].Rotation;

            transform.localRotation = position * additional;
        }

        public void SetData(IBehaviourData data)
        {
            _data = (BehaviourData) data;
        }
    }
}
