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
        private readonly ILanguageCoursesServices _languageCoursesServices;

        public LanguageCoursesController(KeeltekoolV2Context context, ILanguageCoursesServices languageCoursesServices)
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
                })
                .Take(20)
                .ToList();
            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            LanguageCourseViewModel vm = new();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LanguageCourseViewModel vm)
        {
            if (vm == null)
            {
                return RedirectToAction("Error", "Home");
            }
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var dto = new LanguageCourseDTO()
            {
                Id = vm.Id,
                Nimetus = vm.Nimetus,
                Keel = vm.Keel,
                Tase = vm.Tase,
                Kirjeldus = vm.Kirjeldus
            };
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
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
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
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(LanguageCourseViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var dto = new LanguageCourseDTO()
            {
                Id = vm.Id,
                Nimetus = vm.Nimetus,
                Keel = vm.Keel,
                Tase = vm.Tase,
                Kirjeldus = vm.Kirjeldus
            };
            var result = await _languageCoursesServices.Update(dto);
            if (result == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
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
            return View(vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmation(Guid id)
        {
            var result = await _languageCoursesServices.Delete(id);
            if (result == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}