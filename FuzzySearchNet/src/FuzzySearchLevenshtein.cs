using System.Runtime.CompilerServices;

namespace FuzzySearchNet;

public partial class FuzzySearch
{
    /// <summary>
    /// Finds sub sequence in text with max levenshtein distance
    /// </summary>
    /// <param name="subSequence"></param>
    /// <param name="text"></param>    
    public static IEnumerable<MatchResult> FindLevenshtein(string subSequence, string text, FuzzySearchOptions options) =>
        Utils.GetBestMatches(text, FindLevenshteinAll(subSequence, text, options), options.MaxTotalDistance);


    /// <summary>
    /// Finds sub sequence in text with max levenshtein distance
    /// This method finds all matches and does not try to consolidate overlapping matches
    /// </summary>       
    internal static IEnumerable<CandidateMatch> FindLevenshteinAll(string subSequence, string text, FuzzySearchOptions options)
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
                        yield return candidate;
                    }

                    // No point searching for better matches if we find a perfect match
                    if (candidate.Distance == 0)
                    {
                        candidates.Clear();
                        break;
                    }

                    continue;
                }

                HandleCandidate(candidates, candidate, text, subSequence, bestFoundDistance, options, text.Length);
            }
        }
    }


    /// <summary>
    /// Finds sub sequence in text with max levenshtein distance
    /// </summary>     
    public static IAsyncEnumerable<MatchResult> FindLevenshteinAsync(string subSequence, Stream textStream, FuzzySearchOptions options, int bufferSize = 4096, bool leaveOpen = false) =>
        Utils.GetBestMatchesAsync(FindLevenshteinAllAsync(subSequence, textStream, options, bufferSize, leaveOpen), options.MaxTotalDistance);


    /// <summary>
    /// Finds sub sequence in text with max levenshtein distance
    /// This method finds all matches and does not try to consolidate overlapping matches
    /// </summary>    
    internal static async IAsyncEnumerable<MatchResultWithValue> FindLevenshteinAllAsync(string subSequence, Stream textStream, FuzzySearchOptions options, int bufferSize, bool leaveOpen = false)
    {
        if (string.IsNullOrEmpty(subSequence))
        {
            yield break;
        }

        var candidates = new Stack<CandidateMatch>();

        var startBuffer = subSequence.Length + options.MaxTotalDistance;
        bufferSize = ((startBuffer * 2 / bufferSize) + 1) * bufferSize;
        var buffer = new char[bufferSize];
        using var streamReader = new StreamReader(textStream, null, true, -1, leaveOpen);

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
                            yield return new MatchResultWithValue
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

                    HandleCandidate(candidates, candidate, buffer, subSequence, bestFoundDistance, options, bytesRead);
                }
            }

            streamIndexOffset += bytesRead - startBuffer;

            // basically stride to ensure matches spanning chunks are handled correctly
            Array.Copy(buffer, buffer.Length - startBuffer, buffer, 0, startBuffer);
            bytesRead = await streamReader.ReadBlockAsync(buffer, startBuffer, buffer.Length - startBuffer);
            bytesRead += startBuffer;
        }
        while (bytesRead > startBuffer);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void HandleCandidate(Stack<CandidateMatch> candidates, in CandidateMatch candidate, ReadOnlySpan<char> text, string subSequence, int bestFoundDistance, FuzzySearchOptions options, int textLength)
    {
        if (candidate.TextIndex < textLength && text[candidate.TextIndex] == subSequence[candidate.SubSequenceIndex])
        {
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

            // match                   
            candidates.Push(candidate with
            {
                TextIndex = candidate.TextIndex + 1,
                SubSequenceIndex = candidate.SubSequenceIndex + 1,
            });
        }
        else if (candidate.Distance < bestFoundDistance)
        {
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
        }
    }
}
