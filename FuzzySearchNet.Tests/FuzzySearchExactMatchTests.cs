using System.Text;

namespace FuzzySearchNet.Tests
{
    public class FuzzySearchExactMatchTests
    {
        [Test]
        public async Task TestZeroMaxDistanceMultiple()
        {
            var word = "foo";
            var text = "foo-----fo--foo-f--fooo--";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

            var results = (await FuzzySearch.Find(word, stream, 0)).ToList();

            Assert.That(results.Count, Is.EqualTo(3));

            Assert.That(results[0].StartIndex, Is.EqualTo(0));
            Assert.That(results[0].EndIndex, Is.EqualTo(2));
            Assert.That(results[0].Match, Is.EqualTo(word));

            Assert.That(results[1].StartIndex, Is.EqualTo(12));
            Assert.That(results[1].EndIndex, Is.EqualTo(14));
            Assert.That(results[1].Match, Is.EqualTo(word));

            Assert.That(results[2].StartIndex, Is.EqualTo(19));
            Assert.That(results[2].EndIndex, Is.EqualTo(21));
            Assert.That(results[2].Match, Is.EqualTo(word));
        }

        [Test]
        public async Task TestZeroMaxDistanceNoMatch()
        {
            var word = "foo";
            var text = "fo------fo--fo--f--fo-o--";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

            var results = (await FuzzySearch.Find(word, stream, 0)).ToList();

            Assert.That(results.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task TestZeroMaxDistancePerfectMatch()
        {
            var word = "foo";
            var text = "foo";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

            var results = (await FuzzySearch.Find(word, stream, 0)).ToList();

            Assert.That(results.Count, Is.EqualTo(1));

            Assert.That(results[0].StartIndex, Is.EqualTo(0));
            Assert.That(results[0].EndIndex, Is.EqualTo(2));
            Assert.That(results[0].Match, Is.EqualTo(word));

        }
    }
}
