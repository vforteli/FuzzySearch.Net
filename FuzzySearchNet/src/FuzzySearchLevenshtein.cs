namespace FuzzySearchNet;

public partial class FuzzySearch
{
    /// <summary>
    /// Finds sub sequence in text with max levenshtein distance
    /// </summary>
    /// <param name="subSequence"></param>
    /// <param name="text"></param>    
    public static IEnumerable<MatchResult> FindLevenshtein(string subSequence, string text, FuzzySearchOptions options) => Utils.GetBestMatches(FindLevenshteinAll(subSequence, text, options), options.MaxTotalDistance);


    /// <summary>
    /// Finds sub sequence in text with max levenshtein distance
    /// This method finds all matches and does not try to consolidate overlapping matches
    /// </summary>       
    internal static IEnumerable<MatchResult> FindLevenshteinAll(string subSequence, string text, FuzzySearchOptions options)
    {
        if (string.IsNullOrEmpty(subSequence))
        {
            yield break;
        }

        var candidates = new Stack<CandidateMatch>();

        for (var currentIndex = 0; currentIndex < text.Length; currentIndex++)
        {
            candidates.Push(new CandidateMatch(currentIndex, currentIndex));

            // Keep track of the best distance so far, this means we can ignore candidates with higher distance if we already have a match
            var bestFoundDistance = options.MaxTotalDistance;

            while (candidates.TryPop(out var candidate))
            {
                if (candidate.SubSequenceIndex == subSequence.Length)
                {
                    if (candidate.TextIndex <= text.Length)
                    {
                        bestFoundDistance = candidate.Distance;
                        yield return new MatchResult
                        {
                            StartIndex = candidate.StartIndex,
                            EndIndex = candidate.TextIndex,
                            Distance = candidate.Distance,
                            Match = text[candidate.StartIndex..candidate.TextIndex],
                            Deletions = candidate.Deletions,
                            Substitutions = candidate.Substitutions,
                            Insertions = candidate.Insertions,
                        };
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
                        TextIndex = candidate.TextIndex + 1,
                        SubSequenceIndex = candidate.SubSequenceIndex + 1,
                    });

                    if (candidate.Distance < bestFoundDistance && options.CanInsert(candidate.Distance, candidate.Insertions))
                    {
                        // jump over one character in text
                        candidates.Push(candidate with
                        {
                            Insertions = candidate.Insertions + 1,
                            Distance = candidate.Distance + 1,
                            SubSequenceIndex = candidate.SubSequenceIndex + 1,
                            TextIndex = candidate.TextIndex + 2,
                        });
                    }
                }
                else if (candidate.Distance < bestFoundDistance)
                {
                    if (options.CanSubstitute(candidate.Distance, candidate.Substitutions))
                    {
                        // substitute one character
                        candidates.Push(candidate with
                        {
                            Substitutions = candidate.Substitutions + 1,
                            Distance = candidate.Distance + 1,
                            TextIndex = candidate.TextIndex + 1,
                            SubSequenceIndex = candidate.SubSequenceIndex + 1,
                        });
                    }

                    if (options.CanDelete(candidate.Distance, candidate.Deletions))
                    {
                        // jump over one character in subsequence
                        candidates.Push(candidate with
                        {
                            Deletions = candidate.Deletions + 1,
                            Distance = candidate.Distance + 1,
                            SubSequenceIndex = candidate.SubSequenceIndex + 1,
                        });
                    }
                }
            }
        }
    }


    /// <summary>
    /// Finds sub sequence in text with max levenshtein distance
    /// </summary>     
    public static IAsyncEnumerable<MatchResult> FindLevenshteinAsync(string subSequence, Stream textStream, FuzzySearchOptions options, int bufferSize = 4096) => Utils.GetBestMatchesAsync(FindLevenshteinAllAsync(subSequence, textStream, options, bufferSize), options.MaxTotalDistance);


    /// <summary>
    /// Finds sub sequence in text with max levenshtein distance
    /// This method finds all matches and does not try to consolidate overlapping matches
    /// </summary>    
    internal static async IAsyncEnumerable<MatchResult> FindLevenshteinAllAsync(string subSequence, Stream textStream, FuzzySearchOptions options, int bufferSize = 4096)
    {
        if (subSequence.Length > 0)
        {
            var candidates = new Stack<CandidateMatch>();

            var startBuffer = subSequence.Length + options.MaxTotalDistance;
            bufferSize = ((startBuffer * 2 / bufferSize) + 1) * bufferSize;
            var buffer = new char[bufferSize];
            using var streamReader = new StreamReader(textStream);

            var streamIndexOffset = 0;

            var bytesRead = await streamReader.ReadBlockAsync(buffer, 0, buffer.Length);

            do
            {
                for (var currentIndex = 0; currentIndex < bytesRead; currentIndex++)
                {
                    candidates.Push(new CandidateMatch(currentIndex, currentIndex));

                    // Keep track of the best distance so far, this means we can ignore candidates with higher distance if we already have a match
                    var bestFoundDistance = options.MaxTotalDistance;

                    while (candidates.TryPop(out var candidate))
                    {
                        if (candidate.SubSequenceIndex == subSequence.Length)
                        {
                            if (candidate.TextIndex <= bytesRead)
                            {
                                bestFoundDistance = candidate.Distance;
                                yield return new MatchResult
                                {
                                    StartIndex = streamIndexOffset + candidate.StartIndex,
                                    EndIndex = streamIndexOffset + candidate.TextIndex,
                                    Distance = candidate.Distance,
                                    Match = new string(buffer[candidate.StartIndex..candidate.TextIndex]),
                                    Deletions = candidate.Deletions,
                                    Substitutions = candidate.Substitutions,
                                    Insertions = candidate.Insertions,
                                };
                            }

                            // No point searching for better matches if we find a perfect match
                            if (candidate.Distance == 0)
                            {
                                candidates.Clear();
                                break;
                            }

                            continue;
                        }

                        if (candidate.SubSequenceIndex < subSequence.Length && candidate.TextIndex < bytesRead && buffer[candidate.TextIndex] == subSequence[candidate.SubSequenceIndex])
                        {
                            // match
                            candidates.Push(candidate with
                            {
                                TextIndex = candidate.TextIndex + 1,
                                SubSequenceIndex = candidate.SubSequenceIndex + 1,
                            });

                            if (candidate.Distance < bestFoundDistance && options.CanInsert(candidate.Distance, candidate.Insertions))
                            {
                                // jump over one character in text
                                candidates.Push(candidate with
                                {
                                    Insertions = candidate.Insertions + 1,
                                    Distance = candidate.Distance + 1,
                                    SubSequenceIndex = candidate.SubSequenceIndex + 1,
                                    TextIndex = candidate.TextIndex + 2,
                                });
                            }
                        }
                        else if (candidate.Distance < bestFoundDistance)
                        {
                            if (options.CanSubstitute(candidate.Distance, candidate.Substitutions))
                            {
                                // substitute one character
                                candidates.Push(candidate with
                                {
                                    Substitutions = candidate.Substitutions + 1,
                                    Distance = candidate.Distance + 1,
                                    TextIndex = candidate.TextIndex + 1,
                                    SubSequenceIndex = candidate.SubSequenceIndex + 1,
                                });
                            }

                            if (options.CanDelete(candidate.Distance, candidate.Deletions))
                            {
                                // jump over one character in subsequence
                                candidates.Push(candidate with
                                {
                                    Deletions = candidate.Deletions + 1,
                                    Distance = candidate.Distance + 1,
                                    SubSequenceIndex = candidate.SubSequenceIndex + 1,
                                });
                            }
                        }
                    }
                }

                streamIndexOffset += bytesRead - startBuffer;

                Array.Copy(buffer, buffer.Length - startBuffer, buffer, 0, startBuffer);
                bytesRead = await streamReader.ReadBlockAsync(buffer, startBuffer, buffer.Length - startBuffer);
                bytesRead += startBuffer;
            }
            while (bytesRead > startBuffer);
        }
    }
}
