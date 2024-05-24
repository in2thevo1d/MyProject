namespace MyProjectDomain.Enum
{
    public enum ErrorCodes
    {
        ReportsNotFound = 0,
        ReportNotFound = 1,
        ReportAlreadyExist = 2,
        InternalServerError = 3,
        UserAlreadyExist = 4,
        UserNotFound = 5,
        PasswordsDontMatch = 6,
        WrongPassword = 7,
        UserUnauthorizedAccess = 8,
        RoleAlreadyExist = 9,
        RoleNotFound = 10,
        UserAlreadyExistThisRole = 11,
    }
}
