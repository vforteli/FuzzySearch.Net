using System.Text;
using BenchmarkDotNet.Attributes;

namespace FuzzySearchNet.Benchmark;

public class BenchmarkFuzzySearch
{
    private const string term = "foo";
    private const string term2 = "fooo--foo-----fo";
    private const string text = "foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--";

    public static readonly MemoryStream textStream = new MemoryStream(Encoding.UTF8.GetBytes(text));

    [Benchmark]
    public void SubstitutionOnlyShort() => FuzzySearch.FindSubstitutionsOnly(term, text, 1);

    [Benchmark]
    public void SubstitutionOnlyLong() => FuzzySearch.FindSubstitutionsOnly(term2, text, 1);

    [Benchmark]
    public void SubstitutionOnlyShor_3_distance() => FuzzySearch.FindSubstitutionsOnly(term, text, 3);

    [Benchmark]
    public void SubstitutionOnlyLong_3_distance() => FuzzySearch.FindSubstitutionsOnly(term2, text, 3);



    [Benchmark]
    public void LevenshteinLong()
    {
        _ = FuzzySearch.FindLevenshtein(term2, text, new FuzzySearchOptions(3)).ToList();
    }

    [Benchmark]
    public async Task LevenshteinLongAsync()
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));    // this is not ideal, but the effect of doing this here is basically within the stddev of the benchmark
        await foreach (var _ in FuzzySearch.FindLevenshteinAsync(term2, stream, new FuzzySearchOptions(3)))
        {

        }
    }
}