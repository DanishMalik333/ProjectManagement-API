using FluentValidation;
using ProjectManagement.Core.DTOs.Comments;

namespace ProjectManagement.Services.Validators;

public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
{
    public CreateCommentRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Comment content is required")
            .Length(1, 5000).WithMessage("Comment must be between 1 and 5000 characters");
    }
}

public class UpdateCommentRequestValidator : AbstractValidator<UpdateCommentRequest>
{
    public UpdateCommentRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Comment content is required")
            .Length(1, 5000).WithMessage("Comment must be between 1 and 5000 characters");
    }
}
