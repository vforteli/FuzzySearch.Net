using BenchmarkDotNet.Attributes;
using System.Text;

namespace FuzzySearchNet.Benchmark;

public class BenchmarkFuzzySearch
{
    private const string term = "foo";
    private const string text = "foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--";

    [Benchmark]
    public async Task SubstitutionOnly() => await FuzzySearch.FindAsync(term, new MemoryStream(Encoding.UTF8.GetBytes(text)), 1);
}