namespace FuzzySearchNet.Tests.Tests;

public class FuzzySearchExactMatchTests
{
    [Test]
    public void TestZeroMaxDistanceMultiple()
    {
        var word = "foo";
        var text = "foo-----fo--foo-f--fooo--";

        var results = FuzzySearch.Find(word, text, 0).ToList();

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

    [Test]
    public void TestZeroMaxDistanceNoMatch()
    {
        var word = "foo";
        var text = "fo------fo--fo--f--fo-o--";

        var results = FuzzySearch.Find(word, text, 0).ToList();

        Assert.That(results.Count, Is.EqualTo(0));
    }

    [Test]
    public void TestZeroMaxDistancePerfectMatch()
    {
        var word = "foo";
        var text = "foo";

        var results = FuzzySearch.Find(word, text, 0).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results[0].StartIndex, Is.EqualTo(0));
            Assert.That(results[0].EndIndex, Is.EqualTo(3));
            Assert.That(results[0].Match, Is.EqualTo(word));
        });
    }

    [TestCase("", "")]
    [TestCase("", "foo")]
    [TestCase("foo", "")]
    public void TestZeroMaxDistanceEmpty(string word, string text)
    {
        var results = FuzzySearch.Find(word, text, 0).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(0));
        });
    }
}
