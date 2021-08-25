﻿using System;
using BehaviourSystem.Interfaces;
using Unity.Burst;
using Unity.Collections;
using UnityEngine.Jobs;

namespace BehaviourSystem.Default
{
    [BurstCompile]
    public struct DefaultMoveBehaviour : IJobBehaviour
    {
        [DeallocateOnJobCompletion]
        private BehaviourData _data;
        
        public void Execute(int index, TransformAccess transform)
        {
            transform.localPosition = _data.InitialData[index].Position + _data.AdditionalData[index % _data.Lenght].Position;
        }

        public void SetData(IBehaviourData data)
        {
            _data = (BehaviourData) data;
        }

        public void Dispose()
        {
            _data.Dispose();
        }
    }
}
