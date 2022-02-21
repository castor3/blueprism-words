using Ardalis.GuardClauses;
using BluePrism.Words.Domain.Services;
using Microsoft.Extensions.Logging;

namespace BluePrism.Words.Infrastructure.Services;

internal class DictionaryHandler : IDictionaryHandler
{
    private readonly ILogger<DictionaryHandler> _logger;

    public DictionaryHandler(ILogger<DictionaryHandler> logger)
    {
        _logger = Guard.Against.Null(logger);
    }

    public string[] LoadDictionary(string filePath)
    {
        if (!TryLoadDictionary(filePath, out string[] dictionary))
        {
            _logger.LogWarning("Failed to load dictionary from '{filePath}'.", filePath);
            return Array.Empty<string>();
        }

        if (dictionary.Any())
        {
            return dictionary;
        }

        _logger.LogWarning("Dictionary '{filePath}' is empty.", filePath);
        return Array.Empty<string>();
    }

    private static bool TryLoadDictionary(string filePath, out string[] dictionary)
    {
        if (File.Exists(filePath))
        {
            dictionary = File.ReadAllLines(filePath);
            return true;
        }

        dictionary = Array.Empty<string>();
        return false;
    }
}
