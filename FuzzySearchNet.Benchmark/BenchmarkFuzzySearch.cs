using BenchmarkDotNet.Attributes;

namespace FuzzySearchNet.Benchmark;

public class BenchmarkFuzzySearch
{
    private const string term = "foo";
    private const string term2 = "fooo--foo-----fo";
    private const string text = "foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--";

    //[Benchmark]
    //public void SubstitutionOnlyBufferingShort() => FuzzySearch.FindSubstitutionsOnlyBuffering(term, text, 1);

    //[Benchmark]
    //public void SubstitutionOnlyBufferingLong() => FuzzySearch.FindSubstitutionsOnlyBuffering(term2, text, 1);


    //[Benchmark]
    //public void SubstitutionOnlyBufferingLong3distance() => FuzzySearch.FindSubstitutionsOnlyBuffering(term2, text, 3);


    [Benchmark]
    public void LevenshteinLong() => FuzzySearch.FindLevenshtein(term2, text, 3);
}