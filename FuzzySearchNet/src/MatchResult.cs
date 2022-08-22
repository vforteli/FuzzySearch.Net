namespace FuzzySearchNet;

public record struct MatchResult(int StartIndex, int EndIndex, int Distance, string Match, int Deletions, int Substitutions, int Insertions);