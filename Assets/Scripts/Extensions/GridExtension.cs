using Base.BaseTypes;
using Modules.Grid.Model;
using SubModules.Cell.Model;
using UnityEngine;

namespace Extensions
{
    public static class GridExtension
    {
        public static Vector3 LineSize(this GridConfiguration gridConfiguration, RoadDirection outDirection,
                                       TupleInt position)
        {
            var lineSize = outDirection switch
                           {
                               RoadDirection.South => gridConfiguration.GetLineSize(MatrixDimension.Row, position.Item1)
                                                                     .OnlyOneAxis(Axis.Z) * -1,
                               RoadDirection.North => gridConfiguration.GetLineSize(MatrixDimension.Row, position.Item1)
                                                                     .OnlyOneAxis(Axis.Z),
                               RoadDirection.East => gridConfiguration.GetLineSize(MatrixDimension.Column, position.Item2)
                                                                    .OnlyOneAxis(Axis.X),
                               RoadDirection.West => gridConfiguration.GetLineSize(MatrixDimension.Column, position.Item2)
                                                                    .OnlyOneAxis(Axis.X) * -1,
                               _ => Vector3.zero
                           };
            return lineSize;
        }
    }
}
