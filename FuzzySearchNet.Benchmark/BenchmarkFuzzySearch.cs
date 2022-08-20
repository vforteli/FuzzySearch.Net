using BenchmarkDotNet.Attributes;
using System.Text;

namespace FuzzySearchNet.Benchmark;

public class BenchmarkFuzzySearch
{
    private const string term = "foo";
    private const string term2 = "fooo--foo-----fo";
    private const string text = "foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--";

    [Benchmark]
    public async Task SubstitutionOnlyBufferingShort() => await FuzzySearch.FindSubstitutionsOnlyBufferingAsync(term, new MemoryStream(Encoding.UTF8.GetBytes(text)), 1);

    [Benchmark]
    public async Task SubstitutionOnlyBufferingLong() => await FuzzySearch.FindSubstitutionsOnlyBufferingAsync(term2, new MemoryStream(Encoding.UTF8.GetBytes(text)), 1);


    [Benchmark]
    public async Task SubstitutionOnlyBufferingLong3distance() => await FuzzySearch.FindSubstitutionsOnlyBufferingAsync(term2, new MemoryStream(Encoding.UTF8.GetBytes(text)), 3);
}