using FluentValidation;
using MyProjectDomain.Dto;

namespace MyProjectApplication.Validations.FluentValidations
{
    public class CreateReportValidator : AbstractValidator<CreateReportDto>
    {
        public CreateReportValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        }
    }
}
