using KeeltekoolV2.Core.Domain;
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
        //private readonly IEmailServices _emailServices;
        public IActionResult Index()
        {
            return View();
        }

        public AccountsController
            (
                UserManager<ApplicationUser> userManager,
                SignInManager<ApplicationUser> signInManager/*,
                IEmailServices emailServices*/
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            //_emailServices            
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
            return View();
        }
    }
}
