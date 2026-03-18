using FluentValidation;

namespace DddTemplate.Application.TodoItems.Commands;

public sealed class CreateTodoItemCommandValidator : AbstractValidator<CreateTodoItemCommand>
{
    public CreateTodoItemCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("标题不能为空")
            .MaximumLength(200).WithMessage("标题长度不能超过200个字符");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("描述长度不能超过1000个字符")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}
