namespace Words.ConsoleApp;

public class Program
{
    public static void Main()
    {
        const string filePath = "/Users/rui/Downloads/words-english.txt";
        string[] dictionary = File.ReadAllLines(filePath);

        // const string start = "spin";
        // Intermediates = [Spit]
        // const string end = "spot";
        // var sameLengthWords = englishDictionary.Where(str => str.Length == start.Length).ToList();
        // var similarWordsStart = FindOneCharDiffWords(start, sameLengthWords).ToList();
        // var similarWordsEnd = FindOneCharDiffWords(end, sameLengthWords).ToList();
        // var result = similarWordsEnd.Intersect(similarWordsStart).ToList();

        const string start = "hide";
        // Intermediates = [hire, sire, sore]
        const string end = "sort";
        
        
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

        var similarWordMidStart = FindMostSimilarWord(end, similarWordsFromStart);
        var similarWordMidEnd = FindMostSimilarWord(start, similarWordsFromEnd);

        var sequence = string.Join(" - ", start, similarWordMidStart, middleWord, similarWordMidEnd, end);
        
        Console.WriteLine();
    }

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
