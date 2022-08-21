namespace FuzzySearchNet;

public class FuzzySearch
{
    /// <summary>
    /// Find instances of term in text up to maximum distance.
    /// </summary>
    /// <param name="term"></param>
    /// <param name="text"></param>
    /// <param name="maxDistance"></param>
    public static async Task<IEnumerable<MatchResult>> FindAsync(string term, Stream text, int maxDistance = 3, bool substitutionsOnly = true)  // todo change this to enum or something..
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
            return await FindSubstitutionsOnlyBufferingAsync(term, text, maxDistance);
        }
        else
        {
            return await FindBufferingAsync(term, text, maxDistance);
        }
    }


    /// <summary>
    /// Finds term in text with max distance 0, full match that is.
    /// </summary>
    /// <param name="term"></param>
    /// <param name="text"></param>    
    public static IEnumerable<MatchResult> FindExact(string term, Stream text)
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
                    yield return new MatchResult(currentIndex - termLength, currentIndex + 1, 0, term);
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
    public static async Task<IEnumerable<MatchResult>> FindSubstitutionsOnlyBufferingAsync(string term, Stream text, int maxDistance)
    {
        var matches = new List<MatchResult>();

        using var streamReader = new StreamReader(text);
        var textString = await streamReader.ReadToEndAsync();   // todo make this use stream chunks...

        var needlePosition = 0;
        var termLengthMinusOne = term.Length - 1;
        var termLength = term.Length;
        var candidateDistance = 0;
        var termIndex = 0;
        var textStringLength = textString.Length;

        for (var currentIndex = 0; currentIndex < textStringLength - termLengthMinusOne; currentIndex++)
        {
            needlePosition = currentIndex;
            candidateDistance = 0;

            for (termIndex = 0; termIndex < termLength; termIndex++)
            {
                if (textString[needlePosition] != term[termIndex])
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
                matches.Add(new MatchResult(currentIndex, currentIndex + termLength, candidateDistance, textString.Substring(currentIndex, termLength)));
            }
        }

        return matches;
    }


    /// <summary>
    /// Finds term in text with max distance
    /// </summary>
    /// <param name="term"></param>
    /// <param name="text"></param>    
    public static async Task<IEnumerable<MatchResult>> FindBufferingAsync(string term, Stream text, int maxDistance)
    {
        var matches = new List<MatchResult>();

        using var streamReader = new StreamReader(text);
        var textString = await streamReader.ReadToEndAsync();   // todo make this use stream chunks...

        var termLengthMinusOne = term.Length - 1;
        var termLength = term.Length;
        var textStringLength = textString.Length;

        var candidates = new Stack<CandidateMatch>();

        for (var currentIndex = 0; currentIndex <= textStringLength - termLengthMinusOne; currentIndex++)
        {
            candidates.Push(new CandidateMatch(currentIndex, currentIndex, 0, 0, 0, 0));

            // Keep track of the best distance so far, this means we can ignore candidates with higher distance if we already have a match
            var bestFoundDistance = maxDistance;
            while (candidates.TryPop(out var candidate))
            {
                if (candidate.patternIndex == termLength && candidate.distance <= bestFoundDistance)
                {
                    matches.Add(new MatchResult(candidate.startIndex, candidate.textIndex, candidate.distance, textString[candidate.startIndex..candidate.textIndex]));
                    bestFoundDistance = candidate.distance;

                    // No point searching for better matches if we find a perfect match
                    if (candidate.distance == 0)
                    {
                        candidates.Clear();
                        break;
                    }

                    continue;
                }

                if (textString[candidate.textIndex] == term[candidate.patternIndex])
                {
                    candidates.Push(new CandidateMatch(candidate.startIndex, candidate.textIndex + 1, candidate.patternIndex + 1, candidate.distance, candidate.deletions, candidate.substitutions));
                    if (candidate.distance < bestFoundDistance)
                    {
                        candidates.Push(new CandidateMatch(candidate.startIndex, candidate.textIndex, candidate.patternIndex + 1, candidate.distance + 1, candidate.deletions + 1, candidate.substitutions));
                    }
                }
                else
                {
                    if (candidate.distance < bestFoundDistance)
                    {
                        candidates.Push(new CandidateMatch(candidate.startIndex, candidate.textIndex + 1, candidate.patternIndex + 1, candidate.distance + 1, candidate.deletions, candidate.substitutions + 1));
                        candidates.Push(new CandidateMatch(candidate.startIndex, candidate.textIndex, candidate.patternIndex + 1, candidate.distance + 1, candidate.deletions + 1, candidate.substitutions));
                    }
                }
            }
        }

        // todo figure out some sane way of removing overlapping matches...
        return matches.Distinct();
    }
}

public record struct CandidateMatch(int startIndex, int textIndex, int patternIndex, int distance, int deletions, int substitutions);