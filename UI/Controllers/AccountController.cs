using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Models.ViewModels;
using System.Net;
using System.Security.Claims;

namespace UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IMailService _mailService;

        public AccountController(
            IAccountService accountService,
            IMailService mailService)
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

                    return RedirectToAction("SuccessRegistration", "Account");
                }
                ModelState.AddModelError("Error", response.Description);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult SuccessRegistration()
        {
            return View();
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
            if(ModelState.IsValid) 
            {
                var user = _accountService.GetUserByEmail(model.Email);
                if (user.Result.Data == null || user.Result.Data.Email != User.Identity.Name)
                {
                    return View(model);
                    ModelState.AddModelError("", user.Result.Description);
                }
                    
                    
                string emailConfirmationUrl = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { userId = user.Result.Data.Id, code = user.Result.Data.EmailConfirmedToken.ToString() },
                    protocol: HttpContext.Request.Scheme);
                _mailService.SendEmailAsync(model.Email, "Reset your password", emailConfirmationUrl);

                return RedirectToAction("SuccessResetPasswordRequist", "Account");
            }
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(ulong userId, string code)
        {
            ViewBag.Id = userId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            ViewBag.Id = model.Id;
            if(ModelState.IsValid)
            {
                var response = _accountService.ResetPassword(model.Id, model.Password);
                if (response.Result.StatusCode == HttpStatusCode.OK)
                    return RedirectToAction("SuccessResetPasswordResponse", "Account");
                ModelState.AddModelError("", response.Result.Description);
            }
            return View();
        }

        [HttpGet]
        public IActionResult SuccessResetPasswordRequist()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SuccessResetPasswordResponse()
        {
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
            if (user == null)
            {
                return View("Error");
            }
            var result = await _accountService.ConfirmEmailAsync(user.Data.Id, code);
            if (result.Data)
                return RedirectToAction("Login", "Account");
            else
                return View("Error");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var model = await _accountService.GetUserByEmail(User.Identity.Name);
            var user = new AccountProfileViewModels
            {
                Email = model.Data.Email,
                Name = model.Data.Name,
                LastName = model.Data.LastName,
                PhoneNumber = model.Data.PhoneNumber,
                Role = model.Data.Role
            };
            return View(user);
        }
    }
}
