namespace FuzzySearchNet.Tests.Tests;

/// <summary>
/// Testing with similar tests as https://github.com/taleinat/fuzzysearch to ensure somewhat compatible behaviour
/// </summary>
public class FuzzySearchLevenshteinTests
{
    [TestCase("PATTERN", "PATTERN", 0, 0, 7, 0)]
    [TestCase("def", "abcddefg", 0, 4, 7, 0)]
    [TestCase("def", "abcdeffg", 1, 3, 6, 0)]
    [TestCase("defgh", "abcdedefghi", 3, 5, 10, 0)]
    [TestCase("cdefgh", "abcdefghghi", 3, 2, 8, 0)]
    [TestCase("bde", "abcdefg", 1, 1, 5, 1)]
    [TestCase("1234567", "--123567--", 1, 2, 8, 1)]
    [TestCase("1234567", "--1238567--", 1, 2, 9, 1)]
    [TestCase("1234567", "23567-----", 2, 0, 5, 2)]
    [TestCase("1234567", "--23567---", 2, 1, 7, 2)]
    [TestCase("1234567", "-----23567", 2, 4, 10, 2)]
    public void TestSingleMatchWithDeletions(string pattern, string text, int maxDistance, int expectedStart, int expectedEnd, int expectedDistance)
    {
        var results = FuzzySearch.Find(pattern, text, maxDistance).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(1));

            Assert.That(results[0].StartIndex, Is.EqualTo(expectedStart));
            Assert.That(results[0].EndIndex, Is.EqualTo(expectedEnd));
            Assert.That(results[0].Distance, Is.EqualTo(expectedDistance));
            Assert.That(results[0].Match, Is.EqualTo(text[expectedStart..expectedEnd]));
        });
    }


    [TestCase("PATTERN", "----------PATT-ERN---------", 1, 10, 18, 1)]
    [TestCase("PATTERN", "----------PATT-ERN---------", 2, 10, 18, 1)]

    [TestCase("PATTERN", "----------PATTTERN---------", 1, 10, 18, 1)]
    [TestCase("PATTERN", "----------PATTTERN---------", 2, 10, 18, 1)]

    [TestCase("PATTERN", "----------PATTERNN---------", 0, 10, 17, 0)]
    [TestCase("PATTERN", "----------PATTERNN---------", 1, 10, 17, 0)]
    [TestCase("PATTERN", "----------PATTERNN---------", 2, 10, 17, 0)]
    public void TestSingleMatchWithInsertion(string pattern, string text, int maxDistance, int expectedStart, int expectedEnd, int expectedDistance)
    {
        var results = FuzzySearch.Find(pattern, text, maxDistance).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(1));

            Assert.That(results[0].StartIndex, Is.EqualTo(expectedStart));
            Assert.That(results[0].EndIndex, Is.EqualTo(expectedEnd));
            Assert.That(results[0].Distance, Is.EqualTo(expectedDistance));
            Assert.That(results[0].Match, Is.EqualTo(text[expectedStart..expectedEnd]));
        });
    }


    [Test]
    public void TestZeroMaxDistanceMultiple2()
    {
        var word = "pattern";
        var text = "atern----";

        var results = FuzzySearch.Find(word, text, 2).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(1));

            Assert.That(results[0].StartIndex, Is.EqualTo(0));
            Assert.That(results[0].EndIndex, Is.EqualTo(5));
            Assert.That(results[0].Match, Is.EqualTo(text[0..5]));
        });
    }


    [Test]
    public void TestZeroMaxDistanceNoMatch()
    {
        var word = "pattern";
        var text = "--paxxern--";

        var results = FuzzySearch.Find(word, text, 1).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(0));
        });
    }

    [Test]
    public void TestZeroMaxDistanceNoMatch2()
    {
        var word = "pattern";
        var text = "paxxern";

        var results = FuzzySearch.Find(word, text, 1).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(0));
        });
    }


    [Test]
    public void TestZeroMaxDistanceMultiple85()
    {
        var word = "pattern";
        var text = "patern----";

        var results = FuzzySearch.Find(word, text, 1).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(1));

            Assert.That(results[0].StartIndex, Is.EqualTo(0));
            Assert.That(results[0].EndIndex, Is.EqualTo(6));
            Assert.That(results[0].Match, Is.EqualTo(text[0..6]));
        });
    }

    [Test]
    public void TestZeroMaxDistanceMultipleMiddle()
    {
        var word = "pattern";
        var text = "--patern--";

        var results = FuzzySearch.Find(word, text, 1).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(1));

            Assert.That(results[0].StartIndex, Is.EqualTo(2));
            Assert.That(results[0].EndIndex, Is.EqualTo(8));
            Assert.That(results[0].Match, Is.EqualTo(text[2..8]));
        });
    }

    [Test]
    public void TestMultipleMatchesConsecutive()
    {
        var word = "pattern";
        var text = "--patternpattern--";

        var results = FuzzySearch.Find(word, text, 2).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(2));

            Assert.That(results[0].StartIndex, Is.EqualTo(2));
            Assert.That(results[0].EndIndex, Is.EqualTo(9));
            Assert.That(results[0].Match, Is.EqualTo(text[2..9]));

            Assert.That(results[1].StartIndex, Is.EqualTo(9));
            Assert.That(results[1].EndIndex, Is.EqualTo(16));
            Assert.That(results[1].Match, Is.EqualTo(text[9..16]));
        });
    }

    [Test]
    public void TestMultipleMatchesConsecutive2()
    {
        var word = "pattern";
        var text = "--pattern-pattern--";

        var results = FuzzySearch.Find(word, text, 1).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(2));

            Assert.That(results[0].StartIndex, Is.EqualTo(2));
            Assert.That(results[0].EndIndex, Is.EqualTo(9));
            Assert.That(results[0].Match, Is.EqualTo(text[2..9]));

            Assert.That(results[1].StartIndex, Is.EqualTo(10));
            Assert.That(results[1].EndIndex, Is.EqualTo(17));
            Assert.That(results[1].Match, Is.EqualTo(text[10..17]));
        });
    }

    [Test]
    public void TestMultipleMatchesConsecutiveSubstitutions()
    {
        var word = "pattern";
        var text = "--pattermpatyern--";

        var results = FuzzySearch.Find(word, text, 2).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(2));

            Assert.That(results[0].StartIndex, Is.EqualTo(2));
            Assert.That(results[0].EndIndex, Is.EqualTo(9));
            Assert.That(results[0].Match, Is.EqualTo(text[2..9]));

            Assert.That(results[1].StartIndex, Is.EqualTo(9));
            Assert.That(results[1].EndIndex, Is.EqualTo(16));
            Assert.That(results[1].Match, Is.EqualTo(text[9..16]));
        });
    }

    [Test]
    public void TestMultipleMatchesConsecutiveDeletion()
    {
        var word = "pattern";
        var text = "--pattrnpttern--";

        var results = FuzzySearch.Find(word, text, 2).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(2));

            Assert.That(results[0].StartIndex, Is.EqualTo(2));
            Assert.That(results[0].EndIndex, Is.EqualTo(8));
            Assert.That(results[0].Match, Is.EqualTo(text[2..8]));

            Assert.That(results[1].StartIndex, Is.EqualTo(8));
            Assert.That(results[1].EndIndex, Is.EqualTo(14));
            Assert.That(results[1].Match, Is.EqualTo(text[8..14]));
        });
    }

    [TestCase("PATTERN", "")]
    [TestCase("", "sometext")]
    [TestCase("", "")]
    public void TestEmpty(string pattern, string text)
    {
        var results = FuzzySearch.Find(pattern, text, 2).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(0));
        });
    }

    [TestCase("PATTERN", "PATERN", 1)]
    public void TestShorterText(string pattern, string text, int expectedMatches)
    {
        var results = FuzzySearch.Find(pattern, text, 1).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(expectedMatches));
            Assert.That(results[0].StartIndex, Is.EqualTo(0));
            Assert.That(results[0].EndIndex, Is.EqualTo(text.Length));
            Assert.That(results[0].Match, Is.EqualTo(text[0..text.Length]));
        });
    }

    [TestCase("PATTERN", "PAERN", 0)]
    public void TestShorterTextNoMatch(string pattern, string text, int expectedMatches)
    {
        var results = FuzzySearch.Find(pattern, text, 1).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(expectedMatches));
        });
    }
}
