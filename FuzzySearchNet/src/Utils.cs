namespace FuzzySearchNet;

public static class Utils
{
    /// <summary>
    /// Group matches and return best.
    /// Currently assumes the matches are in the same order they are found...
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public static IEnumerable<MatchResult> GetBestMatches(List<MatchResult> list, int maxDistanece)
    {
        var matches = list.Distinct().ToList();

        if (matches.Count > 1)
        {
            var groups = new List<List<MatchResult>>();

            groups.Add(new List<MatchResult>());

            var match = matches[0];
            groups[0].Add(match);

            for (var i = 1; i < matches.Count; i++)
            {
                var currentMatch = matches[i];

                if (currentMatch.StartIndex > (match.StartIndex + maxDistanece))
                {
                    groups.Add(new List<MatchResult>());
                }

                groups.Last().Add(currentMatch);

                match = currentMatch;
            }

            return groups.Select(o => o.OrderBy(o => o.Distance).ThenByDescending(o => o.Match.Length).First()).ToList();
        }
        else
        {
            return matches;
        }
    }
}