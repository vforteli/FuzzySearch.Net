namespace FuzzySearchNet;

public record MatchResult
{
    public int StartIndex { get; set; }
    public int EndIndex { get; set; }
    public int Distance { get; set; }
    public string Match { get; set; } = "";
    public int Deletions { get; set; }
    public int Substitutions { get; set; }
    public int Insertions { get; set; }
}
