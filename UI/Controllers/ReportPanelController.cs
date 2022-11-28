using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Reflection.Metadata;
using UIQueue.Controllers;

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
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return View("Success", "ReportCreatedMessage");
                }
                return View("Success", "ReportDidNotCreatMessage");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ReportsViewPage(byte? createFile,DateOnly? date, string? fullName,
                                                                string? county, int page = 1)
        {
            int pageSize = 15;   // size elemt on page

            ulong userId = _accountService.GetUserByEmail(User.Identity.Name ?? " ").Result.Data.Id;

            var response =
                from reports in _reportPanelService.SelectAllReportList().Result.Data
                join users in _accountService.SelectUserList().Result.Data
                on reports.UserId equals users.Id into reportsUserGroup
                from subUsers in reportsUserGroup.DefaultIfEmpty()
                select new ControlReportViewModal
                {
                    ReportId = reports.Id,
                    UserId = reports.UserId,
                    FullName = subUsers.Name + " " + subUsers.LastName,
                    BeginTime = reports.BeginTime,
                    EndTime = reports.EndTime,
                    ReportDate = reports.ReportDate,
                    WorkingTime = reports.WorkingTime,
                    County = reports.County,
                    DistancePassed = reports.DistancePassed
                };


            if (date is null)
                date = DateOnly.MinValue;

            if (response is null)
                response = new List<ControlReportViewModal>();

            IEnumerable<ControlReportViewModal> source = response;
            if (date != DateOnly.MinValue)
            {
                source = source.Where(p => p.ReportDate == date);
            }
            if (!string.IsNullOrEmpty(county))
            {
                source = source.Where(p => p.County!.Contains(county));
            }
            if (!string.IsNullOrEmpty(fullName))
            {
                source = source.Where(p => p.FullName!.Contains(fullName));
            }

            if (createFile == 1)
            {
                
                string language = Request.Cookies[".AspNetCore.Culture"];
                var fileResponse = await _reportPanelService.GenerateReportsFile(source.ToList(), language);
                return File(new MemoryStream(fileResponse.Data.FileContents, 0, fileResponse.Data.FileContents.Length),
                             fileResponse.Data.ContentType, fileResponse.Data.FileName);
            }


            var count = source.Count();
            var items = source.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            ControlViewReportsViewModel viewModel = new ControlViewReportsViewModel(items, pageViewModel,
                new ReportFilterUserMangeViewModel(fullName, date.Value, county));
            return View("ReportsViewPage", viewModel);

        }

        

        [HttpGet]
        public async Task<IActionResult> GetReportCard(ulong id, bool isJson)
        {
            var response = await _reportPanelService.SelectReportById(id);
            var userName = await _accountService.GetUserById(response.Data.UserId);
            var viewModel = new ReportViewDataViewModel
            {
                County = response.Data.County,
                DistancePassed = response.Data.DistancePassed,
                BeginTime = response.Data.BeginTime,
                EndTime = response.Data.EndTime,
                UserId = response.Data.UserId,
                ReportId = response.Data.Id,
                FullName = userName.Data.Name + " " + userName.Data.LastName
            };
            if (isJson)
                return Json(response.Data);
            return PartialView("GetReportCard", viewModel);
        }

    }
}
