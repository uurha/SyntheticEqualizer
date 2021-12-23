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
        private IChunkComponent[] _entities;

        public BeomCells(IEnumerable<IChunkComponent> cellEntities)
        {
            _entities = cellEntities as IChunkComponent[] ?? cellEntities.ToArray();
        }

        public IChunkComponent GetRoadCell(RoadDirection entityType, Direction direction)
        {
            return direction switch
                   {
                       Direction.In => _entities.FirstOrDefault(x => x.IsRoad && x.CartingRoadComponent.InDirection == entityType),
                       Direction.Out =>
                           _entities.FirstOrDefault(x => x.IsRoad && x.CartingRoadComponent.OutDirection == entityType),
                       _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
                   };
        }

        public readonly IChunkComponent GetRoadCell(RoadDirection inDir, RoadDirection outDir)
        {
            return _entities.FirstOrDefault(x => x.IsRoad && x.CartingRoadComponent.InDirection == inDir &&
                                                 x.CartingRoadComponent.OutDirection == outDir);
        }
        
        public readonly IChunkComponent GetNonRoadCell()
        {
            return _entities.FirstOrDefault(x => !x.IsRoad);
        }
    }
}
