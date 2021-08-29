using Base.BaseTypes;
using CellModule.Model;
using GridModule.Model;
using UnityEngine;

namespace Extensions
{
    public static class GridExtension
    {
        public static Vector3 LineSize(this GridConfiguration gridConfiguration, EntityRoute outDirection, TupleInt position)
        {
            var lineSize = outDirection switch
                           {
                               EntityRoute.South => gridConfiguration.GetLineSize(MatrixDimension.Row, position.Item1)
                                                                     .OnlyOneAxis(Axis.Z) * -1,
                               EntityRoute.North => gridConfiguration.GetLineSize(MatrixDimension.Row, position.Item1)
                                                                     .OnlyOneAxis(Axis.Z),
                               EntityRoute.East => gridConfiguration.GetLineSize(MatrixDimension.Column, position.Item2)
                                                                    .OnlyOneAxis(Axis.X),
                               EntityRoute.West => gridConfiguration.GetLineSize(MatrixDimension.Column, position.Item2)
                                                                    .OnlyOneAxis(Axis.X) * -1,
                               _ => Vector3.zero
                           };
            return lineSize;
        }
    }
}
