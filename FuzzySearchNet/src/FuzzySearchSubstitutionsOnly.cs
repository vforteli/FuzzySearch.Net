namespace FuzzySearchNet;

public partial class FuzzySearch
{
    /// <summary>
    /// Finds term in text with only substitutions
    /// </summary>     
    public static IEnumerable<MatchResult> FindSubstitutionsOnly(string subSequence, string text, int maxDistance)
    {
        if (string.IsNullOrEmpty(subSequence))
        {
            yield break;
        }

        for (var currentIndex = 0; currentIndex < text.Length - (subSequence.Length - 1); currentIndex++)
        {
            var candidateDistance = 0;

            foreach (var (textChar, patternChar) in text[currentIndex..(currentIndex + subSequence.Length)].Zip(subSequence, (f, s) => (f, s)))
            {
                if (textChar != patternChar)
                {
                    candidateDistance++;
                    if (candidateDistance > maxDistance)
                    {
                        break;
                    }
                }
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
    public static async IAsyncEnumerable<MatchResult> FindSubstitutionsOnlyAsync(string subSequence, Stream textStream, int maxDistance, int bufferSize = 4096)
    {
        if (string.IsNullOrEmpty(subSequence))
        {
            yield break;
        }

        var subSequenceLengthMinusOne = subSequence.Length - 1;

        bufferSize = ((subSequence.Length * 2 / bufferSize) + 1) * bufferSize;
        var buffer = new char[bufferSize];
        using var streamReader = new StreamReader(textStream);

        var streamIndexOffset = 0;

        var bytesRead = await streamReader.ReadBlockAsync(buffer, 0, buffer.Length);

        do
        {
            for (var currentIndex = 0; currentIndex < bytesRead - subSequenceLengthMinusOne; currentIndex++)
            {
                var candidateDistance = 0;

                foreach (var (textChar, patternChar) in buffer[currentIndex..(currentIndex + subSequence.Length)].Zip(subSequence, (f, s) => (f, s)))
                {
                    if (textChar != patternChar)
                    {
                        candidateDistance++;
                        if (candidateDistance > maxDistance)
                        {
                            break;
                        }
                    }
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
