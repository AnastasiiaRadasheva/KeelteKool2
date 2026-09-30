using KeelteKool2.Models.LanguageCourses;
using KeeltekoolV2.ApplicationServices;
using KeeltekoolV2.Core.Domain;
using KeeltekoolV2.Core.DTO;
using KeeltekoolV2.Core.ServiceInterface;
using KeeltekoolV2.Data;
using Microsoft.AspNetCore.Authorization;
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
            LanguageCourseViewModel vm = new();
            return View(vm);
        }
        [HttpPost]
      
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Create(LanguageCourseViewModel vm)
        {
                //kontrolliem et vm ei ole 0
                if (vm == null)
            {
                return RedirectToAction("Error", "Home");
            }
            //kontrollime et vmi modelsatte on õige
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Error", "Home");
            }

            //teeme uee DTo objecti
            var dto = new LanguageCourseDTO()
            {
                Id = vm.Id,
                Nimetus = vm.Nimetus,
                Keel = vm.Keel,
                Tase = vm.Tase,
                Kirjeldus = vm.Kirjeldus
            };
            //aseme dtosse vmi andmed 

            //teostatakse päring teenusele
            var result = await _languageCoursesServices.Create(dto);
            //teenus peab objecti tagastama
            //kontrollime kas t´agastatus object on 0
            if (result == null)
            {
                //kui on suuname vealehele
                return RedirectToAction("Error", "Home");
            }
            else
            //kuo ei suuname tagasi indeksisse
            {
                return RedirectToAction("Error", "Home");
            }
 
        }
    }
}
