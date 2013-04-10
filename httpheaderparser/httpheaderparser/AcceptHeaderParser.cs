namespace HttpHeaderParser
{
    using System.Linq;
    using System.Collections.Generic;

    public static class AcceptHeaderParser
    {
        public static string GetPreferedMediaType(string[] mediaTypes, string acceptHeader)
        {
            if (mediaTypes.Length == 0 || string.IsNullOrEmpty(acceptHeader))
                return string.Empty;

            var headers = ParseAndSortByPrecedence(acceptHeader);
            var candidates = ParseAndSortByPrecedence(string.Join(",", mediaTypes));

            var dictionary = new Dictionary<AcceptHeaderPart, int>();
            foreach (var candidate in candidates)
            {
                var match = headers.FirstOrDefault(h => h.IsExactFullTypeWithParametersMatch(candidate));
                if (match != null)
                {
                    dictionary.Add(candidate, headers.IndexOf(match));
                    continue;
                }
                match = headers.FirstOrDefault(h => h.IsExactTypeSubTypeMatch(candidate));
                if (match != null)
                {
                    dictionary.Add(candidate, headers.IndexOf(match));
                    continue;
                }
                match = headers.FirstOrDefault(h => h.IsExactTypeMatch(candidate));
                if (match != null)
                {
                    dictionary.Add(candidate, headers.IndexOf(match));
                    continue;
                }

                match = headers
                    .ToDictionary(h => h, h => h.GetMatchScore(candidate))
                    .Where(kvp => kvp.Value > 0)
                    .OrderByDescending(kvp => kvp.Value)
                    .FirstOrDefault()
                    .Key;
                if (match != null)
                {
                    dictionary.Add(candidate, headers.IndexOf(match));
                }
            }

            var bestMatch = dictionary
                 .OrderBy(kvp => kvp.Value)
                 .Select(kvp => kvp.Key.ToString())
                 .FirstOrDefault();
            return bestMatch ?? string.Empty;
        }

        private static IList<AcceptHeaderPart> ParseAndSortByPrecedence(string header)
        {
            return header
                .Split(new[] { ',' })
                .Select(s => new AcceptHeaderPart(s))
                .OrderByDescending(a => a, new AcceptHeaderPartEqualityComparer())
                .ToList();
        }

        internal static string[] SortByPrecedence(string header)
        {
            return ParseAndSortByPrecedence(header)
                .Select(h => h.ToString())
                .ToArray();
        }
    }
}
