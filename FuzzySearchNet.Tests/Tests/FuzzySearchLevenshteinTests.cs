namespace FuzzySearchNet.Tests;

/// <summary>
/// Testing with similar tests as https://github.com/taleinat/fuzzysearch to ensure somewhat compatible behaviour
/// </summary>
public class FuzzySearchLevenshteinTests
{
    [TestCase("PATTERN", "PATTERN", 0, 0, "PATTERN", 0)]
    [TestCase("def", "abcddefg", 0, 4, "def", 0)]
    [TestCase("def", "abcdeffg", 1, 3, "def", 0)]
    [TestCase("defgh", "abcdedefghi", 3, 5, "defgh", 0)]
    [TestCase("cdefgh", "abcdefghghi", 3, 2, "cdefgh", 0)]
    [TestCase("bde", "abcdefg", 1, 1, "bcde", 1)]
    [TestCase("1234567", "--123567--", 1, 2, "123567", 1)]
    [TestCase("1234567", "--1238567--", 1, 2, "1238567", 1)]
    [TestCase("1234567", "23567-----", 2, 0, "23567", 2)]
    [TestCase("1234567", "--23567---", 2, 1, "-23567", 2)]
    [TestCase("1234567", "-----23567", 2, 4, "-23567", 2)]
    public void TestSingleMatchWithDeletions(string pattern, string text, int maxDistance, int expectedStart, string expectedMatch, int expectedDistance)
    {
        var results = FuzzySearch.Find(pattern, text, maxDistance).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(1));
            TestUtils.AssertMatch(results[0], expectedStart, expectedMatch, expectedDistance);
        });
    }


    [TestCase("PATTERN", "----------PATT-ERN---------", 1, 10, "PATT-ERN", 1)]
    [TestCase("PATTERN", "----------PATT-ERN---------", 2, 10, "PATT-ERN", 1)]

    [TestCase("PATTERN", "----------PATTTERN---------", 1, 10, "PATTTERN", 1)]
    [TestCase("PATTERN", "----------PATTTERN---------", 2, 10, "PATTTERN", 1)]

    [TestCase("PATTERN", "----------PATTERNN---------", 0, 10, "PATTERN", 0)]
    [TestCase("PATTERN", "----------PATTERNN---------", 1, 10, "PATTERN", 0)]
    [TestCase("PATTERN", "----------PATTERNN---------", 2, 10, "PATTERN", 0)]
    public void TestSingleMatchWithInsertion(string pattern, string text, int maxDistance, int expectedStart, string expectedMatch, int expectedDistance)
    {
        var results = FuzzySearch.Find(pattern, text, maxDistance).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(1));
            TestUtils.AssertMatch(results[0], expectedStart, expectedMatch, expectedDistance);
        });
    }


    [Test]
    public void Test2DeletionsBufferStart()
    {
        var word = "pattern";
        var text = "atern----";

        var results = FuzzySearch.Find(word, text, 2).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(1));
            TestUtils.AssertMatch(results[0], 0, "atern", 2);
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
    public void TestSingleDeletionBufferStart()
    {
        var word = "pattern";
        var text = "patern----";

        var results = FuzzySearch.Find(word, text, 1).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(1));
            TestUtils.AssertMatch(results[0], 0, "patern", 1);
        });
    }


    [Test]
    public void TestSingleDeletionBufferMiddle()
    {
        var word = "pattern";
        var text = "--patern--";

        var results = FuzzySearch.Find(word, text, 1).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(1));
            TestUtils.AssertMatch(results[0], 2, "patern", 1);
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
            TestUtils.AssertMatch(results[0], 2, "pattern", 0);
            TestUtils.AssertMatch(results[1], 9, "pattern", 0);
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
            TestUtils.AssertMatch(results[0], 2, "pattern", 0);
            TestUtils.AssertMatch(results[1], 10, "pattern", 0);
        });
    }


    [Test]
    public void TestOptionsMaxSubstitutions()
    {
        var word = "pattern";
        var text = "--patteron--";

        var results = FuzzySearch.FindLevenshtein(word, text, new FuzzySearchOptions(1, 0, 0)).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(1));
            TestUtils.AssertMatch(results[0], 2, "pattero", 1);
        });
    }

    // The substitution can also be made by deleting one character and inserting one, therefore if we dont limit them, and the max distance is 2 or more, the text will still be matched
    [Test]
    public void TestOptionsMaxSubstitutions0()
    {
        var word = "patternsandpractices";
        var text = "--patternsaxdpractices--";

        var results = FuzzySearch.FindLevenshtein(word, text, new FuzzySearchOptions(1, maxSubstitutions: 0)).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(0));
        });
    }


    [Test]
    public void TestOptionsMaxInsertions()
    {
        var word = "pattern";
        var text = "--patteron--";

        var results = FuzzySearch.FindLevenshtein(word, text, new FuzzySearchOptions(0, 0, 1)).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(1));
            TestUtils.AssertMatch(results[0], 2, "patteron", 1);
        });
    }

    [Test]
    public void TestOptionsMaxInsertions0()
    {
        var word = "patternsandpractices";
        var text = "--patternsaxndpractices--";

        var results = FuzzySearch.FindLevenshtein(word, text, new FuzzySearchOptions(3, maxInsertions: 0)).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(0));
        });
    }


    [Test]
    public void TestOptionsMaxDeletions()
    {
        var word = "pattern";
        var text = "--patteron--";

        var results = FuzzySearch.FindLevenshtein(word, text, new FuzzySearchOptions(0, 1, 0)).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(1));
            TestUtils.AssertMatch(results[0], 2, "patter", 1);
        });
    }

    [Test]
    public void TestOptionsMaxDeletions0()
    {
        var word = "patternsandpractices";
        var text = "--patternandpractices--";

        var results = FuzzySearch.FindLevenshtein(word, text, new FuzzySearchOptions(3, maxDeletions: 0)).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(0));
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
            TestUtils.AssertMatch(results[0], 2, "patterm", 1);
            TestUtils.AssertMatch(results[1], 9, "patyern", 1);
        });
    }


    [Test]
    public void TestMultipleMatchesConsecutiveInsertion()
    {
        var word = "pattern";
        var text = "--patyternpatxtern--";

        var results = FuzzySearch.Find(word, text, 1).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(2));
            TestUtils.AssertMatch(results[0], 2, "patytern", 1);
            TestUtils.AssertMatch(results[1], 10, "patxtern", 1);
        });
    }

    [Test]
    public void TestOverlappingMatches()
    {
        var word = "pattern";
        var text = "--pattpatterntern--";

        var results = FuzzySearch.Find(word, text, 2).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(1));
            TestUtils.AssertMatch(results[0], 6, "pattern", 0);
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
            TestUtils.AssertMatch(results[0], 2, "pattrn", 1);
            TestUtils.AssertMatch(results[1], 8, "pttern", 1);
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
            TestUtils.AssertMatch(results[0], 0, "PATERN", 1);
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


    [TestCase("pattern", "pattern---------------------", 0, "pattern", 0)]
    [TestCase("pattern", "attern---------------------", 0, "attern", 1)]
    [TestCase("pattern", "ttern---------------------", 0, "ttern", 2)]
    [TestCase("pattern", "tern---------------------", 0, "tern", 3)]
    [TestCase("pattern", "--------pattttern-------------", 8, "pattttern", 2)]
    [TestCase("pattern", "---------pattttern------------", 9, "pattttern", 2)]
    [TestCase("pattern", "----------pattttern-----------", 10, "pattttern", 2)]
    [TestCase("pattern", "-----------pattttern----------", 11, "pattttern", 2)]
    [TestCase("pattern", "------------pattttern---------", 12, "pattttern", 2)]
    [TestCase("pattern", "-------------pattttern--------", 13, "pattttern", 2)]
    [TestCase("pattern", "--------------pattttern-------", 14, "pattttern", 2)]
    [TestCase("pattern", "---------------pattttern------", 15, "pattttern", 2)]
    [TestCase("pattern", "----------------pattttern-----", 16, "pattttern", 2)]
    [TestCase("pattern", "-----------------pattttern----", 17, "pattttern", 2)]
    [TestCase("pattern", "------------------pattttern---", 18, "pattttern", 2)]
    [TestCase("pattern", "-------------------pattttern--", 19, "pattttern", 2)]
    [TestCase("pattern", "--------------------pattttern-", 20, "pattttern", 2)]
    [TestCase("pattern", "---------------------pattttern", 21, "pattttern", 2)]
    [TestCase("pattern", "---patter", 3, "patter", 1)]
    [TestCase("pattern", "---patte", 3, "patte", 2)]
    [TestCase("pattern", "---patt", 3, "patt", 3)]
    [TestCase("pattern", "----------------------pattttern", 22, "pattttern", 2)]
    public void TestLevenshteinBufferBoundary(string term, string text, int expectedStartIndex, string expectedMatch, int expectedDistance)
    {
        var results = FuzzySearch.FindLevenshtein(term, text, new FuzzySearchOptions(3)).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(1));
            TestUtils.AssertMatch(results[0], expectedStartIndex, expectedMatch, expectedDistance);
        });
    }


    [TestCase("ab", "-a", 1, "a", 1)]
    [TestCase("ab", "b---", 0, "b", 1)]
    [TestCase("ab", "-axb", 1, "axb", 1)]
    [TestCase("ab", "axb-", 0, "axb", 1)]
    [TestCase("ab", "--ax", 2, "ax", 1)]
    [TestCase("ab", "ax--", 0, "ax", 1)]
    [TestCase("ab", "--ab", 2, "ab", 0)]
    [TestCase("ab", "ab--", 0, "ab", 0)]
    [TestCase("ab", "ab", 0, "ab", 0)]
    [TestCase("ab", "-ab", 1, "ab", 0)]
    [TestCase("ab", "ab-", 0, "ab", 0)]
    [TestCase("ab", "b", 0, "b", 1)]
    [TestCase("ab", "a", 0, "a", 1)]
    [TestCase("a", "a", 0, "a", 0)]
    [TestCase("ab", "axb", 0, "axb", 1)]
    public void TestLevenshteinBufferBoundaryShort(string term, string text, int expectedStartIndex, string expectedMatch, int expectedDistance)
    {
        var results = FuzzySearch.FindLevenshtein(term, text, new FuzzySearchOptions(1)).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(1));
            TestUtils.AssertMatch(results[0], expectedStartIndex, expectedMatch, expectedDistance);
        });
    }


    [TestCase("abc", "a", 0, "a", 2)]
    [TestCase("abc", "b", 0, "b", 2)]
    [TestCase("abc", "c", 0, "c", 2)]
    public void TestLevenshteinBufferBoundaryShort2Distance(string term, string text, int expectedStartIndex, string expectedMatch, int expectedDistance)
    {
        var results = FuzzySearch.FindLevenshtein(term, text, new FuzzySearchOptions(2)).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(1));
            TestUtils.AssertMatch(results[0], expectedStartIndex, expectedMatch, expectedDistance);
        });
    }


    [TestCase("abcd", "ax", 0, "ax", 3)]
    [TestCase("abcd", "bx", 0, "bx", 3)]
    [TestCase("abcd", "cx", 0, "cx", 3)]
    [TestCase("abcd", "xa", 1, "a", 3)]
    [TestCase("abcd", "xb", 0, "xb", 3)]
    [TestCase("abcd", "xc", 0, "xc", 3)]
    public void TestLevenshteinBufferBoundaryShort3Distance(string term, string text, int expectedStartIndex, string expectedMatch, int expectedDistance)
    {
        var results = FuzzySearch.FindLevenshtein(term, text, new FuzzySearchOptions(3)).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(1));
            TestUtils.AssertMatch(results[0], expectedStartIndex, expectedMatch, expectedDistance);
        });
    }


    [Test]
    public void TestLevenshteinLinq()
    {
        var text = "---abcc----abc---axc--";
        var term = "abc";

        var results = FuzzySearch.FindLevenshtein(term, text, new FuzzySearchOptions(2)).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(3));
            TestUtils.AssertMatch(results[0], 3, "abc", 0);
            TestUtils.AssertMatch(results[1], 11, "abc", 0);
            TestUtils.AssertMatch(results[2], 17, "axc", 1);
        });

        Assert.Multiple(() =>
        {
            Assert.That(FuzzySearch.FindLevenshtein(term, text, new FuzzySearchOptions(3)).Any());
            TestUtils.AssertMatch(FuzzySearch.FindLevenshtein(term, text, new FuzzySearchOptions(3)).First(), 3, "abc", 0);
        });
    }


    [Test]
    public void TestMultipleMatchesConsecutiveCaseInsensitive()
    {
        var word = "PATTERN";
        var text = "--patternpattern--";

        var results = FuzzySearch.Find(word, text, new FuzzySearchOptions(2, true)).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(2));
            TestUtils.AssertMatch(results[0], 2, "pattern", 0);
            TestUtils.AssertMatch(results[1], 9, "pattern", 0);
        });
    }

    [Test]
    public void TestMultipleMatchesConsecutiveCaseInsensitive2()
    {
        var word = "pattern";
        var text = "--PATTERNPATTERN--";

        var results = FuzzySearch.Find(word, text, new FuzzySearchOptions(2, true)).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(2));
            TestUtils.AssertMatch(results[0], 2, "PATTERN", 0);
            TestUtils.AssertMatch(results[1], 9, "PATTERN", 0);
        });
    }
}
