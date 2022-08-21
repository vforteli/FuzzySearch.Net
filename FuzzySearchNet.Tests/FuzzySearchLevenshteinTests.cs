using System.Text;

namespace FuzzySearchNet.Tests
{
    public class FuzzySearchLevenshteinTests
    {
        [TestCase("PATTERN", "PATTERN", 0, 0, 7, 0)]
        [TestCase("def", "abcddefg", 0, 4, 7, 0)]
        [TestCase("def", "abcdeffg", 1, 3, 6, 0)]
        [TestCase("defgh", "abcdedefghi", 3, 5, 10, 0)]
        [TestCase("cdefgh", "abcdefghghi", 3, 2, 8, 0)]
        [TestCase("bde", "abcdefg", 1, 1, 5, 1)]
        [TestCase("1234567", "--123567--", 1, 2, 8, 1)]
        [TestCase("1234567", "23567-----", 2, 0, 5, 2)]
        [TestCase("1234567", "--23567---", 2, 2, 7, 2)]
        [TestCase("1234567", "-----23567", 2, 4, 10, 2)]
        public async Task TestSingleMatchWithDeletions(string pattern, string text, int maxDistance, int expectedStart, int expectedEnd, int expectedDistance)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

            var results = (await FuzzySearch.FindAsync(pattern, stream, maxDistance, false)).ToList();

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
        public async Task TestZeroMaxDistanceMultiple2()
        {
            var word = "pattern";
            var text = "atern----";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

            var results = (await FuzzySearch.FindAsync(word, stream, 2, false)).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(results.Count, Is.EqualTo(1));

                Assert.That(results[0].StartIndex, Is.EqualTo(0));
                Assert.That(results[0].EndIndex, Is.EqualTo(5));
                Assert.That(results[0].Match, Is.EqualTo(text[0..5]));
            });
        }


        [Test]
        public async Task TestZeroMaxDistanceMultiple85()
        {
            var word = "pattern";
            var text = "patern----";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

            var results = (await FuzzySearch.FindAsync(word, stream, 1, false)).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(results.Count, Is.EqualTo(1));

                Assert.That(results[0].StartIndex, Is.EqualTo(0));
                Assert.That(results[0].EndIndex, Is.EqualTo(6));
                Assert.That(results[0].Match, Is.EqualTo(text[0..6]));
            });
        }

        [Test]
        public async Task TestZeroMaxDistanceMultipleMiddle()
        {
            var word = "pattern";
            var text = "--patern--";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

            var results = (await FuzzySearch.FindAsync(word, stream, 1, false)).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(results.Count, Is.EqualTo(1));

                Assert.That(results[0].StartIndex, Is.EqualTo(2));
                Assert.That(results[0].EndIndex, Is.EqualTo(8));
                Assert.That(results[0].Match, Is.EqualTo(text[2..8]));
            });
        }
    }
}
