using System.Text;
using FuzzySearchNet;

// Console.WriteLine("Press enter to begin");
// Console.ReadLine();

//const string term = "foo";
const string term2 = "fooo--foo-----fo";
const string text = "foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--foo-----fo--foo-f--fooo--";

var count = 100000;

var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

Console.WriteLine($"Running fuzzy search with count {count}");
for (int i = 0; i < count; i++)
{

    await foreach (var _ in FuzzySearch.FindLevenshteinAsync(term2, stream, new FuzzySearchOptions(3), leaveOpen: true))
    {

    }
    stream.Position = 0;
}

// for (int i = 0; i < count; i++)
// {
//     _ = FuzzySearch.FindLevenshtein(term2, text, new FuzzySearchOptions(3)).ToList();
// }