namespace FuzzySearchNet;

public static class Utils
{
    /// <summary>
    /// Group matches and return best.
    /// Currently assumes the matches are in the same order they are found...
    /// </summary>    
    public static IEnumerable<MatchResult> GetBestMatches(IEnumerable<MatchResult> matches, int maxDistanece)
    {
        var matchesEnumerator = matches.GetEnumerator();

        var group = new List<MatchResult>();

        if (matchesEnumerator.MoveNext())
        {
            group.Add(matchesEnumerator.Current);

            var matchStartIndex = matchesEnumerator.Current.StartIndex;

            while (matchesEnumerator.MoveNext())
            {
                var currentMatch = matchesEnumerator.Current;

                if (currentMatch != null)
                {
                    if (currentMatch.StartIndex > (matchStartIndex + maxDistanece))
                    {
                        yield return group.OrderBy(o => o.Distance).ThenByDescending(o => o.Match.Length).First();
                        group.Clear();
                    }

                    group.Add(currentMatch);

                    matchStartIndex = currentMatch.StartIndex;
                }
            }
        }

        if (group.Any())
        {
            yield return group.OrderBy(o => o.Distance).ThenByDescending(o => o.Match.Length).First();
        }
    }


    /// <summary>
    /// Group matches and return best.
    /// Currently assumes the matches are in the same order they are found...
    /// </summary>
    public static async IAsyncEnumerable<MatchResult> GetBestMatchesAsync(IAsyncEnumerable<MatchResult> matches, int maxDistanece)
    {
        var matchesEnumerator = matches.GetAsyncEnumerator();

        var group = new List<MatchResult>();

        if (await matchesEnumerator.MoveNextAsync())
        {
            group.Add(matchesEnumerator.Current);

            var matchStartIndex = matchesEnumerator.Current.StartIndex;

            while (await matchesEnumerator.MoveNextAsync())
            {
                var currentMatch = matchesEnumerator.Current;

                if (currentMatch != null)
                {
                    if (currentMatch.StartIndex > (matchStartIndex + maxDistanece))
                    {
                        yield return group.OrderBy(o => o.Distance).ThenByDescending(o => o.Match.Length).First();
                        group.Clear();
                    }

                    group.Add(currentMatch);

                    matchStartIndex = currentMatch.StartIndex;
                }
            }
        }

        if (group.Any())
        {
            yield return group.OrderBy(o => o.Distance).ThenByDescending(o => o.Match.Length).First();
        }
    }
}