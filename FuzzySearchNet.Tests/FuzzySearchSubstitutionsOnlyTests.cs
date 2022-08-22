namespace FuzzySearchNet.Tests;

public class FuzzySearchSubstitutionsOnlyTests
{
    [Test]
    public void TestZeroMaxDistanceMultiple()
    {
        var word = "foo";
        var text = "foo--fo----f--f-oo";

        var results = FuzzySearch.Find(word, text, 1).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(4));

            Assert.That(results[0].StartIndex, Is.EqualTo(0));
            Assert.That(results[0].EndIndex, Is.EqualTo(3));
            Assert.That(results[0].Match, Is.EqualTo(word));

            Assert.That(results[1].StartIndex, Is.EqualTo(5));
            Assert.That(results[1].EndIndex, Is.EqualTo(8));
            Assert.That(results[1].Match, Is.EqualTo("fo-"));

            Assert.That(results[2].StartIndex, Is.EqualTo(14));
            Assert.That(results[2].EndIndex, Is.EqualTo(17));
            Assert.That(results[2].Match, Is.EqualTo("f-o"));

            Assert.That(results[3].StartIndex, Is.EqualTo(15));
            Assert.That(results[3].EndIndex, Is.EqualTo(18));
            Assert.That(results[3].Match, Is.EqualTo("-oo"));
        });
    }

    [Test]
    public void TestZeroMaxDistanceNoMatch()
    {
        var word = "foo";
        var text = "f-------f----o---f---o-o--";

        var results = FuzzySearch.Find(word, text, 1).ToList();

        Assert.That(results.Count, Is.EqualTo(0));
    }

    [Test]
    public void TestZeroMaxDistancePerfectMatch()
    {
        var word = "foo";
        var text = "foo";

        var results = FuzzySearch.Find(word, text, 1).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results[0].StartIndex, Is.EqualTo(0));
            Assert.That(results[0].EndIndex, Is.EqualTo(3));
            Assert.That(results[0].Match, Is.EqualTo(word));
        });
    }


    [Test]
    public void TestSomething2()
    {
        var pattern = "TGCACTGTAGGGATAACAAT";
        var text = "GACTAGCACTGTAGGGATAACAATTTCACACAGGTGGACAATTACATTGAAAATCACAGATTGGTCACACACACATTGGACATACATAGAAACACACACACATACATTAGATACGAACATAGAAACACACATTAGACGCGTACATAGACACAAACACATTGACAGGCAGTTCAGATGATGACGCCCGACTGATACTCGCGTAGTCGTGGGAGGCAAGGCACACAGGGGATAGG";

        var results = FuzzySearch.Find(pattern, text, 2).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(1));

            Assert.That(results[0].StartIndex, Is.EqualTo(4));
            Assert.That(results[0].EndIndex, Is.EqualTo(24));
            Assert.That(results[0].Match, Is.EqualTo(text[4..24]));
        });
    }


    [Test]
    public void TestSomething()
    {
        var pattern = "GGGTTLTTSS";
        var text = "XXXXXXXXXXXXXXXXXXXGGGTTVTTSSAAAAAAAAAAAAAGGGTTLTTSSAAAAAAAAAAAAAAAAAAAAAABBBBBBBBBBBBBBBBBBBBBBBBBGGGTTLTTSS";

        var results = FuzzySearch.Find(pattern, text, 0).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results.Count, Is.EqualTo(2));

            Assert.That(results[0].StartIndex, Is.EqualTo(42));
            Assert.That(results[0].EndIndex, Is.EqualTo(52));
            Assert.That(results[0].Match, Is.EqualTo(text[42..52]));

            Assert.That(results[1].StartIndex, Is.EqualTo(99));
            Assert.That(results[1].EndIndex, Is.EqualTo(109));
            Assert.That(results[1].Match, Is.EqualTo(text[99..109]));
        });

        var results2 = FuzzySearch.Find(pattern, text, 1).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results2.Count, Is.EqualTo(3));

            Assert.That(results2[0].StartIndex, Is.EqualTo(19));
            Assert.That(results2[0].EndIndex, Is.EqualTo(29));
            Assert.That(results2[0].Match, Is.EqualTo(text[19..29]));

            Assert.That(results2[1].StartIndex, Is.EqualTo(42));
            Assert.That(results2[1].EndIndex, Is.EqualTo(52));
            Assert.That(results2[1].Match, Is.EqualTo(text[42..52]));

            Assert.That(results2[2].StartIndex, Is.EqualTo(99));
            Assert.That(results2[2].EndIndex, Is.EqualTo(109));
            Assert.That(results2[2].Match, Is.EqualTo(text[99..109]));
        });

        var results3 = FuzzySearch.Find(pattern, text, 2).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(results3.Count, Is.EqualTo(3));

            Assert.That(results3[0].StartIndex, Is.EqualTo(19));
            Assert.That(results3[0].EndIndex, Is.EqualTo(29));
            Assert.That(results3[0].Match, Is.EqualTo(text[19..29]));

            Assert.That(results3[1].StartIndex, Is.EqualTo(42));
            Assert.That(results3[1].EndIndex, Is.EqualTo(52));
            Assert.That(results3[1].Match, Is.EqualTo(text[42..52]));

            Assert.That(results3[2].StartIndex, Is.EqualTo(99));
            Assert.That(results3[2].EndIndex, Is.EqualTo(109));
            Assert.That(results3[2].Match, Is.EqualTo(text[99..109]));
        });
    }
}
