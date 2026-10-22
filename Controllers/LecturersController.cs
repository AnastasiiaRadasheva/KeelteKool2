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
    }
}
