namespace FuzzySearchNet;

public partial class FuzzySearch
{
    /// <summary>
    /// Finds term in text with only substitutions
    /// </summary>     
    public static IEnumerable<MatchResult> FindSubstitutionsOnly(string subSequence, string text, int maxDistance, bool invariantCultureIgnoreCase = false)
    {
        if (string.IsNullOrEmpty(subSequence))
        {
            yield break;
        }

        subSequence = invariantCultureIgnoreCase ? subSequence.ToLowerInvariant() : subSequence;

        for (var currentIndex = 0; currentIndex < text.Length - (subSequence.Length - 1); currentIndex++)
        {
            var needlePosition = currentIndex;
            var candidateDistance = 0;

            for (var termIndex = 0; termIndex < subSequence.Length; termIndex++)
            {
                var match = invariantCultureIgnoreCase
                    ? char.ToLowerInvariant(text[needlePosition]) == subSequence[termIndex]
                    : text[needlePosition] == subSequence[termIndex];

                if (!match)
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
                yield return new MatchResult
                {
                    StartIndex = currentIndex,
                    EndIndex = currentIndex + subSequence.Length,
                    Distance = candidateDistance,
                    Match = text[currentIndex..(currentIndex + subSequence.Length)],
                    Deletions = 0,
                    Substitutions = candidateDistance,
                    Insertions = 0,
                };
            }
        }
    }


    /// <summary>
    /// Finds term in text with only substitutions from stream
    /// </summary>
    /// <param name="bufferSize">Default 4096. If bufferSize is less then maxdistance, it will become a multiple of bufferSize</param>
    public static async IAsyncEnumerable<MatchResult> FindSubstitutionsOnlyAsync(string subSequence, Stream textStream, int maxDistance, int bufferSize = 4096, bool invariantCultureIgnoreCase = false)
    {
        if (string.IsNullOrEmpty(subSequence))
        {
            yield break;
        }

        subSequence = invariantCultureIgnoreCase ? subSequence.ToLowerInvariant() : subSequence;

        var subSequenceLengthMinusOne = subSequence.Length - 1;

        bufferSize = ((subSequence.Length * 2 / bufferSize) + 1) * bufferSize;
        var buffer = new char[bufferSize];
        using var streamReader = new StreamReader(textStream);

        var streamIndexOffset = 0;

        var bytesRead = await streamReader.ReadBlockAsync(buffer, 0, buffer.Length);

        do
        {
            if (bytesRead < subSequence.Length)
            {
                // Cant have a match if subsequence is longer than text
                yield break;
            }

            for (var currentIndex = 0; currentIndex < bytesRead - subSequenceLengthMinusOne; currentIndex++)
            {
                var needlePosition = currentIndex;
                var candidateDistance = 0;

                for (var subSequenceIndex = 0; subSequenceIndex < subSequence.Length; subSequenceIndex++)
                {
                    var match = invariantCultureIgnoreCase
                        ? char.ToLowerInvariant(buffer[needlePosition]) == subSequence[subSequenceIndex]
                        : buffer[needlePosition] == subSequence[subSequenceIndex];

                    if (!match)
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
                    yield return new MatchResult
                    {
                        StartIndex = streamIndexOffset + currentIndex,
                        EndIndex = streamIndexOffset + currentIndex + subSequence.Length,
                        Distance = candidateDistance,
                        Match = new string(buffer[currentIndex..(currentIndex + subSequence.Length)]),
                        Deletions = 0,
                        Substitutions = candidateDistance,
                        Insertions = 0,
                    };
                }
            }


            streamIndexOffset += bytesRead - subSequenceLengthMinusOne;

            Array.Copy(buffer, buffer.Length - subSequenceLengthMinusOne, buffer, 0, subSequenceLengthMinusOne);
            bytesRead = await streamReader.ReadBlockAsync(buffer, subSequenceLengthMinusOne, buffer.Length - subSequenceLengthMinusOne);
            bytesRead += subSequenceLengthMinusOne;
        }
        while (bytesRead > subSequenceLengthMinusOne);
    }
}
