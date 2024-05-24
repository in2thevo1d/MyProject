using MyProjectDomain.Result;
using MyProjectDomain.Entity;

namespace MyProjectDomain.Validations
{
    public interface IReportValidator : IBaseValidator<Report>
    {
        BaseResult CreateValidator(Report report, User user);
    }
}
