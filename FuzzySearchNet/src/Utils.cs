namespace FuzzySearchNet;

public static class Utils
{
    /// <summary>
    /// Group matches and return best.
    /// Currently assumes the matches are in the same order they are found...
    /// </summary>    
    internal static IEnumerable<MatchResult> GetBestMatches(string text, IEnumerable<CandidateMatch> matches, int maxDistanece)
    {
        var matchesEnumerator = matches.GetEnumerator();

        if (matchesEnumerator.MoveNext())
        {
            var bestMatch = matchesEnumerator.Current;
            var matchStartIndex = matchesEnumerator.Current.StartIndex;

            while (matchesEnumerator.MoveNext())
            {
                var match = matchesEnumerator.Current;

                if (matchesEnumerator.Current.StartIndex > (matchStartIndex + maxDistanece))
                {
                    yield return new MatchResult
                    {
                        StartIndex = bestMatch.StartIndex,
                        EndIndex = bestMatch.TextIndex,
                        Distance = bestMatch.Distance,
                        Match = text[bestMatch.StartIndex..bestMatch.TextIndex],
                        Deletions = bestMatch.Deletions,
                        Insertions = bestMatch.Insertions,
                        Substitutions = bestMatch.Substitutions,
                    };

                    bestMatch = matchesEnumerator.Current;
                }

                if (match.Distance < bestMatch.Distance || match.Distance == bestMatch.Distance && (match.TextIndex - match.StartIndex) > (bestMatch.TextIndex - bestMatch.StartIndex))
                {
                    bestMatch = match;
                }

                matchStartIndex = matchesEnumerator.Current.StartIndex;
            }

            yield return new MatchResult
            {
                StartIndex = bestMatch.StartIndex,
                EndIndex = bestMatch.TextIndex,
                Distance = bestMatch.Distance,
                Match = text[bestMatch.StartIndex..bestMatch.TextIndex],
                Deletions = bestMatch.Deletions,
                Insertions = bestMatch.Insertions,
                Substitutions = bestMatch.Substitutions,
            };
        }
    }


    /// <summary>
    /// Group matches and return best.
    /// Currently assumes the matches are in the same order they are found...
    /// </summary>
    internal static async IAsyncEnumerable<MatchResult> GetBestMatchesAsync(IAsyncEnumerable<MatchResultWithValue> matches, int maxDistanece)
    {
        var matchesEnumerator = matches.GetAsyncEnumerator();

        if (await matchesEnumerator.MoveNextAsync())
        {
            var bestMatch = matchesEnumerator.Current;
            var matchStartIndex = matchesEnumerator.Current.StartIndex;

            while (await matchesEnumerator.MoveNextAsync())
            {
                var match = matchesEnumerator.Current;

                if (matchesEnumerator.Current.StartIndex > (matchStartIndex + maxDistanece))
                {
                    yield return new MatchResult
                    {
                        StartIndex = bestMatch.StartIndex,
                        EndIndex = bestMatch.EndIndex,
                        Distance = bestMatch.Distance,
                        Match = bestMatch.Match,
                        Deletions = bestMatch.Deletions,
                        Insertions = bestMatch.Insertions,
                        Substitutions = bestMatch.Substitutions,
                    };

                    bestMatch = matchesEnumerator.Current;
                }

                if (match.Distance < bestMatch.Distance || match.Distance == bestMatch.Distance && (match.EndIndex - match.StartIndex) > (bestMatch.EndIndex - bestMatch.StartIndex))
                {
                    bestMatch = match;
                }

                matchStartIndex = matchesEnumerator.Current.StartIndex;
            }

            yield return new MatchResult
            {
                StartIndex = bestMatch.StartIndex,
                EndIndex = bestMatch.EndIndex,
                Distance = bestMatch.Distance,
                Match = bestMatch.Match,
                Deletions = bestMatch.Deletions,
                Insertions = bestMatch.Insertions,
                Substitutions = bestMatch.Substitutions,
            };
        }
    }
}
