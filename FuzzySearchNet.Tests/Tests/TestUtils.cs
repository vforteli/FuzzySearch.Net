namespace FuzzySearchNet.Tests;

internal class TestUtils
{
    public static void AssertMatch(MatchResult match, int expectedStartIndex, int expectedEndIndex, string text)
    {
        Assert.That(match.StartIndex, Is.EqualTo(expectedStartIndex));
        Assert.That(match.EndIndex, Is.EqualTo(expectedEndIndex));
        Assert.That(match.Match, Is.EqualTo(text[expectedStartIndex..expectedEndIndex]));
    }
}
