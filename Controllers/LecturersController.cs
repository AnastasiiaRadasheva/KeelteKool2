using KeelteKool2.Models.Lecturers;
using KeeltekoolV2.Core.DTO;
using KeeltekoolV2.Core.ServiceInterface;
using Microsoft.AspNetCore.Mvc;

namespace KeelteKool2.Controllers
{
    public class LecturersController : Controller
    {
        private readonly ILecturersServices _lecturersServices;

        public LecturersController(ILecturersServices lecturersServices)
        {
            _lecturersServices = lecturersServices;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //küsime teenuselt kõik õppejõud ja teeme need vaatemudeliteks
            var lecturers = await _lecturersServices.GetAll();
            var result = lecturers
                .Select(x => new LecturerViewModel
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Qualifications = x.Qualifications,
                    CreatedAt = x.CreatedAt,
                    ModifiedAt = x.ModifiedAt
                })
                .ToList();
            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            LecturerCreateUpdateViewModel vm = new();
            return View("CreateUpdate", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LecturerCreateUpdateViewModel vm)
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
            var dto = new LectureDTO()
            {
                Id = vm.Id ?? Guid.Empty,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Qualifications = vm.Qualifications ?? ""
            };
            //teostatakse päring teenusele
            var result = await _lecturersServices.Create(dto);
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
            var lecturer = await _lecturersServices.DetailAsync(new LectureDTO { Id = id });
            if (lecturer == null)
            {
                return NotFound();
            }
            var vm = new LecturerViewModel()
            {
                Id = lecturer.Id,
                FirstName = lecturer.FirstName,
                LastName = lecturer.LastName,
                Qualifications = lecturer.Qualifications,
                CreatedAt = lecturer.CreatedAt,
                ModifiedAt = lecturer.ModifiedAt
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
            var lecturerToUpdate = await _lecturersServices.DetailAsync(new LectureDTO { Id = id });
            if (lecturerToUpdate == null)
            {
                return NotFound();
            }
            var vm = new LecturerCreateUpdateViewModel()
            {
                Id = lecturerToUpdate.Id,
                FirstName = lecturerToUpdate.FirstName,
                LastName = lecturerToUpdate.LastName,
                Qualifications = lecturerToUpdate.Qualifications,
                CreatedAt = lecturerToUpdate.CreatedAt,
                ModifiedAt = lecturerToUpdate.ModifiedAt
            };
            return View("CreateUpdate", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(LecturerCreateUpdateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateUpdate", vm);
            }
            var dto = new LectureDTO()
            {
                Id = vm.Id ?? Guid.Empty,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Qualifications = vm.Qualifications ?? "",
                CreatedAt = vm.CreatedAt,
                ModifiedAt = vm.ModifiedAt
            };
            var result = await _lecturersServices.Update(dto);
            if (result == null)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
