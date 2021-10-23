using System;
using SubModules.Cell.Model;
using SubModules.CellItem.Behaviours;
using Unity.Jobs;
using UnityEngine.Jobs;

namespace Extensions
{
    public readonly struct JobsExtensions
    {
        public static JobHandle JobHandleConverter(TransitStruct transitStruct)
        {
            return transitStruct.JobBehaviour switch
                   {
                       DefaultMoveBehaviour behaviour => IJobParallelForTransformExtensions.Schedule(behaviour, transitStruct.AccessArray),
                       DefaultRotateBehaviour behaviour => IJobParallelForTransformExtensions.Schedule(behaviour, transitStruct.AccessArray),
                       DefaultScaleBehaviour behaviour => IJobParallelForTransformExtensions.Schedule(behaviour, transitStruct.AccessArray),
                       _ => throw new InvalidOperationException(nameof(transitStruct.JobBehaviour),
                                                                new
                                                                    Exception($"Unknown type assigned to {nameof(transitStruct.JobBehaviour)}"))
                   };
        }
    }
}
