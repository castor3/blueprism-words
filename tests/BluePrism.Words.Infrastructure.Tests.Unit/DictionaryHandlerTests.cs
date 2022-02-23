using System;
using System.Collections.Generic;
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

public class DictionaryHandlerTests
{
    private readonly IDictionaryHandler _dictionaryHandler;

    private IEnumerable<LogEvent> LogEvents => TestCorrelator.GetLogEventsFromContextGuid(_context.Guid);
    private readonly ITestCorrelatorContext _context;
    
    private const string ValidDictionary = "../../../../test-dictionary.txt";
    private const string EmptyDictionary = "../../../../empty-dictionary.txt";

    public DictionaryHandlerTests()
    {
        _dictionaryHandler = new DictionaryHandler(CreateLogger());

        _context = TestCorrelator.CreateContext();
    }

    private static ILogger<DictionaryHandler> CreateLogger()
    {
        Logger logger = new LoggerConfiguration()
            .WriteTo.TestCorrelator()
            .CreateLogger();
        var loggerFactory = new LoggerFactory();
        loggerFactory.AddProvider(new SerilogLoggerProvider(logger, true));

        return loggerFactory.CreateLogger<DictionaryHandler>();
    }

    [Fact]
    public void Constructor_Should_Throw_Exception_When_Logger_Is_Null()
    {
        ILogger<DictionaryHandler>? logger = null;

        Action act = () => new DictionaryHandler(logger);

        act.Should()
            .ThrowExactly<ArgumentNullException>()
            .And.ParamName.Should()
            .Be(nameof(logger));
    }

    [Fact]
    public void LoadDictionary_Should_Log_Warning_When_File_Is_Not_Found()
    {
        _dictionaryHandler.LoadDictionary("fakePath");

        LogEvents.Should()
            .Contain(x =>
                x.Level == LogEventLevel.Warning &&
                x.MessageTemplate.Text.Contains("Failed to load dictionary from '{filePath}'."));
    }

    [Fact]
    public void LoadDictionary_Should_Return_Empty_Array_When_File_Is_Not_Found()
    {
        string[] dictionary = _dictionaryHandler.LoadDictionary("fakePath");

        dictionary.Should().BeEmpty();
    }

    [Fact]
    public void LoadDictionary_Should_Log_Warning_When_File_Is_Empty()
    {
        _dictionaryHandler.LoadDictionary(EmptyDictionary);

        LogEvents.Should()
            .Contain(x =>
                x.Level == LogEventLevel.Warning &&
                x.MessageTemplate.Text.Contains("Dictionary '{filePath}' is empty."));
    }

    [Fact]
    public void LoadDictionary_Should_Return_Empty_Array_When_File_Is_Empty()
    {
        string[] dictionary = _dictionaryHandler.LoadDictionary(EmptyDictionary);

        dictionary.Should().BeEmpty();
    }

    [Fact]
    public void LoadDictionary_Should_Not_Return_Empty_Array_When_File_Is_Valid()
    {
        string[] dictionary = _dictionaryHandler.LoadDictionary(ValidDictionary);

        dictionary.Should().NotBeEmpty();
    }
}
