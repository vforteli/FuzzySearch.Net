﻿namespace FuzzySearchNet;

public static class Utils
{
    /// <summary>
    /// Group matches and return best.
    /// Currently assumes the matches are in the same order they are found...
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public static IEnumerable<MatchResult> GetBestMatches(IEnumerable<MatchResult> matches, int maxDistanece)
    {
        var matchesEnumerator = matches.GetEnumerator();

        var group = new List<MatchResult>();

        if (matchesEnumerator.MoveNext())
        {
            group.Add(matchesEnumerator.Current);

            var match = matchesEnumerator.Current;

            while (matchesEnumerator.MoveNext())
            {
                var currentMatch = matchesEnumerator.Current;

                if (currentMatch != null)
                {
                    if (currentMatch.StartIndex > (match.StartIndex + maxDistanece))
                    {
                        yield return group.OrderBy(o => o.Distance).ThenByDescending(o => o.Match.Length).First();
                        group.Clear();
                    }

                    group.Add(currentMatch);

                    match = currentMatch;
                }
            }
        }

        if (group.Any())
        {
            yield return group.OrderBy(o => o.Distance).ThenByDescending(o => o.Match.Length).First();
        }
    }
}