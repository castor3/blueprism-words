namespace BluePrism.Words.Domain.Services;

public interface IDictionaryHandler
{
    string[] LoadDictionary(string filePath);
}
