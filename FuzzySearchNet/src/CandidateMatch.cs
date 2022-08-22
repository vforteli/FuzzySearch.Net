namespace FuzzySearchNet;

public record struct CandidateMatch(int StartIndex, int TextIndex, int PatternIndex, int Distance, int Deletions, int Substitutions, int Insertions);