namespace FuzzySearchNet.Tests;

public class FuzzySearchStreamSubstitutionsOnlyTests
{
    [Test]
    public async Task TestSubstitutionOnlyMultiple()
    {
        var word = "foo";
        var text = "foo-----fo--foo-f--fooo--";
        var textStream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        var results = await FuzzySearch.FindSubstitutionsOnlyAsync(word, textStream, 1).ToListAsync();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(5));

            TestUtils.AssertMatch(results[0], 0, 3, text);
            TestUtils.AssertMatch(results[1], 8, 11, text);
            TestUtils.AssertMatch(results[2], 12, 15, text);
            TestUtils.AssertMatch(results[3], 19, 22, text);
            TestUtils.AssertMatch(results[4], 20, 23, text);    // todo this is a bit hohum... do we want to consolidate these or not?
        });
    }

    [Test]
    public async Task TestSubstitutionOnlyPatternLongerThanText()
    {
        var text = "foo";
        var word = "foo-----fo--foo-f--fooo--";
        var textStream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        var results = await FuzzySearch.FindSubstitutionsOnlyAsync(word, textStream, 1).ToListAsync();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(0));
        });
    }

    [TestCase("pattern", "patfern---------------------", 0)]
    [TestCase("pattern", "--------patfern-------------", 8)]
    [TestCase("pattern", "---------patfern------------", 9)]
    [TestCase("pattern", "----------patfern-----------", 10)]
    [TestCase("pattern", "-----------patfern----------", 11)]
    [TestCase("pattern", "---------------patfern------", 15)]
    [TestCase("pattern", "----------------patfern-----", 16)]
    [TestCase("pattern", "-----------------patfern----", 17)]
    [TestCase("pattern", "---------------------patfern", 21)]
    public async Task TestSubstitutionOnlyBufferBoundary(string term, string text, int expectedStartIndex)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        var results = await FuzzySearch.FindSubstitutionsOnlyAsync(term, stream, 1, 16).ToListAsync();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results[0].StartIndex, Is.EqualTo(expectedStartIndex));
            Assert.That(results[0].EndIndex, Is.EqualTo(expectedStartIndex + term.Length));
            Assert.That(results[0].Match, Is.EqualTo("patfern"));
        });
    }

    [Test]
    public async Task TestSubstitutionOnlyTermLongerThanBuffer()
    {
        var text = "------------------------thisislongerthanthebufferandshouldntexplode---------------------";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        var term = "thisislongerthanthebufferandshouldntexplode";

        var results = await FuzzySearch.FindSubstitutionsOnlyAsync(term, stream, 1, 16).ToListAsync();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(1));
            TestUtils.AssertMatch(results[0], 24, 24 + term.Length, text);
        });
    }

    [Test]
    public async Task TestSubstitutionOnlyBiggerBufferLongerSubsequence()
    {
        var text = "thisislongerthanthebufferandshouldntexplo";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        var term = "thisislongerthanthebufferandshouldntexplode";

        var results = await FuzzySearch.FindSubstitutionsOnlyAsync(term, stream, 3).ToListAsync();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(0));
        });
    }

    [TestCase("pattern", "----PATTXRN----")]
    [TestCase("PATTERN", "----pattxrn----")]
    [TestCase("pattERN", "----pattXRN----")]
    public async Task TestCaseInsensitiveMatch(string word, string text)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        var results = await FuzzySearch.FindSubstitutionsOnlyAsync(word, stream, 1, invariantCultureIgnoreCase: true).ToListAsync();

        Assert.Multiple(() =>
        {
            Assert.That(results, Has.Count.EqualTo(1));
            Assert.That(results[0].StartIndex, Is.EqualTo(4));
        });
    }
}
