using System.Data;

namespace UI.Controllers
{

    [Authorize(Roles = "Admin, Manager, Deliveryman")]
    public class ReportPanelController : Controller
    {
        private readonly IReportPanelService _reportPanelService;
        private readonly IAccountService _accountService;

        public ReportPanelController(
            [FromServices] IReportPanelService reportPanelService,
            [FromServices] IAccountService accountService)
        {
            _reportPanelService = reportPanelService;
            _accountService = accountService;
        }


        [HttpGet]
        public IActionResult CreateReport()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateReport(CreateReportViewModel model)
        { 
            if (ModelState.IsValid) 
            {
                model.ReportDate = DateOnly.FromDateTime(DateTime.Now);
                model.UserId = _accountService.GetUserByEmail(User.Identity.Name ?? " ").Result.Data.Id;
                var response = await _reportPanelService.CreateReport(model);   
                if( response.StatusCode == System.Net.HttpStatusCode.OK ) 
                {
                    return View("Success", "ReportCreatedMessage");
                }
                return View("Success", "ReportDidNotCreatMessage");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DelivarymanReportsViewPage(string? lastName, string? name,
                                                                int? month, int? day, int? year,
                                                                string? county, int page = 1)
        {
            DateOnly date;
            int pageSize = 15;   // size elemt on page
            if (month is null || day is null || year is null)
                date = new DateOnly();
            else
                date = new DateOnly((int)year, (int)month, (int)day);
            ulong userId = _accountService.GetUserByEmail(User.Identity.Name ?? " ").Result.Data.Id;
            var response = await _reportPanelService.SelectReportListByIdUser(userId);

            if (response.Data is null)
                response.Data = new List<ReportOfDelivary>();

            IEnumerable<ReportOfDelivary> source = response.Data;
            if (date != DateOnly.MinValue)
            {
                source = source.Where(p => p.ReportDate == date);
            }
            if (!string.IsNullOrEmpty(county))
            {
                source = source.Where(p => p.County!.Contains(county));
            }


            var count = source.Count();
            var items = source.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);

            ViewReportsViewModel viewModel = new ViewReportsViewModel(items, pageViewModel,
                new ReportFilterUserMangeViewModel(lastName, name, date, county));
            return View("ViewReportsViewPage", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ControlReportsViewPage(string? lastName, string? name, 
                                                                int? month, int? day, int? year,
                                                                string? county, int page = 1)
        {
            DateOnly date;
            int pageSize = 15;   // size elemt on page
            if (month is null || day is null || year is null)
                date = new DateOnly();
            else
                date = new DateOnly((int)year, (int)month, (int)day);
            ulong userId = _accountService.GetUserByEmail(User.Identity.Name ?? " ").Result.Data.Id;
            var response =
                from reports in _reportPanelService.SelectAllReportList().Result.Data
                join users in _accountService.SelectUserList().Result.Data
                on reports.UserId equals users.Id into reportsUserGroup
                from subUsers in reportsUserGroup.DefaultIfEmpty()
                select new 
                { 
                    reportId = reports.Id,
                    userId = reports.UserId,
                    fullName = subUsers.Name + subUsers.LastName,
                    beginTime = reports.BeginTime,
                    endTime = reports.EndTime,
                    reportDate = reports.ReportDate,
                    workingtime = reports.WorkingTime
                };


            //if (responseReports.Data is null)
            //    responseReports.Data = new List<ReportOfDelivary>();

            //IEnumerable<ReportOfDelivary> source = response;
            //if (date != DateOnly.MinValue)
            //{
            //    source = source.Where(p => p.ReportDate == date);
            //}
            //if (!string.IsNullOrEmpty(county))
            //{
            //    source = source.Where(p => p.County!.Contains(county));
            //}


            //var count = source.Count();
            //var items = source.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            //PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);

            //ViewReportsViewModel viewModel = new ViewReportsViewModel(items, pageViewModel,
            //    new ReportFilterUserMangeViewModels(lastName, name, date, county));
            return View("ViewReportsViewPage"/*, viewModel*/);
        }

        [HttpGet]
        public async Task<IActionResult> GetReportCard(ulong id, bool isJson)
        {
            var response = await _reportPanelService.SelectReportById(id);
            var viewModel = new ReportViewDataViewModel
            {
                County = response.Data.County,
                DistancePassed = response.Data.DistancePassed,
                BeginTime = response.Data.BeginTime,
                EndTime = response.Data.EndTime
            };
            if (isJson)
                return Json(response.Data);
            return PartialView("GetReportCard", viewModel);


        }

    }
}
