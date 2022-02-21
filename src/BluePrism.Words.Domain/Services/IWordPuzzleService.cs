using BluePrism.Words.Domain.Models;

namespace BluePrism.Words.Domain.Services;

public interface IWordPuzzleService
{
    IEnumerable<string> GetShortestNumberOfStepsBetweenWords(StepsBetweenWordsOptions options);
}
