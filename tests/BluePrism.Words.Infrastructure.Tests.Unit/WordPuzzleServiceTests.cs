using System;
using System.Collections.Generic;
using System.IO;
using BluePrism.Words.Domain.Models;
using BluePrism.Words.Domain.Services;
using BluePrism.Words.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Sinks.TestCorrelator;
using Xunit;

namespace BluePrism.Words.Infrastructure.Tests.Unit;

public class WordPuzzleServiceTests
{
    private readonly string[] _testDictionary;
    private readonly IWordPuzzleService _wordPuzzleService;

    private IEnumerable<LogEvent> LogEvents => TestCorrelator.GetLogEventsFromContextGuid(_context.Guid);
    private readonly ITestCorrelatorContext _context;

    public WordPuzzleServiceTests()
    {
        _wordPuzzleService = new WordPuzzleService(CreateLogger());

        _testDictionary = File.ReadAllLines("../../../../test-dictionary.txt");
        _context = TestCorrelator.CreateContext();
    }

    private static ILogger<WordPuzzleService> CreateLogger()
    {
        Logger logger = new LoggerConfiguration()
            .WriteTo.TestCorrelator()
            .CreateLogger();
        var loggerFactory = new LoggerFactory();
        loggerFactory.AddProvider(new SerilogLoggerProvider(logger, true));

        return loggerFactory.CreateLogger<WordPuzzleService>();
    }

    [Fact]
    public void Constructor_Should_Throw_Exception_When_Logger_Is_Null()
    {
        ILogger<WordPuzzleService>? logger = null;

        Action act = () => new WordPuzzleService(logger);

        act.Should()
            .ThrowExactly<ArgumentNullException>()
            .And.ParamName.Should()
            .Be(nameof(logger));
    }

    [Theory]
    [InlineData("12345", "good")]
    [InlineData("good", "12345")]
    [InlineData("12345", "12345")]
    public void GetShortestNumberOfStepsBetweenWords_Should_Log_Warning_If_Words_Are_Longer_Than_4_Chars(string start, string end)
    {
        var options = new StepsBetweenWordsOptions
        {
            Start = start,
            End = end,
            Dictionary = _testDictionary
        };

        _wordPuzzleService.GetShortestNumberOfStepsBetweenWords(options);

        LogEvents.Should()
            .Contain(x =>
                x.Level == LogEventLevel.Warning &&
                x.MessageTemplate.Text.Contains("Invalid parameters"));
    }

    [Theory]
    [InlineData("12345", "good")]
    [InlineData("good", "12345")]
    [InlineData("12345", "12345")]
    public void GetShortestNumberOfStepsBetweenWords_Should_Return_Empty_Array_If_Words_Are_Longer_Than_4_Chars(string start, string end)
    {
        var options = new StepsBetweenWordsOptions
        {
            Start = start,
            End = end,
            Dictionary = _testDictionary
        };

        IEnumerable<string> sequence = _wordPuzzleService.GetShortestNumberOfStepsBetweenWords(options);

        sequence.Should().BeEmpty();
    }

    [Theory]
    [MemberData(nameof(ValidSequences))]
    public void GetShortestNumberOfStepsBetweenWords_Should_Return_Valid_Sequences(string start, string end, string[] expectedSequence)
    {
        var options = new StepsBetweenWordsOptions
        {
            Start = start,
            End = end,
            Dictionary = _testDictionary
        };

        IEnumerable<string> sequence = _wordPuzzleService.GetShortestNumberOfStepsBetweenWords(options);

        sequence.Should().BeEquivalentTo(expectedSequence);
    }

    public static IEnumerable<object[]> ValidSequences()
    {
        return new List<object[]>
        {
            new object[] { "hide", "sort", new[] { "hide", "hire", "sire", "sore", "sort" } },
            new object[] { "hide", "sore", new[] { "hide", "hire", "sire", "sore" } },
            new object[] { "hide", "sire", new[] { "hide", "hire", "sire" } },
            new object[] { "hide", "hire", new[] { "hide", "hire" } },
            new object[] { "hide", "hide", new[] { "hide" } },
            new object[] { "spin", "spot", new[] { "spin", "spit", "spot" } },
            new object[] { "same", "cost", new[] { "same", "came", "case", "cast", "cost" } },
        };
    }
}
