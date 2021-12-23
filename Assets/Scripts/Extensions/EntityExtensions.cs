using System;
using System.Collections.Generic;
using System.Linq;
using Base.BaseTypes;
using Modules.Grid.Interfaces;
using Modules.Grid.Model;
using UnityEngine;

namespace Extensions
{
    public static class EntityExtensions
    {
        public static RoadDirection Negative(this RoadDirection entityType)
        {
            var current = (int) entityType;
            var next = current * -1;
            return (RoadDirection) next;
        }

        public static Vector3 Sum<T>(this IEnumerable<T> entities, Func<T, Vector3> predicate)
        {
            if (entities == null ||
                predicate == null)
                throw new ArgumentNullException();
            return entities.Aggregate(Vector3.zero, (current, entity) => current + predicate.Invoke(entity));
        }

        public static Vector3 Orient(this IChunkComponent prefabEntity, Vector3 initialPosition, TupleInt items)
        {
            return initialPosition + new Vector3(prefabEntity.ChunkSize.x * items.Item1, 0f,
                                                 prefabEntity.ChunkSize.z * items.Item2);
        }

        public static Orientation Orient(this IChunkComponent prefabEntity, Orientation initialOrientation, TupleInt items)
        {
            var (x, z) = items;

            var position = initialOrientation.Position + new Vector3(prefabEntity.ChunkSize.x * x, 0f,
                                                                     prefabEntity.ChunkSize.z * z);
            var rotation = initialOrientation.Rotation * Quaternion.identity;
            var newOrientation = new Orientation(position, rotation, Vector3.zero);
            return newOrientation;
        }

        public static IInstantiable[] InstantiateLine(this IInstantiable[] line, Transform parent)
        {
            var instancedLine = new IInstantiable[line.Length];
            for (var i = 0; i < line.Length; i++) instancedLine[i] = line[i].CreateInstance(parent);
            return instancedLine;
        }
    }
}
