﻿using FluentValidation;
using MyProjectDomain.Dto;

namespace MyProjectApplication.Validations.FluentValidations
{
    public class UpdateReportValidator : AbstractValidator<UpdateReportDto>
    {
        public UpdateReportValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        }
    }
}
