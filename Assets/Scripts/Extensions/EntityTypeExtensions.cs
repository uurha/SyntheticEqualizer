using System;
using System.Linq;
using Grid;
using UnityEngine;

namespace Extensions
{
    public static class EntityTypeExtensions
    {
        public static EntityRoute Negative(this EntityRoute entityType)
        {
            var current = (int) entityType;

            var next = current * -1;
            
            return (EntityRoute)next;
        }
        
        

        public static Vector3 Orient(this ICellEntity prefabEntity, int x, int z)
        {
            return new Vector3(prefabEntity.CellSize.x * x, 0f,
                               prefabEntity.CellSize.z * z);
        }
        
        public static Vector3 Orient(this ICellEntity prefabEntity, Tuple<int, int> coeffs)
        {
            return new Vector3(prefabEntity.CellSize.x * coeffs.Item1, 0f,
                               prefabEntity.CellSize.z * coeffs.Item2);
        }

        public static IInstantiable[] InstantiateLine(this IInstantiable[] line, Transform parent)
        {
            var instancedLine = new IInstantiable[line.Length];

            for (int i = 0; i < line.Length; i++)
            {
                instancedLine[i] = line[i].CreateInstance(parent);
            }

            return instancedLine;
        }
    }
}
