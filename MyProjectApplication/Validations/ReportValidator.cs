using MyProjectDomain.Entity;
using MyProjectDomain.Enum;
using MyProjectDomain.Result;
using MyProjectDomain.Validations;

namespace MyProjectApplication.Validations
{
    public class ReportValidator : IReportValidator
    {
        public BaseResult CreateValidator(Report report, User user)
        {
            if (report == null)
            {
                return new BaseResult()
                {
                    ErrorMessage = "Report already exist",
                    ErrorCode = (int)ErrorCodes.ReportAlreadyExist
                };
            }

            if (user == null)
            {
                return new BaseResult()
                {
                    ErrorMessage = "User not found",
                    ErrorCode = (int)ErrorCodes.UserNotFound
                };
            }

            return new BaseResult();
        }

        public BaseResult ValidateOnNull(Report model)
        {
            if (model == null)
            {
                return new BaseResult()
                {
                    ErrorMessage = "Not found",
                    ErrorCode = (int)ErrorCodes.ReportNotFound
                };
            }

            return new BaseResult();
        }
    }
}
