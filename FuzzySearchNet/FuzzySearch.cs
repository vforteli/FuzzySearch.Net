namespace FuzzySearchNet;

public class FuzzySearch
{
    /// <summary>
    /// Find instances of term in text up to maximum distance.
    /// </summary>
    /// <param name="term"></param>
    /// <param name="text"></param>
    /// <param name="maxDistance"></param>
    public static IEnumerable<MatchResult> Find(string term, Stream text, int maxDistance = 3)
    {
        if (string.IsNullOrEmpty(term))
        {
            throw new ArgumentException("Term cannot be null", nameof(term));
        }

        if (maxDistance != 0)
        {
            throw new NotImplementedException();
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
}


public record MatchResult(int StartIndex, int EndIndex, string Match);