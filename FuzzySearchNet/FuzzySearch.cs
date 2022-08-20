namespace FuzzySearchNet;

public class FuzzySearch
{
    /// <summary>
    /// Find instances of term in text up to maximum distance.
    /// </summary>
    /// <param name="term"></param>
    /// <param name="text"></param>
    /// <param name="maxDistance"></param>
    public static async Task<IEnumerable<MatchResult>> FindAsync(string term, Stream text, int maxDistance = 3)
    {
        await Task.CompletedTask;
        if (string.IsNullOrEmpty(term))
        {
            throw new ArgumentException("Term cannot be null", nameof(term));
        }

        if (maxDistance != 0)
        {
            return await FindSubstitutionsOnlyBuffering(term, text, maxDistance);
        }
        else
        {
            return FindZeroDistance(term, text);
        }
    }


    /// <summary>
    /// Finds term in text with max distance 0, full match that is.
    /// </summary>
    /// <param name="term"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    internal static IEnumerable<MatchResult> FindZeroDistance(string term, Stream text)
    {
        using var streamReader = new StreamReader(text);

        var needlePosition = 0;
        var termLength = term.Length - 1;
        var currentIndex = 0;

        while (!streamReader.EndOfStream)
        {
            var currentCharacter = (char)streamReader.Read();

            if (currentCharacter == term[needlePosition])
            {
                if (needlePosition == termLength)
                {
                    yield return new MatchResult(currentIndex - termLength, currentIndex, term);
                    needlePosition = 0;
                }
                else
                {
                    needlePosition++;
                }
            }
            else
            {
                needlePosition = 0;
            }

            currentIndex++;
        }
    }


    /// <summary>
    /// Finds term in text with max distance 0, full match that is.
    /// </summary>
    /// <param name="term"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<MatchResult>> FindSubstitutionsOnlyBuffering(string term, Stream text, int maxDistance)
    {
        // foo--fo----f--f-oo--
        // foo
        var matches = new List<MatchResult>();

        using var streamReader = new StreamReader(text);
        var textString = await streamReader.ReadToEndAsync();   // todo make this use stream chunks...

        var needlePosition = 0;
        var termLengthMinusOne = term.Length - 1;
        var termLength = term.Length;
        //var currentIndex = 0;
        var candidateDistance = 0;
        var termIndex = 0;
        var textStringLength = textString.Length;

        for (var currentIndex = 0; currentIndex < textStringLength - termLengthMinusOne; currentIndex++)
        {
            needlePosition = currentIndex;
            candidateDistance = 0;

            for (termIndex = 0; termIndex < termLength; termIndex++)
            {
                if (textString[needlePosition++] != term[termIndex])
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
                var endIndex = currentIndex + termLengthMinusOne;
                matches.Add(new MatchResult(currentIndex, endIndex, textString.Substring(currentIndex, termLength)));
            }            
        }

        return matches;
    }
}

// todo hmm actually this could just be a dictionary
public record CandidateMatch
{
    public int StartIndex { get; init; }
    public int CurrentDistance { get; set; }
    public string Match { get; set; } = "";
}

public record MatchResult(int StartIndex, int EndIndex, string Match);