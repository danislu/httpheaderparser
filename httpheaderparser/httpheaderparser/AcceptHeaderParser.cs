namespace HttpHeaderParser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class AcceptHeaderParser
    {
        public static string GetPreferedMediaType(string[] mediaTypes, string acceptHeader)
        {
            if (mediaTypes.Length == 0 || string.IsNullOrEmpty(acceptHeader))
                return string.Empty;

            var headers = ParseAndSortByPrecedence(acceptHeader.Split(new[] { ',' }));
            var candidates = ParseAndSortByPrecedence(mediaTypes);

            var dictionary = new Dictionary<AcceptHeaderPart, int>();
            foreach (var candidate in candidates)
            {
                var match = GetBestMatchingAcceptedMediaType(headers, candidate);
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

        private static AcceptHeaderPart GetBestMatchingAcceptedMediaType(IList<AcceptHeaderPart> headers, AcceptHeaderPart candidate)
        {
            return headers.FirstOrDefault(h => h.IsExactFullTypeWithParametersMatch(candidate))
                ?? headers.FirstOrDefault(h => h.IsExactTypeSubTypeMatch(candidate))
                    ?? headers.FirstOrDefault(h => h.IsExactTypeMatch(candidate))
                        ?? headers.ToDictionary(h => h, h => h.GetMatchScore(candidate))
                            .Where(kvp => kvp.Value > 0)
                            .OrderByDescending(kvp => kvp.Value)
                            .FirstOrDefault()
                            .Key;
        }

        private static IList<AcceptHeaderPart> ParseAndSortByPrecedence(IEnumerable<string> mediaTypes)
        {
            return mediaTypes
                .Select(s => new AcceptHeaderPart(s))
                .OrderByDescending(a => a, new AcceptHeaderPartEqualityComparer())
                .ToList();
        }

        internal static string[] SortByPrecedence(string header)
        {
            if (string.IsNullOrEmpty(header))
                throw new ArgumentException("Cannot be null or empty", "header");

            return ParseAndSortByPrecedence(header.Split(new[] { ',' }))
                .Select(h => h.ToString())
                .ToArray();
        }
    }
}
