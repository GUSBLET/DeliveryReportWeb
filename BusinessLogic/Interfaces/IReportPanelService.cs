

namespace BusinessLogic.Interfaces
{
    public interface IReportPanelService
    {
        Task<BaseResponse<bool>> CreateReport(CreateReportViewModel model);
        Task<BaseResponse<List<ReportOfDelivary>>> SelectReportListByIdUser(ulong id);
        Task<BaseResponse<ReportOfDelivary>> SelectReportById(ulong id);
        Task<BaseResponse<List<ReportOfDelivary>>> SelectAllReportList();
        Task<BaseResponse<CustomFile>> GenerateReportsFile(List<ControlReportViewModal> model, string language);
    }
}
