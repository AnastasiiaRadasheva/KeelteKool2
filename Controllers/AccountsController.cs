using Microsoft.AspNetCore.Mvc;

namespace KeelteKool2.Controllers
{
    public class AccountsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
