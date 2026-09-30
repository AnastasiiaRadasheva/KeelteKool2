using KeeltekoolV2.ApplicationServices;
using KeeltekoolV2.Core.Domain;
using KeeltekoolV2.Core.ServiceInterface;
using KeeltekoolV2.Data;
using Microsoft.AspNetCore.Mvc;

namespace KeelteKool2.Controllers
{
    public class LanguageCourcesController : Controller
    {
        private readonly KeeltekoolV2Context _context;
        private readonly ILanguageCoursesServices _languageCoursesServices;
        public LanguageCourcesController(
            KeeltekoolV2Context context, ILanguageCoursesServices languageCoursesServices)
        {
            _context = context;
            _languageCoursesServices = languageCoursesServices;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Create()
        {
            LanguageCourseViewModel xm = new();
            return View(vm);
        }
    }
}
