namespace FuzzySearchNet;

internal record struct MatchResultWithValue
{
    public MatchResultWithValue()
    {
    }

    public int StartIndex { get; set; }
    public int EndIndex { get; set; }
    public int Distance { get; set; }
    public string Match { get; set; } = "";
    public int Deletions { get; set; }
    public int Substitutions { get; set; }
    public int Insertions { get; set; }
}
