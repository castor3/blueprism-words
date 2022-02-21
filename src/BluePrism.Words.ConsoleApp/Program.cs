using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using BluePrism.Words.Domain.Models;
using BluePrism.Words.Domain.Services;
using BluePrism.Words.Infrastructure.Services;
using Cocona;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BluePrism.Words.ConsoleApp
{
    internal class Program
    {
        [ExcludeFromCodeCoverage]
        public static async Task Main(string[] args)
        {
            await CoconaApp.CreateHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IWordPuzzleService, WordPuzzleService>();
                    services.AddSingleton<IDictionaryHandler, DictionaryHandler>();
                })
                .RunAsync<Program>(args);
        }

        public void Command(
            [StringLength(maximumLength: 4, MinimumLength = 4)]
            string start,
            [StringLength(maximumLength: 4, MinimumLength = 4)]
            string end,
            string dictionarypath,
            string outputpath,
            [FromService] ILogger<Program> logger,
            [FromService] IDictionaryHandler dictionaryHandler,
            [FromService] IWordPuzzleService wordPuzzleService
        )
        {
            Console.Clear();

            if (start.Equals(end, StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine(start);
                return;
            }

            string[] dictionary = dictionaryHandler.LoadDictionary(dictionarypath);
            if (!dictionary.Any())
            {
                return;
            }

            var options = new StepsBetweenWordsOptions { Start = start, End = end, Dictionary = dictionary };
            IEnumerable<string> wordSequence = wordPuzzleService.GetShortestNumberOfStepsBetweenWords(options).ToList();
            if (!wordSequence.Any())
            {
                logger.LogWarning("Failed to find sequence for\nStart: {start}\nEnd: {end}", start, end);
                return;
            }

            using (FileStream fileStream = File.OpenWrite(outputpath))
            {
                byte[] data = new UTF8Encoding(true).GetBytes(string.Join(" - ", wordSequence)); 
                fileStream.Write(data, 0, data.Length);
            }

            Console.WriteLine(string.Join(" - ", wordSequence));
        }
    }
}
