using FluentValidation;
using ProjectManagement.Core.DTOs.Tasks;

namespace ProjectManagement.Services.Validators;

public class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequest>
{
    public UpdateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Task title is required")
            .Length(1, 200).WithMessage("Task title must be between 1 and 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.StoryPoints)
            .Must(sp => sp == null || new[] { 1, 2, 3, 5, 8, 13, 21 }.Contains(sp.Value))
            .WithMessage("Story points must be one of: 1, 2, 3, 5, 8, 13, 21")
            .When(x => x.StoryPoints.HasValue);

        RuleFor(x => x.Priority)
            .NotEmpty().WithMessage("Priority is required")
            .Must(p => Enum.TryParse<Core.Enums.TaskPriority>(p, true, out _))
            .WithMessage("Priority must be a valid task priority (Low, Medium, High, Urgent)");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Type is required")
            .Must(t => Enum.TryParse<Core.Enums.TaskType>(t, true, out _))
            .WithMessage("Type must be a valid task type (Story, Bug, Task, Improvement)");

        RuleFor(x => x.DueDate)
            .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage("Due date must be in the future")
            .When(x => x.DueDate.HasValue);
    }
}

public class UpdateTaskStatusRequestValidator : AbstractValidator<UpdateTaskStatusRequest>
{
    public UpdateTaskStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required")
            .Must(s => Enum.TryParse<Core.Enums.TaskStatus>(s, true, out _))
            .WithMessage("Status must be a valid task status (Todo, InProgress, InReview, Done)");
    }
}

public class ReorderTaskRequestValidator : AbstractValidator<ReorderTaskRequest>
{
    public ReorderTaskRequestValidator()
    {
        RuleFor(x => x.OrderIndex)
            .GreaterThanOrEqualTo(0).WithMessage("Order index must be non-negative");
    }
}
