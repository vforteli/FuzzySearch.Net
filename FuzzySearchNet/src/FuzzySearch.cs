namespace FuzzySearchNet;

public static partial class FuzzySearch
{
    /// <summary>
    /// Find instances of sub sequence in text with options
    /// </summary>    
    public static IEnumerable<MatchResult> Find(string subSequence, string text, FuzzySearchOptions options) =>
         options switch
         {
             { MaxTotalDistance: 0 } => FindExact(subSequence, text),
             { MaxDeletions: 0, MaxInsertions: 0 } => FindSubstitutionsOnly(subSequence, text, options.MaxTotalDistance),
             _ => FindLevenshtein(subSequence, text, options),
         };


    /// <summary>
    /// Find instances of sub sequence in text with options
    /// </summary>    
    public static IAsyncEnumerable<MatchResult> FindAsync(string subSequence, Stream textStream, FuzzySearchOptions options) =>
        options switch
        {
            { MaxTotalDistance: 0 } => FindExactAsync(subSequence, textStream),
            { MaxDeletions: 0, MaxInsertions: 0 } => FindSubstitutionsOnlyAsync(subSequence, textStream, options.MaxTotalDistance),
            _ => FindLevenshteinAsync(subSequence, textStream, options),
        };


    /// <summary>
    /// Find instances of sub sequence in text up to default maximum distance 3.
    /// </summary>   
    public static IEnumerable<MatchResult> Find(string subSequence, string text) => Find(subSequence, text, 3, SearchOptions.None);


    /// <summary>
    /// Find instances of sub sequence in text up to default maximum distance 3 and search mode.
    /// </summary>    
    public static IEnumerable<MatchResult> Find(string subSequence, string text, SearchOptions searchMode) => Find(subSequence, text, 3, searchMode);


    /// <summary>
    /// Find instances of sub sequence in text up to maximum distance.
    /// </summary>    
    public static IEnumerable<MatchResult> Find(string subSequence, string text, int maxDistance) => Find(subSequence, text, maxDistance, SearchOptions.None);


    /// <summary>
    /// Find instances of sub sequence in text up to maximum distance.
    /// </summary>    
    public static IEnumerable<MatchResult> Find(string subSequence, string text, int maxDistance, SearchOptions searchMode)
    {
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
            return FindLevenshtein(subSequence, text, new FuzzySearchOptions(maxDistance));
        }
    }
}
