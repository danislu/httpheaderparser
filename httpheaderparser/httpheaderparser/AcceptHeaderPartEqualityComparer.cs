namespace HttpHeaderParser
{
    using System.Collections.Generic;
    using System.Linq;

    internal class AcceptHeaderPartEqualityComparer : IComparer<AcceptHeaderPart>
    {
        public int Compare(AcceptHeaderPart x, AcceptHeaderPart y)
        {
            if (x.Quality > y.Quality)
                return 1;
            if (x.Quality < y.Quality)
                return -1;

            var result = CompareType(x.Type, y.Type);
            if (result != 0)
                return result;

            result = CompareType(x.SubType, y.SubType);
            if (result != 0)
                return result;

            var xCount = x.Parameters.Count();
            var yCount = y.Parameters.Count();
            if (xCount > yCount)
                return 1;
            if (xCount < yCount)
                return -1;
            return 0;
        }

        private static int CompareType(string xType, string yType)
        {
            if (xType == "*" && yType != "*")
                return -1;
            if (xType != "*" && yType == "*")
                return 1;
            return 0;
        }
    }
}