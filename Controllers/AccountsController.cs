using KeeltekoolV2.Core.Domain;
using KeeltekoolV2.Core.DTO;
using KeeltekoolV2.Core.ServiceInterface;
using KeelteKool2.Models.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KeelteKool2.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailingServices _emailingServices;
        public IActionResult Index()
        {
            return View();
        }

        public AccountsController
            (
                UserManager<ApplicationUser> userManager,
                SignInManager<ApplicationUser> signInManager,
                IEmailingServices emailingServices
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailingServices = emailingServices;
        }
        // Sisukord:
        //
        // Registreerimine
        // Sisselogimine
        // 2fa
        // Kontotaaste
        // Kustutamine

        /*     R E G I S T R E E R I M I N E     */

        /// <summary>
        /// Viib kasutaja registreerimisvaatesse
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View(); //tagastab vaate
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = vm.Email,
                    Name = vm.Name,
                    Email = vm.Email,
                    Placeholder = vm.PlaceHolder,
                };

                var result = await _userManager.CreateAsync(user, vm.Password);

                if (result.Succeeded)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var confirmationLink = Url.Action("ConfirmEmail", "Accounts", new { userId = user.Id, token = token }, Request.Scheme);

                    EmailTokenDTO newsignup = new();
                    newsignup.Token = token;
                    newsignup.Body = $"Palun kinnita oma konto vajutades <a href=\"{confirmationLink}\">siia</a>";
                    newsignup.Subject = "Keeltekooli registreerimine";
                    newsignup.To = user.Email;

                    if (_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Administrations");
                    }

                    _emailingServices.SendEmailToken(newsignup, token);
                    List<string> errordatas =
                        [
                        "Area", "Accounts",
                        "Issue", "Success",
                        "StatusMessage", "Registration Sucesss",
                        "ActedOn", $"{vm.Email}",
                        "CreatedAccountData", $"{vm.Email}\n{vm.PlaceHolder}\n[password hidden]\n[password hidden]"
                        ];
                    ViewBag.ErrorDatas = errordatas;
                    ViewBag.ErrorTitle = "You have successfully registered";
                    ViewBag.ErrorMessage = "Before you can log in, please confirm email from the link" +
                        "\nwe have emailed to your email address.";
                    return View("ConfirmationEmailMessage");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View();
        }
    }
}
