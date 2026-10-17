using KeeltekoolV2.Core.DTO;
using KeeltekoolV2.Core.ServiceInterface;
using KeeltekoolV2.Data;
using KeelteKool2.Models.LanguageCourses;
using Microsoft.AspNetCore.Mvc;

namespace KeelteKool2.Controllers
{
    public class LanguageCoursesController : Controller
    {
        private readonly KeeltekoolV2Context _context;
        private readonly ILanguageServices _languageCoursesServices;

        public LanguageCoursesController(KeeltekoolV2Context context, ILanguageServices languageCoursesServices)
        {
            _context = context;
            _languageCoursesServices = languageCoursesServices;
        }

        public IActionResult Index()
        {
            var result = _context.LanguageCourses
                .OrderBy(x => x.Keel)
                .Select(x => new LanguageCourseViewModel
                {
                    Id = x.Id,
                    Nimetus = x.Nimetus,
                    Keel = x.Keel,
                    Tase = x.Tase,
                    Kirjeldus = x.Kirjeldus,
                })
                .Take(20)
                .ToList();
            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            LanguageCourseCreateUpdateViewModel vm = new();
            return View("CreateUpdate", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LanguageCourseCreateUpdateViewModel vm)
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
            var dto = new LanguageCourseDTO()
            {
                Id = vm.Id,
                Nimetus = vm.Nimetus,
                Keel = vm.Keel,
                Tase = vm.Tase,
                Kirjeldus = vm.Kirjeldus
            };
            //teostatakse päring teenusele
            var result = await _languageCoursesServices.Create(dto);
            if (result == null)
            {
                return RedirectToAction("Error", "Home");
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
            var languageCourse = await _languageCoursesServices.DetailsAsync(id);
            if (languageCourse == null)
            {
                return NotFound();
            }
            var vm = new LanguageCourseViewModel()
            {
                Id = languageCourse.Id,
                Nimetus = languageCourse.Nimetus,
                Keel = languageCourse.Keel,
                Tase = languageCourse.Tase,
                Kirjeldus = languageCourse.Kirjeldus
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
            var courseToUpdate = await _languageCoursesServices.DetailsAsync(id);
            if (courseToUpdate == null)
            {
                return NotFound();
            }
            var vm = new LanguageCourseCreateUpdateViewModel()
            {
                Id = courseToUpdate.Id,
                Nimetus = courseToUpdate.Nimetus,
                Keel = courseToUpdate.Keel,
                Tase = courseToUpdate.Tase,
                Kirjeldus = courseToUpdate.Kirjeldus,
                CreatedAt = courseToUpdate.CreatedAt,
                ModifiedAt = courseToUpdate.ModifiedAt
            };
            return View("CreateUpdate", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(LanguageCourseCreateUpdateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateUpdate", vm);
            }
            var dto = new LanguageCourseDTO()
            {
                Id = vm.Id,
                Nimetus = vm.Nimetus,
                Keel = vm.Keel,
                Tase = vm.Tase,
                Kirjeldus = vm.Kirjeldus,
                CreatedAt = vm.CreatedAt,
                ModifiedAt = vm.ModifiedAt
            };
            var result = await _languageCoursesServices.Update(dto);
            if (result == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Update), new { id = result.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || id == Guid.Empty)
            {
                return NotFound();
            }
            var languageCourse = await _languageCoursesServices.DetailsAsync((Guid)id);
            if (languageCourse == null)
            {
                return NotFound();
            }
            var vm = new LanguageCourseViewModel()
            {
                Id = languageCourse.Id,
                Nimetus = languageCourse.Nimetus,
                Keel = languageCourse.Keel,
                Tase = languageCourse.Tase,
                Kirjeldus = languageCourse.Kirjeldus
            };

            ViewData["ViewType"] = "delete";

            return View("DetailsDelete", vm);
        }

        [HttpPost]
        [ActionName("DeleteConfirmed")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var courseToDelete = await _languageCoursesServices.Delete(id);
            if (courseToDelete == null)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
