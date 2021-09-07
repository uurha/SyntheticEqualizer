using System;
using System.Collections.Generic;
using System.Linq;
using SubModules.Cell.Interfaces;
using SubModules.Cell.Model;

namespace SubModules.Beom
{
    [Serializable]
    public struct BeomCells
    {
        private ICellEntity[] _entities;

        public BeomCells(IEnumerable<ICellEntity> cellEntities)
        {
            _entities = cellEntities as ICellEntity[] ?? cellEntities.ToArray();
        }

        public ICellEntity GetCell(EntityRoute entityType, Direction direction)
        {
            return direction switch
                   {
                       Direction.In => _entities.FirstOrDefault(x => x.InDirection == entityType),
                       Direction.Out => _entities.FirstOrDefault(x => x.OutDirection == entityType),
                       _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
                   };
        }

        public readonly ICellEntity GetCell(EntityRoute inDir, EntityRoute outDir)
        {
            return _entities.FirstOrDefault(x => x.InDirection == inDir && x.OutDirection == outDir);
        }
    }
}
