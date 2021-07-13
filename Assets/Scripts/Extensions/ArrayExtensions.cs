using System.Linq;

namespace Extensions
{
    public static class ArrayExtensions
    {
        public static T[] GetColumn<T>(this T[,] matrix, int columnNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(1))
                             .Select(x => matrix[columnNumber, x])
                             .ToArray();
        }

        public static T[] GetRow<T>(this T[,] matrix, int rowNumber)
        {
            var length = matrix.GetLength(0);
            var enumerable = Enumerable.Range(0, length);

            var @select = enumerable
               .Select(x => matrix[x, rowNumber]);

            return @select
                  .ToArray();
        }
    }
}
