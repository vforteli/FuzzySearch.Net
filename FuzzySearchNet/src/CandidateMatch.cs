namespace FuzzySearchNet;

public record struct CandidateMatch(int StartIndex, int TextIndex, int SubSequenceIndex = 0, int Distance = 0, int Deletions = 0, int Substitutions = 0, int Insertions = 0);
