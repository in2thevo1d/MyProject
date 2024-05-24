using MyProjectDomain.Dto;
using MyProjectDomain.Result;

namespace MyProjectDomain.Interfaces.Services
{
    public interface IReportService
    {
        Task<CollectionResult<ReportDto>> GetReportsAsync(long userId);

        Task<BaseResult<ReportDto>> GetReportByIdAsync(long Id);

        Task<BaseResult<ReportDto>> CreateReportAsync(CreateReportDto dto);

        Task<BaseResult<ReportDto>> DeleteReportAsync(long Id);

        Task<BaseResult<ReportDto>> UpdateReportAsync(UpdateReportDto dto);
    }
}
