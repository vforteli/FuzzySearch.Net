namespace FuzzySearchNet;

public partial class FuzzySearch
{
    /// <summary>
    /// Finds term in text with max distance 0, full match that is.
    /// </summary>
    /// <param name="subSequence"></param>
    /// <param name="text"></param>    
    public static IEnumerable<MatchResult> FindExact(string subSequence, string text, bool invariantCultureIgnoreCase = false)
    {
        if (string.IsNullOrEmpty(subSequence))
        {
            yield break;
        }

        subSequence = invariantCultureIgnoreCase ? subSequence.ToLowerInvariant() : subSequence;

        // indexof would probably run circles around this...
        var needlePosition = 0;
        var termLength = subSequence.Length - 1;
        var currentIndex = 0;

        foreach (var currentCharacter in text)
        {
            if ((invariantCultureIgnoreCase ? char.ToLowerInvariant(currentCharacter) : currentCharacter) == subSequence[needlePosition])
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
    public static async IAsyncEnumerable<MatchResult> FindExactAsync(string subSequence, Stream textStream, int bufferSize = 4096, bool invariantCultureIgnoreCase = false)
    {
        if (string.IsNullOrEmpty(subSequence))
        {
            yield break;
        }

        subSequence = invariantCultureIgnoreCase ? subSequence.ToLowerInvariant() : subSequence;

        var needlePosition = 0;
        var termLength = subSequence.Length - 1;

        var buffer = new char[bufferSize];
        using var streamReader = new StreamReader(textStream);

        var streamIndexOffset = 0;

        var bytesRead = await streamReader.ReadBlockAsync(buffer, 0, buffer.Length);

        do
        {
            for (var currentIndex = 0; currentIndex < bytesRead; currentIndex++)
            {
                if ((invariantCultureIgnoreCase ? char.ToLowerInvariant(buffer[currentIndex]) : buffer[currentIndex]) == subSequence[needlePosition])
                {
                    if (needlePosition == termLength)
                    {
                        yield return new MatchResult
                        {
                            StartIndex = streamIndexOffset + currentIndex - termLength,
                            EndIndex = streamIndexOffset + currentIndex + 1,
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
                    needlePosition = buffer[currentIndex] == subSequence[0] ? 1 : 0;
                }
            }

            streamIndexOffset += bytesRead;

            bytesRead = await streamReader.ReadBlockAsync(buffer, 0, buffer.Length);
        }
        while (bytesRead > 0);
    }
}
