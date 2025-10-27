
using FarmManagementAPI.FarmManagement.Shared.Dtos;


namespace FarmManagementAPI.FarmManagement.Core.Interfaces.IServices;

public interface IReportService
{
    Task<byte[]> GenerateFarmReportPdfAsync(Guid farmId);
    Task<byte[]> GenerateUserReportPdfAsync(Guid userId);
    Task<byte[]> GenerateFinancialReportPdfAsync(Guid userId, DateTime startDate, DateTime endDate);
    Task<ExportDataDto> ExportFarmDataAsync(Guid userId, string format);
}