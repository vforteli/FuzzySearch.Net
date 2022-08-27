namespace FuzzySearchNet;

public record struct CandidateMatch(int StartIndex, int TextIndex, int SubSequenceIndex = 0, int Position = 0, int Offset = 0, int Distance = 0, int Deletions = 0, int Substitutions = 0, int Insertions = 0);

// using a record struct improves performance around 30% in benchmarks
//public record CandidateMatch
//{
//    public int StartIndex;
//    public int TextIndex => StartIndex + Position;
//    public int SubSequenceIndex => Position + Offset;
//    public int Position = 0;
//    public int Offset = 0;
//    public int Deletions = 0;
//    public int Substitutions = 0;
//    public int Insertions = 0;
//    public int Distance => Deletions + Insertions + Substitutions;
//}