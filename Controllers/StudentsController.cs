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

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }
            var student = await _studentsServices.DetailsAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            var vm = new StudentViewModel()
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
                LanguageLevel = student.LanguageLevel
            };

            ViewData["ViewType"] = "details";

            return View("DetailsDelete", vm);
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }
            var studentToUpdate = await _studentsServices.DetailsAsync(id);
            if (studentToUpdate == null)
            {
                return NotFound();
            }
            var vm = new StudentCreateUpdateViewModel()
            {
                Id = studentToUpdate.Id,
                FirstName = studentToUpdate.FirstName,
                LastName = studentToUpdate.LastName,
                Email = studentToUpdate.Email,
                LanguageLevel = studentToUpdate.LanguageLevel,
                CreatedAt = studentToUpdate.CreatedAt,
                ModifiedAt = studentToUpdate.ModifiedAt
            };
            return View("CreateUpdate", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(StudentCreateUpdateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateUpdate", vm);
            }
            var dto = new StudentDTO()
            {
                Id = vm.Id,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Email = vm.Email,
                LanguageLevel = vm.LanguageLevel,
                CreatedAt = vm.CreatedAt,
                ModifiedAt = vm.ModifiedAt
            };
            var result = await _studentsServices.Update(dto);
            if (result == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Update), new { id = result.Id });
        }
    }
}
