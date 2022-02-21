using BluePrism.Words.Domain.Models;
using FluentValidation;

namespace BluePrism.Words.Infrastructure.ModelValidators;

internal class StepsBetweenWordsOptionsValidator : AbstractValidator<StepsBetweenWordsOptions>
{
    public StepsBetweenWordsOptionsValidator()
    {
        RuleFor(x => x.Start).NotEmpty().Length(4, 4);
        RuleFor(x => x.End).NotEmpty().Length(4, 4);
        RuleFor(x => x.Dictionary).NotEmpty();
    }
}
