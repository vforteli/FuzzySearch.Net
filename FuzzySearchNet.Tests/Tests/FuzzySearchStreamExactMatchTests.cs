namespace FuzzySearchNet.Tests;

public class FuzzySearchStreamExactMatchTests
{
    [Test]
    public async Task TestZeroMaxDistanceMultiple()
    {
        var word = "foo";
        var text = new MemoryStream(Encoding.UTF8.GetBytes("foo-----fo--foo-f--fooo--"));

        var results = (await FuzzySearch.FindExactAsync(word, text)).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(3));
            Assert.That(results[0].StartIndex, Is.EqualTo(0));
            Assert.That(results[0].EndIndex, Is.EqualTo(3));
            Assert.That(results[0].Match, Is.EqualTo(word));

            Assert.That(results[1].StartIndex, Is.EqualTo(12));
            Assert.That(results[1].EndIndex, Is.EqualTo(15));
            Assert.That(results[1].Match, Is.EqualTo(word));

            Assert.That(results[2].StartIndex, Is.EqualTo(19));
            Assert.That(results[2].EndIndex, Is.EqualTo(22));
            Assert.That(results[2].Match, Is.EqualTo(word));
        });
    }

    [TestCase("pattern", "pattern-------------", 0)]
    [TestCase("pattern", "-pattern------------", 1)]
    [TestCase("pattern", "--pattern-----------", 2)]
    [TestCase("pattern", "---pattern----------", 3)]
    [TestCase("pattern", "-------pattern------", 7)]
    [TestCase("pattern", "--------pattern-----", 8)]
    public async Task TestZeroMaxDistanceBufferBoundary(string term, string text, int expectedStartIndex)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        var results = (await FuzzySearch.FindExactAsync(term, stream, 8)).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results[0].StartIndex, Is.EqualTo(expectedStartIndex));
            Assert.That(results[0].EndIndex, Is.EqualTo(expectedStartIndex + term.Length));
            Assert.That(results[0].Match, Is.EqualTo(term));
        });
    }
}
