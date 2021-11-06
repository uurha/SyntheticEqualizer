using System;
using System.Collections.Generic;
using System.Linq;
using Modules.Grid.Interfaces;
using Modules.Grid.Model;

namespace SubModules.Beom
{
    [Serializable]
    public struct BeomCells
    {
        private ICellComponent[] _entities;

        public BeomCells(IEnumerable<ICellComponent> cellEntities)
        {
            _entities = cellEntities as ICellComponent[] ?? cellEntities.ToArray();
        }

        public ICellComponent GetRoadCell(RoadDirection entityType, Direction direction)
        {
            return direction switch
                   {
                       Direction.In => _entities.FirstOrDefault(x => x.IsRoad && x.CartingRoadComponent.InDirection == entityType),
                       Direction.Out =>
                           _entities.FirstOrDefault(x => x.IsRoad && x.CartingRoadComponent.OutDirection == entityType),
                       _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
                   };
        }

        public readonly ICellComponent GetRoadCell(RoadDirection inDir, RoadDirection outDir)
        {
            return _entities.FirstOrDefault(x => x.IsRoad && x.CartingRoadComponent.InDirection == inDir &&
                                                 x.CartingRoadComponent.OutDirection == outDir);
        }
        
        public readonly ICellComponent GetNonRoadCell()
        {
            return _entities.FirstOrDefault(x => !x.IsRoad);
        }
    }
}
