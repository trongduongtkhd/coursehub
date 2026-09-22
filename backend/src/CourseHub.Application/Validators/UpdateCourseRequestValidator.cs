using CourseHub.Application.DTOs.Courses;
using CourseHub.Domain.Enums;
using FluentValidation;

namespace CourseHub.Application.Validators;

public class UpdateCourseRequestValidator : AbstractValidator<UpdateCourseRequest>
{
    public UpdateCourseRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Status)
            .NotEmpty()
            .Must(status => Enum.TryParse<CourseStatus>(status, true, out _))
            .WithMessage("Trạng thái phải là một trong: Draft, Published, Archived.");
    }
}