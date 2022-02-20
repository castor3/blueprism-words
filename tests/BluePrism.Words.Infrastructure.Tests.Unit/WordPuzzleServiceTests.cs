using System;
using System.Collections.Generic;
using BluePrism.Words.Domain.Services;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BluePrism.Words.Infrastructure.Tests.Unit;

public class WordPuzzleServiceTests
{
    private readonly IWordPuzzleService _wordPuzzleService;

    public WordPuzzleServiceTests()
    {
        _wordPuzzleService = Substitute.For<IWordPuzzleService>();
    }

    [Theory]
    [InlineData("12345", "good")]
    [InlineData("good", "12345")]
    [InlineData("12345", "12345")]
    public void GetShortestNumberOfStepsBetweenWords_Should_Throw_Exception_If_Words_Are_Longer_Than_4_Chars(string start, string end)
    {
        Action act = () => _wordPuzzleService.GetShortestNumberOfStepsBetweenWords(start, end);

        act.Should()
            .ThrowExactly<ArgumentException>()
            .WithMessage("Parameters should be exactly 4 characters long.");
    }

    [Fact]
    public void GetShortestNumberOfStepsBetweenWords_Should_Not_Return_Start_And_End_Words()
    {
        const string start = "hide";
        const string end = "sort";
        
        IEnumerable<string> sequence = _wordPuzzleService.GetShortestNumberOfStepsBetweenWords(start, end);

        sequence.Should()
            .NotBeNullOrEmpty()
            .And.NotContain(start)
            .And.NotContain(end);
    }
}
