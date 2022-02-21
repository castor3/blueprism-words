using Ardalis.GuardClauses;
using BluePrism.Words.Domain.Models;
using BluePrism.Words.Domain.Services;
using BluePrism.Words.Infrastructure.ModelValidators;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace BluePrism.Words.Infrastructure.Services;

internal class WordPuzzleService : IWordPuzzleService
{
    private readonly ILogger<WordPuzzleService> _logger;

    public WordPuzzleService(ILogger<WordPuzzleService> logger)
    {
        _logger = Guard.Against.Null(logger);
    }

    public IEnumerable<string> GetShortestNumberOfStepsBetweenWords(StepsBetweenWordsOptions options)
    {
        ValidationResult validationResult = new StepsBetweenWordsOptionsValidator().Validate(options);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Invalid parameters {@Options}.", options);
            return Array.Empty<string>();
        }

        string[] dictionary = File.ReadAllLines(filePath);

        var validWords = SameLengthWords(start, dictionary).ToList();

        var similarWordsFromStart = FindOneCharDiffWords(start, validWords).ToList();
        var similarWordsFromEnd = FindOneCharDiffWords(end, validWords).ToList();
        var firstPermutation = similarWordsFromEnd.Intersect(similarWordsFromStart).ToList();
        if (firstPermutation.Count == 1)
        {
            return new[] { start, firstPermutation.Single(), end };
        }

        if (firstPermutation.Contains(start) && firstPermutation.Contains(end))
        {
            return new[] { start, end };
        }

        var narrowerResultsStart = FindOneCharDiffWords(similarWordsFromStart, validWords).ToList();
        var narrowerResultsEnd = FindOneCharDiffWords(similarWordsFromEnd, validWords).ToList();
        string? middleWord = narrowerResultsStart.Intersect(narrowerResultsEnd).SingleOrDefault();
        if (middleWord is null)
        {
            _logger.LogWarning("Failed to calculate.");
            return Array.Empty<string>();
        }

        string similarWordMidStart = FindMostSimilarWord(end, similarWordsFromStart);
        string similarWordMidEnd = FindMostSimilarWord(start, similarWordsFromEnd);

        return new[] { start, similarWordMidStart, middleWord, similarWordMidEnd, end };
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
