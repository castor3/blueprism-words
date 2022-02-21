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

        string start = options.Start;
        string end = options.End;

        if (start.Equals(end, StringComparison.InvariantCultureIgnoreCase))
        {
            return new[] { start };
        }

        var validWords = GetSameLengthWords(start, options.Dictionary).ToList();

        var similarWordsFromStart = FindOneCharDiffWords(start, validWords).ToList();
        var similarWordsFromEnd = FindOneCharDiffWords(end, validWords).ToList();
        var firstPermutation = similarWordsFromEnd.Intersect(similarWordsFromStart).ToList();
        var isStartAndEndOneCharDiff = firstPermutation.Contains(start) || firstPermutation.Contains(end);
        if (isStartAndEndOneCharDiff)
        {
            return new[] { start, end };
        }

        if (firstPermutation.Count == 1)
        {
            return new[] { start, firstPermutation.Single(), end };
        }

        var narrowerResultsStart = FindOneCharDiffWords(similarWordsFromStart, validWords).ToList();
        var narrowerResultsEnd = FindOneCharDiffWords(similarWordsFromEnd, validWords).ToList();
        var middleWords = narrowerResultsStart.Intersect(narrowerResultsEnd).ToList();
        if (middleWords.Count == 2)
        {
            var result = new List<string>();
            result.Add(start);
            result.AddRange(middleWords);
            result.Add(end);
            return result;
        }

        string nextStep = FindMostSimilarWord(end, similarWordsFromStart);
        string lastStep = FindMostSimilarWord(start, similarWordsFromEnd);
        if (nextStep.Equals(lastStep, StringComparison.InvariantCultureIgnoreCase))
        {
            return new[] { start, nextStep, end };
        }

        bool notHasMiddleWord = middleWords.Any(x =>
            x.Equals(nextStep, StringComparison.InvariantCultureIgnoreCase) ||
            x.Equals(lastStep, StringComparison.InvariantCultureIgnoreCase));

        if (notHasMiddleWord)
        {
            return new[] { start, nextStep, lastStep, end };
        }

        return new[] { start, nextStep, middleWords.Single(), lastStep, end };
    }

    private static IEnumerable<string> GetSameLengthWords(string word, IEnumerable<string> dictionary)
    {
        return dictionary.Where(str => str.Length == word.Length);
    }

    private static string FindMostSimilarWord(string word, List<string> listToCompare)
    {
        var results = new Dictionary<string, int>();

        // ReSharper disable once ForCanBeConvertedToForeach
        for (int i = 0; i < listToCompare.Count; i++)
        {
            int similarChars = word.Where((endWordChar, index) => endWordChar == listToCompare[i][index]).Count();

            results.Add(listToCompare[i], similarChars);
        }

        return results.OrderByDescending(x => x.Value).First().Key;
    }

    private static IEnumerable<string> FindOneCharDiffWords(string original, IEnumerable<string> listToCompare)
    {
        return listToCompare.Where(word => IsOneCharDifferent(original, word));
    }

    private static IEnumerable<string> FindOneCharDiffWords(IReadOnlyList<string> originalWords, IReadOnlyCollection<string> listToCompare)
    {
        List<string> candidates = new();

        // ReSharper disable once ForCanBeConvertedToForeach
        for (int i = 0; i < originalWords.Count; i++)
        {
            candidates.AddRange(listToCompare.Where(word => IsOneCharDifferent(originalWords[i], word)));
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

        return true;
    }
}
