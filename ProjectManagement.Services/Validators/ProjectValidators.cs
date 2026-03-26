using FluentValidation;
using ProjectManagement.Core.DTOs.Projects;

namespace ProjectManagement.Services.Validators;

public class UpdateProjectRequestValidator : AbstractValidator<UpdateProjectRequest>
{
    public UpdateProjectRequestValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Project name must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Status)
            .Must(s => string.IsNullOrEmpty(s) || Enum.TryParse<Core.Enums.ProjectStatus>(s, true, out _))
            .WithMessage("Status must be a valid project status (Planning, Active, OnHold, Completed)")
            .When(x => !string.IsNullOrEmpty(x.Status));
    }
}
