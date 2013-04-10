namespace HttpHeaderParser
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    internal class AcceptHeaderPart
    {
        private readonly string mimeType;

        public AcceptHeaderPart(string mimeType)
        {
            this.Quality = 1;
            this.mimeType = mimeType.Trim();

            var parts = this.mimeType.Split(new[] { ';' });
            var fullType = parts.FirstOrDefault();
            if (string.IsNullOrEmpty(fullType))
                fullType = "*/*";

            var types = fullType.Split(new[] { '/' });
            Type = types[0].Trim();
            SubType = types[1].Trim();

            Parameters = new Dictionary<string, string>();
            foreach (var part in parts.Skip(1))
            {
                var key = string.Empty;
                var value = string.Empty;
                var pair = part.Trim().Split(new[] { '=' });
                if (pair.Length > 0)
                    key = pair[0].Trim();
                if (pair.Length == 2)
                    value = pair[1].Trim();
                else if (pair.Length > 2)
                    value = string.Join("=", pair.Skip(1).ToArray());

                if (!string.IsNullOrEmpty(key))
                {
                    if (key == "q")
                    {
                        double quality;
                        if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out quality))
                            this.Quality = quality;
                    }
                    else
                    {
                        Parameters.Add(key, value);
                    }
                }
            }
        }

        public string Type { get; private set; }

        public string SubType { get; private set; }

        public IDictionary<string, string> Parameters { get; private set; }

        public double Quality { get; private set; }

        public double GetMatchScore(AcceptHeaderPart candidate)
        {
            if (this.Type != "*" && this.Type != candidate.Type)
                return 0;
            if (this.SubType != "*" && this.SubType != candidate.SubType)
                return 0;

            int score = this.Parameters.Count(p => candidate.Parameters.Contains(p));
            return score + this.Quality;
        }

        public bool IsExactFullTypeWithParametersMatch(AcceptHeaderPart candidate)
        {
            return this.Type == candidate.Type 
                && this.SubType == candidate.SubType
                && this.Parameters.Count == candidate.Parameters.Count
                && !this.Parameters.Except(candidate.Parameters).Any();
        }

        public bool IsExactTypeSubTypeMatch(AcceptHeaderPart candidate)
        {
            return this.Type == candidate.Type
                && this.SubType == candidate.SubType
                && !this.Parameters.Any();
        }

        public bool IsExactTypeMatch(AcceptHeaderPart candidate)
        {
            return this.Type == candidate.Type 
                && this.SubType == "*"
                && !this.Parameters.Any();
        }

        public override string ToString()
        {
            return mimeType;
        }
    }
}
