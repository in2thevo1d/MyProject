using MyProjectDomain.Dto;
using MyProjectDomain.Interfaces.Services;
using MyProjectDomain.Result;
using Microsoft.EntityFrameworkCore;
using Serilog;
using MyProjectDomain.Enum;
using MyProjectDomain.Validations;
using AutoMapper;
using MyProjectDomain.Entity;
using MyProjectDomain.Interfaces.Repositories;

namespace MyProjectApplication.Services
{
    public class ReportService : IReportService
    {
        private readonly IBaseRepository<Report> _reportRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IReportValidator _reportValidator;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ReportService(IBaseRepository<Report> reportRepository, ILogger logger, IBaseRepository<User> userRepository, IReportValidator reportValidator, IMapper mapper)
        {
            _reportRepository = reportRepository;
            _logger = logger;
            _mapper = mapper;
            _userRepository = userRepository;
            _reportValidator = reportValidator;
        }

        public async Task<CollectionResult<ReportDto>> GetReportsAsync(long userId)
        {
            ReportDto[] reports;
            try
            {
                reports = await _reportRepository.GetAll()
                    .Where(x => x.UserId == userId)
                    .Select(x => new ReportDto(x.Id, x.Name, x.Description, x.CreatedAt.ToLongDateString()))
                    .ToArrayAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return new CollectionResult<ReportDto>()
                {
                    ErrorMessage = "Internal server error",
                    ErrorCode = (int)ErrorCodes.InternalServerError
                };
            }

            if (!reports.Any())
            {
                _logger.Warning("Reports not found", reports.Length);
                return new CollectionResult<ReportDto>()
                {
                    ErrorMessage = "Reports not found",
                    ErrorCode = (int)ErrorCodes.ReportsNotFound
                };
            }

            return new CollectionResult<ReportDto>()
            {
                Data = reports,
                Count = reports.Length
            };
        }

        public Task<BaseResult<ReportDto>> GetReportByIdAsync(long id)
        {
            ReportDto? report;
            try
            {
                report = _reportRepository.GetAll()
                    .AsEnumerable()
                    .Select(x => new ReportDto(x.Id, x.Name, x.Description, x.CreatedAt.ToLongDateString()))
                    .FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return Task.FromResult(new BaseResult<ReportDto>()
                {
                    ErrorMessage = "Internal server error",
                    ErrorCode = (int)ErrorCodes.InternalServerError
                });
            }

            if (report == null)
            {
                _logger.Warning($"Report with {id} not found", id);
                return Task.FromResult(new BaseResult<ReportDto>()
                {
                    ErrorMessage = "Report not found",
                    ErrorCode = (int)ErrorCodes.ReportNotFound
                });
            }

            return Task.FromResult(new BaseResult<ReportDto>()
            {
                Data = report
            });
        }

        public async Task<BaseResult<ReportDto>> CreateReportAsync(CreateReportDto dto)
        {
            var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.UserId);
            var report = await _reportRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.Name);
            var result = _reportValidator.CreateValidator(report, user);
            if (result.IsSuccess)
            {
                return new BaseResult<ReportDto>()
                {
                    ErrorMessage = result.ErrorMessage,
                    ErrorCode = result.ErrorCode
                };
            }

            report = new Report()
            {
                Name = dto.Name,
                Description = dto.Description,
                UserId = user.Id,
            };
            await _reportRepository.CreateAsync(report);
            return new BaseResult<ReportDto>()
            {
                Data = _mapper.Map<ReportDto>(report)
            };

        }

        public async Task<BaseResult<ReportDto>> DeleteReportAsync(long Id)
        {
            var report = await _reportRepository.GetAll().FirstOrDefaultAsync(x => x.Id == Id);
            var result = _reportValidator.ValidateOnNull(report);
            if (!result.IsSuccess)
            {
                return new BaseResult<ReportDto>()
                {
                    ErrorMessage = result.ErrorMessage,
                    ErrorCode = result.ErrorCode
                };
            }

            _reportRepository.Remove(report);
            await _reportRepository.SaveChangesAsync();
            return new BaseResult<ReportDto>()
            {
                Data = _mapper.Map<ReportDto>(report)
            };
        }

        public async Task<BaseResult<ReportDto>> UpdateReportAsync(UpdateReportDto dto)
        {

            var report = await _reportRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.Id);
            var result = _reportValidator.ValidateOnNull(report);
            if (!result.IsSuccess)
            {
                return new BaseResult<ReportDto>()
                {
                    ErrorMessage = result.ErrorMessage,
                    ErrorCode = result.ErrorCode
                };
            }

            report.Name = dto.Name;
            report.Description = dto.Description;

            var updatedReport = _reportRepository.Update(report);
            await _reportRepository.SaveChangesAsync();
            return new BaseResult<ReportDto>()
            {
                Data = _mapper.Map<ReportDto>(updatedReport)
            };
        }
    }
}
