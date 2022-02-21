using System;
using System.Collections.Generic;
using BluePrism.Words.Domain.Models;
using BluePrism.Words.Domain.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Sinks.TestCorrelator;
using Xunit;

namespace BluePrism.Words.ConsoleApp.Tests.Unit;

public class ProgramTests
{
    private readonly IDictionaryHandler _dictionary;
    private readonly IWordPuzzleService _wordPuzzleService;
    private readonly Program _program;

    private IEnumerable<LogEvent> LogEvents => TestCorrelator.GetLogEventsFromContextGuid(_context.Guid);
    private readonly ITestCorrelatorContext _context;

    public ProgramTests()
    {
        _dictionary = Substitute.For<IDictionaryHandler>();
        _wordPuzzleService = Substitute.For<IWordPuzzleService>();
        _program = new Program();

        _context = TestCorrelator.CreateContext();
    }

    private static ILogger<Program> CreateLogger()
    {
        Logger logger = new LoggerConfiguration()
            .WriteTo.TestCorrelator()
            .CreateLogger();
        var loggerFactory = new LoggerFactory();
        loggerFactory.AddProvider(new SerilogLoggerProvider(logger, true));

        return loggerFactory.CreateLogger<Program>();
    }

    [Fact]
    public void Command_Should_Not_Call_LoadDictionary_If_Start_And_End_Are_Equal()
    {
        _program.Command(
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            CreateLogger(),
            _dictionary,
            _wordPuzzleService);

        _dictionary.DidNotReceive().LoadDictionary(Arg.Any<string>());
    }

    [Fact]
    public void Command_Should_Not_Call_WordPuzzleService_If_LoadDictionary_Returns_Empty_Array()
    {
        _dictionary
            .LoadDictionary(string.Empty)
            .Returns(Array.Empty<string>());
        
        _program.Command(
            "spin",
            "spot",
            string.Empty,
            string.Empty,
            CreateLogger(),
            _dictionary,
            _wordPuzzleService);

        _dictionary.Received().LoadDictionary(Arg.Any<string>());
        _wordPuzzleService
            .DidNotReceive()
            .GetShortestNumberOfStepsBetweenWords(Arg.Any<StepsBetweenWordsOptions>());
    }

    [Fact]
    public void Command_Should_Log_Warning_When_GetShortestNumberOfStepsBetweenWords_Returns_Empty_List()
    {
        _dictionary
            .LoadDictionary(string.Empty)
            .Returns(new[] { "word" });
        _wordPuzzleService
            .GetShortestNumberOfStepsBetweenWords(new StepsBetweenWordsOptions())
            .Returns(Array.Empty<string>());

        _program.Command(
            "spin",
            "spot",
            string.Empty,
            string.Empty,
            CreateLogger(),
            _dictionary,
            _wordPuzzleService);

        LogEvents.Should()
            .Contain(x =>
                x.Level == LogEventLevel.Warning &&
                x.MessageTemplate.Text.Contains("Failed to find sequence for"));
    }
}
