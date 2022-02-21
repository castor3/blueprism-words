using System.ComponentModel.DataAnnotations;
using BluePrism.Words.Domain.Services;
using BluePrism.Words.Infrastructure.Services;
using Cocona;
using Cocona.Builder;
using Microsoft.Extensions.DependencyInjection;

CoconaAppBuilder builder = CoconaApp.CreateBuilder();

builder.Services.AddSingleton<IWordPuzzleService, WordPuzzleService>();

CoconaApp app = builder.Build();

app.AddCommand((
    [StringLength(maximumLength: 4, MinimumLength = 4)]string start, 
    [StringLength(maximumLength: 4, MinimumLength = 4)]string end,
    string filePath, [FromService]IWordPuzzleService wordPuzzleService) =>
{
    IEnumerable<string> wordSequence = wordPuzzleService.GetShortestNumberOfStepsBetweenWords(start, end, filePath);

    Console.WriteLine(string.Join(" - ", wordSequence));
});

app.Run();
