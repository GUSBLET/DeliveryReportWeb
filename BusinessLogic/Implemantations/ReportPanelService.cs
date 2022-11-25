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
