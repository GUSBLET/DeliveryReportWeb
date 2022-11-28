using OfficeOpenXml.Style;

namespace BusinessLogic.Implemantations
{
    public class ReportPanelService : IReportPanelService
    {
        private readonly ILogger<AccountService> _logger;
        private readonly IBaseRepository<ReportOfDelivary> _reportRepository;


        public ReportPanelService(ILogger<AccountService> logger, IBaseRepository<ReportOfDelivary> reportRepository)
        {
            _logger = logger;
            _reportRepository = reportRepository;
        }

        public async Task<BaseResponse<bool>> CreateReport(CreateReportViewModel model)
        {
            try
            {
                var report = new ReportOfDelivary()
                {
                    UserId = model.UserId,
                    County = model.County,
                    BeginTime = model.BeginTime,
                    EndTime = model.EndTime,
                    WorkingTime = model.WorkingTime,
                    ReportDate = model.ReportDate,
                    DistancePassed = model.DistancePassed
                };
                if (await _reportRepository.Add(report))
                    return new BaseResponse<bool>
                    {
                        Data = true,
                        StatusCode = HttpStatusCode.OK,
                    };

                return new BaseResponse<bool>
                {
                    Data = false,
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Report panel service 'Method: CreateReport'" + ex.Message);
                return new BaseResponse<bool>
                {
                    Data = false,
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }

        }

        public async Task<BaseResponse<CustomFile>> GenerateReportsFile(List<ControlReportViewModal> list, string language)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.Commercial;
                var package = new ExcelPackage();

                var sheet = package.Workbook.Worksheets
                    .Add("List Report");
                if(language == "c=en-US|uic=en-US")
                {
                    sheet.Cells[1, 1, 1, 1].LoadFromArrays(new object[][] { new[] { "Report-Id", "User-Id", "County", "Full name",
                                                                    "Date", "Distance passed", "Begin time", "End time", "Working time"} });
                }
                else if(language == "c=de-DE|uic=de-DE")
                {
                    sheet.Cells[1, 1, 1, 1].LoadFromArrays(new object[][] { new[] { "Berichts-ID", "User-Id", "Bezirk", "Vollständiger Name",
                                                                    "Datum", "Strecke zurückgelegt", "Zeit beginnen", "Endzeit", "Arbeitszeit"} });
                }
                
                var row = 2;
                var column = 1;
                //sheet.Cells[1, 1, row, column + 1].AutoFitColumns();
                sheet.Column(1).Width = 20;
                sheet.Column(2).Width = 20;
                sheet.Column(3).Width = 45;
                sheet.Column(4).Width = 20;
                sheet.Column(5).Width = 20;
                sheet.Column(6).Width = 20;
                sheet.Column(7).Width = 20;
                sheet.Column(8).Width = 20;
                sheet.Column(9).Width = 20;
                sheet.Cells[1, 1, 1 + list.Count , 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[1, 1, 1 + list.Count, 9].Style.Font.Bold = true;
                sheet.Cells[1, 1, 1 + list.Count, 9].Style.Border.BorderAround(ExcelBorderStyle.Double);
                sheet.Cells[1, 1, 1 + list.Count, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                foreach (var item in list)
                {
                    int hour = item.WorkingTime.Hour;
                    int minute = item.WorkingTime.Minute;
                    sheet.Cells[row, column].Value = item.ReportId;
                    sheet.Cells[row, column+1].Value =item.UserId;
                    sheet.Cells[row, column+2].Value = item.County;
                    sheet.Cells[row, column+3].Value = item.FullName;
                    sheet.Cells[row, column+4].Value = item.ReportDate;
                    sheet.Cells[row, column+5].Value = item.DistancePassed;
                    sheet.Cells[row, column+6].Value = item.BeginTime;
                    sheet.Cells[row, column+7].Value = item.EndTime;
                    sheet.Cells[row, column+8].Value = $"{hour}:{minute}";
                    row++;
                }

                var result = new CustomFile()
                {
                    FileContents = package.GetAsByteArray(),
                    FileName = "Reports.xlsx",
                    ContentType = "application/octet-stream"
                };

                //File.WriteAllBytes("Report.xlsx", package.GetAsByteArray());

                return new BaseResponse<CustomFile>
                {
                    Data = result,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BaseResponse<List<ReportOfDelivary>>> SelectAllReportList()
        {
            try
            {
                return new BaseResponse<List<ReportOfDelivary>>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Data = await _reportRepository.Select().ToListAsync()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Report panel service 'Method: SelectReportListByIdUser'" + ex.Message);
                return new BaseResponse<List<ReportOfDelivary>>
                {
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<ReportOfDelivary>> SelectReportById(ulong id)
        {
            try
            {
                var result = await (from p in _reportRepository.Select()
                                    where p.Id == id
                                    select new ReportOfDelivary
                                    {
                                        Id = p.Id,
                                        BeginTime = p.BeginTime,
                                        EndTime = p.EndTime,
                                        County = p.County,
                                        DistancePassed = p.DistancePassed,
                                        UserId = p.UserId,
                                        ReportDate = p.ReportDate,
                                        WorkingTime = p.WorkingTime
                                    }).FirstOrDefaultAsync();

                return new BaseResponse<ReportOfDelivary>
                {
                    StatusCode = HttpStatusCode.OK,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Report panel service 'Method: SelectReportById'" + ex.Message);
                return new BaseResponse<ReportOfDelivary>
                {
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<List<ReportOfDelivary>>> SelectReportListByIdUser(ulong id)
        {
            try
            {
                return new BaseResponse<List<ReportOfDelivary>>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Data = await (from p in _reportRepository.Select()
                                  where p.UserId == id
                                  select new ReportOfDelivary
                                  {
                                      Id = p.Id,
                                      UserId = p.UserId,

                                      County = p.County,
                                      BeginTime = p.BeginTime,
                                      EndTime = p.EndTime,
                                      WorkingTime = p.WorkingTime,
                                      ReportDate = p.ReportDate,
                                      DistancePassed = p.DistancePassed
                                  }).ToListAsync()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Report panel service 'Method: SelectReportListByIdUser'" + ex.Message);
                return new BaseResponse<List<ReportOfDelivary>>
                {
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

    }
}
