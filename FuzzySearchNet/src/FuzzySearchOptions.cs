namespace FuzzySearchNet;

/// <summary>
/// Fuzzy search options
/// </summary>
public class FuzzySearchOptions
{
    public int MaxTotalDistance { get; private set; }
    public int MaxSubstitutions { get; private set; }
    public int MaxDeletions { get; private set; }
    public int MaxInsertions { get; private set; }
    public bool InvariantCultureIgnoreCase { get; private set; } = false;

    /// <summary>
    /// Specify total maximum distance
    /// </summary>    
    public FuzzySearchOptions(int maxTotalDistance) : this(maxTotalDistance, false) { }

    /// <summary>
    /// Specify total maximum distance
    /// </summary>    
    public FuzzySearchOptions(int maxTotalDistance, bool invariantCultureIgnoreCase)
    {
        MaxTotalDistance = maxTotalDistance;
        MaxSubstitutions = maxTotalDistance;
        MaxDeletions = maxTotalDistance;
        MaxInsertions = maxTotalDistance;
        InvariantCultureIgnoreCase = invariantCultureIgnoreCase;
    }

    /// <summary>
    /// Specify total maximum distance and optionally limit substitutions, deletions and insertions individually
    /// </summary>    
    public FuzzySearchOptions(int maxTotalDistance, int? maxSubstitutions = null, int? maxDeletions = null, int? maxInsertions = null) : this(maxTotalDistance)
    {
        MaxSubstitutions = maxSubstitutions ?? maxTotalDistance;
        MaxDeletions = maxDeletions ?? maxTotalDistance;
        MaxInsertions = maxInsertions ?? maxTotalDistance;
    }

    /// <summary>
    /// Limit substitutions, deletions and insertions individually
    /// </summary>    
    public FuzzySearchOptions(int maxSubstitutions, int maxDeletions, int maxInsertions)
    {
        MaxSubstitutions = maxSubstitutions;
        MaxDeletions = maxDeletions;
        MaxInsertions = maxInsertions;
        MaxTotalDistance = maxDeletions + maxInsertions + maxSubstitutions;
    }

    public bool CanSubstitute(int currentTotalDistance, int currentSubstitutions) => currentSubstitutions < MaxSubstitutions && currentTotalDistance < MaxTotalDistance;

    public bool CanDelete(int currentTotalDistance, int currentDeletions) => currentDeletions < MaxDeletions && currentTotalDistance < MaxTotalDistance;

    public bool CanInsert(int currentTotalDistance, int currentInsertions) => currentInsertions < MaxInsertions && currentTotalDistance < MaxTotalDistance;
}
