using System;
using SubModules.Cell;
using SubModules.CellItem.Behaviours;
using Unity.Jobs;
using UnityEngine.Jobs;

namespace Extensions
{
    public struct JobsExtensions
    {
        public static JobHandle JobHandleConverter(TransitStruct transitStruct)
        {
            return transitStruct.JobBehaviour switch
                   {
                       DefaultMoveBehaviour behaviour => behaviour.Schedule(transitStruct.AccessArray),
                       DefaultRotateBehaviour behaviour => behaviour.Schedule(transitStruct.AccessArray),
                       DefaultScaleBehaviour behaviour => behaviour.Schedule(transitStruct.AccessArray),
                       _ => throw new InvalidOperationException(nameof(transitStruct.JobBehaviour),
                                                                new
                                                                    Exception($"Unknown type assigned to {nameof(transitStruct.JobBehaviour)}"))
                   };
        }
    }
}
