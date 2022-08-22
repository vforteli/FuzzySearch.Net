namespace FuzzySearchNet;

public class FuzzySearch
{
    /// <summary>
    /// Find instances of term in text up to maximum distance.
    /// </summary>
    /// <param name="term"></param>
    /// <param name="text"></param>
    /// <param name="maxDistance"></param>
    public static IEnumerable<MatchResult> Find(string term, string text, int maxDistance = 3, bool substitutionsOnly = true)  // todo change this to enum or something..
    {
        if (string.IsNullOrEmpty(term))
        {
            throw new ArgumentException("Term cannot be null", nameof(term));
        }

        if (maxDistance == 0)
        {
            return FindExact(term, text);
        }
        else if (substitutionsOnly)
        {
            return FindSubstitutionsOnlyBuffering(term, text, maxDistance);
        }
        else
        {
            return FindBuffering(term, text, maxDistance);
        }
    }


    /// <summary>
    /// Finds term in text with max distance 0, full match that is.
    /// </summary>
    /// <param name="term"></param>
    /// <param name="text"></param>    
    public static IEnumerable<MatchResult> FindExact(string term, string text)
    {
        var needlePosition = 0;
        var termLength = term.Length - 1;
        var currentIndex = 0;

        foreach (var currentCharacter in text)
        {
            if (currentCharacter == term[needlePosition])
            {
                if (needlePosition == termLength)
                {
                    yield return new MatchResult(currentIndex - termLength, currentIndex + 1, 0, term, 0, 0, 0);
                    needlePosition = 0;
                }
                else
                {
                    needlePosition++;
                }
            }
            else if (currentCharacter == term[0])
            {
                needlePosition = 1;
            }
            else
            {
                needlePosition = 0;
            }

            currentIndex++;
        }
    }


    /// <summary>
    /// Finds term in text with only substitutions
    /// </summary>
    /// <param name="term"></param>
    /// <param name="text"></param>    
    public static IEnumerable<MatchResult> FindSubstitutionsOnlyBuffering(string term, string text, int maxDistance)
    {
        var matches = new List<MatchResult>();

        var needlePosition = 0;
        var termLengthMinusOne = term.Length - 1;
        var termLength = term.Length;
        var candidateDistance = 0;
        var termIndex = 0;
        var textStringLength = text.Length;

        for (var currentIndex = 0; currentIndex < textStringLength - termLengthMinusOne; currentIndex++)
        {
            needlePosition = currentIndex;
            candidateDistance = 0;

            for (termIndex = 0; termIndex < termLength; termIndex++)
            {
                if (text[needlePosition] != term[termIndex])
                {
                    candidateDistance++;
                    if (candidateDistance > maxDistance)
                    {
                        break;
                    }
                }

                needlePosition++;
            }

            if (candidateDistance <= maxDistance)
            {
                matches.Add(new MatchResult(currentIndex, currentIndex + termLength, candidateDistance, text.Substring(currentIndex, termLength), 0, candidateDistance, 0));
            }
        }

        return matches;
    }


    /// <summary>
    /// Finds term in text with max distance
    /// </summary>
    /// <param name="term"></param>
    /// <param name="text"></param>    
    public static IEnumerable<MatchResult> FindBuffering(string term, string text, int maxDistance)
    {
        var matches = new List<MatchResult>();

        var termLengthMinusOne = term.Length - 1;
        var termLength = term.Length;
        var textStringLength = text.Length;

        var candidates = new Stack<CandidateMatch>();

        for (var currentIndex = 0; currentIndex <= textStringLength - termLengthMinusOne; currentIndex++)
        {
            candidates.Push(new CandidateMatch(currentIndex, currentIndex, 0, 0, 0, 0, 0));

            // Keep track of the best distance so far, this means we can ignore candidates with higher distance if we already have a match
            var bestFoundDistance = maxDistance;
            while (candidates.TryPop(out var candidate))
            {


                if (candidate.PatternIndex == termLength && candidate.Distance <= bestFoundDistance)
                {
                    matches.Add(new MatchResult(candidate.StartIndex, candidate.TextIndex, candidate.Distance, text[candidate.StartIndex..candidate.TextIndex], candidate.Deletions, candidate.Substitutions, candidate.Insertions));
                    bestFoundDistance = candidate.Distance;

                    // No point searching for better matches if we find a perfect match
                    if (candidate.Distance == 0)
                    {
                        candidates.Clear();
                        break;
                    }

                    continue;
                }

                if (candidate.TextIndex == textStringLength)
                {
                    continue;
                }


                if (text[candidate.TextIndex] == term[candidate.PatternIndex])
                {
                    candidates.Push(new CandidateMatch(candidate.StartIndex, candidate.TextIndex + 1, candidate.PatternIndex + 1, candidate.Distance, candidate.Deletions, candidate.Substitutions, candidate.Insertions));
                    if (candidate.Distance < bestFoundDistance)
                    {
                        candidates.Push(candidate with
                        {
                            PatternIndex = candidate.PatternIndex + 1,
                            Distance = candidate.Distance + 1,
                            Deletions = candidate.Deletions + 1,
                        });

                        candidates.Push(candidate with
                        {
                            TextIndex = candidate.TextIndex + 1,
                            Distance = candidate.Distance + 1,
                            Insertions = candidate.Insertions + 1,
                        });
                    }
                }
                else
                {
                    if (candidate.Distance < bestFoundDistance)
                    {
                        candidates.Push(candidate with
                        {
                            TextIndex = candidate.TextIndex + 1,
                            PatternIndex = candidate.PatternIndex + 1,
                            Distance = candidate.Distance + 1,
                            Substitutions = candidate.Substitutions + 1,
                        });

                        candidates.Push(candidate with
                        {
                            PatternIndex = candidate.PatternIndex + 1,
                            Distance = candidate.Distance + 1,
                            Deletions = candidate.Deletions + 1,
                        });

                        candidates.Push(candidate with
                        {
                            TextIndex = candidate.TextIndex + 1,
                            Distance = candidate.Distance + 1,
                            Insertions = candidate.Insertions + 1,
                        });
                    }
                }
            }
        }

        // todo figure out some sane way of removing overlapping matches...
        // this is just here to make tests green... then we can optimize
        //var derp = matches.Distinct().OrderBy(o => o.Distance).ToList();

        matches = matches.Distinct().ToList();

        if (matches.Count > 1)
        {
            var groups = new List<List<MatchResult>>();

            groups.Add(new List<MatchResult>());

            var match = matches[0];
            groups[0].Add(match);

            for (var i = 0; i < matches.Count - 1; i++)
            {
                var currentMatch = matches[i];
                if ((currentMatch.StartIndex + currentMatch.Insertions) >= (match.EndIndex - match.Insertions))
                {
                    groups.Add(new List<MatchResult>());
                }

                groups.Last().Add(currentMatch);

                match = currentMatch;
            }

            var foo = groups.Select(o => o.OrderBy(o => o.Distance).ThenByDescending(o => o.Match.Length).First()).ToList();
            return foo;
        }
        else
        {
            return matches;
        }
    }
}

public record struct CandidateMatch(int StartIndex, int TextIndex, int PatternIndex, int Distance, int Deletions, int Substitutions, int Insertions);