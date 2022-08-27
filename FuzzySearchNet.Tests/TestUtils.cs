namespace FuzzySearchNet.Tests;

internal class TestUtils
{
    public static void AssertMatch(MatchResult match, int expectedStartIndex, int expectedEndIndex, string text, int? expectedDistance = null)
    {
        Assert.That(match.StartIndex, Is.EqualTo(expectedStartIndex));
        Assert.That(match.EndIndex, Is.EqualTo(expectedEndIndex));
        Assert.That(match.Match, Is.EqualTo(text[expectedStartIndex..expectedEndIndex]));

        if (expectedDistance.HasValue)
        {
            Assert.That(match.Distance, Is.EqualTo(expectedDistance));
        }
    }

    public static void AssertMatch(MatchResult match, int expectedStartIndex, string expectedMatch, int? expectedDistance = null)
    {
        Assert.That(match.StartIndex, Is.EqualTo(expectedStartIndex));
        Assert.That(match.EndIndex, Is.EqualTo(expectedStartIndex + expectedMatch.Length));
        Assert.That(match.Match, Is.EqualTo(expectedMatch));

        if (expectedDistance.HasValue)
        {
            Assert.That(match.Distance, Is.EqualTo(expectedDistance));
        }
    }
}
