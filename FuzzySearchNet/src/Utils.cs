using System.Runtime.CompilerServices;

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

        var group = new List<CandidateMatch>();

        if (matchesEnumerator.MoveNext())
        {
            group.Add(matchesEnumerator.Current);

            var matchStartIndex = matchesEnumerator.Current.StartIndex;

            while (matchesEnumerator.MoveNext())
            {
                if (matchesEnumerator.Current.StartIndex > (matchStartIndex + maxDistanece))
                {
                    yield return GetBestMatchFromGroup(group, text);
                    group.Clear();
                }

                group.Add(matchesEnumerator.Current);

                matchStartIndex = matchesEnumerator.Current.StartIndex;
            }
        }

        if (group.Any())
        {
            yield return GetBestMatchFromGroup(group, text);
        }
    }


    /// <summary>
    /// Group matches and return best.
    /// Currently assumes the matches are in the same order they are found...
    /// </summary>
    internal static async IAsyncEnumerable<MatchResult> GetBestMatchesAsync(IAsyncEnumerable<MatchResultWithValue> matches, int maxDistanece)
    {
        var matchesEnumerator = matches.GetAsyncEnumerator();

        var group = new List<MatchResultWithValue>();

        if (await matchesEnumerator.MoveNextAsync())
        {
            group.Add(matchesEnumerator.Current);

            var matchStartIndex = matchesEnumerator.Current.StartIndex;

            while (await matchesEnumerator.MoveNextAsync())
            {
                if (matchesEnumerator.Current.StartIndex > (matchStartIndex + maxDistanece))
                {
                    yield return GetBestMatchFromGroupWithValue(group);
                    group.Clear();
                }

                group.Add(matchesEnumerator.Current);

                matchStartIndex = matchesEnumerator.Current.StartIndex;
            }
        }

        if (group.Any())
        {
            yield return GetBestMatchFromGroupWithValue(group);
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static MatchResult GetBestMatchFromGroup(List<CandidateMatch> group, string text)
    {
        var bestMatch = group.First();

        foreach (var match in group.Skip(1))
        {
            if (match.Distance < bestMatch.Distance || match.Distance == bestMatch.Distance && (match.TextIndex - match.StartIndex) > (bestMatch.TextIndex - bestMatch.StartIndex))
            {
                bestMatch = match;
            }
        }

        return new MatchResult
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static MatchResult GetBestMatchFromGroupWithValue(List<MatchResultWithValue> group)
    {
        var bestMatch = group.First();

        foreach (var match in group.Skip(1))
        {
            if (match.Distance < bestMatch.Distance || match.Distance == bestMatch.Distance && (match.EndIndex - match.StartIndex) > (bestMatch.EndIndex - bestMatch.StartIndex))
            {
                bestMatch = match;
            }
        }

        return new MatchResult
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
