
using Azure;
using Microsoft.Net.Http.Headers;
using Models.Helpers;
using System.Collections.Specialized;
using System.Security.Claims;

namespace UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IMailService _mailService;

        public AccountController(
            [FromServices] IAccountService accountService,
            [FromServices] IMailService mailService)
        {
            _accountService = accountService;
            _mailService = mailService;
        }

        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registration(RegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _accountService.Registration(model);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var user = _accountService.GetUserByEmail(model.Email);
                    string emailConfirmationUrl = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { userId = user.Result.Data.Id, code = user.Result.Data.EmailConfirmedToken.ToString() },
                        protocol: HttpContext.Request.Scheme);
                    _mailService.SendEmailAsync(model.Email, "Confirm your email", emailConfirmationUrl);

                    return View("Success", "SuccessRegistrationMessge");
                }
                ModelState.AddModelError("Error", response.Description);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult EditEmail()
        {
            // we create model for view
            var result = new EditEmailRequestViewModel
            {
                Email = User.Identity.Name ?? string.Empty
            };

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> EditEmail(EditEmailRequestViewModel model)
        {
            if(!ModelState.IsValid)
                return View(model);
            
            var response = _accountService.GetUserByEmail(model.Email);
            if(response.Result.Data.Password == HashPasswordHelper.HashPassowrd(model.Password))
                return RedirectToAction("ContinueEditEmail");

            return View(model);
        }

        [HttpGet]
        public IActionResult ContinueEditEmail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ContinueEditEmail(EditEmailContinueViewModel model)
        {
            if(!ModelState.IsValid)
                return View(model);

            if(_accountService.GetUserByEmail(model.NewEmail).Result.Data != null)
            {
                ModelState.AddModelError("", "Email Address Exist");
            }
                
            var user = _accountService.GetUserByEmail(User.Identity.Name ?? string.Empty);
            if (user.Result.Data == null)
            {
                ModelState.AddModelError("", user.Result.Description);
                return View(model);
            }

            string emailConfirmationUrl = "Click here for edit your email address: " + Url.Action(
                    "CompleteEditEmail",
                    "Account",
                    new { userId = user.Result.Data.Id, code = user.Result.Data.EmailConfirmedToken.ToString(), email = model.NewEmail.ToString() },
                    protocol: HttpContext.Request.Scheme);
            _mailService.SendEmailAsync(model.NewEmail, "Edit your email", emailConfirmationUrl);

            return View("Success", "CheckEmailForEditEmailMessage");
        }

        [HttpGet]
        public async Task<IActionResult> CompleteEditEmail(string userId, string code, string email)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await _accountService.GetUserById(Convert.ToUInt64(userId));
            if (user.Data == null || user.Data.EmailConfirmedToken.ToString() != code)
            {
                return View("Error", "User don't exist");
            }
            
            var result = _accountService.ChangeEmail(email, Convert.ToUInt64(userId));

            if (result.Result)
                return RedirectToAction("Logout", "Account");
            else
                return View("Error", "Error");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _accountService.Login(model);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(response.Data));

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", response.Description);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPasswordRequist()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPasswordRequist(ResetPasswordRequistViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _accountService.GetUserByEmail(model.Email);
                if (user.Result.Data == null)
                {
                    ModelState.AddModelError("", user.Result.Description);
                    return View(model);
                }

                string emailConfirmationUrl = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { userId = user.Result.Data.Id, code = user.Result.Data.EmailConfirmedToken.ToString() },
                    protocol: HttpContext.Request.Scheme);
                _mailService.SendEmailAsync(model.Email, "Reset your password", emailConfirmationUrl);

                return View("Success", "CheckEmailForResetPasswordMessage");
            }
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(ulong userId)
        {
            ViewBag.Id = userId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            ViewBag.Id = model.Id;
            if (ModelState.IsValid)
            {
                var response = _accountService.ResetPassword(model.Id, model.Password);
                if (response.Result.StatusCode == HttpStatusCode.OK)
                    return View("Success", "PasswordChangedMessage");
                if (response.Result.Description == "UserDosen'tExistMessage")
                    return View("Error", "UserDosen'tExistMessage");
                ModelState.AddModelError("", response.Result.Description);
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await _accountService.GetUserById(Convert.ToUInt64(userId));
            if (user.Data == null || user.Data.EmailConfirmedToken.ToString() != code)
            {
                return View("Error", "User don't exist");
            }
            var result = await _accountService.ConfirmEmailAsync(user.Data.Id, code);
            if (result.Data)
                return RedirectToAction("Login", "Account");
            else
                return View("Error", result.Description);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var model = await _accountService.GetUserByEmail(User.Identity.Name ?? " ");
            if (model.Data == null)
                return RedirectToAction("Logout");
            var user = new AccountProfileViewModels
            {
                Id = model.Data.Id,
                Email = model.Data.Email,
                Name = model.Data.Name,
                LastName = model.Data.LastName,
                PhoneNumber = model.Data.PhoneNumber,
                Role = model.Data.Role
            };
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> ChangeProfile()
        {
            var response = await _accountService.GetUserByEmail(User.Identity.Name ?? string.Empty);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var profileChange = new ChangeUserViewModel()
                {
                    Email = response.Data.Email,
                    Name = response.Data.Name,
                    LastName = response.Data.LastName,
                    PhoneNumber = response.Data.PhoneNumber,
                    Role = response.Data.Role
                };
                return View(profileChange);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> ChangeProfile(ChangeUserViewModel account)
        {
            if (ModelState.IsValid)
            {
                var response = await _accountService.UpdateUser(account);
                if (response.StatusCode == HttpStatusCode.OK)
                    return RedirectToAction("Profile", "Account");
                return RedirectToAction("Error");
            }
            return View(account);

        }




    }
}
