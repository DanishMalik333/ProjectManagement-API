using FluentValidation;
using ProjectManagement.Core.DTOs.Sprints;

namespace ProjectManagement.Services.Validators;

public class CreateSprintRequestValidator : AbstractValidator<CreateSprintRequest>
{
    public CreateSprintRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Sprint name is required")
            .Length(1, 100).WithMessage("Sprint name must be between 1 and 100 characters");

        RuleFor(x => x.Goal)
            .MaximumLength(500).WithMessage("Goal must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Goal));

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .WithMessage("End date must be after start date");
    }
}

public class UpdateSprintRequestValidator : AbstractValidator<UpdateSprintRequest>
{
    public UpdateSprintRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Sprint name is required")
            .Length(1, 100).WithMessage("Sprint name must be between 1 and 100 characters");

        RuleFor(x => x.Goal)
            .MaximumLength(500).WithMessage("Goal must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Goal));

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .WithMessage("End date must be after start date");
    }
}

public class CompleteSprintRequestValidator : AbstractValidator<CompleteSprintRequest>
{
    public CompleteSprintRequestValidator()
    {
        RuleFor(x => x.IncompleteTaskAction)
            .NotEmpty().WithMessage("Incomplete task action is required");
    }
}
