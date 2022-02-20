using BluePrism.Words.Domain.Services;
using BluePrism.Words.Infrastructure.Services;
using Cocona;
using Cocona.Builder;
using Microsoft.Extensions.DependencyInjection;

CoconaAppBuilder builder = CoconaApp.CreateBuilder();

builder.Services.AddSingleton<IWordPuzzleService, WordPuzzleService>();

CoconaApp app = builder.Build();

app.AddCommand((string start, string end) =>
{
    const string filePath = "/Users/rui/Downloads/words-english.txt";
    string[] dictionary = File.ReadAllLines(filePath);

    var validWords = SameLengthWords(start, dictionary).ToList();

    var similarWordsFromStart = FindOneCharDiffWords(start, validWords).ToList();
    var similarWordsFromEnd = FindOneCharDiffWords(end, validWords).ToList();
    var result1 = similarWordsFromEnd.Intersect(similarWordsFromStart).ToList();
    if (result1.Contains(start) && result1.Contains(end))
    {
        Console.WriteLine($"{start} - {end}");
        return;
    }

    var narrowerResultsStart = FindOneCharDiffWords(similarWordsFromStart, validWords).ToList();
    var narrowerResultsEnd = FindOneCharDiffWords(similarWordsFromEnd, validWords).ToList();
    string? middleWord = narrowerResultsStart.Intersect(narrowerResultsEnd).SingleOrDefault();
    if (middleWord is null)
    {
        Console.WriteLine("Failed to calculate.");
        return;
    }

    string similarWordMidStart = FindMostSimilarWord(end, similarWordsFromStart);
    string similarWordMidEnd = FindMostSimilarWord(start, similarWordsFromEnd);

    string sequence = string.Join(" - ", start, similarWordMidStart, middleWord, similarWordMidEnd, end);

    Console.WriteLine(sequence);

});
        private static IEnumerable<string> SameLengthWords(string word, IEnumerable<string> dictionary)
    {
        return dictionary.Where(str => str.Length == word.Length);
    }

    private static string FindMostSimilarWord(string word, IEnumerable<string> listToCompare)
    {
        var results = new Dictionary<string, int>();
        foreach (string wordToCompare in listToCompare)
        {
            int similarChars = word.Where((endWordChar, i) => endWordChar == wordToCompare[i]).Count();

            results.Add(wordToCompare, similarChars);
        }

        return results.OrderByDescending(x => x.Value).First().Key;
    }

    private static IEnumerable<string> FindOneCharDiffWords(string original, IEnumerable<string> listToCompare)
    {
        return listToCompare.Where(word => IsOneCharDifferent(original, word));
    }

    private static IEnumerable<string> FindOneCharDiffWords(IEnumerable<string> originalWords, IReadOnlyCollection<string> listToCompare)
    {
        List<string> candidates = new();

        foreach (string original in originalWords)
        {
            candidates.AddRange(listToCompare.Where(word => IsOneCharDifferent(original, word)));
        }

        return candidates;
    }

    private static bool IsOneCharDifferent(string original, string word)
    {
        int differentChar = 0;

        for (int i = 0; i < original.Length; i++)
        {
            if (word[i] == original[i])
            {
                continue;
            }

            if (differentChar == 1)
            {
                return false;
            }

            differentChar++;
        }

        return differentChar != 0;
    }
}
