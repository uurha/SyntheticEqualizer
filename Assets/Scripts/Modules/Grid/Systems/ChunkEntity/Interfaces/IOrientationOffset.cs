using System.Diagnostics.Contracts;
using Modules.Grid.Model;
using Modules.Grid.Systems.ChunkEntity.Unit;
using UnityEngine;

namespace Modules.Grid.Systems.ChunkEntity.Interfaces
{
    public interface IOrientationOffset
    {
        [Pure]
        public Orientation GetOffsetOrientation(Orientation orientation, IOrientationOffsetParams offsetParams);
    }
}
