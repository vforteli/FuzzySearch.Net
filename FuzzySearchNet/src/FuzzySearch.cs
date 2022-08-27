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
            return FindSubstitutionsOnly(subSequence, text, maxDistance);
        }
        else
        {
            return FindLevenshtein(subSequence, text, maxDistance);
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
    public static IEnumerable<MatchResult> FindSubstitutionsOnly(string subSequence, string text, int maxDistance)
    {
        var matches = new List<MatchResult>();
        var termLengthMinusOne = subSequence.Length - 1;

        for (var currentIndex = 0; currentIndex < text.Length - termLengthMinusOne; currentIndex++)
        {
            var needlePosition = currentIndex;
            var candidateDistance = 0;

            for (var termIndex = 0; termIndex < subSequence.Length; termIndex++)
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
                    EndIndex = currentIndex + subSequence.Length,
                    Distance = candidateDistance,
                    Match = text[currentIndex..(currentIndex + subSequence.Length)],
                    Deletions = 0,
                    Substitutions = candidateDistance,
                    Insertions = 0,
                });
            }
        }

        return matches;
    }


    /// <summary>
    /// Finds term in text with only substitutions from stream
    /// </summary>
    /// <param name="subSequence"></param>    
    /// <param name="textStream"></param>
    /// <param name="maxDistance"></param>
    /// <param name="bufferSize">Default 4096. If bufferSize is less then maxdistance, it will become a multiple of bufferSize</param>
    public static async Task<IEnumerable<MatchResult>> FindSubstitutionsOnlyAsync(string subSequence, Stream textStream, int maxDistance, int bufferSize = 4096)
    {
        var matches = new List<MatchResult>();
        var termLengthMinusOne = subSequence.Length - 1;

        bufferSize = (((subSequence.Length * 2) / bufferSize) + 1) * bufferSize;
        var buffer = new char[bufferSize];
        using var streamReader = new StreamReader(textStream);

        var streamIndexOffset = 0;

        await streamReader.ReadBlockAsync(buffer, 0, termLengthMinusOne);

        while (!streamReader.EndOfStream)
        {
            var bytesRead = await streamReader.ReadBlockAsync(buffer, termLengthMinusOne, buffer.Length - termLengthMinusOne);

            for (var currentIndex = 0; currentIndex < buffer.Length - termLengthMinusOne; currentIndex++)
            {
                var needlePosition = currentIndex;
                var candidateDistance = 0;

                for (var termIndex = 0; termIndex < subSequence.Length; termIndex++)
                {
                    if (buffer[needlePosition] != subSequence[termIndex])
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
                        StartIndex = streamIndexOffset + currentIndex,
                        EndIndex = streamIndexOffset + currentIndex + subSequence.Length,
                        Distance = candidateDistance,
                        Match = new string(buffer[currentIndex..(currentIndex + subSequence.Length)]),
                        Deletions = 0,
                        Substitutions = candidateDistance,
                        Insertions = 0,
                    });
                }
            }

            streamIndexOffset += bytesRead;

            // We have to overlap with the next buffer to ensure matches spanning multiple "chunks" can be read
            Array.Copy(buffer, bytesRead, buffer, 0, termLengthMinusOne);
        }

        return matches;
    }


    /// <summary>
    /// Finds term in text with max distance
    /// </summary>
    /// <param name="subSequence"></param>
    /// <param name="text"></param>    
    public static IEnumerable<MatchResult> FindLevenshtein(string subSequence, string text, int maxDistance)
    {
        var matches = new List<MatchResult>();
        var candidates = new Stack<CandidateMatch>();

        for (var currentIndex = 0; currentIndex < text.Length; currentIndex++)
        {
            candidates.Push(new CandidateMatch(currentIndex, currentIndex));

            // Keep track of the best distance so far, this means we can ignore candidates with higher distance if we already have a match
            var bestFoundDistance = maxDistance;

            while (candidates.TryPop(out var candidate))
            {
                if (candidate.SubSequenceIndex == subSequence.Length)
                {
                    if (candidate.TextIndex <= text.Length)
                    {
                        bestFoundDistance = candidate.Distance;
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
                    }

                    // No point searching for better matches if we find a perfect match
                    if (candidate.Distance == 0)
                    {
                        candidates.Clear();
                        break;
                    }

                    continue;
                }

                if (candidate.SubSequenceIndex < subSequence.Length && candidate.TextIndex < text.Length && text[candidate.TextIndex] == subSequence[candidate.SubSequenceIndex])
                {
                    // match
                    candidates.Push(candidate with
                    {
                        Position = candidate.Position + 1,
                        TextIndex = candidate.TextIndex + 1,
                        SubSequenceIndex = candidate.SubSequenceIndex + 1,
                    });

                    if (candidate.Distance < bestFoundDistance)
                    {
                        // jump over one character in text
                        candidates.Push(candidate with
                        {
                            Insertions = candidate.Insertions + 1,
                            Distance = candidate.Distance + 1,
                            Position = candidate.Position + 2,
                            SubSequenceIndex = candidate.SubSequenceIndex + 1,
                            TextIndex = candidate.TextIndex + 2,
                            Offset = candidate.Offset - 1,
                        });
                    }
                }
                else if (candidate.Distance < bestFoundDistance)
                {
                    // substitute one character
                    candidates.Push(candidate with
                    {
                        Substitutions = candidate.Substitutions + 1,
                        Distance = candidate.Distance + 1,
                        Position = candidate.Position + 1,
                        TextIndex = candidate.TextIndex + 1,
                        SubSequenceIndex = candidate.SubSequenceIndex + 1,
                    });

                    // jump over one character in subsequence
                    candidates.Push(candidate with
                    {
                        Deletions = candidate.Deletions + 1,
                        Distance = candidate.Distance + 1,
                        Offset = candidate.Offset + 1,
                        SubSequenceIndex = candidate.SubSequenceIndex + 1,
                    });
                }
            }
        }

        return Utils.GetBestMatches(matches.OrderBy(o => o.StartIndex).ToList(), maxDistance);
    }
}
