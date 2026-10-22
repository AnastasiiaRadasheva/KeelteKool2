using KeeltekoolV2.Core.DTO;
using KeeltekoolV2.Core.ServiceInterface;
using KeeltekoolV2.Data;
using KeelteKool2.Models.Students;
using Microsoft.AspNetCore.Mvc;

namespace KeelteKool2.Controllers
{
    public class StudentsController : Controller
    {
        private readonly KeeltekoolV2Context _context;
        private readonly IStudentsServices _studentsServices;

        public StudentsController(KeeltekoolV2Context context, IStudentsServices studentsServices)
        {
            _context = context;
            _studentsServices = studentsServices;
        }

        public IActionResult Index()
        {
            var result = _context.Students
                .OrderBy(x => x.LastName)
                .Select(x => new StudentViewModel
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    LanguageLevel = x.LanguageLevel,
                })
                .Take(20)
                .ToList();
            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            StudentCreateUpdateViewModel vm = new();
            return View("CreateUpdate", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentCreateUpdateViewModel vm)
        {
            //kontrollime et vm ei oleks null
            if (vm == null)
            {
                return RedirectToAction("Error", "Home");
            }
            //kontrollime et vmi modelstate on õige
            if (!ModelState.IsValid)
            {
                return View("CreateUpdate", vm);
            }
            //teeme uue DTO-objekti ja asetame dtosse vmi andmed
            var dto = new StudentDTO()
            {
                Id = vm.Id,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Email = vm.Email,
                LanguageLevel = vm.LanguageLevel
            };
            //teostatakse päring teenusele
            var result = await _studentsServices.Create(dto);
            if (result == null)
            {
                //teenus lükkas andmed tagasi (nt tühi nimi või vale e-post)
                ModelState.AddModelError("", "Õpilase andmed ei ole korrektsed.");
                return View("CreateUpdate", vm);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
