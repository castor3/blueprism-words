namespace BluePrism.Words.Domain.Services;

public interface IWordPuzzleService
{
    IEnumerable<string> GetShortestNumberOfStepsBetweenWords(string start, string end);
}
