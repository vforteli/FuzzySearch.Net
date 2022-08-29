namespace FuzzySearchNet.Tests;

public class FuzzySearchOptionsTests
{
    [Test]
    public void TestCanSubstituteTotalDistance()
    {
        var options = new FuzzySearchOptions(3);

        Assert.Multiple(() =>
        {
            Assert.That(options.CanSubstitute(3, 1), Is.False);
            Assert.That(options.CanSubstitute(2, 1));
        });
    }

    [Test]
    public void TestCanSubstituteTotalAndMaxDistance()
    {
        var options = new FuzzySearchOptions(3, 1, 1, 1);

        Assert.Multiple(() =>
        {
            Assert.That(options.CanSubstitute(2, 1), Is.False);
            Assert.That(options.CanSubstitute(2, 0));
        });
    }

    [Test]
    public void TestCanSubstituteMaxSubstitutions()
    {
        var options = new FuzzySearchOptions(1, 1, 1);

        Assert.Multiple(() =>
        {
            Assert.That(options.CanSubstitute(100, 1), Is.False);
            Assert.That(options.CanSubstitute(0, 0));
        });
    }

    [Test]
    public void TestCanDeleteTotalDistance()
    {
        var options = new FuzzySearchOptions(3);

        Assert.Multiple(() =>
        {
            Assert.That(options.CanDelete(3, 1), Is.False);
            Assert.That(options.CanDelete(2, 1));
        });
    }

    [Test]
    public void TestCanDeleteTotalAndMaxDistance()
    {
        var options = new FuzzySearchOptions(3, 1, 1, 1);

        Assert.Multiple(() =>
        {
            Assert.That(options.CanDelete(2, 1), Is.False);
            Assert.That(options.CanDelete(2, 0));
        });
    }

    [Test]
    public void TestCanDeleteMaxSubstitutions()
    {
        var options = new FuzzySearchOptions(1, 1, 1);

        Assert.Multiple(() =>
        {
            Assert.That(options.CanDelete(100, 1), Is.False);
            Assert.That(options.CanDelete(0, 0));
        });
    }

    [Test]
    public void TestCanInsertTotalDistance()
    {
        var options = new FuzzySearchOptions(3);

        Assert.Multiple(() =>
        {
            Assert.That(options.CanInsert(3, 1), Is.False);
            Assert.That(options.CanInsert(2, 1));
        });
    }

    [Test]
    public void TestCanInsertTotalAndMaxDistance()
    {
        var options = new FuzzySearchOptions(3, 1, 1, 1);

        Assert.Multiple(() =>
        {
            Assert.That(options.CanInsert(2, 1), Is.False);
            Assert.That(options.CanInsert(2, 0));
        });
    }

    [Test]
    public void TestCanInsertMaxSubstitutions()
    {
        var options = new FuzzySearchOptions(1, 1, 1);

        Assert.Multiple(() =>
        {
            Assert.That(options.CanInsert(100, 1), Is.False);
            Assert.That(options.CanInsert(0, 0));
        });
    }
}
