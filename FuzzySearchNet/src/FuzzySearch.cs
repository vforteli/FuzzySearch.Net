namespace FuzzySearchNet;

public class FuzzySearch
{
    /// <summary>
    /// Find instances of sub sequence in text up to default maximum distance 3.
    /// </summary>
    /// <param name="subSequence"></param>
    /// <param name="text"></param>    
    public static IEnumerable<MatchResult> Find(string subSequence, string text) => Find(subSequence, text, 3, SearchOptions.None);


    /// <summary>
    /// Find instances of sub sequence in text up to default maximum distance 3 and search mode.
    /// </summary>
    /// <param name="subSequence"></param>
    /// <param name="text"></param>    
    /// <param name="searchMode"></param>
    public static IEnumerable<MatchResult> Find(string subSequence, string text, SearchOptions searchMode) => Find(subSequence, text, 3, searchMode);


    /// <summary>
    /// Find instances of sub sequence in text up to maximum maximum distance.
    /// </summary>
    /// <param name="subSequence"></param>
    /// <param name="text"></param>    
    /// <param name="searchMode"></param>
    public static IEnumerable<MatchResult> Find(string subSequence, string text, int maxDistance) => Find(subSequence, text, maxDistance, SearchOptions.None);


    /// <summary>
    /// Find instances of sub sequence in text up to maximum distance.
    /// </summary>
    /// <param name="subSequence"></param>
    /// <param name="text"></param>
    /// <param name="maxDistance"></param>
    public static IEnumerable<MatchResult> Find(string subSequence, string text, int maxDistance, SearchOptions searchMode)
    {
        if (string.IsNullOrEmpty(subSequence))
        {
            return new List<MatchResult>();
        }

        if (maxDistance == 0)
        {
            return FindExact(subSequence, text);
        }
        else if (searchMode == SearchOptions.SubstitutionsOnly)
        {
            return FindSubstitutionsOnlyBuffering(subSequence, text, maxDistance);
        }
        else
        {
            return FindBuffering(subSequence, text, maxDistance);
        }
    }


    /// <summary>
    /// Finds term in text with max distance 0, full match that is.
    /// </summary>
    /// <param name="subSequence"></param>
    /// <param name="text"></param>    
    public static IEnumerable<MatchResult> FindExact(string subSequence, string text)
    {
        // ok so this whole method is a bit redundant... but the idea is to have this using a stream instead of text... later
        var needlePosition = 0;
        var termLength = subSequence.Length - 1;
        var currentIndex = 0;

        foreach (var currentCharacter in text)
        {
            if (currentCharacter == subSequence[needlePosition])
            {
                if (needlePosition == termLength)
                {
                    yield return new MatchResult
                    {
                        StartIndex = currentIndex - termLength,
                        EndIndex = currentIndex + 1,
                        Distance = 0,
                        Match = subSequence,
                        Deletions = 0,
                        Substitutions = 0,
                        Insertions = 0,
                    };

                    needlePosition = 0;
                }
                else
                {
                    needlePosition++;
                }
            }
            else
            {
                needlePosition = currentCharacter == subSequence[0] ? 1 : 0;
            }

            currentIndex++;
        }
    }


    /// <summary>
    /// Finds term in text with max distance 0, full match that is from stream
    /// </summary>
    /// <param name="subSequence"></param>
    /// <param name="text"></param>    
    public static async Task<IEnumerable<MatchResult>> FindExactAsync(string subSequence, Stream textStream, int bufferSize = 4096)
    {
        var matches = new List<MatchResult>();

        var needlePosition = 0;
        var termLength = subSequence.Length - 1;

        var buffer = new char[bufferSize];
        using var streamReader = new StreamReader(textStream);

        var streamIndexOffset = 0;

        while (!streamReader.EndOfStream)
        {
            var bytesRead = await streamReader.ReadBlockAsync(buffer, 0, buffer.Length);

            for (var currentIndex = 0; currentIndex < bytesRead; currentIndex++)
            {
                if (buffer[currentIndex] == subSequence[needlePosition])
                {
                    if (needlePosition == termLength)
                    {
                        matches.Add(new MatchResult
                        {
                            StartIndex = streamIndexOffset + currentIndex - termLength,
                            EndIndex = streamIndexOffset + currentIndex + 1,
                            Distance = 0,
                            Match = subSequence,
                            Deletions = 0,
                            Substitutions = 0,
                            Insertions = 0,
                        });

                        needlePosition = 0;
                    }
                    else
                    {
                        needlePosition++;
                    }
                }
                else
                {
                    needlePosition = buffer[currentIndex] == subSequence[0] ? 1 : 0;
                }
            }

            streamIndexOffset += bytesRead;
        }

        return matches;
    }


    /// <summary>
    /// Finds term in text with only substitutions
    /// </summary>
    /// <param name="subSequence"></param>
    /// <param name="text"></param>    
    public static IEnumerable<MatchResult> FindSubstitutionsOnlyBuffering(string subSequence, string text, int maxDistance)
    {
        var matches = new List<MatchResult>();

        var needlePosition = 0;
        var termLengthMinusOne = subSequence.Length - 1;
        var termLength = subSequence.Length;
        var candidateDistance = 0;
        var termIndex = 0;
        var textStringLength = text.Length;

        for (var currentIndex = 0; currentIndex < textStringLength - termLengthMinusOne; currentIndex++)
        {
            needlePosition = currentIndex;
            candidateDistance = 0;

            for (termIndex = 0; termIndex < termLength; termIndex++)
            {
                if (text[needlePosition] != subSequence[termIndex])
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
                matches.Add(new MatchResult
                {
                    StartIndex = currentIndex,
                    EndIndex = currentIndex + termLength,
                    Distance = candidateDistance,
                    Match = text[currentIndex..(currentIndex + termLength)],
                    Deletions = 0,
                    Substitutions = candidateDistance,
                    Insertions = 0,
                });
            }
        }

        return matches;
    }


    /// <summary>
    /// Finds term in text with max distance
    /// </summary>
    /// <param name="subSequence"></param>
    /// <param name="text"></param>    
    public static IEnumerable<MatchResult> FindBuffering(string subSequence, string text, int maxDistance)
    {
        var matches = new List<MatchResult>();
        var candidates = new Stack<CandidateMatch>();

        for (var currentIndex = 0; currentIndex <= text.Length - (subSequence.Length - 1); currentIndex++)
        {
            candidates.Push(new CandidateMatch(currentIndex, currentIndex, 0, 0, 0, 0, 0));

            // Keep track of the best distance so far, this means we can ignore candidates with higher distance if we already have a match
            var bestFoundDistance = maxDistance;

            while (candidates.TryPop(out var candidate))
            {
                if (candidate.PatternIndex == subSequence.Length && candidate.Distance <= bestFoundDistance)
                {
                    matches.Add(new MatchResult
                    {
                        StartIndex = candidate.StartIndex,
                        EndIndex = candidate.TextIndex,
                        Distance = candidate.Distance,
                        Match = text[candidate.StartIndex..candidate.TextIndex],
                        Deletions = candidate.Deletions,
                        Substitutions = candidate.Substitutions,
                        Insertions = candidate.Insertions,
                    });

                    bestFoundDistance = candidate.Distance;

                    // No point searching for better matches if we find a perfect match
                    if (candidate.Distance == 0)
                    {
                        candidates.Clear();
                        break;
                    }

                    continue;
                }

                if (candidate.TextIndex == text.Length)
                {
                    continue;
                }

                if (text[candidate.TextIndex] == subSequence[candidate.PatternIndex])
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

            return groups.Select(o => o.OrderBy(o => o.Distance).ThenByDescending(o => o.Match.Length).First()).ToList();
        }
        else
        {
            return matches;
        }
    }
}
