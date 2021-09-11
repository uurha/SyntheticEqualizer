using System;
using System.Collections.Generic;
using System.Linq;
using Base.BaseTypes;
using SubModules.Cell.Interfaces;
using SubModules.Cell.Model;
using UnityEngine;

namespace Extensions
{
    public static class EntityExtensions
    {
        public static EntityRoute Negative(this EntityRoute entityType)
        {
            var current = (int) entityType;
            var next = current * -1;
            return (EntityRoute) next;
        }

        public static Vector3 Sum<T>(this IEnumerable<T> entities, Func<T, Vector3> predicate)
        {
            if (entities == null ||
                predicate == null)
                throw new ArgumentNullException();
            return entities.Aggregate(Vector3.zero, (current, entity) => current + predicate.Invoke(entity));
        }

        public static Vector3 Orient(this ICellEntity prefabEntity, Vector3 initialPosition, TupleInt items)
        {
            return initialPosition + new Vector3(prefabEntity.CellSize.x * items.Item1, 0f,
                                                 prefabEntity.CellSize.z * items.Item2);
        }

        public static Orientation Orient(this ICellEntity prefabEntity, Orientation initialOrientation, TupleInt items)
        {
            var position = initialOrientation.Position + new Vector3(prefabEntity.CellSize.x * items.Item1, 0f,
                                                                     prefabEntity.CellSize.z * items.Item2);
            var rotation = initialOrientation.Rotation * Quaternion.identity;
            var newOrientation = new Orientation(position, rotation);
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
