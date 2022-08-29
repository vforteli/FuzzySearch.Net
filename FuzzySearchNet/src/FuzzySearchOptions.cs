namespace FuzzySearchNet;

/// <summary>
/// Fuzzy search options
/// </summary>
public class FuzzySearchOptions
{
    public int MaxTotalDistance { get; private set; }
    public int? MaxSubstitutions { get; private set; }
    public int? MaxDeletions { get; private set; }
    public int? MaxInsertions { get; private set; }

    /// <summary>
    /// Specify total maximum distance
    /// </summary>
    /// <param name="maxTotalDistance"></param>
    public FuzzySearchOptions(int maxTotalDistance)
    {
        MaxTotalDistance = maxTotalDistance;
    }

    /// <summary>
    /// Specify total maximum distance and optionally limit substitutions, deletions and insertions individually
    /// </summary>
    /// <param name="maxTotalDistance"></param>
    /// <param name="maxSubstitutions"></param>
    /// <param name="maxDeletions"></param>
    /// <param name="maxInsertions"></param>
    public FuzzySearchOptions(int maxTotalDistance, int? maxSubstitutions = null, int? maxDeletions = null, int? maxInsertions = null) : this(maxTotalDistance)
    {
        MaxSubstitutions = maxSubstitutions;
        MaxDeletions = maxDeletions;
        MaxInsertions = maxInsertions;
    }

    /// <summary>
    /// Limit substitutions, deletions and insertions individually
    /// </summary>
    /// <param name="maxSubstitutions"></param>
    /// <param name="maxDeletions"></param>
    /// <param name="maxInsertions"></param>
    public FuzzySearchOptions(int maxSubstitutions, int maxDeletions, int maxInsertions)
    {
        MaxSubstitutions = maxSubstitutions;
        MaxDeletions = maxDeletions;
        MaxInsertions = maxInsertions;
        MaxTotalDistance = maxDeletions + maxInsertions + maxSubstitutions;
    }

    public bool CanSubstitute(int currentTotalDistance, int currentSubstitutions) => (!MaxSubstitutions.HasValue || currentSubstitutions < MaxSubstitutions) && (currentTotalDistance < MaxTotalDistance);

    public bool CanDelete(int currentTotalDistance, int currentDeletions) => (!MaxDeletions.HasValue || currentDeletions < MaxDeletions) && (currentTotalDistance < MaxTotalDistance);

    public bool CanInsert(int currentTotalDistance, int currentInsertions) => (!MaxInsertions.HasValue || currentInsertions < MaxInsertions) && (currentTotalDistance < MaxTotalDistance);
}
