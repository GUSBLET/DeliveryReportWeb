
namespace UI.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class AdminPanelController : Controller
    {
        private readonly IAdminPanelService _adminPanelService;
        

        public AdminPanelController([FromServices] IAdminPanelService adminServer ,IAccountService accountService)
        {
            _adminPanelService = adminServer;
        }

        [HttpGet]
        public async Task<IActionResult> CheckReport()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UserManagement(Role? role, string? email, int page = 1)
        {
            int pageSize = 15;   // size elemt on page

            var response = await _adminPanelService.SelectUserList();

            if (response.Data is null)
                response.Data = new List<Account>();

            IEnumerable<Account> source = response.Data;
            if (role != null)
            {
                source = source.Where(p => p.Role == role);
            }
            if (!string.IsNullOrEmpty(email))
            {
                source = source.Where(p => p.Email!.Contains(email));
            }


            List<string> roles = new List<string>();
            roles.Add(Role.User.ToString());
            roles.Add(Role.Admin.ToString());
            roles.Add(Role.Deliveryman.ToString());
            roles.Add(Role.Manager.ToString());
            var count = source.Count();
            var items = source.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);

            IndexViewModel viewModel = new IndexViewModel(items, pageViewModel, 
                new FilterUserMangeViewModels(roles, role.ToString(), email));
            return View(viewModel);            
        }

        [HttpGet]
        public async Task<ActionResult> GetUserCard(ulong id, bool isJson)
        {
            var response = await _adminPanelService.GetUserCardById(id);
            if (isJson)
                return Json(response.Data);
            return PartialView("GetUserCard", response.Data);
        }

        [HttpPost]
        public async Task<IActionResult> ChangingRole(ChangeUserViewModel account)
        {
            BaseResponse<Account> response = new BaseResponse<Account>();
            if (User.IsInRole("Admin"))
                response = await _adminPanelService.AdminChangesRole(account);
            else if (User.IsInRole("Manager"))
                response = await _adminPanelService.ManagerChangesRole(account);

            if(response.StatusCode == HttpStatusCode.OK)
            {
                return RedirectToAction("UserManagement", "AdminPanel");
            }
            else
                ModelState.AddModelError("", response.Description);
            return View("Error", response.Description);
        }

        [HttpGet]
        public IActionResult GetDeleteUserCard(ulong id, bool isJson)
        {
            if (isJson)
                return Json(" ");
            return PartialView("GetDeleteUserCard", id);
        }

        [HttpPost] 
        public async Task<IActionResult> DeleteUser(ulong id)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            if (User.IsInRole("Admin"))
                response = await _adminPanelService.AdminDeletesUserById(id);
            else if (User.IsInRole("Manager"))
                response = await _adminPanelService.ManagerDeletesUserById(id);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return RedirectToAction("UserManagement", "AdminPanel");
            }
            else
                ModelState.AddModelError("", response.Description);
            return View("Error", response.Description);
            
        }
    }
}
