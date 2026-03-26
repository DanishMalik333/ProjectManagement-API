using FluentValidation;
using ProjectManagement.Core.DTOs.Teams;

namespace ProjectManagement.Services.Validators;

public class CreateTeamRequestValidator : AbstractValidator<CreateTeamRequest>
{
    public CreateTeamRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Team name is required")
            .Length(1, 100).WithMessage("Team name must be between 1 and 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}

public class AddTeamMemberRequestValidator : AbstractValidator<AddTeamMemberRequest>
{
    public AddTeamMemberRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be valid");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required")
            .Must(r => Enum.TryParse<Core.Enums.TeamMemberRole>(r, true, out _))
            .WithMessage("Role must be a valid team member role (Admin, Lead, Member, Viewer)");
    }
}

public class UpdateTeamMemberRoleRequestValidator : AbstractValidator<UpdateTeamMemberRoleRequest>
{
    public UpdateTeamMemberRoleRequestValidator()
    {
        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required")
            .Must(r => Enum.TryParse<Core.Enums.TeamMemberRole>(r, true, out _))
            .WithMessage("Role must be a valid team member role (Admin, Lead, Member, Viewer)");
    }
}
